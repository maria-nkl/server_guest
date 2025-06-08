using System;
using System.Collections.Generic;

namespace api_hotel.Models;

public partial class User
{
    public int Id { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? RoleId { get; set; }

    public string FullName { get; set; } = null!;

    public virtual Role? Role { get; set; }
}
