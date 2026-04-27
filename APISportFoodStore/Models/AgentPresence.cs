using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APISportFoodStore.Models;

public partial class AgentPresence
{
    [Key]                       // <- EF поймет, что это PK
    public int? Id { get; set; }

    public int? AgentUserId { get; set; }

    public bool IsOnline { get; set; }

    public int Capacity { get; set; }

    public int CurrentActive { get; set; }

    public DateTime UpdatedAt { get; set; }
}