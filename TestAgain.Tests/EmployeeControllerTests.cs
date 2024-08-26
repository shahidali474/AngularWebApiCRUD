using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TestAgain.Controllers;
using TestAgain.Models;
using TestAgain.Repository;

namespace TestAgain.Tests
{
    public class EmployeeControllerTests
    {
        private readonly EmployeeController _EmployeeController;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DbContextOptions<APIDbContext> _options;
        private readonly IConfiguration _configuration;

        public EmployeeControllerTests()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = configurationBuilder.Build();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            _options = new DbContextOptionsBuilder<APIDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            _employeeRepository = new EmployeeRepository(new APIDbContext(_options));
            _departmentRepository = new DepartmentRepository(new APIDbContext(_options));

            _webHostEnvironment = Mock.Of<IWebHostEnvironment>(env =>
                env.ContentRootPath == Directory.GetCurrentDirectory());

            _EmployeeController = new EmployeeController(_employeeRepository, _departmentRepository, _webHostEnvironment);
        }

        [Fact]
        public async Task GetEmployee()
        {
            var result = await _EmployeeController.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Employee>>(okResult.Value);

            Assert.True(returnValue.Count > 0);
        }

        [Fact]
        public async Task GetEmployeeByID()
        {
            var employeeId = 16;

            var result = await _EmployeeController.GetEmpByID(employeeId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Employee>(okResult.Value);
            Assert.Equal(employeeId, returnValue.EmployeeID);
        }

        [Fact]
        public async Task AddEmployee()
        {
            
            var newEmployee = new Employee
            {
                EmployeeName = "Hasnain",
                Department = "IT", 
                DOJ = DateTime.Now
            };

            var result = await _EmployeeController.Post(newEmployee);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Added Successfully", okResult.Value);

            using (var context = new APIDbContext(_options))
            {
                var addedEmployee = await context.Employees
                    .FirstOrDefaultAsync(d => d.EmployeeName == "Hasnain");
                Assert.NotNull(addedEmployee);
                Assert.Equal("Hasnain", addedEmployee.EmployeeName);
                Assert.Equal("IT", addedEmployee.Department);
            }
        }
    }
}