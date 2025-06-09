/*using api_hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using api_hotel.Services;
using api_hotel.Models;


namespace api_hotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly HotelContext _context;
        private readonly BookingService _bookingService;

        public BookingsController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        *//*public BookingsController(HotelContext context)
        {
            _context = context;
        }*//*

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return await _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.RoomNumberNavigation)
                .ToListAsync();
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.RoomNumberNavigation)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return booking;
        }

        // GET: api/Bookings/5/services
        [HttpGet("{id}/services")]
        public async Task<ActionResult<IEnumerable<Service>>> GetBookingServices(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Services)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null || booking.Services == null)
            {
                return NotFound();
            }

            return Ok(booking.Services.ToList());
        }

        // POST: api/Bookings
        [HttpPost]
        public async Task<ActionResult<Booking>> PostBooking([FromBody] BookingRequest request)
        {
            var start = DateOnly.FromDateTime(request.StartDate);
            var end = DateOnly.FromDateTime(request.EndDate);

            var isRoomAvailable = !await _context.Bookings
                .AnyAsync(b => b.RoomNumber == request.RoomNumber &&
                               b.StartDate <= end &&
                               b.EndDate >= start);

            if (!isRoomAvailable)
            {
                return BadRequest("Номер уже забронирован на выбранные даты");
            }

            var room = await _context.Rooms.FindAsync(request.RoomNumber);
            if (room == null)
            {
                return BadRequest("Номер не найден");
            }

            var days = (int)(end.DayNumber - start.DayNumber) + 1;
            var roomCost = room.PricePerDay * days;

            decimal servicesCost = 0;
            if (request.Services != null && request.Services.Any())
            {
                servicesCost = await _context.Services
                    .Where(s => request.Services.Contains(s.Id))
                    .SumAsync(s => s.Price) * days;
            }

            var guest = await _context.Guests.FindAsync(request.GuestId);
            decimal discount = 0;
            if (guest != null && guest.Discount != "None")
            {
                if (decimal.TryParse(guest.Discount.Replace("%", ""), out var percent))
                {
                    discount = (roomCost + servicesCost) * percent / 100;
                }
            }

            var booking = new Booking
            {
                RoomNumber = request.RoomNumber,
                GuestId = request.GuestId,
                StartDate = start,
                EndDate = end,
                TotalPrice = roomCost + servicesCost - discount
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Связывание с услугами
            if (request.Services != null && request.Services.Any())
            {
                foreach (var serviceId in request.Services)
                {
                    var bs = new BookingService
                    {
                        BookingId = booking.Id,
                        ServiceId = serviceId
                    };
                    _context.BookingServices.Add(bs);
                }
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetBooking", new { id = booking.Id }, booking);
        }


        // GET: api/Bookings/statistics
        [HttpGet("statistics")]
        public async Task<ActionResult<BookingStatistics>> GetStatistics()
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Today);

            var total = await _context.Bookings.CountAsync();
            var current = await _context.Bookings
                .CountAsync(b => b.StartDate <= currentDate && b.EndDate >= currentDate);
            var upcoming = await _context.Bookings
                .CountAsync(b => b.StartDate > currentDate);

            return new BookingStatistics
            {
                Total = total,
                Current = current,
                Upcoming = upcoming
            };
        }

        // GET: api/Bookings/available-rooms
        [HttpGet("available-rooms")]
        public async Task<ActionResult<IEnumerable<Room>>> GetAvailableRooms(
            DateTime startDate, DateTime endDate,
            string category = null, int? minCapacity = null)
        {
            var query = _context.Rooms
                .Where(r => !_context.Bookings
                    .Any(b => b.RoomNumber == r.RoomNumber &&
                              b.StartDate.ToDateTime(TimeOnly.MinValue) <= endDate &&
                              b.EndDate.ToDateTime(TimeOnly.MinValue) >= startDate));

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(r => r.Category == category);
            }

            if (minCapacity.HasValue)
            {
                query = query.Where(r => r.Capacity >= minCapacity.Value);
            }

            return await query.ToListAsync();
        }
    }

    public class BookingStatistics
    {
        public int Total { get; set; }
        public int Current { get; set; }
        public int Upcoming { get; set; }
    }
}*//*


using api_hotel.Models;
using api_hotel.Services;
using Microsoft.AspNetCore.Mvc;

namespace api_hotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly BookingService _bookingService;

        public BookingsController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return Ok(await _bookingService.GetAllBookingsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound();

            return Ok(booking);
        }

        [HttpGet("{id}/services")]
        public async Task<ActionResult<IEnumerable<Service>>> GetBookingServices(int id)
        {
            var services = await _bookingService.GetServicesByBookingIdAsync(id);
            if (services == null)
                return NotFound();

            return Ok(services);
        }

        [HttpPost]
        public async Task<ActionResult<Booking>> PostBooking([FromBody] BookingRequest request)
        {
            var result = await _bookingService.CreateBookingAsync(request);
            if (result == null)
                return BadRequest("Ошибка при создании брони (возможно, номер уже занят или некорректные данные)");

            return CreatedAtAction("GetBooking", new { id = result.Id }, result);
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<BookingStatistics>> GetStatistics()
        {
            var stats = await _bookingService.GetStatisticsAsync();
            return Ok(stats);
        }

        [HttpGet("available-rooms")]
        public async Task<ActionResult<IEnumerable<Room>>> GetAvailableRooms(
            DateTime startDate, DateTime endDate,
            string? category = null, int? minCapacity = null)
        {
            var rooms = await _bookingService.GetAvailableRoomsAsync(startDate, endDate, category, minCapacity);
            return Ok(rooms);
        }
    }
}
*/

using api_hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace api_hotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly HotelContext _context;

        public BookingsController(HotelContext context)
        {
            _context = context;
        }

        // POST: api/Bookings
        [HttpPost]
        public async Task<ActionResult<Booking>> CreateBooking([FromBody] BookingRequest request)
        {
            var guest = await _context.Guests.FindAsync(request.GuestId);
            if (guest == null)
                return NotFound("Гость не найден.");

            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomNumber == request.RoomNumber);
            if (room == null)
                return NotFound("Номер не найден.");

            int totalDays = (request.EndDate - request.StartDate).Days + 1;
            decimal roomCost = room.PricePerDay * totalDays;

            decimal servicesCost = 0;
            if (request.Services != null && request.Services.Count > 0)
            {
                var services = await _context.Services
                    .Where(s => request.Services.Contains(s.Id))
                    .ToListAsync();
                servicesCost = services.Sum(s => s.Price);
            }

            var booking = new Booking
            {
                RoomNumber = request.RoomNumber,
                GuestId = request.GuestId,
                StartDate = DateOnly.FromDateTime(request.StartDate),
                EndDate = DateOnly.FromDateTime(request.EndDate),
                TotalPrice = roomCost + servicesCost,
                Services = request.Services ?? new List<int>()
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
        }


        // POST: api/Bookings/bulk
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBookingsBulk([FromBody] List<BookingRequest> requests)
        {
            if (requests == null || !requests.Any())
                return BadRequest("Список бронирований пуст.");

            var guestId = requests.First().GuestId;
            var guest = await _context.Guests.FindAsync(guestId);
            if (guest == null)
                return NotFound("Гость не найден.");

            var allRoomNumbers = requests.Select(r => r.RoomNumber).ToList();
            var rooms = await _context.Rooms
                .Where(r => allRoomNumbers.Contains(r.RoomNumber))
                .ToListAsync();

            var allServiceIds = requests.SelectMany(r => r.Services ?? new List<int>()).Distinct().ToList();
            var servicesDict = await _context.Services
                .Where(s => allServiceIds.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id);

            var bookings = new List<Booking>();

            foreach (var req in requests)
            {
                var room = rooms.FirstOrDefault(r => r.RoomNumber == req.RoomNumber);
                if (room == null)
                    return NotFound($"Номер {req.RoomNumber} не найден.");

                int totalDays = (req.EndDate - req.StartDate).Days + 1;
                decimal roomCost = room.PricePerDay * totalDays;

                decimal serviceCost = 0;
                if (req.Services != null)
                {
                    foreach (var serviceId in req.Services)
                    {
                        if (servicesDict.TryGetValue(serviceId, out var service))
                            serviceCost += service.Price;
                    }
                }

                bookings.Add(new Booking
                {
                    RoomNumber = req.RoomNumber,
                    GuestId = req.GuestId,
                    StartDate = DateOnly.FromDateTime(req.StartDate),
                    EndDate = DateOnly.FromDateTime(req.EndDate),
                    TotalPrice = roomCost + serviceCost,
                    Services = req.Services ?? new List<int>()
                });
            }

            _context.Bookings.AddRange(bookings);
            await _context.SaveChangesAsync();

            return Ok("Бронирования успешно созданы.");
        }


        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Services)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return NotFound();

            return booking;
        }

        // DTO для возврата информации о бронировании
        public class BookingDto
        {
            public int Id { get; set; }
            public int RoomNumber { get; set; }
            public string GuestName { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public List<int> Services { get; set; }
            public decimal TotalPrice { get; set; }
        }

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetAllBookings()
        {
            var serviceDict = await _context.Services
                .ToDictionaryAsync(s => s.Id, s => s.ServiceName);

            var bookings = await _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.RoomNumberNavigation)
                .ToListAsync();

            var result = bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                RoomNumber = (int)b.RoomNumber,
                GuestName = b.Guest.FullNameOrOrganization,
                StartDate = b.StartDate.ToDateTime(TimeOnly.MinValue),
                EndDate = b.EndDate.ToDateTime(TimeOnly.MinValue),
                Services = b.Services ?? new List<int>(),
                TotalPrice = (decimal)b.TotalPrice,
            });

            return Ok(result);
        }

    }
}
