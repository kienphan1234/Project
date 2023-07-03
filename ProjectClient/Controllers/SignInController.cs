using Microsoft.AspNetCore.Mvc;
using ProjectClient.Models;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ProjectClient.Controllers
{
    public class SignInController : Controller
    {
        private readonly HttpClient client = null;

        public SignInController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.BaseAddress = new Uri("https://localhost:7164/");
            client.DefaultRequestHeaders.Accept.Add(contentType);
        }

        Account? GetAccountByEmail(string email)
        {
            Account? a;
            HttpResponseMessage response = client
                .GetAsync("api/Account/GetAccountByEmail/" + email)
                .GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                string strData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                a = JsonSerializer.Deserialize<Account>(strData, options);
            }
            else a = null;

            return a;
        }

        Student? GetStudetnById(string id)
        {
            Student? s;
            HttpResponseMessage response = client
                .GetAsync("api/student/GetStudentById/" + id)
                .GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                string strData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                s = JsonSerializer.Deserialize<Student>(strData, options);
            }
            else s = null;

            return s;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index([Bind] Account account)
        {
            if (!ModelState.IsValid)
                return View(account);

            Account? a = GetAccountByEmail(account.Email);
            if (a == null)
            {
                ViewData["msg"] = "Account does not exist";
                return View(account);
            }
            else
            {
                if (!a.Password.Equals(account.Password))
                {
                    ViewData["msg"] = "Wrong password";
                    return View(account);
                }

                HttpContext.Session.SetString("AccountSession", JsonSerializer.Serialize(a));
                if (a.Role == 1)
                    return RedirectToAction("Index", "Teachers");
                else return RedirectToAction("Index", "Students");
            }
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignUp([Bind] SignUpModel model)
        {
            if (!ModelState.IsValid)
                return View();

            ViewData["msg"] = "";
            if (GetStudetnById(model.StudentId) != null)
            {
                ViewData["msg"] += "Student ID already exists. ";
            }
            if (GetAccountByEmail(model.Email) != null)
            {
                ViewData["msg"] += "Email is already used. ";
            }
            if (!ViewData["msg"].Equals(""))
                return View();

            HttpResponseMessage response;
            Student s = new Student
            {
                Id = model.StudentId,
                Name = model.StudentName
            };
            response = client.PostAsJsonAsync("api/student/AddStudent", s)
                .GetAwaiter().GetResult();
            Account a = new Account
            {
                Email = model.Email,
                Password = model.Password,
                StudentId = model.StudentId,
                Role = 2
            };
            response = client.PostAsJsonAsync("api/account/AddAccount", a)
                .GetAwaiter().GetResult();

            return RedirectToAction("Index");
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("AccountSession");
            return RedirectToAction("Index");
        }
    }

    public class SignUpModel
    {
        [Required(ErrorMessage = "Student ID is required")]
        public string StudentId { get; set; } = null!;
        [Required(ErrorMessage = "Student Name is required")]
        public string StudentName { get; set; } = null!;
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
