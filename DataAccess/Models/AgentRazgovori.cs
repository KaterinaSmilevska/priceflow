using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class AgentRazgovori
{
    public int Id { get; set; }

    public int KorisnikId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<AgentPoraki> AgentPoraki { get; set; } = new List<AgentPoraki>();

    public virtual Korisnici Korisnik { get; set; } = null!;
}
