using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Uloga
{
    public int Id { get; set; }

    public string Ime { get; set; } = null!;

    public virtual ICollection<KorisnikUloga> KorisnikUlogas { get; set; } = new List<KorisnikUloga>();
}
