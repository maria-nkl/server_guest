using System;
using System.Collections.Generic;

namespace api_hotel.Models;

public partial class Service
{
    public int Id { get; set; }

    public string ServiceName { get; set; } = null!;

    public decimal Price { get; set; }
}
