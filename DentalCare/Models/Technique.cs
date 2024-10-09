using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Technique
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Techposition> Techpositions { get; set; } = new List<Techposition>();
}
