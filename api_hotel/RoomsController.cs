using api_hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_hotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly HotelContext _context;

        public RoomsController(HotelContext context)
        {
            _context = context;
        }

        // GET: api/Rooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            return await _context.Rooms.ToListAsync();
        }

        // GET: api/Rooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            return room;
        }

        // GET: api/Rooms/5/info
        [HttpGet("{id}/info")]
        public async Task<ActionResult<RoomInfo>> GetRoomInfo(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            return new RoomInfo
            {
                Floor = room.Floor,
                Capacity = room.Capacity,
                Category = room.Category
            };
        }

        // GET: api/Rooms/5/current-booking
        [HttpGet("{id}/current-booking")]
        public async Task<ActionResult<BookingInfo>> GetCurrentBooking(int id)
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Today); // Конвертируем DateTime в DateOnly
            var booking = await _context.Bookings
                .Include(b => b.Guest)
                .FirstOrDefaultAsync(b => b.RoomNumber == id &&
                                        b.StartDate <= currentDate &&
                                        b.EndDate >= currentDate);

            if (booking == null)
            {
                return NotFound();
            }

            return new BookingInfo
            {
                BookingId = booking.Id,
                GuestName = booking.Guest.FullNameOrOrganization,
                GuestPhone = booking.Guest.Phone,
                StartDate = booking.StartDate.ToDateTime(TimeOnly.MinValue), // Конвертируем обратно для DTO
                EndDate = booking.EndDate.ToDateTime(TimeOnly.MinValue)
            };
        }

        // PUT: api/Rooms/5/request
        [HttpPut("{id}/request")]
        public async Task<IActionResult> UpdateRoomRequest(int id, [FromBody] string requestDetails)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            room.RequestDetails = requestDetails;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Rooms/5/request
        [HttpDelete("{id}/request")]
        public async Task<IActionResult> ClearRoomRequest(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            room.RequestDetails = null;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Rooms/with-requests
        [HttpGet("with-requests")]
        public async Task<ActionResult<IEnumerable<Room>>> GetRoomsWithRequests()
        {
            return await _context.Rooms
                .Where(r => !string.IsNullOrEmpty(r.RequestDetails))
                .ToListAsync();
        }

        // GET: api/Rooms/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Room>>> SearchRooms(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string category,
            [FromQuery] int minCapacity)
        {
            try
            {
                // Логирование входящих параметров
                Console.WriteLine($"Поиск номеров: {startDate} - {endDate}, категория: {category}, вместимость: {minCapacity}");

                if (startDate > endDate)
                    return BadRequest("Дата начала не может быть позже даты окончания.");

                DateOnly startDateOnly = DateOnly.FromDateTime(startDate);
                DateOnly endDateOnly = DateOnly.FromDateTime(endDate);

                // Получаем номера занятые в этот период
                var bookedRoomNumbers = await _context.Bookings
                    .Where(b => b.StartDate <= endDateOnly && b.EndDate >= startDateOnly)
                    .Select(b => b.RoomNumber)
                    .Distinct()
                    .ToListAsync();

                Console.WriteLine($"Занятые номера: {string.Join(", ", bookedRoomNumbers)}");

                // Получаем все номера, соответствующие критериям
                var allRooms = await _context.Rooms
                    .Where(r => r.Category == category && r.Capacity >= minCapacity)
                    .OrderBy(r => r.RoomNumber)
                    .ToListAsync();

                Console.WriteLine($"Всего номеров по критериям: {allRooms.Count}");

                // Фильтруем свободные номера
                var availableRooms = allRooms
                    .Where(r => !bookedRoomNumbers.Contains(r.RoomNumber))
                    .ToList();

                Console.WriteLine($"Свободных номеров: {availableRooms.Count}");

                if (!availableRooms.Any())
                    return NotFound("Свободные номера не найдены.");

                return availableRooms;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при поиске номеров: {ex}");
                return StatusCode(500, "Произошла ошибка при обработке запроса");
            }
        }

        // GET: api/Rooms/searchForOrganization
        [HttpGet("searchForOrganization")]
        public async Task<ActionResult<IEnumerable<Room>>> SearchRoomsForOrganization(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int minCapacity)
        {
            try
            {
                if (startDate > endDate)
                    return BadRequest("Дата начала не может быть позже даты окончания.");

                DateOnly startDateOnly = DateOnly.FromDateTime(startDate);
                DateOnly endDateOnly = DateOnly.FromDateTime(endDate);

                // Получаем номера занятые в этот период
                var bookedRoomNumbers = await _context.Bookings
                    .Where(b => b.StartDate <= endDateOnly && b.EndDate >= startDateOnly)
                    .Select(b => b.RoomNumber)
                    .Distinct()
                    .ToListAsync();

                // Получаем все номера с достаточной вместимостью (без фильтра по категории)
                var availableRooms = await _context.Rooms
                    .Where(r => r.Capacity >= minCapacity && !bookedRoomNumbers.Contains(r.RoomNumber))
                    .OrderBy(r => r.Floor)  // Сортируем по этажу для удобства организаций
                    .ThenBy(r => r.Capacity) // Затем по вместимости
                    .ToListAsync();

                if (!availableRooms.Any())
                    return NotFound("Нет доступных номеров, удовлетворяющих критериям поиска.");

                return availableRooms;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при поиске номеров для организаций");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

    }

    public class RoomInfo
    {
        public int Floor { get; set; }
        public int Capacity { get; set; }
        public string Category { get; set; }
    }

    public class BookingInfo
    {
        public int BookingId { get; set; }
        public string GuestName { get; set; }
        public string GuestPhone { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}