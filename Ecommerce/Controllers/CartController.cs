using Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.EnterpriseServices.CompensatingResourceManager;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ecommerce.Controllers
{
    public class CartController : Controller
    {
        EcommerceEntities db = new EcommerceEntities();
        public ActionResult AddToCart(int id)
        {
            var num = (int)Session["UserID"];
            var check = db.ShoppingCarts.Where(model => model.UserID == num).FirstOrDefault();

            if (check == null)
            {
                var userCard = new ShoppingCart
                {
                    UserID = num,
                    CreatedAt = DateTime.Now,
                };

                db.ShoppingCarts.Add(userCard);
                db.SaveChanges();
            }


            var checkProduct = db.ShoppingCartItems.FirstOrDefault(model => model.ProductID == id && model.CartID == check.CartID);

            if (checkProduct == null)
            {
                var check2 = db.ShoppingCarts.Where(model => model.UserID == num).FirstOrDefault();

                var userCardItems = new ShoppingCartItem
                {
                    CartID = check2.CartID,
                    ProductID = id,
                    Quantity = 1,
                    CreatedAt = DateTime.Now
                };
                db.ShoppingCartItems.Add(userCardItems);
                db.SaveChanges();

                return RedirectToAction("Products", "Home");
            }
            else
            {
                checkProduct.Quantity = checkProduct.Quantity + 1;
                db.Entry(checkProduct).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Products", "Home");
            }
        }


        public ActionResult Cart()
        {
            var userId = Session["UserId"] as int?;
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var shoppingCart = db.ShoppingCarts.FirstOrDefault(m => m.UserID == userId);
            if (shoppingCart == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var shoppingCartItems = db.ShoppingCartItems
                .Include(x => x.Product)
                .Where(m => m.CartID == shoppingCart.CartID)
                .ToList();

            var totalAmount = shoppingCartItems.Sum(item => item.Quantity * item.Product.price);

            ViewBag.TotalAmount = totalAmount;

            return View(shoppingCartItems);
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int id, string operation)
        {
            var item = db.ShoppingCartItems.Where(model => model.ProductID == id).FirstOrDefault();

            if (item != null)
            {
                if (operation == "increase")
                {
                    item.Quantity++;
                }
                else if (operation == "decrease" && item.Quantity > 1)
                {
                    item.Quantity--;
                }

                db.SaveChanges(); 
            }

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public ActionResult DeleteItem(int id)
        {
            var num = (int)Session["UserID"];
            var check = db.ShoppingCarts.Where(model => model.UserID == num).FirstOrDefault();
            var item = db.ShoppingCartItems.FirstOrDefault(model => model.ProductID == id && model.CartID == check.CartID);

            if (item != null)
            {
                db.ShoppingCartItems.Remove(item);
                db.SaveChanges();
            }

            return RedirectToAction("Cart");
        }
        }
 }