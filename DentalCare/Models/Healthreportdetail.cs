using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Healthreportdetail
{
    public int Id { get; set; }

    public string Healthreportid { get; set; } = null!;

    public string Customerid { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual Healthreport Healthreport { get; set; } = null!;
}
