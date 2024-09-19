using LeaveManagementSystemNew.Data;
using LeaveManagementSystemNew.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;


namespace LeaveManagementSystemNew.Controllers
{
    public class DataRepository : IDataRepository
    {
        public readonly LeaveManagementDBContext _context;

        private readonly IDataRepository _dataRepository;

        private readonly IMemoryCache _cache;

        private readonly ILogger<DataRepository> _logger;

        private const string cacheKey = "EmployeeList";

        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        public DataRepository(LeaveManagementDBContext context, IMemoryCache cache, ILogger<DataRepository> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;

        }
        public async Task AddAsync(Employee employee)
        {
            _context.Employees.Add(employee);

            //yukarıdaki kod yeni varlığı bağlama ekler(context'e).
            await _context.SaveChangesAsync();//
            //bu değişiklikleri veritabanına kaydeder.
            
            //Aşağıdaki kod ile cache temizlenir
            //çünkü veri değişikliği yapılmıştır.
            _cache.Remove(cacheKey);
            
        }

        public async Task DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                _cache.Remove(cacheKey);
            }
    }

        public async Task AddAsync<T>(T entity) where T : class
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
        }


        public async Task<List<Employee>> GetAllAsync()
        {
            if (_cache.TryGetValue(cacheKey, out List<Employee> employees))
            {
                _logger.Log(LogLevel.Information, "Data found in cache.");
                return employees;
            }

            await Semaphore.WaitAsync();
            try
            {
                if (_cache.TryGetValue(cacheKey, out employees))
                {
                    _logger.Log(LogLevel.Information, "Data found in cache after waiting.");
                    return employees;
                }

                _logger.Log(LogLevel.Information, "Fetching data from database.");
                employees = await _context.Employees.ToListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(20))
                    .SetPriority(CacheItemPriority.Normal);

                _cache.Set(cacheKey, employees, cacheEntryOptions);
            }
            finally
            {
                Semaphore.Release();
            }

            return employees;
        }


        public async Task<Employee> GetByIdAsync(int id)
        {
            _logger.Log(LogLevel.Information, "Trying to fetch data...");

            if (_cache.TryGetValue(cacheKey, out List<Employee> employeeCache))
            {
                var employee = employeeCache.FirstOrDefault(e => e.Id == id);
                if (employee != null)
                {
                    _logger.Log(LogLevel.Information, "Fetching employee from cache...");
                    return employee;
                }
            }

            _logger.Log(LogLevel.Information, "Fetching employee from database...");
            var dbEmployee = await _context.Employees.FindAsync(id);

            if (dbEmployee != null)
            {
                _logger.Log(LogLevel.Information, "Employee found in database, updating cache...");
                if (employeeCache == null)
                {
                    employeeCache = new List<Employee>();
                }
                employeeCache.Add(dbEmployee);

                _cache.Set(cacheKey, employeeCache, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(5),
                    Priority = CacheItemPriority.Normal
                });
            }
            else
            {
                _logger.Log(LogLevel.Warning, "Employee not found.");
            }

            return dbEmployee;
        }


        public async Task UpdateAsync(Employee employee)
        {
            _context.Update(employee);
            await _context.SaveChangesAsync();
            _cache.Remove(cacheKey);
        }
        public async Task UpdateAsync(Leave leave)
        {
            _context.Leaves.Update(leave);
            await _context.SaveChangesAsync();
        }

    }
}
