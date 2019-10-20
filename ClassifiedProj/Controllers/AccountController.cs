using ClassifiedProj.Models;
using DAL;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ClassifiedProj.Controllers
{
    public class AccountController : Controller
    {
        private static string _error;

        private DbManager dbManager;

        public AccountController()
        {
            dbManager = new DbManager();
        }

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [Route("Login")]
        [HttpGet]
        public PartialViewResult Login()
        {
            return PartialView();
        }

        [HttpPost]
        [Route("Login")]
        public ActionResult Login(User user)
        {

            var userExist = dbManager.User.Get().SingleOrDefault(checkUser => checkUser.UserName.ToLower() == user.UserName.ToLower() && checkUser.Password == user.Password);


            if (userExist == null)
            {
                _error = "User or Password is invalied";
                ViewBag.Sucsess = _error;
                return RedirectToAction("Error", "Account");
            }

            FormsAuthentication.SetAuthCookie(userExist.UserName, true);

            return RedirectToAction("Index", "Product");
        }

        [HttpGet]
        public ActionResult Error()
        {
            ViewBag.Sucsess = _error;
            return View();
        }

        [HttpGet]
        [Authorize]
        [Route("EditAllUsers")]
        public ActionResult EditAllUsers()
        {
            var logedInUser = HttpContext.User.Identity.Name;
            var user = dbManager.User.Get().FirstOrDefault(userDb => userDb.UserName == logedInUser);
            if (user.Role == Role.Admin)
            {
                var allUsers = dbManager.User.Get().ToList();
                return View(allUsers);
            }
            return RedirectToAction("Index", "Product");
        }

        [HttpGet]
        [Route("Register")]
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Product");

            return View();
        }

        [HttpPost]
        [Route("Register")]
        public ActionResult Register(User user)
        {
            if (!ModelState.IsValid)
                return View();


            var newUser = dbManager.User.Get().SingleOrDefault(dbUser => dbUser.UserName.ToLower() == user.UserName.ToLower());

            if (newUser != null)
            {
                ViewBag.Sucsess = "Try anther user name";
                return View();
            }
            user.UserRoleSet(Role.User);
            dbManager.User.Insert(user);
            dbManager.Save();
            FormsAuthentication.SetAuthCookie(user.UserName, true);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        [Route("Edit")]
        public ActionResult Edit(string userName)
        {
            if (userName != null)
            {
                var logedInUser = HttpContext.User.Identity.Name;
                var user = dbManager.User.Get().FirstOrDefault(userDb => userDb.UserName == logedInUser);
                if (user.Role == Role.Admin)
                {
                    var editUser = dbManager.User.Get().ToList().SingleOrDefault(dbUser => dbUser.UserName == userName);
                    if (editUser == null)
                        return RedirectToAction("Error", "Home");

                    return View(editUser);
                }

                if (logedInUser == user.UserName)
                    return View(user);

                return RedirectToAction("Index", "Product");
            }
            return RedirectToAction("Index", "Product");
        }

        [Authorize]
        [HttpPost]
        [Route("Edit")]
        public ActionResult Edit(User user)
        {
            if (!ModelState.IsValid)
                return View();

            var logedInUser = HttpContext.User.Identity.Name;
            var userDb = dbManager.User.GetByID(user.Id);
            if (logedInUser == userDb.UserName || user.Role == Role.Admin)
            {
                userDb.UserName = user.UserName;
                userDb.FirstName = user.FirstName;
                userDb.LastName = user.LastName;
                userDb.Password = userDb.Password;
                userDb.VerifiedPassword = user.VerifiedPassword;
                userDb.Email = user.Email;
                userDb.BirthDate = user.BirthDate;

                dbManager.User.Update(userDb);
                dbManager.Save();
                return RedirectToAction("Index", "Product");
            }
            return RedirectToAction("Index", "Product");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            var logedInUser = HttpContext.User.Identity.Name;
            var user = dbManager.User.Get().FirstOrDefault(userDb => userDb.UserName == logedInUser);

            if(id == null)
                return RedirectToAction("Error", "Home");

            if (user.Role == Role.User)
                return RedirectToAction("Index", "Product");

            if (user.Role == Role.Admin)
            {
                var dbUser = dbManager.User.GetByID(id);
                if(dbUser == null || dbUser.Id == 1)
                    return RedirectToAction("Error", "Home");

                dbManager.User.Delete(dbUser);
                dbManager.Save();
                return RedirectToAction("EditAllUsers", "Account");
            }
            return RedirectToAction("Index", "Product");
        }

        public PartialViewResult GetUser()
        {
            var logedInUser = HttpContext.User.Identity.Name;
            var user = dbManager.User.Get().FirstOrDefault(userDb => userDb.UserName == logedInUser);
            return PartialView("~/Views/Shared/_LogedIn.cshtml", user);
        }

        [HttpGet]
        [Route("Signout")]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Product");
        }
    }
}