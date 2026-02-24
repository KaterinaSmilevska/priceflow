using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class IzvestuvanjaPortfolija
{
    public int Id { get; set; }

    public int PortfolioId { get; set; }

    public bool Ovozmozeno { get; set; }

    public string Frekvencija { get; set; } = null!;

    public DateTime? PoslednoIsprateno { get; set; }

    public virtual Portfolija Portfolio { get; set; } = null!;
}
