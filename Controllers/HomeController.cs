using LeaveManagementSystemNew.Data;
using LeaveManagementSystemNew.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using LeaveManagementSystemNew.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace LeaveManagementSystemNew.Controllers
{
    public class HomeController : Controller
    {
        private readonly LeaveManagementDBContext _context;

        public HomeController(LeaveManagementDBContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {

            if (Request.Cookies.ContainsKey("LeaveManagementAccessToken"))
            {
                var token = Request.Cookies["LeaveManagementAccessToken"];
            }

            int time = DateTime.Now.Hour;

            ViewData["Greetings"] = time < 12 ? "Good Morning" : "Good Afternoon";
            int userCount = await _context.Employees.CountAsync();
            //int UserCount = Repository.Users.Where(info=> info.WillAttend == true).Count();

            var model = new IndexViewModel
            {
                Id = 1,
                NumberOfPeople = userCount
            };
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}
