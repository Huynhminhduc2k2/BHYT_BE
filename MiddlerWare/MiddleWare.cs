using BHYT_BE.Internal.Services.UserService;
using System.Net;
using System.Security.Claims;

namespace BHYT_BE.MiddlerWare
{
    public class MiddleWare
    {
        private readonly RequestDelegate next;
        private readonly ILogger _logger;

        public MiddleWare(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke (HttpContext context, ILogger _logger)
        {

            /* Console.WriteLine("Go to Middleware 1");
             await next.Invoke ( context );
             Console.WriteLine("Retur to Middleware 1");*/

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }

        private Task _next(HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}
