using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Equipmentdetail
{
    public int Id { get; set; }

    public string EquipmentsheetId { get; set; } = null!;

    public string Equipmentid { get; set; } = null!;

    public short Quantity { get; set; }

    public virtual Equipment Equipment { get; set; } = null!;

    public virtual EquipmentSheet EquipmentSheet { get; set; } = null!;
}
