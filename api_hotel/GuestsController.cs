using api_hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_hotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestsController : ControllerBase
    {
        private readonly HotelContext _context;

        public GuestsController(HotelContext context)
        {
            _context = context;
        }

        // GET: api/Guests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Guest>>> GetGuests()
        {
            return await _context.Guests
                .Where(g => g.IsOrganization == false || g.IsOrganization == null) // только частные лица
                .ToListAsync();
        }

        // GET: api/Guests/search?query=...
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Guest>>> SearchGuests(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Параметр поиска не может быть пустым.");

            var guests = await _context.Guests
                .Where(g => (g.IsOrganization == false || g.IsOrganization == null) &&
                            (EF.Functions.ILike(g.FullNameOrOrganization, $"%{query}%") ||
                             EF.Functions.ILike(g.Phone, $"%{query}%")))
                .ToListAsync();

            if (!guests.Any())
                return NotFound("Гости не найдены.");

            return guests;
        }

        // GET: api/Guests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Guest>> GetGuest(int id)
        {
            var guest = await _context.Guests
                .FirstOrDefaultAsync(g => g.Id == id && (g.IsOrganization == false || g.IsOrganization == null));

            if (guest == null)
            {
                return NotFound();
            }

            return guest;
        }

        // POST: api/Guests
        [HttpPost]
        public async Task<ActionResult<Guest>> CreateGuest(Guest guest)
        {

            _context.Guests.Add(guest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGuest), new { id = guest.Id }, guest);
        }

        // PUT: api/Guests/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGuest(int id, Guest guest)
        {
            if (id != guest.Id)
            {
                return BadRequest();
            }

            // Убедимся, что IsOrganization не изменится
            guest.IsOrganization = false;

            _context.Entry(guest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Guests.Any(e => e.Id == id && (e.IsOrganization == false || e.IsOrganization == null)))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Guests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGuest(int id)
        {
            var guest = await _context.Guests
                .FirstOrDefaultAsync(g => g.Id == id && (g.IsOrganization == false || g.IsOrganization == null));
            if (guest == null)
            {
                return NotFound();
            }

            _context.Guests.Remove(guest);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}

