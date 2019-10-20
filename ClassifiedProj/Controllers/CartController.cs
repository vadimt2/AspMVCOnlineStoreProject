using ClassifiedProj.Models;
using DAL;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace ClassifiedProj.Controllers
{
    public class CartController : Controller
    {

        private DbManager dbManager;

        public CartController()
        {
            dbManager = new DbManager();
            //CheckIfInCart();
        }
        // GET: Cart

        public ActionResult Index()
        {
            List<Product> products = (List<Product>)HttpContext.Session["cart"];
            if (products == null)
                return View("Cart");
            else
            {
                return View("Cart", products);
            }

        }
  
        public ActionResult AddItem(int? id)
        {
            
            if(id == null)
                RedirectToAction("Error", "Home");

            if (HttpContext.Session["cart"] == null)
            {
                
                List<Product> cart = new List<Product>();
                List<Product> tempList = new List<Product>();
                var item = dbManager.Product.GetByID(id);

                if (item == null)
                   return RedirectToAction("Error", "Home");

                var user = dbManager.User.Get().SingleOrDefault(userDb => userDb.UserName.ToLower() == User.Identity.Name.ToLower());

                if (user != null)
                {
                    if (item.OwnerId == user.Id)
                        return RedirectToAction("OwnItem", "Cart");
                }
               
                cart.Add(item);
                item.isAvailable = false;
                tempList.Add(item);
                ViewData["temp"] = tempList;
                HttpContext.Session["cart"] = cart;
                HttpContext.Application["cart"] = cart;
                foreach (Product product in (List<Product>)HttpContext.Application["cart"])
                {
                    product.isAvailable = false;
                }
            }
            else
            {
                List<Product> cart = (List<Product>)HttpContext.Session["cart"];
                var item = dbManager.Product.GetByID(id);        

                if (item == null)
                   return RedirectToAction("Error", "Home");

                var user = dbManager.User.Get().SingleOrDefault(userDb => userDb.UserName.ToLower() == User.Identity.Name.ToLower());

                if (user != null)
                {
                    if (item.OwnerId == user.Id)
                        return RedirectToAction("Index", "Home");
                }         

                var checkItemInCart = cart.SingleOrDefault(itemInList => itemInList.Id == id);
                if (checkItemInCart != null)
                    return RedirectToAction("Index", "Home");

                item.isAvailable = false;
                cart.Add(item);
                HttpContext.Session["cart"] = cart;
                HttpContext.Application["cart"] = cart;
            }
            return View("Cart");
        }

        public ActionResult RemoveItem(int id)
        {
            List<Product> cart = (List<Product>)HttpContext.Session["cart"];
            List<Product> cartApplication = (List<Product>)HttpContext.Application["cart"];
            var removeItem = cart.SingleOrDefault(item => item.Id == id);
            var removeItemApplication = cart.SingleOrDefault(item => item.Id == id);
            removeItemApplication.isAvailable = true;
            cart.Remove(removeItem);
            cartApplication.Remove(removeItemApplication);
                return View("Cart");
        }

        [HttpPost]
        public ActionResult SoldItem()
        {
            if (HttpContext.Session["cart"] != null)
            {
                List<Product> products = (List<Product>)HttpContext.Session["cart"];
                foreach (var item in products)
                {
                    var itemDb = dbManager.Product.GetByID(item.Id);
                    if (itemDb.isSold)
                        return RedirectToAction("Index", "Cart");

                    itemDb.isAvailable = false;
                    itemDb.isSold = true;
                    dbManager.Save();
                    products = null;
                    HttpContext.Session["cart"] = products;
                    //HttpContext.Application["cart"] = products;
                    return RedirectToAction("ThankYou","Home");
                }
            }

            ViewBag.empty = "You add item to cart";
            return RedirectToAction("Index","Cart");
        }

        public ActionResult OwnItem()
        {
            return View();
        }
    }
}