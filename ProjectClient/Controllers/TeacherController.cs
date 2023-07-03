using Microsoft.AspNetCore.Mvc;
using ProjectClient.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ProjectClient.Controllers
{
    public class TeacherController : Controller
    {
        private readonly HttpClient client = null;

        public TeacherController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.BaseAddress = new Uri("http://localhost:7164/");
            client.DefaultRequestHeaders.Accept.Add(contentType);
        }

      

        Teacher? GetTeacherFromSession()
        {
            string str = HttpContext.Session.GetString("AccountSession");
            if (str == null) return null;
            Account account = JsonSerializer.Deserialize<Account>(str);

            try
            {
                HttpResponseMessage response = client.GetAsync("api/Teacher/GetTeacherById/" + account.TeacherId).Result;
                if (response.IsSuccessStatusCode)
                {
                    string strData = response.Content.ReadAsStringAsync().Result;
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    Teacher teacher = JsonSerializer.Deserialize<Teacher>(strData, options);
                    return teacher;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occurred during the API call
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return null;
        }

        List<Class>? GetClassesByTeacherId(int id)
        {
            HttpResponseMessage response = client
                .GetAsync("api/Teacher/GetClassesByTeacherId/" + id)
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

        List<Student>? GetStudentsByClassId(int id)
        {
            HttpResponseMessage response = client
                .GetAsync("api/Class/GetStudentsByClassId/" + id)
                .GetAwaiter().GetResult();
            List<Student>? students;
            if (response.IsSuccessStatusCode)
            {
                string strData = response.Content.ReadAsStringAsync()
                    .GetAwaiter()
                    .GetResult();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                students = JsonSerializer.Deserialize<List<Student>>(strData, options);
                return students;
            }
            else students = null;

            return students;
        }

        List<Student>? GetStudents()
        {
            HttpResponseMessage response = client
                .GetAsync("api/Student/GetStudents")
                .GetAwaiter()
                .GetResult();
            List<Student>? students;
            if (response.IsSuccessStatusCode)
            {
                string strData = response.Content.ReadAsStringAsync()
                    .GetAwaiter()
                    .GetResult();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                students = JsonSerializer.Deserialize<List<Student>>(strData, options);
                return students;
            }
            else students = null;

            return students;
        }
        Student? GetStudentById(string id)
        {
            HttpResponseMessage response = client
                .GetAsync("api/Student/GetStudentById/" + id)
                .GetAwaiter()
                .GetResult();
            Student? student;
            if (response.IsSuccessStatusCode)
            {
                string strData = response.Content.ReadAsStringAsync()
                    .GetAwaiter()
                    .GetResult();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                student = JsonSerializer.Deserialize<Student>(strData, options);
                return student;
            }
            else student = null;

            return student;

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
            Teacher? teacher = GetTeacherFromSession();
            if (teacher == null)
                return Unauthorized();

            List<Class>? classes = GetClassesByTeacherId(teacher.Id);
            var viewModel = new TeacherViewModel
            {
                Teacher = teacher,
                Classes = classes
            };

            return View(viewModel);
        }

        public IActionResult ClassDetails(int id)
        {
            Teacher? teacher = GetTeacherFromSession();
            if (teacher == null)
                return Unauthorized();

            Class? c = GetClassById(id);
            if (c == null)
                return NotFound();

            if (c.TeacherId != teacher.Id)
                return Unauthorized();

            List<Resource> resources = GetResourcesFromClass(id);
            ViewData["resources"] = resources;

            List<Student> students = GetStudentsByClassId(c.Id);
            List<Student> newStudents = GetStudents();
            foreach (Student s in students)
            {
                foreach (Student ns in newStudents)
                {
                    if (s.Id.Equals(ns.Id))
                    {
                        newStudents.Remove(ns);
                        break;
                    }
                }
            }
            var viewModel = new ClassViewModel
            {
                Class = c,
                Students = students,
                newStudents = newStudents

            };
            return View(viewModel);
        }

        public IActionResult AddStudentToClass(string id, int id2)
        {
            Teacher? teacher = GetTeacherFromSession();
            if (teacher == null)
                return Unauthorized();

            Class? clss = GetClassById(id2);
            if (clss != null)
            {
                if (clss.TeacherId != teacher.Id)
                    return Unauthorized();
            }

            string studentId = id;
            int classId = id2;

            HttpResponseMessage response = client
                .PostAsync("api/Class/AddStudentToClass/" + studentId + "/" + classId, null)
                .GetAwaiter()
                .GetResult();
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ClassDetails", new { id = classId });
            }
            else return BadRequest();
        }

        public IActionResult DeleteClass(int id)
        {
            Teacher? teacher = GetTeacherFromSession();
            if (teacher == null)
                return Unauthorized();

            Class? clss = GetClassById(id);
            if (clss == null)
                return NotFound();

            if (clss.TeacherId != teacher.Id)
                return Unauthorized();

            return View(clss);
        }

        [HttpPost, ActionName("DeleteClass")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            HttpResponseMessage response = client
                .PostAsync("api/Class/DeleteClass/" + id, null)
                .GetAwaiter()
                .GetResult();
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");
            else return BadRequest();
        }

        public IActionResult AddClass()
        {
            Teacher? teacher = GetTeacherFromSession();
            if (teacher == null)
                return Unauthorized();
            ViewData["Teacher"] = new Teacher()
            {
                Id = teacher.Id,
                Name = teacher.Name
            };
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddClass([Bind("Name")] Class c)
        {
            Teacher? teacher = GetTeacherFromSession();
            if (teacher == null)
                return Unauthorized();
            Class clss = new Class()
            {
                Id = c.Id,
                Name = c.Name,
                TeacherId = c.TeacherId,
                Teacher = new Teacher
                {
                    Id = 0,
                    Name = ""
                }
            };
            HttpResponseMessage response = client
                .PostAsJsonAsync("api/Class/AddClass/" + teacher.Id, clss)
                .GetAwaiter()
                .GetResult();
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else return BadRequest();
        }

        public IActionResult RemoveStudentFromClass(string id, int id2)
        {
            Teacher? teacher = GetTeacherFromSession();
            if (teacher == null)
                return Unauthorized();

            Class? clss = GetClassById(id2);
            if (clss != null)
            {
                if (clss.TeacherId != teacher.Id)
                    return Unauthorized();
            }

            string studentId = id;
            int classId = id2;

            HttpResponseMessage response = client
                .DeleteAsync("api/Class/RemoveStudentFromClass/" + studentId + "/" + classId)
                .GetAwaiter()
                .GetResult();
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ClassDetails", new { id = classId });
            }
            else return BadRequest();
        }

        public IActionResult RemoveResource(int id)
        {
            Teacher? teacher = GetTeacherFromSession();
            if (teacher == null)
                return Unauthorized();

            Resource? data = GetResourceDataFromId(id);
            if (data == null)
                return NotFound();

            Class? clss = GetClassById(data.ClassId);
            if (clss != null)
            {
                if (clss.TeacherId != teacher.Id)
                    return Unauthorized();
            }

            HttpResponseMessage response = client
                .DeleteAsync("api/Resource/RemoveResourceById/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ClassDetails", new { id = data.ClassId });
            }
            else return BadRequest();
        }

        public IActionResult DownloadResource(int id)
        {
            Teacher? teacher = GetTeacherFromSession();
            if (teacher == null)
                return Unauthorized();

            Resource? data = GetResourceDataFromId(id);
            if (data == null)
                return NotFound();

            Class? clss = GetClassById(data.ClassId);
            if (clss != null)
            {
                if (clss.TeacherId != teacher.Id)
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

        public IActionResult AddResource(int id)
        {
            Teacher? teacher = GetTeacherFromSession();
            if (teacher == null)
                return Unauthorized();

            Class? clss = GetClassById(id);
            if (clss == null)
                return NotFound();

            if (clss.TeacherId != teacher.Id)
                return Unauthorized();

            ViewData["classId"] = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddResource(int id, [Bind] AddResourceModel data)
        {
            if (!ModelState.IsValid)
            {
                ViewData["classId"] = id;
                return View(data);
            }

            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StringContent(data.Name), nameof(data.Name));
                content.Add(new StreamContent(data.File.OpenReadStream())
                {
                    Headers =
                    {
                        ContentLength = data.File.Length,
                        ContentType = new MediaTypeHeaderValue(data.File.ContentType)
                    }
                }, nameof(data.File), data.File.FileName);

                HttpResponseMessage response = client
                    .PostAsync("api/resource/AddResource/" + id, content)
                    .GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("ClassDetails", new { id = id });
                else return BadRequest();
            }
        }
    }
}
