using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Ulogi
{
    public int Id { get; set; }

    public string Ime { get; set; } = null!;

    public DateTime DateModified { get; set; }

    public virtual ICollection<KorisniciUlogi> KorisniciUlogi { get; set; } = new List<KorisniciUlogi>();
}
