using ApiAfterDotnetCourse.Bll.Interfaces;
using ApiAfterDotnetCourse.Bll.Services;
using ApiAfterDotnetCourse.Data;
using ApiAfterDotnetCourse.Data.Entities;
using ApiAfterDotnetCourse.Data.Interfaces;
using ApiAfterDotnetCourse.Data.SeedIdentityData;
using ApiAfterDotnetCourse.WebAPI.Services;
using ApiAfterDotnetCourse.WebAPI.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(30); // Jelszó helyreállítási token érvényessége fél óra
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
            builder.Services.AddScoped<IUserService, UserService>();

            // JWT tokengenerálás
            builder.Services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
                    };
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Role-ok seedelése
            var roleSeeder = app.Services.CreateScope().ServiceProvider.GetRequiredService<IRoleSeedService>();
            await roleSeeder.SeedRoleAsync();

            // Userek seedelése
            var userSeeder = app.Services.CreateScope().ServiceProvider.GetRequiredService<IUserSeedService>();
            await userSeeder.SeedUserAsync();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(options =>
                {
                    // Az alábbi beállítással törölheted a felesleges végpontokat
                    options.PreSerializeFilters.Add((swaggerDoc, httpRequest) =>
                    {
                        // Például eltávolítjuk a '/' végpontot a Swagger dokumentációból
                        var path = "/"; // vagy a konkrét nem kívánt végpontot
                        swaggerDoc.Paths.Remove(path);
                    });
                });

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                    options.RoutePrefix = string.Empty; // A Swagger UI alapértelmezett URL-je
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.MapGet("/", () => Results.Redirect("/swagger"));

            app.Run();
        }
    }
}
