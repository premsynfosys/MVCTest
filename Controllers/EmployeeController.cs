using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Test.Models;
using Test.Repository;

namespace Test.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        // GET: Employee

        //private IEmployeeRepository _repository;

        //public EmployeeController() : this(new EmployeeRepository()) { }

        //public EmployeeController(IEmployeeRepository repository)
        //{
        //    _repository = repository;
        //}


        readonly IEmployeeRepository _repository;
        public EmployeeController(IEmployeeRepository repository)
        {
            this._repository = repository;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login() => View();


        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Employee emp)
        {
            try
            {
                if (!string.IsNullOrEmpty(emp.UserName) && !string.IsNullOrEmpty(emp.Pwd))
                {

                    bool IsValidUser = _repository.Login(emp);
                    if (IsValidUser)
                    {
                        FormsAuthentication.SetAuthCookie(emp.UserName, false);
                        return RedirectToAction("EmpList", "Employee");
                    }
                }
                ModelState.AddModelError("", "invalid Username or Password");
                return View();
            }
            catch
            {
                ModelState.AddModelError("", "Error");
                return View();
            }
        }


        [HttpGet]
        public ViewResult EmpList()
        {

            IEnumerable<Employee> employees = _repository.GetAllEmployess();
            return View("EmpList", employees);
        }

        [HttpGet]
        public ActionResult EmpCreate()
        {
            return View("EmpCreate");
        }

        [HttpPost]
        public ActionResult EmpCreate([Bind(Exclude = "ID")] Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (employee.Img != null)
                    {
                        string path = Server.MapPath("~/images/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        employee.Img.SaveAs(path + Path.GetFileName(employee.Img.FileName));
                        employee.ImgName = Path.GetFileName(employee.Img.FileName);
                    }

                    #region mail
                    employee.Pwd = Membership.GeneratePassword(8, 3);
                    string msg = "Hi " + employee.Name + ".Your new temporary password is here.";
                    msg += "   Passsword:" + employee.Pwd;

                    ThreadStart mailThraed = delegate () { SendEmail(employee.Email, "Temporary Password", msg); };
                    Thread thread = new Thread(mailThraed);
                    thread.Start();

                    #endregion

                    bool res = _repository.AddEmmployee(employee);
                    if (res)
                        return RedirectToAction("EmpList");
                    else
                        return View("EmpCreate");
                }
                return View("EmpCreate");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View("EmpCreate");
            }

        }



        [HttpGet]
        public ActionResult EmpEdit(int id = 0)
        {
          
            Employee employee = _repository.GetAllEmployess().FirstOrDefault(emp => emp.ID == id);
            return View("EmpEdit", employee);
        }

        [HttpPost]
        public ActionResult EmpEdit(Employee employee, HttpPostedFileBase Img)
        {
            try
            {
                if (employee.Img != null)
                {
                    string path = Server.MapPath("~/images/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    employee.Img.SaveAs(path + Path.GetFileName(employee.Img.FileName));
                    employee.ImgName = Path.GetFileName(employee.Img.FileName);
                }

                bool res = _repository.UpdateEmmployee(employee);
                if (res)
                    return RedirectToAction("EmpList");
                else
                    return View(employee);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    ViewData["EditError"] = ex.InnerException.ToString();
                else
                    ViewData["EditError"] = ex.ToString();
            }
            //foreach (var modelState in ModelState.Values)
            //{
            //    foreach (var error in modelState.Errors)
            //    {
            //        if (error.Exception != null)
            //        {
            //            throw modelState.Errors[0].Exception;
            //        }
            //    }
            //}

            return View(employee);
        }


        public ActionResult EmpDelete(int id)
        {
            try
            {
                _repository.DeleteEmployee(id);
                return RedirectToAction("EmpList");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }


        [NonAction]
        public void SendEmail(string receiver, string subject, string message)
        {
            try
            {
                var senderEmail = new MailAddress("premkumardot123@gmail.com", "OfficeMail");
                var receiverEmail = new MailAddress(receiver, "Receiver");
                var password = "premkumar143";
                var sub = subject;
                var body = message;
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password)
                };
                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(mess);
                }


            }
            catch (Exception)
            {
                ViewBag.Error = "Some Error";
            }

        }

    }
}


//[HttpPost]
//public ActionResult Create(FormCollection formCollection)
//{
//    Employee employee = new Employee();
//    // Retrieve form data using form collection
//    employee.Name = formCollection["Name"];
//    employee.Gender = formCollection["Gender"];
//    employee.City = formCollection["City"];
//    employee.Salary = Convert.ToDecimal(formCollection["Salary"]);
//    employee.DateOfBirth = Convert.ToDateTime(formCollection["DateOfBirth"]);
//    EmployeeBusinessLayer employeeBusinessLayer = new EmployeeBusinessLayer();
//    employeeBusinessLayer.AddEmmployee(employee);
//    return RedirectToAction("EmpList");
//}
//[HttpPost]
//public ActionResult Create(Employee employee)
//{
//    if (ModelState.IsValid)
//    {
//        EmployeeBusinessLayer employeeBusinessLayer = new EmployeeBusinessLayer();
//        employeeBusinessLayer.AddEmmployee(employee);
//        return RedirectToAction("EmpList");
//    }
//    return View();
//}

