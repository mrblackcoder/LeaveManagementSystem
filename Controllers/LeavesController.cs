using Microsoft.EntityFrameworkCore;
using LeaveManagementSystemNew.Data.Entities;
using LeaveManagementSystemNew.Models;
using LeaveManagementSystemNew.Data.Models;
using LeaveManagementSystemNew.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;


namespace LeaveManagementSystemNew.Controllers
{
    public class LeavesController : Controller
    {
        private readonly LeaveManagementDBContext _context;

        private readonly IDataRepository _dataRepository;

        private readonly IMemoryCache _cache;




        public LeavesController(LeaveManagementDBContext context,IDataRepository dataRepository, IMemoryCache cache)
        {
            _context = context;
            _dataRepository = dataRepository;
            _cache = cache;
        }
        String cacheKey = "LeaveList";

        public async Task<IActionResult> Index()
        {
            // Retrieve the registration number from claims
            var registrationNumber = User.Claims.FirstOrDefault(c => c.Type == "RegistrationNumber")?.Value;

            // Convert the registration number to an integer if needed
            if (int.TryParse(registrationNumber, out var employeeId))
            {
                // Retrieve leaves for the employee
                var leaves = await _context.Leaves
                    .Where(l => l.EmployeeId == employeeId)
                    .ToListAsync();

                // Show the list of leaves
                return View(leaves);
            }

            return RedirectToAction("Details", "Employees", new { id = employeeId });
        }

        // GET: Leaves/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leave = await _context.Leaves
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (leave == null)
            {
                return NotFound();
            }

            return View(leave);
        }

        // GET: Leaves/Create
        public IActionResult Create(int? employeeId)
        {
            ViewBag.EmployeeId = new SelectList(_context.Employees, "Id", "Name");


            ViewBag.SelectedEmployeeId = employeeId ?? 0;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateLeaveReqModel cerm)
        {
            // Check if the model is valid
            if (ModelState.IsValid)
            {
                // Create a new Leave entity based on the input model (CreateLeaveReqModel)
                var leave = new Leave
                {
                    EmployeeId = cerm.EmployeeId,
                    StartTime = cerm.StartTime.ToUniversalTime(),
                    LeaveDuration = cerm.LeaveDuration
                };

                // Add the new leave record to the context and save changes
                _context.Leaves.Add(leave);
                await _context.SaveChangesAsync();

                // Redirect to the Index action after successful creation
                return RedirectToAction("Details", "Employees", new { id = cerm.EmployeeId });
            }

            // If the model state is invalid, redisplay the form with the current employee list
            ViewBag.EmployeeId = new SelectList(_context.Employees, "Id", "Name", cerm.EmployeeId);
            return RedirectToAction("Details", "Employees", new { id = cerm.EmployeeId });
        }



        // GET: Leaves/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var leave = await _context.Leaves
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leave == null)
            {
                return NotFound();
            }

            var model = new updtLeaveReqModel
            {
                Id = leave.Id,
                EmployeeId = leave.EmployeeId,
                LeaveDuration = leave.LeaveDuration
            };

            ViewBag.EmployeeList = new SelectList(await _context.Employees.ToListAsync(), "Id", "Name", model.EmployeeId);

            return View(model);
        }

        // POST: Leaves/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(updtLeaveReqModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.EmployeeList = new SelectList(await _context.Employees.ToListAsync(), "Id", "Name", model.EmployeeId);
                return View(model);
            }

            var leave = await _context.Leaves.FindAsync(model.Id);
            if (leave == null)
            {
                return NotFound();
            }

            if (!await _context.Employees.AnyAsync(e => e.Id == model.EmployeeId))
            {
                ModelState.AddModelError("EmployeeId", "Invalid EmployeeId.");
                ViewBag.EmployeeList = new SelectList(await _context.Employees.ToListAsync(), "Id", "Name", model.EmployeeId);
                return View(model);
            }

            leave.EmployeeId = model.EmployeeId;
            leave.LeaveDuration = model.LeaveDuration;

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Employees", new { id = model.EmployeeId });
        }


        // GET: Leaves/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
        if (id == null)
        {
            return NotFound();
        }

        var leave = await _context.Leaves
            .Include(l => l.Employee)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (leave == null)
        {
            return NotFound();
        }

        return View(leave);
        }

        // POST: Leaves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leave = await _context.Leaves.FindAsync(id);
            if (leave != null)
            {
                var employeeId = leave.EmployeeId;
                _context.Leaves.Remove(leave);
                await _context.SaveChangesAsync();

                _cache.Remove("LeaveList");

                return RedirectToAction("Details", "Employees", new { id = employeeId });
            }

            return NotFound();
        }



        private bool LeaveExists(int id)
        {
            return _context.Leaves.Any(e => e.Id == id);
        }
    }
}
