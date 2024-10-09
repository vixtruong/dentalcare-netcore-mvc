using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Equipmentdetail
{
    public int Id { get; set; }

    public string Medicalexaminationid { get; set; } = null!;

    public string Equipmentid { get; set; } = null!;

    public DateTime Date { get; set; }

    public short Quantity { get; set; }

    public virtual Equipment Equipment { get; set; } = null!;

    public virtual Medicalexamination Medicalexamination { get; set; } = null!;
}
