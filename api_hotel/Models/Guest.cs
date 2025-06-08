using System;
using System.Collections.Generic;

namespace api_hotel.Models;

public partial class Guest
{
    public int Id { get; set; }

    public string FullNameOrOrganization { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Discount { get; set; }

    public bool? IsOrganization { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
