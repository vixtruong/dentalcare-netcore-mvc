using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Appointment
{
    public string Id { get; set; } = null!;

    public string Doctorid { get; set; } = null!;

    public string Customerid { get; set; } = null!;

    public DateTime Date { get; set; }

    public TimeOnly Time { get; set; }

    public string Demand { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;
}
