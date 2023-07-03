using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectAPI.Models;

namespace ProjectAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private Prn231dbContext dbContext = new Prn231dbContext();
        [HttpGet]
        public IActionResult GetStudents()
        {
            var students = dbContext.Students;
            return Ok(students);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetStudentById(string id)
        {
            var student = dbContext.Students.SingleOrDefault(s => s.Id.Equals(id));
            if (student == null)
                return NotFound();
            return Ok(student);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetClassesByStudentId(string id)
        {
            var studentList = dbContext.Students.
                Include(s => s.Classes).
                SingleOrDefault(c => c.Id.Equals(id)).
                Classes.Select(s => new Class
                {
                    Id = s.Id,
                    Name = s.Name
                });
            return Ok(studentList);
        }

        [HttpPost]
        public IActionResult AddStudent(Student s)
        {
            var student = new Student()
            {
                Id = s.Id,
                Name = s.Name
            };
            dbContext.Students.Add(student);
            dbContext.SaveChanges();
            return Ok(student);
        }
    }
}
