using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Transakcii
{
    public int Id { get; set; }

    public int PortfolioId { get; set; }

    public int KolicinaAkcii { get; set; }

    public int EdinecnaCenaAkcija { get; set; }

    public DateOnly Datum { get; set; }

    public decimal Iznos { get; set; }

    public decimal BerzanskaProvizija { get; set; }

    public decimal BrokerskaProvizija { get; set; }

    public decimal Cdhvprovizija { get; set; }

    public string TipTransakcija { get; set; } = null!;

    public string Realna { get; set; } = null!;

    public int Hvid { get; set; }

    public virtual HartiiOdVrednost Hv { get; set; } = null!;

    public virtual Portfolio Portfolio { get; set; } = null!;
}
