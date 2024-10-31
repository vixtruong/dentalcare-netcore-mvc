using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Techdetail
{
    public int Id { get; set; }

    public string Techpositionid { get; set; } = null!;

    public string TechsheetId { get; set; } = null!;

    public short Quantity { get; set; }

    public virtual Techsheet Techsheet { get; set; } = null!;

    public virtual Techposition Techposition { get; set; } = null!;
}
