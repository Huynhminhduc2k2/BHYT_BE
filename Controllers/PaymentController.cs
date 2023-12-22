using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Services.UserService;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
namespace BHYT_BE.Controllers
{
    [Route("v2/api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        [HttpPost]
        public ActionResult CreateCheckoutSession()
        {
            var domain = "http://localhost:3000";
            var option= new SessionCreateOptions()
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
                SuccessUrl = domain + "/payment.html",
                CancelUrl = domain + "/payment.html",
            };  
            var service = new SessionService();
            Session session = service.Create(option);
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
    }
}
