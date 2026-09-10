using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class AgentPoraki
{
    public int Id { get; set; }

    public int RazgovorId { get; set; }

    public string Uloga { get; set; } = null!;

    public string Sodrzina { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual AgentRazgovori Razgovor { get; set; } = null!;
}
