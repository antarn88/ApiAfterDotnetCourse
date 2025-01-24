using ApiAfterDotnetCourse.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiAfterDotnetCourse.Data;

public class ApiAfterDotnetCourseDBContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public ApiAfterDotnetCourseDBContext(DbContextOptions<ApiAfterDotnetCourseDBContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ApplicationUser konfiguráció
        modelBuilder.Entity<ApplicationUser>(builder =>
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.Id);
        });
    }
}
