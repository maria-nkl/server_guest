using api_hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_hotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly HotelContext _context;

        public UsersController(HotelContext context)
        {
            _context = context;
        }

        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<ActionResult<UserInfo>> Login(LoginModel model)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            return new UserInfo
            {
                FullName = user.FullName,
                RoleName = user.Role.RoleName
            };
        }
    }

    public class LoginModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class UserInfo
    {
        public string FullName { get; set; }
        public string RoleName { get; set; }
    }
}
