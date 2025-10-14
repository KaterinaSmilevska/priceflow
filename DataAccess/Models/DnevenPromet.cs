using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class DnevenPromet
{
    public int Id { get; set; }

    public int Hvid { get; set; }

    public DateTime Datum { get; set; }

    public decimal? CenaPoslednaTransakcija { get; set; }

    public decimal? MaxCena { get; set; }

    public decimal? MinCena { get; set; }

    public decimal? ProsecnaCena { get; set; }

    public decimal? ProcentPromena { get; set; }

    public int? KolicinaIstrguvaniAkcii { get; set; }

    public int? PrometBestdenari { get; set; }

    public int? VkupenPrometDenari { get; set; }

    public virtual HartiiOdVrednost Hv { get; set; } = null!;
}
