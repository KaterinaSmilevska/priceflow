using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Izdavachi
{
    public int Id { get; set; }

    public string Ime { get; set; } = null!;

    public string Grad { get; set; } = null!;

    public string Drzava { get; set; } = null!;

    public int SektorId { get; set; }

    public virtual ICollection<FinansiskiPokazateli> FinansiskiPokazateli { get; set; } = new List<FinansiskiPokazateli>();

    public virtual ICollection<HartiiOdVrednost> HartiiOdVrednost { get; set; } = new List<HartiiOdVrednost>();

    public virtual Sektori Sektor { get; set; } = null!;
}
