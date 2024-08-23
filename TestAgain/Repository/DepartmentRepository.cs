using TestAgain.Models;
using Microsoft.EntityFrameworkCore;

namespace TestAgain.Repository
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly APIDbContext _appDBContext;
        public DepartmentRepository(APIDbContext context)
        {
            _appDBContext = context;
        }
        public async Task<IEnumerable<Department>> GetDepartment()
        {
            return await _appDBContext.Departments.ToListAsync();
        }
        public async Task<Department> GetDepartmentByID(int ID)
        {
            return await _appDBContext.Departments.FindAsync(ID);
        }
        public async Task<Department> InsertDepartment(Department objDepartment)
        {
            _appDBContext.Departments.Add(objDepartment);
            await _appDBContext.SaveChangesAsync();
            return objDepartment;
        }
        public async Task<Department> UpdateDepartment(Department objDepartment)
        {
            _appDBContext.Entry(objDepartment).State = EntityState.Modified;
            await _appDBContext.SaveChangesAsync();
            return objDepartment;
        }
        public bool DeleteDepartment(int ID)
        {
            var department = _appDBContext.Departments.Find(ID);
            if (department != null)
            {
                _appDBContext.Departments.Remove(department);
                _appDBContext.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
