using System;
using System.Collections.Generic;

namespace api_hotel.Models;

public partial class Room
{
    public int RoomNumber { get; set; }

    public int Floor { get; set; }

    public string Category { get; set; } = null!;

    public int Capacity { get; set; }

    public decimal PricePerDay { get; set; }

    public string? RequestDetails { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
