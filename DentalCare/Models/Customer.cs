using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Customer
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime Birthday { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Medicalexamination> Medicalexaminations { get; set; } = new List<Medicalexamination>();
}
