using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Account
{
    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string? DoctorId { get; set; }

    public string? ReceptionistId { get; set; }

    public virtual Doctor? Doctor { get; set; }

    public virtual Receptionist? Receptionist { get; set; }
}
