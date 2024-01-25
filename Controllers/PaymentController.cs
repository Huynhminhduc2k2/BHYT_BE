using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Services.UserService;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;

namespace BHYT_BE.Controllers
{
    [Route("v2/api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        [HttpPost("Basic")]
        public IActionResult CreateCheckoutSession()
        {
            var domain = "http://localhost:3000";
            var option = new SessionCreateOptions()
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>()
                {
                    new SessionLineItemOptions()
                    {
                        Price = "price_1OQ7ZAGzNiwrigilxUUykPTF",
                        Quantity = 1,
                    },
                },
                Mode = "subscription",
                SuccessUrl = domain + "/subscription.html",
                CancelUrl = domain + "/subscription.html",
            };
            var service = new SessionService();
            Session session = service.Create(option);
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        [HttpPost("Advance")]
        public IActionResult CreateCheckoutSession1()
        {
            var domain = "http://localhost:3000";
            var option = new SessionCreateOptions()
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>()
                {
                    new SessionLineItemOptions()
                    {
                        Price = "price_1OQ7ZVGzNiwrigild9J56zT1",
                        Quantity = 1,
                    },
                },
                Mode = "subscription",
                SuccessUrl = domain + "/subscription.html",
                CancelUrl = domain + "/subscription.html",
            };
            var service = new SessionService();
            Session session = service.Create(option);
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        [HttpPost("Premium")]
        public IActionResult CreateCheckoutSession2()
        {
            var domain = "http://localhost:3000";
            var option = new SessionCreateOptions()
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>()
                {
                    new SessionLineItemOptions()
                    {
                        Price = "price_1OQMhgGzNiwrigilrkzJH1VY",
                        Quantity = 1,
                    },
                },
                Mode = "subscription",
                SuccessUrl = domain + "/subscription.html",
                CancelUrl = domain + "/subscription.html",
            };

            var service = new SessionService();
            Session session = service.Create(option);
         
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        [HttpGet("GetCustomerId")]
        public IActionResult GetCustomerId(string email)
        {
            try
            {
                var options = new CustomerListOptions
                {
                    Email = email,
                    Limit = 1,
                };
                var service = new CustomerService();
                var customers = service.List(options);
                if (customers.Data.Count > 0)
                {
                    return Ok(new { CustomerId = customers.Data[0].Id });
                }
                return NotFound();

            }
            catch (StripeException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("GetSubscription")]
        public IActionResult GetSubscription(string email)
        {
            string CustomerId = "";
            var Customeroptions = new CustomerListOptions
            {
                Email = email,
                Limit = 1,
            };
            var service = new CustomerService();
            var customers = service.List(Customeroptions);
            if (customers.Data.Count > 0)
            {
                CustomerId = customers.Data[0].Id ;
            }
            
            var options = new SubscriptionListOptions
            {
                Customer = CustomerId,
                Status = "all" 
            };

            var subscriptionService = new SubscriptionService();
            var subscriptions = subscriptionService.List(options);

           
            if (subscriptions.Data.Count > 0)
            {
               
                var subscriptionDetails = subscriptions.Data.Select(subscription => new
                {
                    SubscriptionId = subscription.Id,
                    Status = subscription.Status,
                    PriceId = subscription.Items.Data[0].Price.Id,
                    CurrentPeriodStart = subscription.CurrentPeriodStart,
                    CurrentPeriodEnd = subscription.CurrentPeriodEnd
                });

                return Ok(subscriptionDetails);
            }
            else
            {
                return NotFound("Customer has no subscriptions.");
            }
        }
    }
   
}
