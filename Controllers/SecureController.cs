using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
[Authorize] // Đánh dấu controller cần xác thực
namespace BHYT_BE.Controllers
{
    public class SecureController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetSecureData() {
            return Ok(new { Message = " This is secure data " });        
        }
    }
}
