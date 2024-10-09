using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Faculty
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
