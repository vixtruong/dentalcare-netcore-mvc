using DentalCare.Models;
using DentalCare.Services;
using Microsoft.AspNetCore.Mvc;
using SelectPdf;

namespace DentalCare.Controllers
{
    public class MedicalExamController : Controller
    {
        private readonly DoctorService _doctorService;
        private readonly CustomerService _customerService;
        private readonly AppointmentService _appointmentService;
        private readonly MedicalExamService _medicalExamService;

        public MedicalExamController(DoctorService doctorService, CustomerService customerService, AppointmentService appointmentService, MedicalExamService medicalExamService)
        {
            _doctorService = doctorService;
            _customerService = customerService;
            _appointmentService = appointmentService;
            _medicalExamService = medicalExamService;
        }

        public IActionResult Add(string appointmentId)
        {
            var appointment = _appointmentService.Get(appointmentId);
            var doctor = _doctorService.Get(appointment.Doctorid);
            var customer = _customerService.Get(appointment.Customerid);

            var medicalExam = new Medicalexamination
            {
                Id = _medicalExamService.GenerateID(),
                Doctorid = doctor.Id,
                Customerid = customer.Id,
                Status = appointment.Demand,
                Date = appointment.Date
            };
            _medicalExamService.Add(medicalExam);

            return Print(medicalExam, appointment, doctor, customer);
        }


        public IActionResult Print(Medicalexamination medicalExam, Appointment appointment, Doctor doctor, Customer customer)
        {
            string htmlContent = $@"
                            <html>
                            <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                                <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 10px; overflow: hidden;'>
                                    <!-- Header Section -->
                                    <div style='background-color: #0066cc; color: white; padding: 20px; text-align: center;'>
                                        <h1 style='font-size: 1.5em;'>Medical Examination Slip</h1>
                                    </div>

                                    <div style='padding: 20px; font-size: 1.2em;'> <!-- Tăng kích thước font của nội dung -->
                                        <h2 style='color: #0066cc;'>Medical Exam Information</h2>
                                        <p><strong>ID:</strong> {medicalExam.Id}</p>
                                        <p><strong>Status:</strong> {medicalExam.Status}</p>

                                        <h2 style='color: #0066cc;'>Appointment Information</h2>
                                        <p><strong>Date:</strong> {appointment.Date.ToString("dd/MM/yyyy")}</p>
                                        <p><strong>Time:</strong> {appointment.Time}</p>
                                        <p><strong>Demand Description:</strong> {appointment.Demand}</p>

                                        <h2 style='color: #0066cc;'>Doctor Information</h2>
                                        <p><strong>ID:</strong> {doctor.Id}</p>
                                        <p><strong>Name:</strong> {doctor.Name}</p>
                                        <p><strong>Phone:</strong> {doctor.Phone}</p>
                                        <p><strong>Email:</strong> {doctor.Email}</p>
                                        <p><strong>Faculty:</strong> {doctor.FacultyName}</p>

                                        <h2 style='color: #0066cc;'>Customer Information</h2>
                                        <p><strong>ID:</strong> {customer.Id}</p>
                                        <p><strong>Full Name:</strong> {customer.Name}</p>
                                        <p><strong>Phone:</strong> {customer.Phone}</p>
                                        <p><strong>Email:</strong> {customer.Email}</p>
                                    </div>

                                    <div style='background-color: #0066cc; color: white; text-align: center; padding: 10px; font-size: 1.1em;'>
                                        <p>Thank you for trusting us with your medical care. We look forward to serving you again.</p>
                                        <p>&copy; 2024 DentalCare</p>
                                    </div>
                                </div>
                            </body>
                            </html>";

            // Tạo đối tượng HtmlToPdf và cấu hình các tùy chọn
            HtmlToPdf converter = new HtmlToPdf();
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.MarginLeft = 0;
            converter.Options.MarginTop = 0;
            converter.Options.MarginRight = 0;
            converter.Options.MarginBottom = 0;

            PdfDocument document = converter.ConvertHtmlString(htmlContent);

            string filePath = Path.Combine("D:", "User", "Desktop", "MedicalExaminationSlip", $"{medicalExam.Id}.pdf");

            document.Save(filePath);
            document.Close();

            // Trả về phản hồi JSON thành công
            return Json(new { success = true, message = "Medical Examination Slip has been created and printed successfully.", filePath = filePath });
        }
    }
}
