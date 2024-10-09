using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Prescription
{
    public string Id { get; set; } = null!;

    public DateTime Date { get; set; }

    public string Doctorid { get; set; } = null!;

    public string Medicalexaminationid { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Medicalexamination Medicalexamination { get; set; } = null!;

    public virtual ICollection<Prescriptiondetail> Prescriptiondetails { get; set; } = new List<Prescriptiondetail>();
}
