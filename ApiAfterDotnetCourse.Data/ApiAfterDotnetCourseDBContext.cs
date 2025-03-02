using ApiAfterDotnetCourse.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiAfterDotnetCourse.Data;

public class ApiAfterDotnetCourseDBContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public ApiAfterDotnetCourseDBContext(DbContextOptions<ApiAfterDotnetCourseDBContext> options) : base(options) { }

    // Entitások felvétele
    public DbSet<UsedToken> UsedTokens { get; set; }
    public DbSet<Product> Products { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ApplicationUser konfiguráció
        modelBuilder.Entity<ApplicationUser>(builder =>
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.Id);
        });

        // Product konfiguráció
        modelBuilder.Entity<Product>(builder =>
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.ProductId); // Egyedi kulcs beállítása

            // Price mező pontosítása (pl. 18 teljes számjegy, 2 tizedesjegy)
            builder.Property(p => p.Price)
                .HasPrecision(18, 2)
                .IsRequired();
        });
    }

}
