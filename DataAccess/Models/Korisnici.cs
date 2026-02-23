using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Korisnici
{
    public int Id { get; set; }

    public string Ime { get; set; } = null!;

    public string Username { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool IsEmailVerified { get; set; }

    public Guid? EmailVerificationToken { get; set; }

    public Guid? ResetPasswordToken { get; set; }

    public DateTime? ResetPasswordTokenExpiry { get; set; }

    public DateTime DateModified { get; set; }

    public virtual ICollection<HvPromenaCena> HvPromenaCena { get; set; } = new List<HvPromenaCena>();

    public virtual ICollection<KorisniciUlogi> KorisniciUlogi { get; set; } = new List<KorisniciUlogi>();

    public virtual ICollection<Portfolija> Portfolija { get; set; } = new List<Portfolija>();
}
