using api_hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_hotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationsController : ControllerBase
    {
        private readonly HotelContext _context;

        public OrganizationsController(HotelContext context)
        {
            _context = context;
        }

        // GET: api/Organizations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Guest>>> GetOrganizations()
        {
            return await _context.Guests
                .Where(g => g.IsOrganization == true) // Явное сравнение с true для nullable bool
                .ToListAsync();
        }

        // GET: api/Organizations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Guest>> GetOrganization(int id)
        {
            var organization = await _context.Guests
                .FirstOrDefaultAsync(g => g.Id == id && g.IsOrganization == true); // Явное сравнение с true

            if (organization == null)
            {
                return NotFound();
            }

            return organization;
        }

        // GET: api/Organizations/search?query=...
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Guest>>> SearchOrganizations(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Параметр поиска не может быть пустым.");

            var organizations = await _context.Guests
                .Where(g => g.IsOrganization == true &&
                            (EF.Functions.ILike(g.FullNameOrOrganization, $"%{query}%") ||
                             EF.Functions.ILike(g.Phone, $"%{query}%")))
                .ToListAsync();

            if (!organizations.Any())
                return NotFound("Организации не найдены.");

            return organizations;
        }
    }

}