using DentalCare.Models;
using DentalCare.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace DentalCare
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<DentalcareContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DBDefault")));

            // Đăng ký services
            if (true)
            {
                builder.Services.AddScoped<NurseService>();
                builder.Services.AddScoped<DoctorService>();
                builder.Services.AddScoped<ReceptionistService>();
                builder.Services.AddScoped<FacultyService>();
                builder.Services.AddScoped<CustomerService>();
                builder.Services.AddScoped<AccountService>();
                builder.Services.AddScoped<ShiftService>();
                builder.Services.AddScoped<AppointmentService>();
                builder.Services.AddScoped<MedicalExamService>();
                builder.Services.AddScoped<MedicineService>();
                builder.Services.AddScoped<MedicineTypeService>();
                builder.Services.AddScoped<TechWorkService>();
                builder.Services.AddScoped<TechniqueService>();
                builder.Services.AddScoped<EquipmentService>();
                builder.Services.AddScoped<EquipmentTypeService>();
            }

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Index";
                    options.LogoutPath = "/Account/Logout";
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSession();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
