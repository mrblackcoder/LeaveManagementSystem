using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeaveManagementSystemNew.Data;
using LeaveManagementSystemNew.Data.Entities;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Threading.Tasks;
using LeaveManagementSystemNew.Models;

namespace LeaveManagementSystemNew.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly LeaveManagementDBContext _context;
        private readonly IDataRepository _dataRepository;
        private readonly IMemoryCache _cache;

        public EmployeesController(LeaveManagementDBContext context, IDataRepository dataRepository, IMemoryCache cache)
        {
            _context = context;
            _dataRepository = dataRepository;
            _cache = cache;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {


            var userRole = HttpContext.Session.GetInt32("Role") ?? 1;
            var userId = HttpContext.Session.GetInt32("UserId"); 

            if (userRole == 1 && userId.HasValue)
            {
                return RedirectToAction("Details", "Employees", new { id = userId.Value });
            }


            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var employeeId = HttpContext.Session.GetInt32("EmployeeId");
            var role = HttpContext.Session.GetInt32("Role");

            if (!employeeId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            List<Employee> employees;
            if (!_cache.TryGetValue("EmployeeList", out employees))
            {
                employees = await _dataRepository.GetAllAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                _cache.Set("EmployeeList", employees, cacheEntryOptions);
            }

            var filteredEmployees = role == 2 // Admin role
                ? employees.Select(e => new EmployeeReqIndexVm
                {
                    Id = e.Id,
                    Name = e.Name ?? "No Name",
                    RegistrationNumber = e.RegistrationNumber ?? "N/A"
                })
                : employees
                    .Where(e => e.Id == employeeId.Value)
                    .Select(e => new EmployeeReqIndexVm
                    {
                        Id = e.Id,
                        Name = e.Name ?? "No Name",
                        RegistrationNumber = e.RegistrationNumber ?? "N/A"
                    });

            return View(filteredEmployees);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var employeeId = HttpContext.Session.GetInt32("EmployeeId");
            var userRole = HttpContext.Session.GetInt32("Role");

            if (userRole == 1 && employeeId != id)
            {
                return Unauthorized(); 
            }

            var employee = await _context.Employees
                .Include(e => e.Leaves)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }
        
        // GET: Employees/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeReqModel model)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                var employee = new Employee
                {
                    Name = model.Name,
                    RegistrationNumber = model.RegistrationNumber
                };

                var account = new Account
                {
                    Username = model.Username,
                    Password = model.Password,
                    Role = 1,
                    Employee = employee

                };

                // Add the Employee and Account to the context
                await _dataRepository.AddAsync(employee);
                await _dataRepository.AddAsync(account);

                return RedirectToAction("Details", new { id = employee.Id });
            }

            return View(model);
        }



        // GET: Employees/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var employeeId = HttpContext.Session.GetInt32("EmployeeId");
            var userRole = HttpContext.Session.GetInt32("Role");

            if (userRole == 1 && employeeId != id)
            {
                return Unauthorized();
            }

            if (id == null)
            {
                return NotFound();
            }

            var employee = await _dataRepository.GetByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(updtEmployeeReqModel updtReq)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(updtReq);
            }
            var employee = await _dataRepository.GetByIdAsync(updtReq.Id);
            if (employee == null)
            {
                return NotFound();
            }
            employee.Name = updtReq.Name;
            employee.RegistrationNumber = updtReq.RegistrationNumber;

            await _dataRepository.UpdateAsync(employee);
            _cache.Remove("EmployeeList");
            return RedirectToAction("Details", new { id = employee.Id });
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var employee = await _dataRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Check if user is logged in
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var employee = await _dataRepository.GetByIdAsync(id);
            if (employee != null)
            {
                await _dataRepository.DeleteAsync(id);
                _cache.Remove("EmployeeList");
            }

            return RedirectToAction("Login", "Account");
        }

    }
}
