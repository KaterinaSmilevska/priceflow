using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Brokeri
{
    public int Id { get; set; }

    public string Kompanija { get; set; } = null!;

    public decimal ProcentProvizija { get; set; }

    public DateTime DateModified { get; set; }
}
