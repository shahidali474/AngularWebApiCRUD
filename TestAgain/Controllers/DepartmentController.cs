using TestAgain.Models;
using TestAgain.Repository;
using Microsoft.AspNetCore.Mvc;

namespace TestAgain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentRepository _department;
        public DepartmentController(IDepartmentRepository department)
        {
            _department = department;
        }

        [HttpGet]
        [Route("GetDepartment")]
        public async Task<IActionResult> Get()
        {
            var departments = await _department.GetDepartment();
            return Ok(departments);
        }

        [HttpGet]
        [Route("GetDepartmentByID/{Id}")]
        public async Task<IActionResult> GetDeptById(int Id)
        {
            return Ok(await _department.GetDepartmentByID(Id));
        }

        [HttpPost]
        [Route("AddDepartment")]
        public async Task<IActionResult> Post(Department dep)
        {
            var result = await _department.InsertDepartment(dep);
            if (result.DepartmentId == 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something Went Wrong");
            }
            return Ok("Added Successfully");
        }

        [HttpPut]
        [Route("UpdateDepartment")]
        public async Task<IActionResult> Put(Department dep)
        
        {
            await _department.UpdateDepartment(dep);
            return Ok("Updated Successfully");
        }

        [HttpDelete("DeleteDepartment/{id}")]
        public JsonResult Delete(int id)
        {
            bool isDeleted = _department.DeleteDepartment(id);

            if (isDeleted)
            {
                return new JsonResult(new { message = "Deleted Successfully" });
            }
            else
            {
                return new JsonResult(new { message = "Error: Department not found" });
            }
        }
    }
}