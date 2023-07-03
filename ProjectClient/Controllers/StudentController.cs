using Microsoft.AspNetCore.Mvc;
using ProjectClient.Models;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace ProjectClient.Controllers
{
    public class StudentController : Controller
    {
        private readonly HttpClient client = null;

        public StudentController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.BaseAddress = new Uri("http://localhost:7164/");
            client.DefaultRequestHeaders.Accept.Add(contentType);
        }

        List<Class>? GetClassesByStudentId(string id)
        {
            HttpResponseMessage response = client
                .GetAsync("api/Student/GetClassesByStudentId/" + id)
                .GetAwaiter()
                .GetResult();
            List<Class>? classes;

            if (response.IsSuccessStatusCode)
            {
                string strData = response.Content.ReadAsStringAsync()
                    .GetAwaiter()
                    .GetResult();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                classes = JsonSerializer.Deserialize<List<Class>>(strData, options);
                return classes;
            }
            else classes = null;

            return classes;
        }

        Student? GetStudentFromSession()
        {
            string str = HttpContext.Session.GetString("AccountSession");
            if (str == null) return null;
            Account account = JsonSerializer.Deserialize<Account>(str);

            try
            {
                HttpResponseMessage response = client.GetAsync("api/Student/GetStudentById/" + account.StudentId).Result;
                if (response.IsSuccessStatusCode)
                {
                    string strData = response.Content.ReadAsStringAsync().Result;
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    Student student = JsonSerializer.Deserialize<Student>(strData, options);
                    return student;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occurred during the API call
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return null;
        }

        Class? GetClassById(int id)
        {
            HttpResponseMessage response = client
                .GetAsync("api/Class/GetClassById/" + id)
                .GetAwaiter()
                .GetResult();
            Class? classDetail;
            if (response.IsSuccessStatusCode)
            {
                string strData = response.Content.ReadAsStringAsync()
                    .GetAwaiter()
                    .GetResult();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                classDetail = JsonSerializer.Deserialize<Class>(strData, options);
                return classDetail;
            }
            else classDetail = null;

            return classDetail;
        }

        List<Resource>? GetResourcesFromClass(int classId)
        {
            HttpResponseMessage response = client
                .GetAsync("api/Resource/GetResourcesFromClass/" + classId)
                .Result;
            List<Resource>? resources;
            if (response.IsSuccessStatusCode)
            {
                string strData = response.Content.ReadAsStringAsync().Result;
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                resources = JsonSerializer.Deserialize<List<Resource>>(strData, options);
            }
            else resources = null;

            return resources;
        }

        Resource? GetResourceDataFromId(int id)
        {
            HttpResponseMessage response = client
                .GetAsync("api/Resource/GetResourceDataById/" + id)
                .Result;
            Resource? resource;
            if (response.IsSuccessStatusCode)
            {
                string strData = response.Content.ReadAsStringAsync().Result;
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                resource = JsonSerializer.Deserialize<Resource>(strData, options);
            }
            else resource = null;

            return resource;
        }

        public IActionResult Index()
        {
            Student? student = GetStudentFromSession();
            if (student == null)
                return Unauthorized();
            List<Class>? classes = GetClassesByStudentId(student.Id);
            var viewModel = new StudentViewModel
            {
                Student = student,
                Classes = classes
            };
            return View(viewModel);
        }

        public IActionResult ClassDetails(int id)
        {
            Student? student = GetStudentFromSession();
            if (student == null)
                return Unauthorized();

            Class? clss = GetClassById(id);
            if (clss == null)
                return NotFound();

            var classes = GetClassesByStudentId(student.Id);
            bool authorized = false;
            foreach (var c in classes)
            {
                if (c.Id == clss.Id)
                {
                    authorized = true;
                    break;
                }
            }
            if (!authorized)
                return Unauthorized();

            List<Resource> resources = GetResourcesFromClass(id);
            ViewData["resources"] = resources;

            return View(clss);
        }

        public IActionResult DownloadResource(int id)
        {
            Student? student = GetStudentFromSession();
            if (student == null)
                return Unauthorized();

            Resource? data = GetResourceDataFromId(id);
            if (data == null)
                return NotFound();

            Class? clss = GetClassById(data.ClassId);
            if (clss != null)
            {
                var classes = GetClassesByStudentId(student.Id);
                bool authorized = false;
                foreach (var c in classes)
                {
                    if (c.Id == clss.Id)
                    {
                        authorized = true;
                        break;
                    }
                }
                if (!authorized)
                    return Unauthorized();
            }

            HttpResponseMessage response = client
                .GetAsync("api/Resource/GetResourceFileById/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                var stream = response.Content.ReadAsStream();
                string name = data.Path;
                string contentType = data.ContentType;
                return File(stream, contentType, name);
            }
            else return NotFound();
        }
    }
}
