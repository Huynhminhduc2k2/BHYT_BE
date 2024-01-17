using AspNetCoreHero.ToastNotification.Abstractions;
using IS220_PROJECT.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IS220_PROJECT.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly dbFrameContext _context;
        private readonly INotyfService _notyfService;

        public AccountController(dbFrameContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        public string ValidateEmail(string email)
        {
            try
            {
                var customer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.ToLower().Equals(email.ToLower()));
                if(customer != null)
                {
                    return "Email đã tồn tại!";
                }
                return "True";
            }
            catch(Exception e)
            {
                return "Có lỗi xảy ra";
            }
        }
        public string ValidatePhoneNumber(string phone)
        {
            try
            {
                var customer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Phone.ToLower().Equals(phone.ToLower()));
                if (customer != null)
                {
                    return "Số điện thoại đã tồn tại!";
                }
                return "True";
            }
            catch (Exception e)
            {
                return "Có lỗi xảy ra";
            }
        }

        public IActionResult Dashboard()
        {
            var accountId = HttpContext.Session.GetString("AccountId");
            if (accountId != null)
            {
                var customer = _context.Customers.AsNoTracking().Include(p => p.Account).FirstOrDefault(p => p.AccountId == int.Parse(accountId));
                if (customer != null)
                    return View(customer);
            }
            //HttpContext.Session.Remove("AccoutId");
            return RedirectToAction("Logout","Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Dashboard(Customer customer, [FromForm(Name = "fileAva")] IFormFile fileAva)
        {
            try
            {
                var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { x.Key, x.Value.Errors })
                        .ToArray();
                if (ModelState.IsValid)
                {
                    customer.FullName = Utils.Utils.ToTitleCase(customer.FullName);
                    if (fileAva != null)
                    {
                        string extension = Path.GetExtension(fileAva.FileName);
                        string img = Utils.Utils.formatVNString(customer.FullName) + extension;
                        customer.Avatar = await Utils.Utils.UploadFile(fileAva, @"customers", img.ToLower());
                    }
                    if (string.IsNullOrEmpty(customer.Avatar))
                        customer.Avatar = "default.png";
                    customer.ModifiedDate = DateTime.Now;
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Cập nhật thành công");
                }
            }   catch(Exception e)
            {
                return View(customer);
            }
            return View(customer);
        }

        //Get: /login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();
                if (ModelState.IsValid)
                {
                    Account customer = _context.Accounts.AsNoTracking().Include(p => p.Customers).FirstOrDefault(p => p.AccountName == (string)model.UserName);
                    if (customer == null)
                    {
                        return RedirectToAction("Register", "Account");
                    }

                    if (model.Password.ToString() != customer.Password.ToString())
                    {
                        _notyfService.Error("Sai thông tin đăng nhập");
                        return View(model);
                    }
                    customer.LastLogin = DateTime.Now;
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString("AccountId", customer.AccountId.ToString());
                    var userNameId = HttpContext.Session.GetString("AccountId");
                    var claim = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, customer.AccountName),
                        new Claim("AccountId", customer.AccountId.ToString())
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claim, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Register", "Account");
            }
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register([Bind("AccountId, FullName, Email, PhoneNumber, UserName, Password, ConfirmPassword")]RegisterViewModel newCustomer)
        {
            try
            {
                var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();
                if (ModelState.IsValid)
                {
                    var _account = _context.Accounts.AsNoTracking().FirstOrDefault(p => p.AccountName == newCustomer.UserName);
                    if(_account != null)
                    {
                        _notyfService.Error("Tên đăng nhập đã tồn tại");
                        return View(newCustomer);
                    }

                    if(ValidateEmail(newCustomer.Email).IndexOf("True") == -1)
                    {
                        _notyfService.Error("Email đã tồn tại");
                        return View(newCustomer);
                    }

                    if (ValidatePhoneNumber(newCustomer.PhoneNumber).IndexOf("True") == -1)
                    {
                        _notyfService.Error("Số điện thoại đã tồn tại");
                        return View(newCustomer);
                    }
                    //var _customer = _context.Customers.AsNoTracking().FirstOrDefault(p => p.Phone == newCustomer.PhoneNumber);
                    //if (_customer != null)
                    //{
                    //    _notyfService.Error("Số điện thoại đã được sử dụng");
                    //    return View(newCustomer);
                    //}
                    Account newAccount = new Account
                    {
                        AccountName = newCustomer.UserName,
                        Password = newCustomer.Password,
                        RoleId = 2,
                        Active = true,
                        CreateDate = DateTime.Now,
                        LastLogin = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };
                    try
                    {
                        _context.Accounts.Add(newAccount);
                        await _context.SaveChangesAsync();
                        var account = _context.Accounts.FirstOrDefault(p => p.AccountName == newAccount.AccountName);
                        Customer _newCustomer = new Customer
                        {
                              FullName = newCustomer.FullName,
                              Email = newCustomer.Email,
                              Phone = newCustomer.PhoneNumber,
                              AccountId = account.AccountId,
                              CreateDate = DateTime.Now,
                              ModifiedDate = DateTime.Now,
                              Active = true
                        };
                        _context.Customers.Add(_newCustomer);
                        await _context.SaveChangesAsync();
                        HttpContext.Session.SetString("AccountId", account.AccountId.ToString());
                        var userNameId = HttpContext.Session.GetString("AccountId");
                        var claim = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, newCustomer.FullName),
                            new Claim("AccountId", account.AccountId.ToString())
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claim, "login");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);
                        return RedirectToAction("Dashboard", "Account");
                    }
                    catch (Exception e)
                    {
                        return RedirectToAction("Register", "Account");
                    }
                }
                else
                {
                    return View(newCustomer);
                }
            }
            catch(Exception e)
            {
                return View(newCustomer);
            }
            return View(newCustomer);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("AccoutId");
            return RedirectToAction("Index", "Home");
        }

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePassword)
		{
			try
			{
                var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();
                if (ModelState.IsValid)
				{
                    if(changePassword.OldPassword == null || changePassword.NewPassword == null || changePassword.ConfirmNewPassword == null)
					{
                        _notyfService.Error("Không được để trống ô nào!");
                        return RedirectToAction("Dashboard", "Account");
					}

                    if(changePassword.NewPassword != changePassword.ConfirmNewPassword)
					{
                        _notyfService.Error("Mật khẩu phải khớp nhau!");
                        return RedirectToAction("Dashboard", "Account");
                    }
                    var account = _context.Accounts.FirstOrDefault(a => a.AccountId == changePassword.AccountId);
                    if(account.Password != changePassword.OldPassword)
					{
                        _notyfService.Error("Mật khẩu cũ chưa chính xác!");
                        return RedirectToAction("Dashboard", "Account");
                    }

                    account.Password = changePassword.NewPassword;
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Đổi mật khẩu thành công!");
                    return RedirectToAction("Dashboard", "Account");
                }
			}
			catch (Exception)
			{

				throw;
			}
            _notyfService.Error("Đã có lỗi xảy ra");
            return RedirectToAction("Dashboard", "Account");
        }
    }
}
