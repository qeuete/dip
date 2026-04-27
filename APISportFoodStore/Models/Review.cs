using System;
using System.Collections.Generic;

namespace APISportFoodStore.Models;

public partial class Review
{
    public int? IdReview { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool Deleted { get; set; }

    public virtual Product? Product { get; set; } = null!;

    public virtual ICollection<ReviewImage> ReviewImages { get; set; } = new List<ReviewImage>();

    public virtual User? User { get; set; } = null!;
}
