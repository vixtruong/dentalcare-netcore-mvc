using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Techposition
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public int Price { get; set; }

    public string Techniqueid { get; set; } = null!;

    public virtual ICollection<Techdetail> Techdetails { get; set; } = new List<Techdetail>();

    public virtual Technique Technique { get; set; } = null!;
}
