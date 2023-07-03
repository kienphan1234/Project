using System;
using System.Collections.Generic;

namespace ProjectClient.Models;

public partial class Student
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}
