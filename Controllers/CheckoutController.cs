using Microsoft.AspNetCore.Mvc;
using StripeDemo.Models;
using Stripe.Checkout;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using SessionService = Stripe.Checkout.SessionService;
using Session = Stripe.Checkout.Session;

namespace StripeDemo.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            List<ProductEntity> products = new List<ProductEntity>();

            products = new List<ProductEntity>
            {
                new ProductEntity
                {
                    Id = 1,
                    Product = "Waterproof Military",
                    Rate = 1500,
                    Quantity = 2,
                    ImagePath = "Img/Waterproof Military.jpg"
                },
                 new ProductEntity
                {
                    Id = 2,
                    Product = "Watch Timer",
                    Rate = 1000,
                    Quantity = 1,
                    ImagePath = "Img/Watch 3.jpg"
                },
                 new ProductEntity
                {
                    Id = 3,
                    Product = "Watch Timer 2",
                    Rate = 1000,
                    Quantity = 2,
                    ImagePath = "Img/Watch 2.jpg"
                }
            };

            return View(products);
        }

        public IActionResult OrderConfirmation()
        {
            var service = new SessionService();
            Session session = service.Get(TempData["Session"].ToString());

            if (session.PaymentStatus == "paid")
            {
                var transaction = session.PaymentIntentId.ToString();
                return View("Success");
            }
            return View("Login");
        }
        public IActionResult Success()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult CheckOut()
        {
            List<ProductEntity> products = new List<ProductEntity>();

            products = new List<ProductEntity>
            {
                 new ProductEntity
                {
                    Product = "Watch Timer",
                    Rate = 1000,
                    Quantity = 1,
                    ImagePath = "Img/Watch 3.jpg"
                },
                 new ProductEntity
                {
                    Product = "Watch Timer 2",
                    Rate = 1000,
                    Quantity = 2,
                    ImagePath = "Img/Watch 2.jpg"
                },
                 new ProductEntity
                {
                    Product = "Waterproof Military",
                    Rate = 1500,
                    Quantity = 2,
                    ImagePath = "Img/Waterproof Military.jpg"
                },
            };

            var domain = "https://localhost:7262/";

            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"CheckOut/Success",
                CancelUrl = domain + $"CheckOut/Login",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = "quocanhit58@gmail.com"
            };

            foreach (var item in products)
            {
                var sessionListItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Rate * item.Quantity),
                        Currency = "vnd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.ToString()
                        }
                    },
                    Quantity = item.Quantity
                };
                options.LineItems.Add(sessionListItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);

            TempData["Session"] = session.Id;

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
    }
}
