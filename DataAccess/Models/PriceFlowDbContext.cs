using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Models;

public partial class PriceFlowDbContext : DbContext
{
    public PriceFlowDbContext()
    {
    }

    public PriceFlowDbContext(DbContextOptions<PriceFlowDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AplikativniParametri> AplikativniParametri { get; set; }

    public virtual DbSet<Brokeri> Brokeri { get; set; }

    public virtual DbSet<DnevenPromet> DnevenPromet { get; set; }

    public virtual DbSet<FinansiskiPokazateli> FinansiskiPokazateli { get; set; }

    public virtual DbSet<HartiiOdVrednost> HartiiOdVrednost { get; set; }

    public virtual DbSet<Izdavachi> Izdavachi { get; set; }

    public virtual DbSet<Korisnici> Korisnici { get; set; }

    public virtual DbSet<KorisniciUlogi> KorisniciUlogi { get; set; }

    public virtual DbSet<Portfolija> Portfolija { get; set; }

    public virtual DbSet<PortfolioPrinosi> PortfolioPrinosi { get; set; }

    public virtual DbSet<Sektori> Sektori { get; set; }

    public virtual DbSet<TipHv> TipHv { get; set; }

    public virtual DbSet<Transakcii> Transakcii { get; set; }

    public virtual DbSet<Ulogi> Ulogi { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:PriceFlowDatabase");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Macedonian_FYROM_100_CI_AS");

        modelBuilder.Entity<AplikativniParametri>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_AplikativniParametri");

            entity.Property(e => e.BerzanskaProvizija).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Cdhvprovizija)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("CDHVProvizija");
            entity.Property(e => e.PersonalenDanok).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Brokeri>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_Brokeri");

            entity.HasIndex(e => e.Kompanija, "UX_Brokeri_Kompanija")
                .IsUnique()
                .HasFilter("([Kompanija] IS NOT NULL)");

            entity.HasIndex(e => e.Kompanija, "un_Brokeri_Kompanija").IsUnique();

            entity.Property(e => e.Kompanija).HasMaxLength(100);
            entity.Property(e => e.ProcentProvizija).HasColumnType("decimal(18, 3)");
        });

        modelBuilder.Entity<DnevenPromet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_DnevenPromet");

            entity.HasIndex(e => e.Hvid, "IX_DnevenPromet_HVId").HasFilter("([HVId] IS NOT NULL)");

            entity.HasIndex(e => new { e.Datum, e.Hvid }, "un_DnevenPromet_Datum_HVId").IsUnique();

            entity.Property(e => e.CenaPoslednaTransakcija).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.Hvid).HasColumnName("HVId");
            entity.Property(e => e.MaxCena).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MinCena).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProcentPromena).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PrometBestdenari).HasColumnName("PrometBESTDenari");
            entity.Property(e => e.ProsecnaCena).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Hv).WithMany(p => p.DnevenPromet)
                .HasForeignKey(d => d.Hvid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_DnevenPromet_HartiiOdVrednost");
        });

        modelBuilder.Entity<FinansiskiPokazateli>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_FinansiskiPokazateli");

            entity.HasIndex(e => e.IzdavachId, "IX_FinansiskiPokazateli_IzdavachId").HasFilter("([IzdavachId] IS NOT NULL)");

            entity.Property(e => e.DividendaPoAkcija).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DividendenPrinos).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.KnigovodstvenaVrednostPoAkcija).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.KoefCenaDobivkaPoAkcija).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.KoefCenaKnigovodstvenaVrednostPoAkcija).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NetoDobivkaPoAkcija).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OperativnaDobivka).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Izdavach).WithMany(p => p.FinansiskiPokazateli)
                .HasForeignKey(d => d.IzdavachId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_FinansiskiPokazateli_Izdavachi");
        });

        modelBuilder.Entity<HartiiOdVrednost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_HartiiOdVrednost");

            entity.HasIndex(e => e.IzdavachId, "IX_HartiiOdVrednost_IzdavachId").HasFilter("([IzdavachId] IS NOT NULL)");

            entity.HasIndex(e => e.TipHvid, "IX_HartiiOdVrednost_TipHVId").HasFilter("([TipHVId] IS NOT NULL)");

            entity.HasIndex(e => e.Kod, "UX_HartiiOdVrednost_Kod")
                .IsUnique()
                .HasFilter("([Kod] IS NOT NULL)");

            entity.HasIndex(e => e.Kod, "un_HartiiOdVrednost_Kod").IsUnique();

            entity.Property(e => e.Isin)
                .HasMaxLength(12)
                .HasColumnName("ISIN");
            entity.Property(e => e.Kod).HasMaxLength(50);
            entity.Property(e => e.TipHvid).HasColumnName("TipHVId");

            entity.HasOne(d => d.Izdavach).WithMany(p => p.HartiiOdVrednost)
                .HasForeignKey(d => d.IzdavachId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_HartiiOdVrednost_Izdavachi");

            entity.HasOne(d => d.TipHv).WithMany(p => p.HartiiOdVrednost)
                .HasForeignKey(d => d.TipHvid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_HartiiOdVrednost_TipHV");
        });

        modelBuilder.Entity<Izdavachi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_Izdavachi");

            entity.HasIndex(e => e.SektorId, "IX_Izdavachi_SektorId").HasFilter("([SektorId] IS NOT NULL)");

            entity.HasIndex(e => e.Ime, "UX_Izdavachi_Ime")
                .IsUnique()
                .HasFilter("([Ime] IS NOT NULL)");

            entity.HasIndex(e => e.Ime, "un_Izdavachi_Ime").IsUnique();

            entity.Property(e => e.Drzava).HasMaxLength(100);
            entity.Property(e => e.Grad).HasMaxLength(100);
            entity.Property(e => e.Ime).HasMaxLength(100);

            entity.HasOne(d => d.Sektor).WithMany(p => p.Izdavachi)
                .HasForeignKey(d => d.SektorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Izdavachi_Sektori");
        });

        modelBuilder.Entity<Korisnici>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_Korisnici");

            entity.HasIndex(e => e.Ime, "IX_Korisnici_Ime").HasFilter("([Ime] IS NOT NULL)");

            entity.HasIndex(e => e.EmailVerificationToken, "UX_Korisnici_EmailVerificationToken")
                .IsUnique()
                .HasFilter("([EmailVerificationToken] IS NOT NULL)");

            entity.HasIndex(e => e.ResetPasswordToken, "UX_Korisnici_ResetPasswordToken")
                .IsUnique()
                .HasFilter("([ResetPasswordToken] IS NOT NULL)");

            entity.HasIndex(e => e.Username, "UX_Korisnici_Username")
                .IsUnique()
                .HasFilter("([Username] IS NOT NULL)");

            entity.HasIndex(e => e.Username, "un_Korisnici_Username").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Ime).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(48);
            entity.Property(e => e.ResetPasswordTokenExpiry).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<KorisniciUlogi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_KorisniciUlogi");

            entity.HasIndex(e => e.KorisnikId, "IX_KorisniciUlogi_KorisnikId").HasFilter("([KorisnikId] IS NOT NULL)");

            entity.HasIndex(e => e.UlogaId, "IX_KorisniciUlogi_UlogaId").HasFilter("([UlogaId] IS NOT NULL)");

            entity.HasOne(d => d.Korisnik).WithMany(p => p.KorisniciUlogi)
                .HasForeignKey(d => d.KorisnikId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_KorisniciUlogi_Korisnici");

            entity.HasOne(d => d.Uloga).WithMany(p => p.KorisniciUlogi)
                .HasForeignKey(d => d.UlogaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_KorisniciUlogi_Ulogi");
        });

        modelBuilder.Entity<Portfolija>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_Portfolija");

            entity.HasIndex(e => e.KorisnikId, "IX_Portfolija_KorisnikId").HasFilter("([KorisnikId] IS NOT NULL)");

            entity.HasIndex(e => e.Ime, "UX_Portfolija_Ime")
                .IsUnique()
                .HasFilter("([Ime] IS NOT NULL)");

            entity.HasIndex(e => new { e.KorisnikId, e.Ime }, "un_Portfolija_KorisnikId_Ime").IsUnique();

            entity.Property(e => e.Ime).HasMaxLength(50);
            entity.Property(e => e.Opis).HasMaxLength(100);

            entity.HasOne(d => d.Korisnik).WithMany(p => p.Portfolija)
                .HasForeignKey(d => d.KorisnikId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Portfolija_Korisnici");
        });

        modelBuilder.Entity<PortfolioPrinosi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_PortfolioPrinosi");

            entity.HasIndex(e => e.Hvid, "IX_PortfolioPrinosi_HVId").HasFilter("([HVId] IS NOT NULL)");

            entity.HasIndex(e => e.PortfolioId, "IX_PortfolioPrinosi_PortfolioId").HasFilter("([PortfolioId] IS NOT NULL)");

            entity.Property(e => e.Danok).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Hvid).HasColumnName("HVId");
            entity.Property(e => e.NetoIznos).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Hv).WithMany(p => p.PortfolioPrinosi)
                .HasForeignKey(d => d.Hvid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_PortfolioPrinosi_HartiiOdVrednost");

            entity.HasOne(d => d.Portfolio).WithMany(p => p.PortfolioPrinosi)
                .HasForeignKey(d => d.PortfolioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_PortfolioPrinosi_Portfolija");
        });

        modelBuilder.Entity<Sektori>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_Sektori");

            entity.HasIndex(e => e.Ime, "UX_Sektori_Ime")
                .IsUnique()
                .HasFilter("([Ime] IS NOT NULL)");

            entity.HasIndex(e => e.Ime, "un_Sektori_Ime").IsUnique();

            entity.Property(e => e.Ime).HasMaxLength(100);
        });

        modelBuilder.Entity<TipHv>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_TipHV");

            entity.ToTable("TipHV");

            entity.HasIndex(e => e.Ime, "UX_TipHV_Ime")
                .IsUnique()
                .HasFilter("([Ime] IS NOT NULL)");

            entity.HasIndex(e => e.Ime, "un_TipHV_Ime").IsUnique();

            entity.Property(e => e.Ime).HasMaxLength(10);
        });

        modelBuilder.Entity<Transakcii>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_Transakcii");

            entity.HasIndex(e => e.Hvid, "IX_Transakcii_HVId").HasFilter("([HVId] IS NOT NULL)");

            entity.HasIndex(e => e.PortfolioId, "IX_Transakcii_PortfolioId").HasFilter("([PortfolioId] IS NOT NULL)");

            entity.Property(e => e.BerzanskaProvizija).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BrokerskaProvizija).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Cdhvprovizija)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("CDHVProvizija");
            entity.Property(e => e.Hvid).HasColumnName("HVId");
            entity.Property(e => e.Iznos).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Realna).HasMaxLength(2);
            entity.Property(e => e.TipTransakcija).HasMaxLength(20);

            entity.HasOne(d => d.Hv).WithMany(p => p.Transakcii)
                .HasForeignKey(d => d.Hvid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Transakcii_HartiiOdVrednost");

            entity.HasOne(d => d.Portfolio).WithMany(p => p.Transakcii)
                .HasForeignKey(d => d.PortfolioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Transakcii_Portfolija");
        });

        modelBuilder.Entity<Ulogi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_Ulogi");

            entity.HasIndex(e => e.Ime, "UX_Ulogi_Ime")
                .IsUnique()
                .HasFilter("([Ime] IS NOT NULL)");

            entity.HasIndex(e => e.Ime, "un_Ulogi_Ime").IsUnique();

            entity.Property(e => e.Ime).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
