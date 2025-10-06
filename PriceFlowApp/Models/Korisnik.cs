using System;
using System.Collections.Generic;

namespace PriceFlowApp.Models;

public partial class Korisnik
{
    public int Id { get; set; }

    public string Ime { get; set; } = null!;

    public string Username { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Uloga { get; set; } = null!;

    public virtual ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();
}
