using ClassifiedProj.Models;
using DAL;
using Models.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ClassifiedProj.Controllers
{
    public class ProductController : Controller
    {
        private DbManager dbManager;
        public ProductController()
        {
            dbManager = new DbManager();
        }

        // GET: Product
        public ActionResult Index()
        {
            var products = dbManager.Product.Get().ToList();
            var tempList = new List<Product>();

            foreach (var item in products)
            {
                //if (HttpContext.Session["cart"] != null)
                //{

                //    List<Product> productsSession = (List<Product>)HttpContext.Session["cart"];
                //    List<Product> productsaApplication = (List<Product>)HttpContext.Application["cart"];
                //    var dbItem = productsSession.SingleOrDefault(dbpord => dbpord.Id == item.Id);

                //    if (dbItem == null && !item.isSold)
                //    {
                //        //item.isAvailable = true;
                //        tempList.Add(item);
                //    }
                //}


                if (HttpContext.Application["cart"] != null)
                {
                    List<Product> productsaApplication = (List<Product>)HttpContext.Application["cart"];
                    var dbItem = productsaApplication.SingleOrDefault(dbpord => dbpord.Id == item.Id);

                    if (dbItem == null && !item.isSold)
                    {
                        item.isAvailable = false;
                        tempList.Add(item);
                    }
                }

                else if (!item.isSold)
                {
                    tempList.Add(item);
                }

            }
            return View(tempList);
        }

        public ActionResult SoldOut()
        {
            return View();
        }

        public ActionResult AllTrue()
        {
            var products = dbManager.Product.Get().ToList();

            foreach (var item in products)
            {
                item.isSold = false;
                item.isAvailable = true;
            }
            dbManager.Save();
            return RedirectToAction("Index", "Product");
        }


        [Authorize]
        [HttpGet]
        public ActionResult AddProduct()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddProduct(Product product, HttpPostedFileBase image1, HttpPostedFileBase image2, HttpPostedFileBase image3)
        {
            var logedInUser = HttpContext.User.Identity.Name;
            var currentUser = dbManager.User.Get().SingleOrDefault(user => user.UserName == logedInUser);
            product.OwnerId = currentUser.Id;
            product.Date = DateTime.Now;
            product.isAvailable = true;
            product.isSold = false;

            if (!ModelState.IsValid)
                return View();

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    if (image1 != null)
                    {
                        image1.InputStream.CopyTo(ms);
                        product.Picture1 = ms.GetBuffer();
                    }

                }

                using (MemoryStream ms = new MemoryStream())
                {
                    if (image2 != null)
                    {
                        image2.InputStream.CopyTo(ms);
                        product.Picture2 = ms.GetBuffer();
                    }
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    if (image3 != null)
                    {
                        image3.InputStream.CopyTo(ms);
                        product.Picture3 = ms.GetBuffer();
                    }
                }

                dbManager.Product.Insert(product);
                dbManager.Save();
                return RedirectToAction("Index");


            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public ActionResult ShowAll()
        {
            var products = dbManager.Product.Get().ToList();
            return View(products);
        }

        [HttpPost]
        public ActionResult Index(string orderBy)
        {
            var products = dbManager.Product.Get().ToList();

            if (orderBy == "Name")
            {

                var byName = products.OrderBy(prod => prod.Title).Where(pro => pro.isAvailable && !pro.isSold);

                return View(ListSesstion(byName.ToList()));
            }

            if (orderBy == "Date-nw")
            {
                var byDate = products.OrderBy(prod => prod.Date).OrderByDescending(x => x.Date).Where(pro => pro.isAvailable && !pro.isSold);
                return View(ListSesstion(byDate.ToList()));
            }

            if (orderBy == "Date-od")
            {
                var byDate = products.OrderBy(prod => prod.Date).Where(pro => pro.isAvailable && !pro.isSold);
                return View(ListSesstion(byDate.ToList()));
            }

            return View();
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
                return RedirectToAction("Error", "Home");

            var product = dbManager.Product.GetByID(id);

            if (product == null)
                return RedirectToAction("Error", "Home");

            if (product.isSold == true)
                return RedirectToAction("SoldOut", "Product");



            return View(product);

        }

        [HttpPost]
        public ActionResult Search(string title)
        {
            var products = dbManager.Product.Get();
            var tempList = new List<Product>();

            foreach (var item in products)
            {
                if (!item.isSold)
                {
                    if (item.Title.ToLower().Contains(title.ToLower()))
                        tempList.Add(item);
                }
            }

            if (tempList != null)
                return View("Index", ListSesstion(tempList));

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var logedInUser = HttpContext.User.Identity.Name;
            var currentUser = dbManager.User.Get().SingleOrDefault(user => user.UserName == logedInUser);

            Product product = dbManager.Product.GetByID(id);
            if (product == null)
                return HttpNotFound();

            if (product.isSold)
                return RedirectToAction("Index");

            if (currentUser.Role == Role.Admin)
                return View(product);

            if (product.OwnerId != currentUser.Id)
                return RedirectToAction("Index");


            return View(product);
        }


        [Authorize]
        [HttpPost]
        public ActionResult Edit(Product product, HttpPostedFileBase image1, HttpPostedFileBase image2, HttpPostedFileBase image3)
        {
            try
            {
                var logedInUser = HttpContext.User.Identity.Name;
                var currentUser = dbManager.User.Get().SingleOrDefault(user => user.UserName == logedInUser);

                if (product.OwnerId != currentUser.Id)
                    return RedirectToAction("Index");

                if (currentUser.Role == Role.Admin)
                    return View(product);

                var dbProd = dbManager.Product.GetByID(product.Id);
                if (dbProd == null)
                    return RedirectToAction("Index");

                if (product.isSold)
                    return RedirectToAction("Index");

                if (!ModelState.IsValid)
                    return View();

                dbProd.Title = product.Title;
                dbProd.ShortDescription = product.ShortDescription;
                dbProd.LongDescription = product.LongDescription;
                dbProd.Price = product.Price;

                using (MemoryStream ms = new MemoryStream())
                {
                    if (image1 != null)
                    {
                        image1.InputStream.CopyTo(ms);
                        product.Picture1 = ms.GetBuffer();
                        dbProd.Picture1 = product.Picture1;
                    }

                }

                using (MemoryStream ms = new MemoryStream())
                {
                    if (image2 != null)
                    {
                        image1.InputStream.CopyTo(ms);
                        product.Picture2 = ms.GetBuffer();
                        dbProd.Picture2 = product.Picture2;
                    }
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    if (image3 != null)
                    {
                        image3.InputStream.CopyTo(ms);
                        product.Picture3 = ms.GetBuffer();
                        dbProd.Picture1 = product.Picture3;
                    }
                }

                dbManager.Save();
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }
        }


        [Authorize]
        [HttpGet]
        public ActionResult AllLisOfAdstInTheWeb()
        {
            var logedInUser = HttpContext.User.Identity.Name;
            var currentUser = dbManager.User.Get().SingleOrDefault(user => user.UserName == logedInUser);
            if (currentUser.Role == Role.Admin)
            {
                var allList = dbManager.Product.Get().ToList();
                return View("MyList", allList);
            }
            return View("Index", "Home");
        }


        [Authorize]
        [HttpGet]
        public ActionResult MyList()
        {
            var logedInUser = HttpContext.User.Identity.Name;
            var currentUser = dbManager.User.Get().SingleOrDefault(user => user.UserName == logedInUser);

            var list = dbManager.Product.Get(prod => prod.OwnerId == currentUser.Id);

            return View(list);
        }

        public List<Product> ListSesstion(List<Product> products)
        {
            var tempList = new List<Product>();

            foreach (var item in products)
            {
                if (HttpContext.Session["cart"] != null)
                {
                    List<Product> productsSession = (List<Product>)HttpContext.Session["cart"];
                    var dbItem = productsSession.SingleOrDefault(dbpord => dbpord.Id == item.Id);

                    if (dbItem == null && !item.isSold)
                    {
                        item.isAvailable = true;
                        tempList.Add(item);
                    }
                }

                else if (!item.isSold)
                {
                    tempList.Add(item);
                }

            }
            return tempList;
        }

    }
}