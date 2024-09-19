using LeaveManagementSystemNew.Data.Entities;

namespace LeaveManagementSystemNew.Controllers
{
    public interface IDataRepository
    {
        Task<List<Employee>> GetAllAsync();
        Task<Employee> GetByIdAsync(int id);
        Task AddAsync(Employee employee);

        Task AddAsync<T>(T entity) where T : class;

        Task UpdateAsync(Employee employee);
        Task DeleteAsync(int id);
        Task UpdateAsync(Leave leave);
    }
}
