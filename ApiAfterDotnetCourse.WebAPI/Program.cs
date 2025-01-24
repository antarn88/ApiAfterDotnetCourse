using ApiAfterDotnetCourse.Data;
using ApiAfterDotnetCourse.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApiAfterDotnetCourse.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
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

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // app.UseRouting(); // TODO Ez kell?
            // app.UseAuthentication(); // TODO Ez kell?
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
