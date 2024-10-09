using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Medicinetype
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
}
