using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Bill
{
    public string Id { get; set; } = null!;

    public DateTime Date { get; set; }

    public bool Payment { get; set; }

    public int Total { get; set; }

    public byte Discount { get; set; }

    public int Finaltotal { get; set; }

    public string Receptionistid { get; set; } = null!;

    public string Medicalexaminationid { get; set; } = null!;

    public bool Status { get; set; }

    public virtual Medicalexamination Medicalexamination { get; set; } = null!;

    public virtual Receptionist Receptionist { get; set; } = null!;
}
