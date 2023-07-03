namespace ProjectAPI.Models
{
    public class AddResourceModel
    {
        public string Name { get; set; } = null!;
        public IFormFile File { get; set; } = null!;

    }
}
