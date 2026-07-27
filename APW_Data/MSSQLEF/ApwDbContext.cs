using APW.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection.Emit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace APW.Data.MSSQLEF;

// DbContext principal de la aplicacion, mapea los Models a la BD APW
public class ApwDbContext : DbContext
{
    public ApwDbContext(DbContextOptions<ApwDbContext> options) : base(options)
    {
    }

    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Source> Sources { get; set; }
    public DbSet<SourceItem> SourceItems { get; set; }
    public DbSet<Setting> Settings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuracion de Role
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configuracion de User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);

            // Relacion User -> Role
            entity.HasOne(e => e.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(e => e.RoleId);
        });

        // Configuracion de Source
        modelBuilder.Entity<Source>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Url).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ComponentType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.RequiresSecret).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
        });

        // Configuracion de SourceItem
        modelBuilder.Entity<SourceItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Json).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            // Relacion SourceItem -> Source
            entity.HasOne(e => e.Source)
                .WithMany(s => s.SourceItems)
                .HasForeignKey(e => e.SourceId);
        });

        // Configuracion de Setting
        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.KeyName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.KeyValue).HasMaxLength(500).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);

            // Relacion Setting -> Source (opcional, puede ser NULL)
            entity.HasOne(e => e.Source)
                .WithMany(s => s.Settings)
                .HasForeignKey(e => e.SourceId)
                .IsRequired(false);
        });
    }
}