using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Portfolio
{
    public int Id { get; set; }

    public string Ime { get; set; } = null!;

    public string? Opis { get; set; }

    public int KorisnikId { get; set; }

    public virtual Korisnik Korisnik { get; set; } = null!;

    public virtual ICollection<PortfolioPrinosi> PortfolioPrinosis { get; set; } = new List<PortfolioPrinosi>();

    public virtual ICollection<Transakcii> Transakciis { get; set; } = new List<Transakcii>();
}
