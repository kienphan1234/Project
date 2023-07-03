using Microsoft.AspNetCore.Mvc;
using ProjectAPI.Models;

namespace ProjectAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private Prn231dbContext db = new Prn231dbContext();

        [HttpPost]
        [Route("{classId}")]
        public IActionResult AddResource(int classId, [FromForm] AddResourceModel data)
        {
            var clss = db.Classes.SingleOrDefault(c => c.Id == classId);
            if (clss == null)
                return NotFound();

            Resource resource = new Resource()
            {
                UploadDate = DateTime.Now,
                Path = "",
                Name = data.Name,
                ContentType = data.File.ContentType,
                ClassId = classId
            };
            db.Resources.Add(resource);
            db.SaveChanges();

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string oldName = data.File.FileName;
            int insertPosi = oldName.LastIndexOf(".");
            if (insertPosi == -1) insertPosi = oldName.Length;
            string fileName = oldName.Insert(insertPosi, "_" + resource.Id);
            string fileNameWithPath = Path.Combine(path, fileName);
            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                data.File.CopyTo(stream);
            }

            resource.Path = fileName;
            db.SaveChanges();

            return Ok();
        }

        [HttpGet]
        [Route("{classId}")]
        public IActionResult GetResourcesFromClass(int classId)
        {
            var clss = db.Classes.SingleOrDefault(c => c.Id == classId);
            if (clss == null)
                return NotFound();

            List<Resource> resources = db.Resources.Where(r => r.ClassId == classId)
                .Select(r => new Resource
                {
                    Id = r.Id,
                    UploadDate = r.UploadDate,
                    Path = r.Path,
                    Name = r.Name,
                    ContentType = r.ContentType,
                    ClassId = r.ClassId
                }).ToList();
            return Ok(resources);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetResourceDataById(int id)
        {
            var resource = db.Resources.SingleOrDefault(r => r.Id == id);
            if (resource == null)
                return NotFound();
            return Ok(resource);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetResourceFileById(int id)
        {
            var resource = db.Resources.SingleOrDefault(r => r.Id == id);
            if (resource == null)
                return NotFound();

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files", resource.Path);
            byte[] b = System.IO.File.ReadAllBytes(path);
            return File(b, resource.ContentType, resource.Path);
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult RemoveResourceById(int id)
        {
            var resource = db.Resources.SingleOrDefault(r => r.Id == id);
            if (resource == null)
                return NotFound();

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files", resource.Path);
            System.IO.File.Delete(path);

            db.Resources.Remove(resource);
            db.SaveChanges();

            return Ok();
        }
    }
}
