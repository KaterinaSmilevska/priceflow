using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Sektori
{
    public int Id { get; set; }

    public string Ime { get; set; } = null!;

    public DateTime DateModified { get; set; }

    public virtual ICollection<Izdavachi> Izdavachi { get; set; } = new List<Izdavachi>();
}
