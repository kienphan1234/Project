using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectClient.Models;

public partial class Account
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Email is required")]

    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]

    public string Password { get; set; } = null!;

    public string? StudentId { get; set; }

    public int? TeacherId { get; set; }

    public int Role { get; set; }

    public virtual Student? Student { get; set; }

    public virtual Teacher? Teacher { get; set; }
}
