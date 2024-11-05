using DentalCare.Middlewares;
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


            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Để giữ nguyên tên thuộc tính
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
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
                builder.Services.AddScoped<TechDetailService>();
                builder.Services.AddScoped<EquipmentService>();
                builder.Services.AddScoped<EquipmentTypeService>();
                builder.Services.AddScoped<EquipmentSheetService>();
                builder.Services.AddScoped<EquipmentDetailService>();
                builder.Services.AddScoped<BillService>();
                builder.Services.AddScoped<PrescriptionDetailService>();
                builder.Services.AddScoped<PrescriptionService>();
                builder.Services.AddScoped<TechSheetService>();
                builder.Services.AddScoped<HealthReportService>();
                builder.Services.AddScoped<InvoiceService>();
            }

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    options.LoginPath = "/Account/Index";
                    options.LogoutPath = "/Account/Logout";
                    options.SlidingExpiration = true;
                });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<SessionCheckMiddleware>();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
