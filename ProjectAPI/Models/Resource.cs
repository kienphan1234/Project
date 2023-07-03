using System;
using System.Collections.Generic;

namespace ProjectAPI.Models;

public partial class Resource
{
    public int Id { get; set; }

    public DateTime UploadDate { get; set; }

    public string Path { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public int ClassId { get; set; }

    public virtual Class Class { get; set; } = null!;
}
