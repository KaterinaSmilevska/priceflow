using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class KorisniciUlogi
{
    public int Id { get; set; }

    public int KorisnikId { get; set; }

    public int UlogaId { get; set; }

    public DateTime DateModified { get; set; }

    public virtual Korisnici Korisnik { get; set; } = null!;

    public virtual Ulogi Uloga { get; set; } = null!;
}
