using ApiAfterDotnetCourse.Data;
using ApiAfterDotnetCourse.Data.Entities;
using ApiAfterDotnetCourse.Data.Interfaces;
using ApiAfterDotnetCourse.Data.SeedIdentityData;
using ApiAfterDotnetCourse.WebAPI.Services;
using ApiAfterDotnetCourse.WebAPI.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace ApiAfterDotnetCourse.WebAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApiAfterDotnetCourseDBContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("ApiAfterDotnetCourseDB"));
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>()
               .AddEntityFrameworkStores<ApiAfterDotnetCourseDBContext>()
               .AddDefaultTokenProviders();

            // Identity szabályok
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.User.RequireUniqueEmail = false;
                options.SignIn.RequireConfirmedAccount = true;
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // MailSettings
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            // Service-ek
            builder.Services.AddScoped<IRoleSeedService, RoleSeedService>();
            builder.Services.AddScoped<IUserSeedService, UserSeedService>();

            var app = builder.Build();

            // Role-ok seedelése
            var roleSeeder = app.Services.CreateScope().ServiceProvider.GetRequiredService<IRoleSeedService>();
            await roleSeeder.SeedRoleAsync();

            // Userek seedelése
            var userSeeder = app.Services.CreateScope().ServiceProvider.GetRequiredService<IUserSeedService>();
            await userSeeder.SeedUserAsync();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
