using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PriceFlowApp.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AplikativniParametri> AplikativniParametris { get; set; }

    public virtual DbSet<Broker> Brokers { get; set; }

    public virtual DbSet<DnevenPromet> DnevenPromets { get; set; }

    public virtual DbSet<FinansiskiPokazateli> FinansiskiPokazatelis { get; set; }

    public virtual DbSet<HartiiOdVrednost> HartiiOdVrednosts { get; set; }

    public virtual DbSet<Izdavach> Izdavaches { get; set; }

    public virtual DbSet<Korisnik> Korisniks { get; set; }

    public virtual DbSet<Portfolio> Portfolios { get; set; }

    public virtual DbSet<PortfolioPrinosi> PortfolioPrinosis { get; set; }

    public virtual DbSet<Sektor> Sektors { get; set; }

    public virtual DbSet<TipHv> TipHvs { get; set; }

    public virtual DbSet<Transakcii> Transakciis { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:PriceFlowDatabase");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Macedonian_FYROM_100_CI_AS");

        modelBuilder.Entity<AplikativniParametri>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_AplikativniParametri");

            entity.ToTable("AplikativniParametri");

            entity.Property(e => e.BerzanskaProvizija).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Cdhvprovizija)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("CDHVProvizija");
            entity.Property(e => e.PersonalenDanok).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Broker>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_Broker");

            entity.ToTable("Broker");

            entity.HasIndex(e => e.Kompanija, "un_Kompanija").IsUnique();

            entity.Property(e => e.Kompanija).HasMaxLength(100);
            entity.Property(e => e.ProcentProvizija).HasColumnType("decimal(18, 3)");
        });

        modelBuilder.Entity<DnevenPromet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_DnevenPromet");

            entity.ToTable("DnevenPromet");

            entity.HasIndex(e => new { e.Datum, e.Hvid }, "un_DnevenPromet_Datum_HVId").IsUnique();

            entity.Property(e => e.CenaPoslednaTransakcija).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.Hvid).HasColumnName("HVId");
            entity.Property(e => e.MaxCena).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MinCena).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProcentPromena).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PrometBestdenari).HasColumnName("PrometBESTDenari");
            entity.Property(e => e.ProsecnaCena).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Hv).WithMany(p => p.DnevenPromets)
                .HasForeignKey(d => d.Hvid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_DnevenPromet_HartiiOdVrednost");
        });

        modelBuilder.Entity<FinansiskiPokazateli>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_FinansiskiPokazateli");

            entity.ToTable("FinansiskiPokazateli");

            entity.Property(e => e.DividendaPoAkcija).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DividendenPrinos).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.KnigovodstvenaVrednostPoAkcija).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.KoefCenaDobivkaPoAkcija).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.KoefCenaKnigovodstvenaVrednostPoAkcija).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NetoDobivkaPoAkcija).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OperativnaDobivka).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Izdavach).WithMany(p => p.FinansiskiPokazatelis)
                .HasForeignKey(d => d.IzdavachId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_FinansiskiPokazateli_Izdavach");
        });

        modelBuilder.Entity<HartiiOdVrednost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_HartiiOdVrednost");

            entity.ToTable("HartiiOdVrednost");

            entity.HasIndex(e => e.Kod, "un_Kod").IsUnique();

            entity.Property(e => e.Isin)
                .HasMaxLength(12)
                .HasColumnName("ISIN");
            entity.Property(e => e.Kod).HasMaxLength(50);
            entity.Property(e => e.TipHvid).HasColumnName("TipHVId");

            entity.HasOne(d => d.Izdavach).WithMany(p => p.HartiiOdVrednosts)
                .HasForeignKey(d => d.IzdavachId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_HartiiOdVrednost_Izdavach");

            entity.HasOne(d => d.TipHv).WithMany(p => p.HartiiOdVrednosts)
                .HasForeignKey(d => d.TipHvid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_HartiiOdVrednost_TipHV");
        });

        modelBuilder.Entity<Izdavach>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_Izdavach");

            entity.ToTable("Izdavach");

            entity.HasIndex(e => e.Ime, "un_Izdavach_Ime").IsUnique();

            entity.Property(e => e.Drzava).HasMaxLength(100);
            entity.Property(e => e.Grad).HasMaxLength(100);
            entity.Property(e => e.Ime).HasMaxLength(100);

            entity.HasOne(d => d.Sektor).WithMany(p => p.Izdavaches)
                .HasForeignKey(d => d.SektorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Izdavach_Sektor");
        });

        modelBuilder.Entity<Korisnik>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_Korisnik");

            entity.ToTable("Korisnik");

            entity.HasIndex(e => e.Username, "un_Username").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Ime).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(48);
            entity.Property(e => e.Uloga).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<Portfolio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_Portfolio");

            entity.ToTable("Portfolio");

            entity.HasIndex(e => new { e.KorisnikId, e.Ime }, "un_Portfolio_KorisnikId_Ime").IsUnique();

            entity.Property(e => e.Ime).HasMaxLength(50);
            entity.Property(e => e.Opis).HasMaxLength(100);

            entity.HasOne(d => d.Korisnik).WithMany(p => p.Portfolios)
                .HasForeignKey(d => d.KorisnikId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Portfolio_Korisnik");
        });

        modelBuilder.Entity<PortfolioPrinosi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_PortfolioPrinosi");

            entity.ToTable("PortfolioPrinosi");

            entity.Property(e => e.Danok).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Hvid).HasColumnName("HVId");
            entity.Property(e => e.NetoIznos).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Hv).WithMany(p => p.PortfolioPrinosis)
                .HasForeignKey(d => d.Hvid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_PortfolioPrinosi_HartiiOdVrednost");

            entity.HasOne(d => d.Portfolio).WithMany(p => p.PortfolioPrinosis)
                .HasForeignKey(d => d.PortfolioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_PortfolioPrinosi_Portfolio");
        });

        modelBuilder.Entity<Sektor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_Sektor");

            entity.ToTable("Sektor");

            entity.HasIndex(e => e.Ime, "un_Sektor_Ime").IsUnique();

            entity.Property(e => e.Ime).HasMaxLength(100);
        });

        modelBuilder.Entity<TipHv>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_TipHV");

            entity.ToTable("TipHV");

            entity.HasIndex(e => e.Ime, "un_TipHV_Ime").IsUnique();

            entity.Property(e => e.Ime).HasMaxLength(10);
        });

        modelBuilder.Entity<Transakcii>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_Transakcii");

            entity.ToTable("Transakcii");

            entity.Property(e => e.BerzanskaProvizija).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BrokerskaProvizija).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Cdhvprovizija)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("CDHVProvizija");
            entity.Property(e => e.Hvid).HasColumnName("HVId");
            entity.Property(e => e.Iznos).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Realna).HasMaxLength(2);
            entity.Property(e => e.TipTransakcija).HasMaxLength(20);

            entity.HasOne(d => d.Hv).WithMany(p => p.Transakciis)
                .HasForeignKey(d => d.Hvid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Transakcii_HartiiOdVrednost");

            entity.HasOne(d => d.Portfolio).WithMany(p => p.Transakciis)
                .HasForeignKey(d => d.PortfolioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Transakcii_Portfolio");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
