using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class AgentKoristenje
{
    public int Id { get; set; }

    public int KorisnikId { get; set; }

    public DateOnly Datum { get; set; }

    public int BrojPoraki { get; set; }

    public virtual Korisnici Korisnik { get; set; } = null!;
}
