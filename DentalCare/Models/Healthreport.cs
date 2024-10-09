using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Healthreport
{
    public string Id { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime Date { get; set; }

    public int Frequency { get; set; }

    public string Doctorid { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual ICollection<Healthreportdetail> Healthreportdetails { get; set; } = new List<Healthreportdetail>();
}
