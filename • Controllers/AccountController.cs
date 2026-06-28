using System;
using System.Data;
using System.Web.Mvc;
using Oracle.ManagedDataAccess.Client;
using ECommerceApp.Helpers;
using ECommerceApp.Models;

namespace ECommerceApp.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            if (Session["UserId"] != null)
                return Session["UserRole"].ToString() == "Admin"
                    ? RedirectToAction("Dashboard", "Admin")
                    : RedirectToAction("Dashboard", "User");
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            string query = "SELECT * FROM USERS WHERE EMAIL=:email AND PASSWORD=:password AND IS_ACTIVE=1";
            OracleParameter[] p = {
                new OracleParameter("email", email),
                new OracleParameter("password", password)
            };
            DataTable dt = OracleDbHelper.ExecuteQuery(query, p);

            if (dt.Rows.Count > 0)
            {
                Session["UserId"]    = dt.Rows[0]["USER_ID"].ToString();
                Session["UserName"]  = dt.Rows[0]["USERNAME"].ToString();
                Session["UserRole"]  = dt.Rows[0]["ROLE"].ToString();
                Session["UserEmail"] = dt.Rows[0]["EMAIL"].ToString();
                return Session["UserRole"].ToString() == "Admin"
                    ? RedirectToAction("Dashboard", "Admin")
                    : RedirectToAction("Dashboard", "User");
            }
            ViewBag.Error = "Invalid email or password.";
            return View();
        }

        public ActionResult Register() => View();

        [HttpPost]
        public ActionResult Register(User user)
        {
            int count = Convert.ToInt32(OracleDbHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM USERS WHERE EMAIL=:email",
                new[] { new OracleParameter("email", user.Email) }));

            if (count > 0) { ViewBag.Error = "Email already registered."; return View(); }

            OracleDbHelper.ExecuteNonQuery(
                @"INSERT INTO USERS(USERNAME,EMAIL,PASSWORD,ROLE,PHONE,ADDRESS,IS_ACTIVE,CREATED_AT)
                  VALUES(:u,:e,:p,'User',:ph,:a,1,SYSDATE)",
                new[] {
                    new OracleParameter("u",  user.Username),
                    new OracleParameter("e",  user.Email),
                    new OracleParameter("p",  user.Password),
                    new OracleParameter("ph", user.Phone ?? ""),
                    new OracleParameter("a",  user.Address ?? "")
                });
            TempData["Success"] = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            Session.Clear(); Session.Abandon();
            return RedirectToAction("Login");
        }
    }
}
