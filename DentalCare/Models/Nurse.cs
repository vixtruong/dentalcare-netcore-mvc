using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Nurse
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime Birthday { get; set; }

    public DateTime Firstdayofwork { get; set; }

    public bool Gender { get; set; }

    public string? Avatar { get; set; }

    public bool? Fired { get; set; }

    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();
}
