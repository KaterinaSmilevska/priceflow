using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class TipHv
{
    public int Id { get; set; }

    public string Ime { get; set; } = null!;

    public DateTime DateModified { get; set; }

    public virtual ICollection<HartiiOdVrednost> HartiiOdVrednost { get; set; } = new List<HartiiOdVrednost>();
}
