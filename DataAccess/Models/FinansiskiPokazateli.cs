using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class FinansiskiPokazateli
{
    public int Id { get; set; }

    public int IzdavachId { get; set; }

    public int Godina { get; set; }

    public decimal? OperativnaDobivka { get; set; }

    public decimal? NetoDobivkaPoAkcija { get; set; }

    public decimal? KoefCenaDobivkaPoAkcija { get; set; }

    public decimal? KnigovodstvenaVrednostPoAkcija { get; set; }

    public decimal? KoefCenaKnigovodstvenaVrednostPoAkcija { get; set; }

    public decimal? DividendaPoAkcija { get; set; }

    public decimal? DividendenPrinos { get; set; }

    public DateTime DateModified { get; set; }

    public virtual Izdavachi Izdavach { get; set; } = null!;
}
