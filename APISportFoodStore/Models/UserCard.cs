using System;
using System.Collections.Generic;

namespace APISportFoodStore.Models;

public partial class UserCard
{
    public int? IdUserCard { get; set; }

    public int UserId { get; set; }

    public string CardNumber { get; set; } = null!;

    public string ExpiryDate { get; set; } = null!;

    public string Cvv { get; set; } = null!;

    public bool Deleted { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

}
