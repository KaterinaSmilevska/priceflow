using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Sektor
{
    public int Id { get; set; }

    public string Ime { get; set; } = null!;

    public virtual ICollection<Izdavach> Izdavaches { get; set; } = new List<Izdavach>();
}
