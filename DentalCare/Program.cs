using DentalCare.Models;
using DentalCare.Services;
using Microsoft.EntityFrameworkCore;

namespace DentalCare
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<DentalcareContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DBDefault")));

            builder.Services.AddScoped<NurseService>();
            builder.Services.AddScoped<DoctorService>();
            builder.Services.AddScoped<ReceptionistService>();
            builder.Services.AddScoped<FacultyService>();
            builder.Services.AddScoped<CustomerService>();
            builder.Services.AddScoped<AccountService>();
            builder.Services.AddScoped<ShiftService>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Dashboard}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
