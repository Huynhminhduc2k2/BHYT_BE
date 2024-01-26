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
                SuccessUrl = domain + "/thankyou",
                CancelUrl = domain + "/subscription",
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
                SuccessUrl = domain + "/home",
                CancelUrl = domain + "/subscription",
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

        private async Task<string> GetProductNameAsync(string productId)
        {
            var productService = new ProductService();
            var product = await productService.GetAsync(productId);

            return product?.Name ?? "Unknown Product";
        }
        [HttpGet("GetSubscription")]
        public IActionResult GetSubscription(string email)
        {
            string CustomerId = "";
            var customerOptions = new CustomerListOptions
            {
                Email = email,
            };

            var customerService = new CustomerService();
            var customers = customerService.List(customerOptions);

            if (customers.Data.Count > 0)
            {
                var subscriptionDetailsList = new List<object>();

                foreach (var customer in customers.Data)
                {
                    var options = new SubscriptionListOptions
                    {
                        Customer = customer.Id,
                    };

                    var subscriptionService = new SubscriptionService();
                    var subscriptions = subscriptionService.List(options);

                    if (subscriptions.Data.Count > 0)
                    {
                        var subscriptionDetails = subscriptions.Data.Select(subscription => new
                        {
                            SubscriptionId = subscription.Id,

                            Status = subscription.Status,
                            Items = subscription.Items.Data.Select(async  item => new
                            {
                                PriceId = item.Price.Id,
                                SubscriptionName = await GetProductNameAsync(item.Price.ProductId)

                            }).ToList(),
                            CurrentPeriodStart = subscription.CurrentPeriodStart,
                            CurrentPeriodEnd = subscription.CurrentPeriodEnd
                        }).ToList();

                        subscriptionDetailsList.AddRange(subscriptionDetails);
                    }
                }

                if (subscriptionDetailsList.Count > 0)
                {
                    return Ok(subscriptionDetailsList);
                }
                else
                {
                    return NotFound("Customer(s) have no subscriptions.");
                }
            }
            else
            {
                return NotFound("Customer not found.");
            }
        

        }
      
        [HttpPost("CancelSubscription")]
        public IActionResult CancelSubscription(string subscriptionId)
        {
            try
            {
                var subscriptionService = new SubscriptionService();
                var canceledSubscription = subscriptionService.Cancel(subscriptionId);

                // Handle the canceled subscription as needed
                // For example, you might log the cancellation or perform additional actions

                return Ok("Subscription canceled successfully.");
            }
            catch (StripeException stripeException)
            {
                // Handle Stripe-specific exceptions
                return StatusCode((int)stripeException.HttpStatusCode, stripeException.Message);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, ex.Message);
            }
        }
    }
}
