using Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        EcommerceEntities db = new EcommerceEntities();

        public ActionResult Index()
        {

            return View(db.Categories.ToList());
        }

        public ActionResult About()
        {
            return View();
        }


        public ActionResult Services()
        {
            return View();
        }

        public ActionResult Contact() { 
        
            return View();
        }

        public ActionResult Products()
        {

            return View(db.Products.ToList());

        }

        
      

        public ActionResult Profile()
        {
            var userInfo = db.UserInformations.Find((int)Session["UserID"]);
           
            return View(userInfo);
        }
        [HttpPost]
        public ActionResult Profile(UserInformation updateUserInfo, string currentPassword, string newPassword, string confirmNewPassword)
        {
            ViewBag.passwordError = " ";
            ViewBag.newPasswordError = " ";

            var userInfo = db.UserInformations.Find((int)Session["UserID"]);

            userInfo.first_name = updateUserInfo.first_name;
            userInfo.last_name = updateUserInfo.last_name;

            userInfo.email = updateUserInfo.email;
            userInfo.phoneNumber = updateUserInfo.phoneNumber;
            userInfo.alternativePhoneNumber = updateUserInfo.alternativePhoneNumber;

            userInfo.country = updateUserInfo.country;
            userInfo.city = updateUserInfo.city;
            
            userInfo.address = updateUserInfo.address;
            userInfo.postcode = updateUserInfo.postcode;

            userInfo.picture = updateUserInfo.picture;
            userInfo.otp = updateUserInfo.otp;


            if (!string.IsNullOrEmpty(currentPassword))
            {
                if (userInfo.password == currentPassword)
                {
                    ViewBag.passwordError = " ";

                    if (newPassword == confirmNewPassword)
                    {
                        userInfo.password = newPassword;
                        ViewBag.newPasswordError = " ";
                    }
                    else
                    {
                        ViewBag.newPasswordError = "Passwords don't match";
                    }
                }
                else
                {
                    ViewBag.passwordError = "Incorrect Password";
                }
            }
          

            db.Entry(userInfo).State = EntityState.Modified;
            db.SaveChanges();


            return RedirectToAction("Profile", "Home");
        }

    }
}