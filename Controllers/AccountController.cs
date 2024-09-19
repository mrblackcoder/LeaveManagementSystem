using Esprima.Ast;
using LeaveManagementSystemNew.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LeaveManagementSystemNew.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly LeaveManagementDBContext _context;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, LeaveManagementDBContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, bool rememberMe)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var account = await _context.Accounts
                .Where(a => a.Username.ToLower() == username.ToLower())
                .FirstOrDefaultAsync();

            if (account == null || account.Password != password)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View();
            }
            if(account.Role == 1)
            {

                HttpContext.Session.SetString("Username", username);
                HttpContext.Session.SetInt32("EmployeeId", account.EmployeeId);
                HttpContext.Session.SetInt32("Role", account.Role);
                return RedirectToAction("Details", "Employees",new {id = account.EmployeeId});
            }
            else
            {
                HttpContext.Session.SetString("Username", username);
                HttpContext.Session.SetInt32("EmployeeId", account.EmployeeId);
                HttpContext.Session.SetInt32("Role", account.Role);
                return RedirectToAction("Index", "Employees");

            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            
            HttpContext.Session.Clear();


            return RedirectToAction("Login", "Account");
        }

    }
}
