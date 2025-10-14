using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class PortfolioPrinosi
{
    public int Id { get; set; }

    public DateOnly Datum { get; set; }

    public decimal NetoIznos { get; set; }

    public decimal Danok { get; set; }

    public int PortfolioId { get; set; }

    public int Hvid { get; set; }

    public virtual HartiiOdVrednost Hv { get; set; } = null!;

    public virtual Portfolio Portfolio { get; set; } = null!;
}
