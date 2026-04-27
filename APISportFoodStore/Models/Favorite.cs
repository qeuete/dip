using System;
using System.Collections.Generic;

namespace APISportFoodStore.Models;

public partial class Favorite
{
    public int? IdFavorite { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
