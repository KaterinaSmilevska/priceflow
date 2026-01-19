using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Portfolija
{
    public int Id { get; set; }

    public string Ime { get; set; } = null!;

    public string? Opis { get; set; }

    public int KorisnikId { get; set; }

    public DateTime DateModified { get; set; }

    public virtual Korisnici Korisnik { get; set; } = null!;

    public virtual ICollection<PortfolioPrinosi> PortfolioPrinosi { get; set; } = new List<PortfolioPrinosi>();

    public virtual ICollection<Transakcii> Transakcii { get; set; } = new List<Transakcii>();
}
