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
    [UserOnly]
    public class UserController : Controller
    {
        public ActionResult Dashboard()
        {
            int uid = Convert.ToInt32(Session["UserId"]);
            ViewBag.TotalOrders     = OracleDbHelper.ExecuteScalar("SELECT COUNT(*) FROM ORDERS WHERE USER_ID=:id", new[]{new OracleParameter("id",uid)});
            ViewBag.PendingOrders   = OracleDbHelper.ExecuteScalar("SELECT COUNT(*) FROM ORDERS WHERE USER_ID=:id AND STATUS='Pending'", new[]{new OracleParameter("id",uid)});
            ViewBag.DeliveredOrders = OracleDbHelper.ExecuteScalar("SELECT COUNT(*) FROM ORDERS WHERE USER_ID=:id AND STATUS='Delivered'", new[]{new OracleParameter("id",uid)});
            return View();
        }

        public ActionResult Products(string category="", string search="")
        {
            string q = "SELECT * FROM PRODUCTS WHERE IS_ACTIVE=1";
            var p = new List<OracleParameter>();
            if (!string.IsNullOrEmpty(category)) { q+=" AND CATEGORY=:c"; p.Add(new OracleParameter("c",category)); }
            if (!string.IsNullOrEmpty(search))   { q+=" AND LOWER(NAME) LIKE :s"; p.Add(new OracleParameter("s","%"+search.ToLower()+"%")); }
            q += " ORDER BY CREATED_AT DESC";
            DataTable dt = OracleDbHelper.ExecuteQuery(q, p.ToArray());
            var list = new List<Product>();
            foreach (DataRow r in dt.Rows)
                list.Add(new Product {
                    ProductId=Convert.ToInt32(r["PRODUCT_ID"]),
                    Name=r["NAME"].ToString(), Description=r["DESCRIPTION"].ToString(),
                    Price=Convert.ToDecimal(r["PRICE"]), Stock=Convert.ToInt32(r["STOCK"]),
                    Category=r["CATEGORY"].ToString(), ImageUrl=r["IMAGE_URL"].ToString()
                });
            ViewBag.Category=category; ViewBag.Search=search;
            return View(list);
        }

        [HttpPost]
        public ActionResult PlaceOrder(int productId,int quantity,string paymentMethod,string address)
        {
            int uid = Convert.ToInt32(Session["UserId"]);
            DataTable pd = OracleDbHelper.ExecuteQuery(
                "SELECT * FROM PRODUCTS WHERE PRODUCT_ID=:id AND IS_ACTIVE=1",
                new[]{new OracleParameter("id",productId)});
            if (pd.Rows.Count==0){TempData["Error"]="Product not found.";return RedirectToAction("Products");}
            decimal unit = Convert.ToDecimal(pd.Rows[0]["PRICE"]);
            int stock = Convert.ToInt32(pd.Rows[0]["STOCK"]);
            if (quantity>stock){TempData["Error"]="Insufficient stock ("+stock+" left).";return RedirectToAction("Products");}
            decimal total = unit*quantity;

            int newId;
            using (var conn=OracleDbHelper.GetConnection())
            {
                conn.Open();
                using (var cmd=new OracleCommand(
                    @"INSERT INTO ORDERS(USER_ID,TOTAL_AMOUNT,STATUS,PAYMENT_METHOD,PAYMENT_STATUS,DELIVERY_ADDRESS,ORDER_DATE)
                      VALUES(:uid,:t,'Pending',:pm,'Pending',:a,SYSDATE) RETURNING ORDER_ID INTO :nid",conn))
                {
                    cmd.Parameters.Add(new OracleParameter("uid",uid));
                    cmd.Parameters.Add(new OracleParameter("t",total));
                    cmd.Parameters.Add(new OracleParameter("pm",paymentMethod));
                    cmd.Parameters.Add(new OracleParameter("a",address));
                    var op=new OracleParameter("nid",OracleDbType.Int32){Direction=System.Data.ParameterDirection.Output};
                    cmd.Parameters.Add(op); cmd.ExecuteNonQuery();
                    newId=Convert.ToInt32(op.Value.ToString());
                }
            }

            OracleDbHelper.ExecuteNonQuery(
                @"INSERT INTO ORDER_ITEMS(ORDER_ID,PRODUCT_ID,PRODUCT_NAME,QUANTITY,UNIT_PRICE,TOTAL_PRICE)
                  VALUES(:oid,:pid,:pn,:q,:u,:t)",
                new[]{new OracleParameter("oid",newId),new OracleParameter("pid",productId),
                      new OracleParameter("pn",pd.Rows[0]["NAME"].ToString()),new OracleParameter("q",quantity),
                      new OracleParameter("u",unit),new OracleParameter("t",total)});

            OracleDbHelper.ExecuteNonQuery("UPDATE PRODUCTS SET STOCK=STOCK-:q WHERE PRODUCT_ID=:id",
                new[]{new OracleParameter("q",quantity),new OracleParameter("id",productId)});

            OracleDbHelper.ExecuteNonQuery(
                @"INSERT INTO PAYMENTS(ORDER_ID,AMOUNT,METHOD,STATUS,PAYMENT_DATE) VALUES(:oid,:a,:m,'Pending',SYSDATE)",
                new[]{new OracleParameter("oid",newId),new OracleParameter("a",total),new OracleParameter("m",paymentMethod)});

            TempData["Success"]="Order placed! Order ID: #"+newId;
            return RedirectToAction("MyOrders");
        }

        public ActionResult MyOrders()
        {
            int uid = Convert.ToInt32(Session["UserId"]);
            DataTable dt = OracleDbHelper.ExecuteQuery(
                "SELECT * FROM ORDERS WHERE USER_ID=:id ORDER BY ORDER_DATE DESC",
                new[]{new OracleParameter("id",uid)});
            return View(dt);
        }
    }
}
