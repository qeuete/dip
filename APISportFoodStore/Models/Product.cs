using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APISportFoodStore.Models;

public partial class Product
{
    public int? IdProduct { get; set; }

    public string Name { get; set; } = null!;

    public string Article { get; set; } = null!;

    public int CategoryId { get; set; }

    public int ManufacturerId { get; set; }

    public string Unit { get; set; } = null!;

    [Range(0, double.MaxValue, ErrorMessage = "Значение не может быть отрицательным")]
    public decimal VolumeOrWeight { get; set; }

    public string Description { get; set; } = null!;

    public string? Image { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
    public decimal Price { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть не менее 1")]
    public int Quantity { get; set; }

    public bool IsAvailable { get; set; }

    public bool Deleted { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть не менее 1")]
    public decimal? CaloriesKcal { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть не менее 1")]
    public decimal? ProteinG { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть не менее 1")]
    public decimal? FatG { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть не менее 1")]
    public decimal? CarbsG { get; set; }

    public string? Composition { get; set; } 

   }
