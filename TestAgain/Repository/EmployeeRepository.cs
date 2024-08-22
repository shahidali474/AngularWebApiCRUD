using TestAgain.Models;
using Microsoft.EntityFrameworkCore;

namespace TestAgain.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly APIDbContext _appDBContext;
        public EmployeeRepository(APIDbContext context)
        {
            _appDBContext = context ??
                throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<Employee>> GetEmployees()
        {
            return await _appDBContext.Employees.ToListAsync();
        }
        public async Task<Employee> GetEmployeeByID(int ID)
        {
            return await _appDBContext.Employees.FindAsync(ID);
        }
        public async Task<Employee> InsertEmployee(Employee objEmployee)
        {
            _appDBContext.Employees.Add(objEmployee);
            await _appDBContext.SaveChangesAsync();
            return objEmployee;
        }
        public async Task<Employee> UpdateEmployee(Employee objEmployee)
        {
            _appDBContext.Entry(objEmployee).State = EntityState.Modified;
            await _appDBContext.SaveChangesAsync();
            return objEmployee;
        }
        public bool DeleteEmployee(int ID)
        {
            bool result = false;
            var department = _appDBContext.Employees.Find(ID);
            if (department != null)
            {
                _appDBContext.Entry(department).State = EntityState.Deleted;
                _appDBContext.SaveChanges();
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
    }
}
