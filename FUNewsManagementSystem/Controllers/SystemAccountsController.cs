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

        // GET: All Accounts with optional search query
        public async Task<IActionResult> Index(string searchQuery)
        {
            var accounts = await _accountService.GetAllAccountsAsync();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                accounts = accounts.Where(a => a.AccountName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
            }

            ViewData["CurrentFilter"] = searchQuery;
            return View(accounts);
        }

        // GET: Account Details
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null) return NotFound();

            var account = await _accountService.GetAccountByIdAsync(id.Value);
            return account == null ? NotFound() : View(account);
        }

        // GET: Create Account
        public IActionResult Create() => View();

        // POST: Create Account
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

        // GET: Edit Account
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null) return NotFound();

            var account = await _accountService.GetAccountByIdAsync(id.Value);
            return account == null ? NotFound() : View(account);
        }

        // POST: Edit Account
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, SystemAccount account)
        {
            if (id != account.AccountId) return BadRequest();

            if (!ModelState.IsValid) return View(account);

            try
            {
                await _accountService.UpdateAccountAsync(account);
                TempData["SuccessMessage"] = "Account updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the account.";
                ModelState.AddModelError("", ex.Message);
            }

            return View(account);
        }

        // GET: Delete Account
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null) return NotFound();

            var account = await _accountService.GetAccountByIdAsync(id.Value);
            return account == null ? NotFound() : View(account);
        }

        // POST: Confirm Delete Account
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            await _accountService.DeleteAccountAsync(id);
            TempData["SuccessMessage"] = "Account deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Login Page
        [HttpGet]
        public IActionResult Login() => View();

        // POST: Authenticate User and Sign In
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var adminEmail = _configuration["AdminEmail"];
            var adminPassword = _configuration["AdminPassword"];

            // Check Admin Credentials from Configuration
            if (email == adminEmail && password == adminPassword)
            {
                return await SignInUser("Admin", "Admin");
            }

            // Check User Credentials from Service
            var user = (await _accountService.GetAllAccountsAsync())
                .FirstOrDefault(u => u.AccountEmail == email && u.AccountPassword == password);

            if (user != null)
            {
                var roleName = user.AccountRole switch
                {
                    1 => "Staff",
                    2 => "Lecturer",
                    _ => "User"
                };

                return await SignInUser(user.AccountName, roleName, user.AccountId);
            }

            ViewBag.Error = "Invalid username or password";
            return View();
        }

        // POST: Logout User
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // POST: Clear Authentication Claims
        [HttpPost]
        public IActionResult ClearClaims()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            return Ok();
        }

        // Helper Method: Sign In User
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
