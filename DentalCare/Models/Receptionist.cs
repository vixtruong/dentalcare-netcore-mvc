using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Receptionist
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

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
