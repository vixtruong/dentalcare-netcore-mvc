using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Equipment
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public short Quantity { get; set; }

    public int Price { get; set; }

    public string Equipmenttypeid { get; set; } = null!;

    public virtual ICollection<Equipmentdetail> Equipmentdetails { get; set; } = new List<Equipmentdetail>();

    public virtual Equipmenttype Equipmenttype { get; set; } = null!;
}
