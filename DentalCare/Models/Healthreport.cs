using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Healthreport
{
    public string Id { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime Date { get; set; }

    public int Frequency { get; set; }

    public string MedicalexaminationId { get; set; } = null!;

    public string CustomerId { get; set; }

    public virtual Medicalexamination Medicalexamination { get; set; } = null!;
}
