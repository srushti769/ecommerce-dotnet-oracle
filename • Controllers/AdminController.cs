using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using Oracle.ManagedDataAccess.Client;
using ECommerceApp.Helpers;
using ECommerceApp.Models;
using ECommerceApp.Filters;

namespace ECommerceApp.Controllers
{
    [AdminOnly]
    public class AdminController : Controller
    {
        // DASHBOARD
        public ActionResult Dashboard()
        {
            ViewBag.TotalProducts = OracleDbHelper.ExecuteScalar("SELECT COUNT(*) FROM PRODUCTS WHERE IS_ACTIVE=1");
            ViewBag.TotalUsers    = OracleDbHelper.ExecuteScalar("SELECT COUNT(*) FROM USERS WHERE ROLE='User'");
            ViewBag.TotalOrders   = OracleDbHelper.ExecuteScalar("SELECT COUNT(*) FROM ORDERS");
            ViewBag.TotalRevenue  = OracleDbHelper.ExecuteScalar("SELECT NVL(SUM(TOTAL_AMOUNT),0) FROM ORDERS WHERE STATUS!='Cancelled'");
            return View();
        }

        // LIST PRODUCTS
        public ActionResult Products()
        {
            DataTable dt = OracleDbHelper.ExecuteQuery("SELECT * FROM PRODUCTS ORDER BY CREATED_AT DESC");
            var list = new List<Product>();
            foreach (DataRow r in dt.Rows)
                list.Add(new Product {
                    ProductId   = Convert.ToInt32(r["PRODUCT_ID"]),
                    Name        = r["NAME"].ToString(),
                    Description = r["DESCRIPTION"].ToString(),
                    Price       = Convert.ToDecimal(r["PRICE"]),
                    Stock       = Convert.ToInt32(r["STOCK"]),
                    Category    = r["CATEGORY"].ToString(),
                    ImageUrl    = r["IMAGE_URL"].ToString(),
                    IsActive    = r["IS_ACTIVE"].ToString() == "1"
                });
            return View(list);
        }

        // ADD PRODUCT
        [HttpGet]
        public ActionResult AddProduct() => View();

        [HttpPost]
        public ActionResult AddProduct(Product p)
        {
            OracleDbHelper.ExecuteNonQuery(
                @"INSERT INTO PRODUCTS(NAME,DESCRIPTION,PRICE,STOCK,CATEGORY,IMAGE_URL,IS_ACTIVE,CREATED_AT)
                  VALUES(:n,:d,:pr,:s,:c,:i,1,SYSDATE)",
                new[] {
                    new OracleParameter("n",  p.Name),
                    new OracleParameter("d",  p.Description),
                    new OracleParameter("pr", p.Price),
                    new OracleParameter("s",  p.Stock),
                    new OracleParameter("c",  p.Category),
                    new OracleParameter("i",  p.ImageUrl ?? "")
                });
            TempData["Success"] = "Product added!";
            return RedirectToAction("Products");
        }

        // EDIT PRODUCT
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            DataTable dt = OracleDbHelper.ExecuteQuery(
                "SELECT * FROM PRODUCTS WHERE PRODUCT_ID=:id",
                new[] { new OracleParameter("id", id) });
            if (dt.Rows.Count == 0) return HttpNotFound();
            return View(new Product {
                ProductId   = Convert.ToInt32(dt.Rows[0]["PRODUCT_ID"]),
                Name        = dt.Rows[0]["NAME"].ToString(),
                Description = dt.Rows[0]["DESCRIPTION"].ToString(),
                Price       = Convert.ToDecimal(dt.Rows[0]["PRICE"]),
                Stock       = Convert.ToInt32(dt.Rows[0]["STOCK"]),
                Category    = dt.Rows[0]["CATEGORY"].ToString(),
                ImageUrl    = dt.Rows[0]["IMAGE_URL"].ToString()
            });
        }

        [HttpPost]
        public ActionResult EditProduct(Product p)
        {
            OracleDbHelper.ExecuteNonQuery(
                @"UPDATE PRODUCTS SET NAME=:n,DESCRIPTION=:d,PRICE=:pr,
                  STOCK=:s,CATEGORY=:c,IMAGE_URL=:i WHERE PRODUCT_ID=:id",
                new[] {
                    new OracleParameter("n",  p.Name),
                    new OracleParameter("d",  p.Description),
                    new OracleParameter("pr", p.Price),
                    new OracleParameter("s",  p.Stock),
                    new OracleParameter("c",  p.Category),
                    new OracleParameter("i",  p.ImageUrl ?? ""),
                    new OracleParameter("id", p.ProductId)
                });
            TempData["Success"] = "Product updated!";
            return RedirectToAction("Products");
        }

        // DELETE (Soft)
        public ActionResult DeleteProduct(int id)
        {
            OracleDbHelper.ExecuteNonQuery(
                "UPDATE PRODUCTS SET IS_ACTIVE=0 WHERE PRODUCT_ID=:id",
                new[] { new OracleParameter("id", id) });
            TempData["Success"] = "Product removed!";
            return RedirectToAction("Products");
        }

        // ORDERS
        public ActionResult Orders()
        {
            DataTable dt = OracleDbHelper.ExecuteQuery(
                @"SELECT O.ORDER_ID,U.USERNAME,O.TOTAL_AMOUNT,O.STATUS,
                         O.PAYMENT_METHOD,O.PAYMENT_STATUS,O.ORDER_DATE
                  FROM ORDERS O JOIN USERS U ON O.USER_ID=U.USER_ID
                  ORDER BY O.ORDER_DATE DESC");
            return View(dt);
        }

        [HttpPost]
        public ActionResult UpdateOrderStatus(int orderId, string status)
        {
            OracleDbHelper.ExecuteNonQuery(
                "UPDATE ORDERS SET STATUS=:s WHERE ORDER_ID=:id",
                new[] { new OracleParameter("s",status), new OracleParameter("id",orderId) });
            return RedirectToAction("Orders");
        }

        // USERS
        public ActionResult Users()
        {
            DataTable dt = OracleDbHelper.ExecuteQuery(
                "SELECT USER_ID,USERNAME,EMAIL,PHONE,CREATED_AT FROM USERS WHERE ROLE='User' ORDER BY CREATED_AT DESC");
            return View(dt);
        }
    }
}
