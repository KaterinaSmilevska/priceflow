using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Izdavach
{
    public int Id { get; set; }

    public string Ime { get; set; } = null!;

    public string Grad { get; set; } = null!;

    public string Drzava { get; set; } = null!;

    public int SektorId { get; set; }

    public virtual ICollection<FinansiskiPokazateli> FinansiskiPokazatelis { get; set; } = new List<FinansiskiPokazateli>();

    public virtual ICollection<HartiiOdVrednost> HartiiOdVrednosts { get; set; } = new List<HartiiOdVrednost>();

    public virtual Sektor Sektor { get; set; } = null!;
}
