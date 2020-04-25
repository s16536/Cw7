using lab05.DAL;
using lab05.DTOs;
using lab05.Models;
using Microsoft.AspNetCore.Mvc;

namespace lab05.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public EnrollmentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            var student = new Student()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.BirthDate,
                Faculty = request.Studies,
                IndexNumber = request.IndexNumber

            };

            var enrollment = _dbService.AddStudent(student);
            if (enrollment == null)
            {
                return BadRequest();
            }

            return Ok(enrollment);
        }

    }
}