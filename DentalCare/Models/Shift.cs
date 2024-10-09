using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Shift
{
    public string Id { get; set; } = null!;

    public DateTime Date { get; set; }

    public string Doctorid { get; set; } = null!;

    public string Nurseid { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Nurse Nurse { get; set; } = null!;
}
