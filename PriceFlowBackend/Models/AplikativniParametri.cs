using System;
using System.Collections.Generic;

namespace PriceFlowApp.Models;

public partial class AplikativniParametri
{
    public int Id { get; set; }

    public decimal PersonalenDanok { get; set; }

    public decimal BerzanskaProvizija { get; set; }

    public decimal Cdhvprovizija { get; set; }
}
