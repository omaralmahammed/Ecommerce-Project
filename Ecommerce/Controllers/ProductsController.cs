using Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ecommerce.Controllers
{
    public class ProductsController : Controller
    {
        EcommerceEntities db = new EcommerceEntities();
        public ActionResult Shop(int? id)
        {
            
           var products = db.Products.Where(model => model.categoryID == id).ToList();

            return View(products);
        }

        public ActionResult Details(int? id)
        {

            var productDetails = db.Products.Where(model => model.ID == id).FirstOrDefault();

            return View(productDetails);
        }
    }
}