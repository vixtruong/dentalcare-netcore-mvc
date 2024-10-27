using DentalCare.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DentalCare.Services;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace DentalCare.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DoctorService _doctorService;
        private readonly FacultyService _facultyService;
        private readonly AppointmentService _appointmentService;
        private readonly CustomerService _customerService;

        public HomeController(ILogger<HomeController> logger, DoctorService doctorService,
            FacultyService facultyService, AppointmentService appointmentService, CustomerService customerService)
        {
            _logger = logger;
            _doctorService = doctorService;
            _facultyService = facultyService;
            _appointmentService = appointmentService;
            _customerService = customerService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Appointment()
        {
            ViewBag.Faculties = _facultyService.GetAll();
            return View();
        }

        [HttpPost]
        public IActionResult Appointment(AppointmentViewModel model)
        {
            ViewBag.Faculties = _facultyService.GetAll();
            var customer = _customerService.GetByPhone(model.Phone);
            if (customer != null)
            {
                var appointment = new Appointment
                {
                    Id = _appointmentService.GenerateID(),
                    Date = model.Date,
                    Time = TimeOnly.Parse(model.Time),
                    Demand = model.Demand.Trim(),
                    Customerid = customer.Id,
                    Doctorid = model.DoctorId
                };

                _appointmentService.Add(appointment);
                SendAccountToEmail(appointment);
                ViewBag.Error = false;
                ViewBag.Message = "Make appointment successfully! Please check your email to get more information";
                return View();
            }

            var newCustomer = new Customer
            {
                Id = _customerService.GenerateID(),
                Name = model.CustomerName,
                Email = model.Email,
                Phone = model.Phone,
                Birthday = model.Birthday,
            };

            _customerService.Add(newCustomer);

            var newAppointment = new Appointment
            {
                Id = _appointmentService.GenerateID(),
                Date = model.Date,
                Time = TimeOnly.Parse(model.Time),
                Demand = model.Demand,
                Customerid = newCustomer.Id,
                Doctorid = model.DoctorId
            };

            _appointmentService.Add(newAppointment);
            SendAccountToEmail(newAppointment);

            ViewBag.Error = false;
            ViewBag.Message = "Make appointment successfully! Please check your email to get more information";
            return View();
        }

        public IActionResult Team()
        {
            return View();
        }

        public IActionResult Service()
        {
            return View();
        }

        public IActionResult Pricing()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void SendAccountToEmail(Appointment appointment)
        {
            var customer = _customerService.Get(appointment.Customerid);
            var doctor = _doctorService.Get(appointment.Doctorid);

            string from = "n8cnpm2024@gmail.com";
            string pass = "ymhp dxyc jgti ohne";
            string to = customer.Email;

            // Nội dung email với HTML chứa thông tin lịch hẹn
            string messageBody = $@"
    <html>
    <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
        <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 10px; overflow: hidden;'>
            <div style='background-color: #0066cc; color: white; padding: 20px; text-align: center;'>
                <h1>Appointment Confirmation</h1>
            </div>
            <div style='padding: 20px;'>
                <h2 style='color: #0066cc;'>Customer Information</h2>
                <p><strong>Full Name:</strong> {customer.Name}</p>
                <p><strong>Phone:</strong> {customer.Phone}</p>
                <p><strong>Email:</strong> {customer.Email}</p>
                
                <h2 style='color: #0066cc;'>Doctor Information</h2>
                <p><strong>Doctor Name:</strong> {doctor.Name}</p>
                <p><strong>Phone:</strong> {doctor.Phone}</p>
                <p><strong>Email:</strong> {doctor.Email}</p>
                <p><strong>Faculty:</strong> {doctor.FacultyName}</p>

                <h2 style='color: #0066cc;'>Appointment Information</h2>
                <p><strong>Date:</strong> {appointment.Date.ToString("dd/MM/yyyy")}</p>
                <p><strong>Time:</strong> {appointment.Time}</p>
                <p><strong>Demand Description:</strong> {appointment.Demand}</p>
            </div>
            <div style='background-color: #0066cc; color: white; text-align: center; padding: 10px;'>
                <p>Thank you for scheduling an appointment with DentalCare! We look forward to serving you.</p>
                <p>&copy; 2024 DentalCare</p>
            </div>
        </div>
    </body>
    </html>";

            MailMessage message = new MailMessage();
            message.To.Add(to);
            message.From = new MailAddress(from);
            message.Body = messageBody;
            message.Subject = "Appointment Confirmation - DentalCare";
            message.IsBodyHtml = true; // Kích hoạt HTML trong email

            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.EnableSsl = true;
            smtp.Port = 587;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(from, pass);

            try
            {
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
            }
        }
    }
}
