using System;
using System.Collections.Generic;

namespace PriceFlowApp.Models;

public partial class TipHv
{
    public int Id { get; set; }

    public string Ime { get; set; } = null!;

    public virtual ICollection<HartiiOdVrednost> HartiiOdVrednosts { get; set; } = new List<HartiiOdVrednost>();
}
