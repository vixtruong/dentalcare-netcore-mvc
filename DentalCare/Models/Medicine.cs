using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Medicine
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public short Quantity { get; set; }

    public int Price { get; set; }

    public string Medicinetypeid { get; set; } = null!;

    public virtual Medicinetype Medicinetype { get; set; } = null!;

    public virtual ICollection<Prescriptiondetail> Prescriptiondetails { get; set; } = new List<Prescriptiondetail>();
}
