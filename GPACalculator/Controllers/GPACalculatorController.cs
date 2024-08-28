using GPACalculator.Models;
using GPACalculator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace GPACalculator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GPACalculatorController(ILogger<GPACalculatorController> logger) : ControllerBase
    {
        private readonly GPACalculatorService _gpaCalculatorService = new();
        private readonly DbManager _dbManager = new();
        private readonly ILogger<GPACalculatorController> _logger = logger;
        [HttpPost]
        [Route("AddGrade")]
        [Authorize(Roles = "Student,Teacher,Director")]  
        public IActionResult AddGrade([FromBody] List<Course> courses)
        {
            if (courses == null || courses.Count == 0)
            {
                return BadRequest("Course list is null or empty.");
            }
            try
            {
                double gpa = _dbManager.CalculateGPA(courses);
                return Ok(new { GPA = gpa });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid data in AddGrade");
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetCourses")]
        [Authorize(Roles = "Student,Teacher,Director")]  
        public IActionResult GetCourses()
        {
            var courses = _gpaCalculatorService.GetCourses();
            return Ok(courses);
        }
        [HttpGet]
        [Route("GetAvailableCourses")]
        [Authorize(Roles = "Student,Teacher,Director")]  
        public IActionResult GetAvailableCourses()
        {
            var availableCourses = _gpaCalculatorService.GetAvailableCourses();
            return Ok(availableCourses);
        }
        [HttpDelete]
        [Route("DeleteGrade")]
        [Authorize(Roles = "Teacher,Director")]  
        public IActionResult DeleteGrade(int id)
        {
            if (id == 0)
            {
                return BadRequest("Course ID is invalid.");
            }
            try
            {
                _dbManager.DeleteGrade(id);
                return Ok(new { Message = "Grade deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invalid course ID in DeleteGrade");
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        [Route("DeleteCourse")]
        [Authorize(Roles = "Director")] 
        public IActionResult DeleteCourse(int id)
        {
            if (id == 0)
            {
                return BadRequest("Course name is null or empty.");
            }
            try
            {
                _dbManager.DeleteCourse(id);
                return Ok(new { Message = "Course deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invalid course name in DeleteCourse");
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        [Route("UpdateCourse")]
        [Authorize(Roles = "Teacher,Director")]
        public IActionResult UpdateCourse([FromBody] AddCourse updatedCourse)
        {
            if (updatedCourse == null)
            {
                return BadRequest("Course is null.");
            }
            try
            {
                _dbManager.UpdateCourse(updatedCourse);
                return Ok(new { Message = "Course updated" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid data in UpdateCourse");
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        [Route("AddCourse")]
        [Authorize(Roles = "Director")]
        public IActionResult AddCourse([FromBody] AddCourse updatedCourse)
        {
            if (updatedCourse == null)
            {
                return BadRequest("Course is null.");
            }
            try
            {
                _gpaCalculatorService.UpdateCourse(updatedCourse);
                return Ok(new { Message = "Course updated" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid data in UpdateCourse");
                return BadRequest(ex.Message);
            }
        }
    }
}
