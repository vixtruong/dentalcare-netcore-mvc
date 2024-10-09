using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Prescriptiondetail
{
    public int Id { get; set; }

    public short Quantity { get; set; }

    public string Prescriptionid { get; set; } = null!;

    public string Medicineid { get; set; } = null!;

    public virtual Medicine Medicine { get; set; } = null!;

    public virtual Prescription Prescription { get; set; } = null!;
}
