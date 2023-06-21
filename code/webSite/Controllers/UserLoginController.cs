using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security;
using System.Net;
using System.Net.Mail;
using System.Text;
namespace web.Controllers
{
    public class UserLoginController : baseController
    {
        // GET: UserLogin
        //private Sys_userService sys_userService = new Sys_userService();
        // GET: Sys_user
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            Session["username"] = username;
            if ((username != null) && (username != ""))
            {
                Session["isLogin"] = true;             
                db.bks_Customer entry = db.bll.bks_Customer.getEntryByName(username, dc);
                if (!rui.typeHelper.isNullOrEmpty(entry))
                {
                    if (entry.password == password)
                    {        
                            return RedirectToAction("Index","Store");
                    }
                    else
                    {
                        Session["pwd"] = 1;
                    }
                }
                else
                {
                    Session["user"] = 1;
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            //db.bks_Customer model = new db.bks_Customer();
            return View();
        }

        //保存新增
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Register(string username, string password, string emailaddress)
        {
            password = password.Trim();
            try
            {
                if (ModelState.IsValid)
                {
                    if (db.bll.bks_Customer.getEntryByName(username))
                    {
                        if (!db.bll.bks_Customer.IsValidEmail(emailaddress))
                        {
                            string rowID = db.bll.bks_Customer.insert(username, password, emailaddress, dc);
                            return RedirectToAction("Login");
                        }
                        else
                        {
                            return Content("Fail: Email is exist");
                        }
                    }
                    else
                    {
                        return Content("用户已存在");
                    }

                }
                else
                {
                    return Content("输入不合法");
                }
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
            }
            return View();
        }
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            return View();
        }
        [HttpGet]
        public ActionResult ResetPassword()
        {

            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(string newpassword)
        {
            db.bll.bks_Customer.UpdateByEmail(Session["email"].ToString(), newpassword, dc);
            return new RedirectResult("Login");
        }

        [HttpPost]
        public ActionResult SendCode(FormCollection form)
        {
            // 获取表单数据
            string email = form["email"];

            Session["email"] = email;
            // Generate a random verification code
            Random random = new Random();
            string code = random.Next(100000, 999999).ToString();

            // Save the verification code to the session
            Session["VerificationCode"] = code;
            string smtpServer = "smtp.qq.com";
            string mailForm = "1256244128@qq.com";
            string userPassword = "hipzvnfsvowafhij";
            // Send an email containing the verification code

            SmtpClient smtp = new SmtpClient();
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Host = smtpServer;
            smtp.Port = 587;
            smtp.EnableSsl = true;
            //smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential(mailForm, userPassword);

            MailMessage message = new MailMessage(mailForm, email);
            message.To.Add(email);
            message.Subject = "Verification Code";
            message.Body = "Your verification code is: " + code;
            message.BodyEncoding = Encoding.UTF8;
            //message.IsBodyHtml = true;
            message.Priority = MailPriority.High;
            if (db.bll.bks_Customer.IsValidEmail(email))
            {
                smtp.Send(message);
                return Content("Success");
            }
            else
            {
                return Content("Fail");
            }



        }

        [HttpPost]
        public ActionResult VerifyCode(FormCollection form)
        {
            string verc = form["verifyc"];
            // Get the verification code from the session
            string verificationCode = Session["VerificationCode"] as string;

            // Check if the verification code matches the user input
            if (verificationCode == verc)
            {
                // Return a success message
                return View("ResetPassword", "UserLogin");
            }
            else
            {
                // Return an error message
                return Content("验证码错误");
            }
        }

    }
}