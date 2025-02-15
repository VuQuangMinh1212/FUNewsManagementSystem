using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FUNewsManagementSystem.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Diagnostics;

namespace FUNewsManagementSystem.Controllers
{
    public class SystemAccountsController : Controller
    {
            private readonly FunewsManagementContext _context;
            private readonly IConfiguration _configuration;

            public SystemAccountsController(FunewsManagementContext context, IConfiguration configuration)
            {
                _context = context;
                _configuration = configuration;
            }


            // GET: SystemAccounts
            public async Task<IActionResult> Index()
            {
                return View(await _context.SystemAccounts.ToListAsync());
            }

            // GET: SystemAccounts/Details/5
            public async Task<IActionResult> Details(short? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var systemAccount = await _context.SystemAccounts
                    .FirstOrDefaultAsync(m => m.AccountId == id);
                if (systemAccount == null)
                {
                    return NotFound();
                }

                return View(systemAccount);
            }

            // GET: SystemAccounts/Create
            public IActionResult Create()
            {
                return View();
            }

            // POST: SystemAccounts/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create([Bind("AccountId,AccountName,AccountEmail,AccountRole,AccountPassword")] SystemAccount systemAccount)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(systemAccount);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(systemAccount);
            }

            // GET: SystemAccounts/Edit/5
            public async Task<IActionResult> Edit(short? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var systemAccount = await _context.SystemAccounts.FindAsync(id);
                if (systemAccount == null)
                {
                    return NotFound();
                }
                return View(systemAccount);
            }

            // POST: SystemAccounts/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(short id, [Bind("AccountId,AccountName,AccountEmail,AccountRole,AccountPassword")] SystemAccount systemAccount)
            {
                if (id != systemAccount.AccountId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(systemAccount);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!SystemAccountExists(systemAccount.AccountId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(systemAccount);
            }

            // GET: SystemAccounts/Delete/5
            public async Task<IActionResult> Delete(short? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var systemAccount = await _context.SystemAccounts
                    .FirstOrDefaultAsync(m => m.AccountId == id);
                if (systemAccount == null)
                {
                    return NotFound();
                }

                return View(systemAccount);
            }

            // POST: SystemAccounts/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(short id)
            {
                var systemAccount = await _context.SystemAccounts.FindAsync(id);
                if (systemAccount != null)
                {
                    _context.SystemAccounts.Remove(systemAccount);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            private bool SystemAccountExists(short id)
            {
                return _context.SystemAccounts.Any(e => e.AccountId == id);
            }

            [HttpGet]
            public IActionResult Login()
            {
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> Login(string email, string password)
            {
                string adminEmail = _configuration.GetSection("AdminEmail").Value;
                string adminPassword = _configuration.GetSection("AdminPassword").Value;
            if(email == adminEmail && password == adminPassword)
                {
                    {
                        string roleName = "Admin";
                        // Create the user claims
                        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Admin"),
                new Claim(ClaimTypes.Role, roleName)
            };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true
                        };

                        // Sign in the user
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);
                    return RedirectToAction("Index", "Home");
                }
                }
                else
                {
                    // Check the database for a matching user
                   var user = await _context.SystemAccounts
                       .FirstOrDefaultAsync(u => u.AccountEmail == email && u.AccountPassword == password);

                    if (user != null)
                    {
                        string roleName = user.AccountRole == 1 ? "Staff" : "Lecturer";
                        // Create the user claims
                        var claims = new List<Claim>
            {

                new Claim(ClaimTypes.Name, user.AccountName),
                new Claim(ClaimTypes.Role, roleName )
            };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true
                        };

                        // Sign in the user
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        return RedirectToAction("Index", "Home");
                    }

                    // Invalid credentials, show error message
                    ViewBag.Error = "Invalid username or password";
                    return View();
                }
            }

            public async Task<IActionResult> Logout()
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login");
            }
        }
    }

