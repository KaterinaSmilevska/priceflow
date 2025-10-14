using System;
using System.Collections.Generic;

namespace PriceFlowApp.Models;

public partial class Broker
{
    public int Id { get; set; }

    public string Kompanija { get; set; } = null!;

    public decimal ProcentProvizija { get; set; }
}
