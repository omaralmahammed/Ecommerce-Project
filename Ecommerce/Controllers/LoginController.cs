using Ecommerce.Models;
using System;
using MimeKit;
using MailKit.Net.Smtp;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using static System.Net.WebRequestMethods;


namespace Ecommerce.Controllers
{
    public class LoginController : Controller
    {
        EcommerceEntities db = new EcommerceEntities();
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserInformation info, string confirmPassword)
        {

            if (confirmPassword != info.password)
            {
                @ViewBag.confirmError = "Password don't match";
            }
            else
            {
                @ViewBag.confirmError = "";
                db.UserInformations.Add(info);
                db.SaveChanges();
                return RedirectToAction("Login", "Login");

            }

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserInformation info)
        {

            var checkInputs = db.UserInformations.Where(model => model.email == info.email && model.password == info.password).FirstOrDefault();
            

            if (checkInputs == null)
            {
                ViewBag.loginError = true;
                return View();
            }
            else
            {
                Session["UserID"] = checkInputs.id;
                ViewBag.loginError = false;
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Logout()
        {
            Session["UserID"] = null;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            var checkInputs = db.UserInformations.FirstOrDefault(model => model.email == email);

            if (checkInputs != null)
            {
                Session["UserID"] = checkInputs.id;
                ViewBag.emailError = " ";
                ViewBag.alert = true;

                Random rand = new Random();
                string randomNumber = rand.Next(100000, 1000000).ToString();
                checkInputs.otp = randomNumber;
                db.Entry(checkInputs).State = EntityState.Modified;
                db.SaveChanges();

                try
                {
                    string fromEmail = "election2024jordan@gmail.com";
                    string fromName = "Support Team";
                    string subjectText = "Your OTP Code";
                    string messageText = $@"
                <html>
                <body dir='rtl'>
                    <h2>Hello {checkInputs.first_name}</h2>
                    <p><strong>Your OTP code is {randomNumber}. This code is valid for a short period of time.</strong></p>
                    <p>If you have any questions or need additional assistance, please feel free to contact our support team.</p>
                    <p>Best wishes,<br>Support Team</p>
                </body>
                </html>";

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(fromName, fromEmail));
                    message.To.Add(new MailboxAddress("", checkInputs.email));
                    message.Subject = subjectText;
                    message.Body = new TextPart("html") { Text = messageText };

                    using (var client = new MailKit.Net.Smtp.SmtpClient())
                    {
                        client.Connect("smtp.gmail.com", 465, true); 
                        client.Authenticate("election2024jordan@gmail.com", "zwht jwiz ivfr viyt");
                        client.Send(message);
                        client.Disconnect(true);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.emailError = "An error occurred while sending the email. Please try again later.";
                    return View();
                }

                return RedirectToAction("SetNewPassword", "Login");
            }
            else
            {
                ViewBag.emailError = "Invalid Email";
                ViewBag.alert = false;
                return View();
            }
        }

        public ActionResult SetNewPassword(string otp, string newPassword, string confirmNewPassword)
        {
            var user = db.UserInformations.Find((int)Session["UserID"]);

            if (newPassword == confirmNewPassword && user.otp == otp) {

                user.password = newPassword;

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Login", "Login");
            }
            else
            {
                ViewBag.error = "Passwords don't match!";
                return View();
            }

        }
    }
}