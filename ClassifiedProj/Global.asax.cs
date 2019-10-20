using DAL;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace ClassifiedProj
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            DbManager dbManager = new DbManager();
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            if (Context.Session == null)
            {
               var products = dbManager.Product.Get().ToList();
                if (products != null)
                {
                    foreach (var item in products)
                    {
                        if (!item.isSold)
                        {
                            item.isAvailable = true;
                        }
                        dbManager.Save();
                    }

                }
            }
        }
    }
}
