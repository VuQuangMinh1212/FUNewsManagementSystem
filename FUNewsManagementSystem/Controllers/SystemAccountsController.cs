using FUNewsManagementSystem.BLL.Interfaces;
using FUNewsManagementSystem.DAL.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FUNewsManagementSystem.Controllers
{
    public class SystemAccountsController : Controller
    {
        private readonly ISystemAccountService _accountService;
        private readonly IConfiguration _configuration;

        public SystemAccountsController(ISystemAccountService accountService, IConfiguration configuration)
        {
            _accountService = accountService;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(string searchQuery)
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                accounts = accounts.Where(a => a.AccountName.Contains(searchQuery));
            }
            ViewData["CurrentFilter"] = searchQuery;
            return View(accounts);
        }

        public async Task<IActionResult> Details(short? id)
        {
            if (id == null) return NotFound();
            var account = await _accountService.GetAccountByIdAsync(id.Value);
            return account == null ? NotFound() : View(account);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SystemAccount account)
        {
            if (!ModelState.IsValid) return View(account);
            try
            {
                await _accountService.AddAccountAsync(account);
                TempData["SuccessMessage"] = "Account created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while creating the account.";
                ModelState.AddModelError("", ex.Message);
            }
            return View(account);
        }

        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null) return NotFound();
            var account = await _accountService.GetAccountByIdAsync(id.Value);
            return account == null ? NotFound() : View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, SystemAccount account)
        {
            if (id != account.AccountId) return NotFound();
            if (!ModelState.IsValid) return View(account);
            try
            {
                await _accountService.UpdateAccountAsync(account);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(account);
        }

        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null) return NotFound();
            var account = await _accountService.GetAccountByIdAsync(id.Value);
            return account == null ? NotFound() : View(account);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            await _accountService.DeleteAccountAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var adminEmail = _configuration["AdminEmail"];
            var adminPassword = _configuration["AdminPassword"];
            if (email == adminEmail && password == adminPassword)
            {
                return await SignInUser("Admin", "Admin");
            }

            var user = (await _accountService.GetAllAccountsAsync())
                .FirstOrDefault(u => u.AccountEmail == email && u.AccountPassword == password);

            if (user != null)
            {
                var roleName = user.AccountRole == 1 ? "Staff" : "Lecturer";
                return await SignInUser(user.AccountName, roleName, user.AccountId);
            }
            ViewBag.Error = "Invalid username or password";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult ClearClaims()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            return Ok();
        }

        private async Task<IActionResult> SignInUser(string userName, string role, short? accountId = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, role)
            };
            if (accountId.HasValue)
            {
                claims.Add(new Claim("AccountId", accountId.Value.ToString()));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties { IsPersistent = false });
            return RedirectToAction("Index", "Home");
        }
    }
}
