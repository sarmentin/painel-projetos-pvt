using Microsoft.EntityFrameworkCore;
using PainelProjetos.Api.Models;

namespace PainelProjetos.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Projeto> Projetos => Set<Projeto>();
    public DbSet<Analista> Analistas => Set<Analista>();
    public DbSet<Alocacao> Alocacoes => Set<Alocacao>();
    public DbSet<Etapa> Etapas => Set<Etapa>();
    public DbSet<Apontamento> Apontamentos => Set<Apontamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // codigoRm: tamanho limitado e único, porque a sincronização depende dele
        modelBuilder.Entity<Cliente>().Property(c => c.CodigoRm).HasMaxLength(50);
        modelBuilder.Entity<Cliente>().HasIndex(c => c.CodigoRm).IsUnique();
        modelBuilder.Entity<Projeto>().Property(p => p.CodigoRm).HasMaxLength(50);
        modelBuilder.Entity<Projeto>().HasIndex(p => p.CodigoRm).IsUnique();
        modelBuilder.Entity<Analista>().Property(a => a.CodigoRm).HasMaxLength(50);
        modelBuilder.Entity<Analista>().HasIndex(a => a.CodigoRm).IsUnique();
        modelBuilder.Entity<Apontamento>().Property(a => a.CodigoRm).HasMaxLength(50);

        // Precisão das horas e do peso
        modelBuilder.Entity<Projeto>().Property(p => p.HorasVendidas).HasPrecision(10, 2);
        modelBuilder.Entity<Alocacao>().Property(a => a.HorasPlanejadas).HasPrecision(10, 2);
        modelBuilder.Entity<Apontamento>().Property(a => a.Horas).HasPrecision(5, 2);
        modelBuilder.Entity<Etapa>().Property(e => e.Peso).HasPrecision(5, 2);

        // Enums gravados como texto, para o banco ficar legível
        modelBuilder.Entity<Projeto>().Property(p => p.Situacao).HasConversion<string>().HasMaxLength(20);
        modelBuilder.Entity<Apontamento>().Property(a => a.StatusSincronizacao).HasConversion<string>().HasMaxLength(20);
    }
}