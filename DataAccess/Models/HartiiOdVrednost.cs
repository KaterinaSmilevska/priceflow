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

    public DateTime DateModified { get; set; }

    public virtual ICollection<DnevenPromet> DnevenPromet { get; set; } = new List<DnevenPromet>();

    public virtual HvPromenaCena? HvPromenaCena { get; set; }

    public virtual Izdavachi Izdavach { get; set; } = null!;

    public virtual ICollection<PortfolioPrinosi> PortfolioPrinosi { get; set; } = new List<PortfolioPrinosi>();

    public virtual TipHv TipHv { get; set; } = null!;

    public virtual ICollection<Transakcii> Transakcii { get; set; } = new List<Transakcii>();
}
