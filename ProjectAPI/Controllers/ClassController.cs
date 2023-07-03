using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectAPI.Models;
using ProjectAPI.ModelsDTO;
using ProjectAPI.Profiles;

namespace ProjectAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private Prn231dbContext dbContext = new Prn231dbContext();
        private readonly IMapper mapper;
        public ClassController()
        {
            var config = new MapperConfiguration(cf => cf.AddProfile(new ClassProfile())); ;
            mapper = config.CreateMapper();
        }

        [HttpGet]
        public IActionResult GetClasses()
        {
            List<Class> cList = dbContext.Classes
                .Include(o => o.Teacher).ToList();
            List<ClassDTO> classesList = new List<ClassDTO>();
            foreach (Class item in cList)
            {
                classesList.Add(mapper.Map<ClassDTO>(item));
            }
            classesList = cList.Select(mapper.Map<Class, ClassDTO>).ToList();
            return Ok(classesList);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetStudentsByClassId(int id)
        {
            try
            {
                var studentList = dbContext.Classes.
                Include(s => s.Students).
                SingleOrDefault(c => c.Id == id).
                Students.Select(s => new Student
                {
                    Id = s.Id,
                    Name = s.Name,
                });
                return Ok(studentList);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("{studentId}/{classId}")]
        public IActionResult AddStudentToClass(string studentId, int classId)
        {
            var student = dbContext.Students.SingleOrDefault(s => s.Id.Equals(studentId));
            if (student == null)
                return NotFound();
            var clss = dbContext.Classes.SingleOrDefault(c => c.Id == classId);
            if (clss == null)
                return NotFound();
            try
            {
                clss.Students.Add(student);
                student.Classes.Add(clss);
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                return BadRequest();
            }

            return Ok();
        }

   
        [HttpPost]
        [Route("{tId}")]
        public IActionResult AddClass(Class c, int tId)
        {
            var newClass = new Class()
            {
                Name = c.Name,
                TeacherId = tId
            };
            dbContext.Classes.Add(newClass);
            dbContext.SaveChanges();
            return Ok(newClass);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetClassById(int id)
        {
            var clss = dbContext.Classes.SingleOrDefault(c => c.Id == id);
            if (clss == null)
                return NotFound();
            return Ok(clss);
        }

        [HttpPost]
        [Route("{classId}")]
        public IActionResult DeleteClass(int classId)
        {
            var clss = dbContext.Classes.SingleOrDefault(c => c.Id == classId);
            List<Student> studentList = dbContext.Classes.
                Include(c => c.Students).
                SingleOrDefault(c => c.Id == classId).Students.ToList();
            List<Resource> resources = dbContext.Resources.Where(r => r.ClassId == classId).ToList();

            foreach (Student s in studentList.ToList())
            {
                studentList.Remove(s);
                s.Classes.Remove(clss);
                dbContext.SaveChanges();
            }

            foreach (Resource r in resources.ToList())
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files", r.Path);
                System.IO.File.Delete(path);
                dbContext.Resources.Remove(r);
                clss.Resources.Remove(r);
                dbContext.SaveChanges();
            }

            dbContext.Classes.Remove(clss);
            dbContext.SaveChanges();
            return Ok("Deleted");
        }

        [HttpDelete]
        [Route("{studentId}/{classId}")]
        public IActionResult RemoveStudentFromClass(string studentId, int classId)
        {
            var clss = dbContext.Classes.SingleOrDefault(c => c.Id == classId);
            if (clss == null)
                return NotFound();

            List<Student> studentList = dbContext.Classes.
                Include(c => c.Students).
                SingleOrDefault(c => c.Id == classId).Students.ToList();
            var student = studentList.SingleOrDefault(s => s.Id.Equals(studentId));
            if (student == null)
                return NotFound();

            studentList.Remove(student);
            student.Classes.Remove(clss);
            dbContext.SaveChanges();
            return Ok("Deleted");

        }
    }
}
