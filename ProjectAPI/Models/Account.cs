using System;
using System.Collections.Generic;

namespace ProjectAPI.Models;

public partial class Account
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? StudentId { get; set; }

    public int? TeacherId { get; set; }

    public int Role { get; set; }

    public virtual Student? Student { get; set; }

    public virtual Teacher? Teacher { get; set; }
}
