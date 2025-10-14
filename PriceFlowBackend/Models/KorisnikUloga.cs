using System;
using System.Collections.Generic;

namespace PriceFlowApp.Models;

public partial class KorisnikUloga
{
    public int Id { get; set; }

    public int KorisnikId { get; set; }

    public int UlogaId { get; set; }

    public virtual Korisnik Korisnik { get; set; } = null!;

    public virtual Uloga Uloga { get; set; } = null!;
}
