using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Equipmenttype
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
}
