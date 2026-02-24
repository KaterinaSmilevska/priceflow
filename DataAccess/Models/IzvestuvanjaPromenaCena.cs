using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class IzvestuvanjaPromenaCena
{
    public int Id { get; set; }

    public int KorisnikId { get; set; }

    public int Hvid { get; set; }

    public decimal ProcentPromena { get; set; }

    public DateTime DatumTrguvanje { get; set; }

    public string Poraka { get; set; } = null!;

    public bool Procitano { get; set; }

    public DateTime DateModified { get; set; }

    public virtual HartiiOdVrednost Hv { get; set; } = null!;

    public virtual Korisnici Korisnik { get; set; } = null!;
}
