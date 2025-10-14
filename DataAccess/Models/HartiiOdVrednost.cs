using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class HartiiOdVrednost
{
    public int Id { get; set; }

    public string Isin { get; set; } = null!;

    public string Kod { get; set; } = null!;

    public int VkupenBrojAkcii { get; set; }

    public int TipHvid { get; set; }

    public int IzdavachId { get; set; }

    public virtual ICollection<DnevenPromet> DnevenPromets { get; set; } = new List<DnevenPromet>();

    public virtual Izdavach Izdavach { get; set; } = null!;

    public virtual ICollection<PortfolioPrinosi> PortfolioPrinosis { get; set; } = new List<PortfolioPrinosi>();

    public virtual TipHv TipHv { get; set; } = null!;

    public virtual ICollection<Transakcii> Transakciis { get; set; } = new List<Transakcii>();
}
