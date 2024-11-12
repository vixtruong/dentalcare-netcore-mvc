using DentalCare.Models;
using DentalCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using X.PagedList.Extensions;

namespace DentalCare.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly DoctorService _doctorService;
        private readonly CustomerService _customerService;
        private readonly FacultyService _facultyService;
        private readonly AppointmentService _appointmentService;

        public AppointmentController(DoctorService doctorService, CustomerService customerService, FacultyService facultyService, AppointmentService appointmentService)
        {
            _doctorService = doctorService;
            _customerService = customerService;
            _facultyService = facultyService;
            _appointmentService = appointmentService;
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetDoctorsByFaculty(string facultyId)
        {
            var doctors = _doctorService.GetByFacultyId(facultyId);
            var doctorList = doctors.Select(d => new { id = d.Id, name = d.Name }).ToList();
            return Json(doctorList);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetDoctorFreeTime(string doctorId, DateTime date)
        {
            List<string> allTimeSlots = new List<string>();
            DateTime startTime = DateTime.Parse("07:30");
            DateTime endTime = DateTime.Parse("17:00");

            while (startTime <= endTime)
            {
                allTimeSlots.Add(startTime.ToString("HH:mm"));
                startTime = startTime.AddMinutes(30); // Thêm 30 phút mỗi lần
            }

            var bookedList = _appointmentService.GetAll()
                .Where(x => x.Date.Date == date.Date && x.Doctorid == doctorId)
                .Select(a => a.Time.ToString("HH:mm"))
                .ToList();

            var freeTimeSlots = allTimeSlots.Except(bookedList).ToList();

            return Json(freeTimeSlots);
        }


        [HttpGet]
        public IActionResult GetCustomerByPhone(string phone)
        {
            var customer = _customerService.GetByPhone(phone);
            if (customer != null)
            {
                return Json(new
                {
                    success = true,
                    customer = new
                    {
                        name = customer.Name,
                        email = customer.Email,
                        birthday = customer.Birthday.ToString("yyyy-MM-dd")
                    }
                });
            }
            return Json(new { success = false });
        }

        public IActionResult Add()
        {
            ViewBag.Faculties = _facultyService.GetAll();
            if (ViewBag.Faculties == null)
            {
                return Content("Data not found");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Add(AppointmentViewModel model)
        {
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
                return RedirectToAction("Manage", "Appointment");
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
            return RedirectToAction("Manage", "Appointment");
        }

        [HttpGet]
        [Route("appointment")]
        public IActionResult Manage(int? page, string sortColumn, string sortDirection, string searchQuery)
        {
            ViewBag.Customers = _customerService.GetAll();
            ViewBag.Doctors = _doctorService.GetAll();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var appointments = _appointmentService.GetAll();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                appointments = appointments.Where(a => a.Id.Contains(searchQuery) ||
                                                       a.Time.ToString().Contains(searchQuery) ||
                                                       a.Date.ToString("dd-MM-yyyy").Contains(searchQuery) ||
                                                       a.Doctorid.Contains(searchQuery) ||
                                                       a.Customerid.Contains(searchQuery)).ToList();

                ViewBag.SearchQuery = searchQuery;
            }

            appointments = (sortColumn switch
            {
                "Date" => sortDirection == "desc" ? appointments.OrderByDescending(a => a.Date) : appointments.OrderBy(a => a.Date),
                "Doctor" => sortDirection == "desc" ? appointments.OrderByDescending(a => a.Doctor.Name) : appointments.OrderBy(a => a.Doctor.Name),
                "Doctor ID" => sortDirection == "desc" ? appointments.OrderByDescending(a => a.Doctor.Id) : appointments.OrderBy(a => a.Doctor.Id),
                "Customer" => sortDirection == "desc" ? appointments.OrderByDescending(a => a.Customer.Name) : appointments.OrderBy(a => a.Customer.Name),
                "Customer ID" => sortDirection == "desc" ? appointments.OrderByDescending(a => a.Customer.Id) : appointments.OrderBy(a => a.Customer.Id),
                "Time" => sortDirection == "desc" ? appointments.OrderByDescending(a => a.Time) : appointments.OrderBy(a => a.Time),
                "Id" => sortDirection == "desc" ? appointments.OrderByDescending(a => a.Id) : appointments.OrderBy(a => a.Id),
                _ => appointments.OrderBy(a => a.Id)
            }).ToList();

            ViewBag.SortColumn = sortColumn;
            ViewBag.SortDirection = sortDirection;
            ViewBag.NextSortDirection = sortDirection == "asc" ? "desc" : "asc";

            var UserId = HttpContext.Session.GetString("UserId");
            if (UserId.Contains("D"))
            {
                appointments = appointments.Where(x => x.Doctorid == UserId).ToList();
            }

            var pagedList = appointments.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var appointment = _appointmentService.Get(id);

            ViewBag.Doctor = _doctorService.Get(appointment.Doctorid);
            ViewBag.Faculties = _facultyService.GetAll();
            ViewBag.Customer = _customerService.Get(appointment.Customerid);
            ViewBag.Time = appointment.Time.ToString(@"hh\:mm");

            return View(appointment);
        }

        [HttpPost]
        public IActionResult Edit(Appointment model)
        {
            var appointment = _appointmentService.Get(model.Id);
            appointment.Date = model.Date;
            appointment.Doctorid = model.Doctorid;
            appointment.Time = model.Time;
            appointment.Demand = model.Demand;

            _appointmentService.Update(appointment);
            return RedirectToAction("Manage", "Appointment");
        }

        public IActionResult Delete(string id)
        {
            _appointmentService.Delete(id);
            return RedirectToAction("Manage", "Appointment");
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
