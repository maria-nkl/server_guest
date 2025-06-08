using System;
using System.Collections.Generic;

namespace api_hotel.Models;

public partial class Booking
{
    public int Id { get; set; }

    public int? RoomNumber { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int? GuestId { get; set; }

    public List<int>? Services { get; set; }

    public decimal? TotalPrice { get; set; }

    public virtual Guest? Guest { get; set; }

    public virtual Room? RoomNumberNavigation { get; set; }
}
