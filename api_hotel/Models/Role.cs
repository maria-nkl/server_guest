using System;
using System.Collections.Generic;

namespace api_hotel.Models;

public partial class Role
{
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;

    public string Position { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
