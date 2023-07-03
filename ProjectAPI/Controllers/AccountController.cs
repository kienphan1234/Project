using Microsoft.AspNetCore.Mvc;
using ProjectAPI.Models;

namespace ProjectAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private Prn231dbContext dbContext = new Prn231dbContext();

        [HttpGet]
        [Route("{email}")]
        public IActionResult GetAccountByEmail(string email)
        {
            var account = dbContext.Accounts
                .SingleOrDefault(a => a.Email.Equals(email));
            if (account == null)
                return NotFound();
            return Ok(account);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetStudentByAccountId(int id)
        {
            var account = dbContext.Accounts.SingleOrDefault(a => a.Id == id);
            if (account == null)
                return NotFound();

            var student = dbContext.Students
                .Select(s => new Student
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .SingleOrDefault(s => s.Id.Equals(account.StudentId));
            if (student == null)
                return NotFound();

            return Ok(student);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetTeacherByAccountId(int id)
        {
            var account = dbContext.Accounts.SingleOrDefault(a => a.Id == id);
            if (account == null)
                return NotFound();

            var teacher = dbContext.Teachers
                .Select(s => new Teacher
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .SingleOrDefault(s => s.Id.Equals(account.TeacherId));
            if (teacher == null)
                return NotFound();

            return Ok(teacher);
        }

        [HttpPost]
        public IActionResult AddAccount(Account account)
        {
            dbContext.Accounts.Add(account);
            dbContext.SaveChanges();
            return Ok();
        }
    }
}
