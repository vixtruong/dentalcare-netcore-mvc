using System;
using System.Collections.Generic;

namespace DentalCare.Models;

public partial class Owner
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;
}
