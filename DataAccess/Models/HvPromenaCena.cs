using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class HvPromenaCena
{
    public int Id { get; set; }

    public int KorisnikId { get; set; }

    public int Hvid { get; set; }

    public decimal DolnaGranica { get; set; }

    public decimal GornaGranica { get; set; }

    public DateTime DateModified { get; set; }

    public virtual HartiiOdVrednost Hv { get; set; } = null!;

    public virtual Korisnici Korisnik { get; set; } = null!;
}
