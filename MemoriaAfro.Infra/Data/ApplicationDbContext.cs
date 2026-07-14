using MemoriaAfro.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MemoriaAfro.Infra.Data;

/// <summary>
/// Contexto do EF Core. Estende IdentityDbContext para trazer as tabelas
/// de autenticação (ApplicationIdentityUser/roles) junto com as entidades
/// de domínio definidas no Core.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationIdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<UploadMemoria> UploadsMemoria => Set<UploadMemoria>();
    public DbSet<HistoricoPontos> HistoricosPontos => Set<HistoricoPontos>();
    public DbSet<PontoInteresse> PontosInteresse => Set<PontoInteresse>();
    public DbSet<Documentario> Documentarios => Set<Documentario>();
    public DbSet<ArquivoImersivo> ArquivosImersivos => Set<ArquivoImersivo>();
    public DbSet<TutorialOficina> TutoriaisOficinas => Set<TutorialOficina>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Usuario>(entity =>
        {
            // Mesma PK do ApplicationIdentityUser (Id é string, gerado pelo Identity).
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).ValueGeneratedNever();
            entity.Property(u => u.NomeCompleto).HasMaxLength(150).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(256).IsRequired();
        });

        builder.Entity<UploadMemoria>(entity =>
        {
            entity.Property(u => u.NomeAutor).HasMaxLength(150).IsRequired();
            entity.Property(u => u.Titulo).HasMaxLength(200).IsRequired();
            entity.Property(u => u.Descricao).HasMaxLength(4000);

            entity.HasOne(u => u.Usuario)
                  .WithMany(us => us.Uploads)
                  .HasForeignKey(u => u.UsuarioId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<HistoricoPontos>(entity =>
        {
            entity.Property(h => h.Descricao).HasMaxLength(300).IsRequired();

            entity.HasOne(h => h.Usuario)
                  .WithMany(u => u.MovimentacoesPontos)
                  .HasForeignKey(h => h.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(h => h.UploadMemoria)
                  .WithMany()
                  .HasForeignKey(h => h.UploadMemoriaId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<PontoInteresse>(entity =>
        {
            entity.Property(p => p.Nome).HasMaxLength(150).IsRequired();
            entity.Property(p => p.Descricao).HasMaxLength(2000);
        });

        builder.Entity<Documentario>(entity =>
        {
            entity.Property(d => d.Titulo).HasMaxLength(200).IsRequired();
            entity.Property(d => d.VideoUrl).HasMaxLength(500).IsRequired();
        });

        builder.Entity<ArquivoImersivo>(entity =>
        {
            entity.Property(a => a.Titulo).HasMaxLength(200).IsRequired();
            entity.Property(a => a.VideoUrl).HasMaxLength(500).IsRequired();

            entity.HasOne(a => a.PontoInteresse)
                  .WithMany(p => p.ArquivosImersivos)
                  .HasForeignKey(a => a.PontoInteresseId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<TutorialOficina>(entity =>
        {
            entity.Property(t => t.Titulo).HasMaxLength(200).IsRequired();
            entity.Property(t => t.VideoUrl).HasMaxLength(500).IsRequired();
        });
    }
}
