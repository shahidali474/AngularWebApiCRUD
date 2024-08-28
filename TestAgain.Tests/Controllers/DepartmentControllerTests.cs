using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TestAgain.Controllers;
using TestAgain.Models;
using TestAgain.Repository;
using Xunit;

namespace TestAgain.Tests.Controllers
{
    public class DepartmentControllerTests
    {
        private readonly DepartmentController _departmentController;
        private readonly IDepartmentRepository _repository;
        private readonly DbContextOptions<APIDbContext> _options;
        private readonly IConfiguration _configuration;

        public DepartmentControllerTests()
        {
            var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = configurationBuilder.Build();

            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            _options = new DbContextOptionsBuilder<APIDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            _repository = new DepartmentRepository(new APIDbContext(_options));
            _departmentController = new DepartmentController(_repository);
        }

        [Fact]
        public async Task GetDepartment()
        {

            var result = await _departmentController.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Department>>(okResult.Value);

            Assert.True(returnValue.Count > 0);
        }

        [Fact]
        public async Task GetDepartmentByID()
        {

            var departmentId = 24;

            var result = await _departmentController.GetDeptById(departmentId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Department>(okResult.Value);
            Assert.Equal(departmentId, returnValue.DepartmentId);
        }

        [Fact]
        public async Task AddDepartment()
        {
            var newDepartment = new Department { DepartmentName = "IT" };

            var result = await _departmentController.Post(newDepartment);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Added Successfully", okResult.Value);

            using (var context = new APIDbContext(_options))
            {
                var addedDepartment = await context.Departments.FirstOrDefaultAsync(d => d.DepartmentName == "IT");
                Assert.NotNull(addedDepartment);
                Assert.Equal("IT", addedDepartment.DepartmentName);
            }
        }

        [Fact]
        public async Task DeleteDepartment_ReturnsJsonResult_WithSuccessMessage_WhenDepartmentIsDeleted()
        {
            var departmentId = 1;

            using (var context = new APIDbContext(_options))
            {
                var existingDepartment = await context.Departments.FindAsync(departmentId);
                Assert.NotNull(existingDepartment);
            }

            var result = await _departmentController.Delete(departmentId);

            var jsonResult = Assert.IsType<JsonResult>(result);
            var resultValue = jsonResult.Value;

            Assert.NotNull(resultValue);

            var dictionary = resultValue as IDictionary<string, object>;
            Assert.NotNull(dictionary);

            Assert.True(dictionary.ContainsKey("message"));
            Assert.Equal("Deleted Successfully", dictionary["message"].ToString());

            using (var context = new APIDbContext(_options))
            {
                var deletedDepartment = await context.Departments.FindAsync(departmentId);
                Assert.Null(deletedDepartment);
            }
        }

    }
}