using Microsoft.EntityFrameworkCore;

namespace ShopAdmin.Models;

public partial class ShopDbContext : DbContext
{
    public ShopDbContext()
    {
    }

    public ShopDbContext(DbContextOptions<ShopDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SPermission> SPermissions { get; set; }

    public virtual DbSet<SRole> SRoles { get; set; }

    public virtual DbSet<SRolePermission> SRolePermissions { get; set; }

    public virtual DbSet<SUser> SUsers { get; set; }

    public virtual DbSet<SUserRole> SUserRoles { get; set; }
    public virtual DbSet<Collection> Collections { get; set; }
    public virtual DbSet<OrderItem> OrderItems { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<ProductVariant> ProductVariants { get; set; }
    //public virtual DbSet<Variant> Variants { get; set; }
    public virtual DbSet<ProductImage> ProductImages { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProductVariantValue> ProductVariantValues { get; set; }
    public virtual DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
    public virtual DbSet<ProductAttribute> ProductAttributes { get; set; }
    public virtual DbSet<SCoupon> SCoupons { get; set; }
    public virtual DbSet<SNew> SNews { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SPermission>(entity =>
        {
            entity.ToTable("SPermission");

            entity.Property(e => e.Action).HasMaxLength(20);
            entity.Property(e => e.ActionName).HasMaxLength(50);
            entity.Property(e => e.Controller).HasMaxLength(20);
            entity.Property(e => e.ControllerName).HasMaxLength(100);
        });

        modelBuilder.Entity<SRole>(entity =>
        {
            entity.ToTable("SRole");

            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.Ma).HasMaxLength(50);
            entity.Property(e => e.Ten).HasMaxLength(200);
        });

        modelBuilder.Entity<SRolePermission>(entity =>
        {
            entity.ToTable("SRolePermission");

            entity.HasOne(d => d.Permission).WithMany(p => p.SRolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SRolePermission_SPermission");

            entity.HasOne(d => d.Role).WithMany(p => p.SRolePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SRolePermission_SRole");

            entity.HasOne(d => d.User).WithMany(p => p.SRolePermissions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SRolePermission_SUser");
        });

        modelBuilder.Entity<SUser>(entity =>
        {
            entity.ToTable("SUser");

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.GioiTinh).HasMaxLength(5);
            entity.Property(e => e.HoTen)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.NgaySinh).HasColumnType("datetime");
            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.SDT)
                .HasMaxLength(13)
                .HasColumnName("SDT");
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<SUserRole>(entity =>
        {
            entity.ToTable("SUserRole");

            entity.HasOne(d => d.Role).WithMany(p => p.SUserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_SUserRole_SRole");

            entity.HasOne(d => d.User).WithMany(p => p.SUserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_SUserRole_SUser");
        });

        modelBuilder.Entity<Product>()
        .HasMany(p => p.Attributes)
        .WithOne(a => a.Product)
        .HasForeignKey(a => a.ProductId);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.Variants)
            .WithOne(v => v.Product)
            .HasForeignKey(v => v.ProductId);

        modelBuilder.Entity<ProductAttribute>()
            .HasMany(a => a.Values)
            .WithOne(v => v.ProductAttribute)
            .HasForeignKey(v => v.ProductAttributeId);

        modelBuilder.Entity<ProductVariant>()
            .HasMany(v => v.VariantValues)
            .WithOne(vv => vv.ProductVariant)
            .HasForeignKey(vv => vv.ProductVariantId);

        modelBuilder.Entity<ProductAttributeValue>()
            .HasMany(v => v.VariantValues)
            .WithOne(vv => vv.ProductAttributeValue)
            .HasForeignKey(vv => vv.ProductAttributeValueId);
        modelBuilder.Entity<SCoupon>()
            .HasKey(v => v.Code);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
