using api_hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_hotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly HotelContext _context;

        public ServicesController(HotelContext context)
        {
            _context = context;
        }

        // GET: api/Services
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            return await _context.Services.ToListAsync();
        }

        // GET: api/Services/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetService(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
            {
                return NotFound();
            }

            return service;
        }

        // GET: api/Services/by-name?name=Развлечения
        [HttpGet("by-name")]
        public async Task<ActionResult<Service>> GetServiceByName([FromQuery] string name)
        {
            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.ServiceName == name);

            if (service == null)
                return NotFound();

            return Ok(service);
        }


        // GET: api/Services/by-names?names=Завтрак&names=Уборка
        [HttpGet("by-names")]
        public async Task<ActionResult<IEnumerable<Service>>> GetServicesByNames([FromQuery] List<string> names)
        {
            var services = await _context.Services
                .Where(s => names.Contains(s.ServiceName))
                .ToListAsync();

            return Ok(services);
        }
    }
}