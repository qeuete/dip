using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; // Добавьте этот using

namespace APISportFoodStore.Models;

public partial class Order
{
    public int? IdOrder { get; set; }

    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public DateOnly DeliveryDate { get; set; }

    public int? OrderStatusId { get; set; }

    [ForeignKey("DeliveryTimeSlot")]
    public int? DeliverySlotId { get; set; }
    [JsonIgnore]
    public virtual DeliveryTimeSlot? DeliveryTimeSlot { get; set; }

    [ForeignKey("UserAddress")]
    public int? AddressId { get; set; }
    public virtual UserAddress? UserAddress { get; set; }

    [ForeignKey("UserCard")]
    public int? UserCardId { get; set; }
    [JsonIgnore]
    public virtual UserCard? UserCard { get; set; }

    [ForeignKey("CourierId ")]
    public int? CourierId { get; set; }

    public virtual ICollection<OrderDetail>? OrderDetails { get; set; } = new List<OrderDetail>();


}
