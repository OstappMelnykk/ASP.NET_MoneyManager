using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;
using WebApplication1.Models;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            var path = Directory.GetCurrentDirectory();
            builder.Services.AddSingleton<ILoggerFactory>(new LoggerFactory().AddFile($"{path}\\Logs\\Log.log"));






            string connection = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services
                .AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connection));


            builder.Services
                .AddIdentity<Person, IdentityRole>(config =>
                {
                    config.Password.RequireDigit = false;
                    config.Password.RequireLowercase = false;
                    config.Password.RequireNonAlphanumeric = false;
                    config.Password.RequireUppercase = false;
                    config.Password.RequiredLength = 4;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();




            builder.Services.ConfigureApplicationCookie(config =>
            {
                config.LoginPath = "/Account/Login";
                config.AccessDeniedPath = "/Default/AccessDenied";
            });

            builder.Services.AddAuthorization(options => {
                options.AddPolicy("User", b => b.RequireAssertion(x =>
                                                x.User.HasClaim(ClaimTypes.Role, "User") ||
                                                x.User.HasClaim(ClaimTypes.Role, "Administrator")));

                options.AddPolicy("Administrator", b => b.RequireAssertion(x =>
                                                x.User.HasClaim(ClaimTypes.Role, "Administrator")));
            });


            builder.Services.AddControllersWithViews();

            var app = builder.Build();


            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Default/Error");
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


         /*   
*/



            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Default}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
