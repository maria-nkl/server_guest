/*namespace api_hotel.Models
{
    public class BookingRequest
    {
        public int GuestId { get; set; }
        public int RoomNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<int>? Services { get; set; } // может быть null
    }
}
*/

using System;
using System.Collections.Generic;

namespace api_hotel.Controllers
{
    public class BookingRequest
    {
        public int RoomNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int GuestId { get; set; }
        public List<int> Services { get; set; } = new();
        public decimal TotalPrice { get; set; }  // этот параметр игнорируется на сервере
    }
}
