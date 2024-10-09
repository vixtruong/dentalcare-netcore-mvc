using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Medicalexamination
{
    public string Id { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime Date { get; set; }

    public string Doctorid { get; set; } = null!;

    public string Customerid { get; set; } = null!;

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual Customer Customer { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual ICollection<Equipmentdetail> Equipmentdetails { get; set; } = new List<Equipmentdetail>();

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

    public virtual ICollection<Techdetail> Techdetails { get; set; } = new List<Techdetail>();
}
