using System.Net.NetworkInformation;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks.Sources;
using DentalCare.Services;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;

namespace DentalCare.Controllers
{
    public class StatisticController : Controller
    {
        private readonly InvoiceService _invoiceService;
        private readonly MedicalExamService _medicalExamService;
        private readonly DoctorService _doctorService;
        private readonly CustomerService _customerService;

        public StatisticController(InvoiceService invoiceService, MedicalExamService medicalExamService, DoctorService doctorService, CustomerService customerService)
        {
            _invoiceService = invoiceService;
            _medicalExamService = medicalExamService;
            _doctorService = doctorService;
            _customerService = customerService;
        }

        public IActionResult GetInvoiceRevenueByMonthAndYear(int year, int month)
        {
            int days = DateTime.DaysInMonth(year, month);

            Dictionary<int, int> invoiceRevenue = new Dictionary<int, int>();

            for (int i = 1; i <= 30; i++)
            {
                int sum = 0;
                foreach (var invoice in _invoiceService.GetAll())
                {
                    if (invoice.Date.Year == year && invoice.Date.Month == month && invoice.Date.Day == i)
                    {
                        sum += invoice.Finaltotal;
                    }
                }
                invoiceRevenue.Add(i, sum);
            }

            return Json(invoiceRevenue);
        }

        public IActionResult GetDoctorRevenueByMonthAndYear(int year, int month)
        {
            int days = DateTime.DaysInMonth(year, month);

            Dictionary<string, int> doctorRevenue = new Dictionary<string, int>();

            foreach (var doctor in _doctorService.GetAll())
            {
                int sum = 0;
                foreach (var invoice in _invoiceService.GetAll())
                {
                    var mes = _medicalExamService.Get(invoice.Medicalexaminationid);
                    if (mes != null && mes.Doctorid == doctor.Id && invoice.Date.Year == year && invoice.Date.Month == month)
                    {
                        sum += invoice.Finaltotal;
                    }
                }
                doctorRevenue.Add(doctor.Id, sum);
            }
            return Json(doctorRevenue);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Export()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ExportInvoices(DateTime from, DateTime to)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var baseFileName = $"Invoices_{from:ddMMyyyy}_to_{to:ddMMyyyy}.xlsx";

            var fileDirectory = @"D:\User\Desktop\DentalCareResources\Revenue\InvoicesExcel";

            var filePath = Path.Combine(fileDirectory, baseFileName);

            int counter = 1;
            while (System.IO.File.Exists(filePath))
            {
                var newFileName = $"Invoices_{from:ddMMyyyy}_to_{to:ddMMyyyy} ({counter}).xlsx";
                filePath = Path.Combine(fileDirectory, newFileName);
                counter++;
            }

            var invoices = _invoiceService.GetAll().Where(x => x.Date >= from && x.Date <= to).ToList();

            var dailyTotals = invoices
                .GroupBy(i => i.Date.Date)
                .Select(g => new { Date = g.Key, TotalDue = g.Sum(i => i.Finaltotal) })
                .OrderBy(x => x.Date)
                .ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Invoices");

                worksheet.Cells[1, 1].Value = "Invoice ID";
                worksheet.Cells[1, 2].Value = "MES ID";
                worksheet.Cells[1, 3].Value = "Date";
                worksheet.Cells[1, 4].Value = "Receptionist ID";
                worksheet.Cells[1, 5].Value = "Customer ID";
                worksheet.Cells[1, 6].Value = "Customer Name";
                worksheet.Cells[1, 7].Value = "Payment";
                worksheet.Cells[1, 8].Value = "Total";
                worksheet.Cells[1, 9].Value = "Discount";
                worksheet.Cells[1, 10].Value = "Total Due";

                using (var range = worksheet.Cells["A1:J1"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                int row = 2;
                foreach (var invoice in invoices)
                {
                    var mes = _medicalExamService.Get(invoice.Medicalexaminationid);
                    var customer = _customerService.Get(mes.Customerid);
                    worksheet.Cells[row, 1].Value = invoice.Id;
                    worksheet.Cells[row, 2].Value = invoice.Medicalexaminationid;
                    worksheet.Cells[row, 3].Value = invoice.Date.ToString("dd-MM-yyyy");
                    worksheet.Cells[row, 4].Value = invoice.Receptionistid;
                    worksheet.Cells[row, 5].Value = customer.Id;
                    worksheet.Cells[row, 6].Value = customer.Name;
                    worksheet.Cells[row, 7].Value = invoice.Payment;
                    worksheet.Cells[row, 8].Value = invoice.Total;
                    worksheet.Cells[row, 9].Value = invoice.Discount;
                    worksheet.Cells[row, 10].Value = invoice.Finaltotal;

                    row++;
                }

                var chartStartRow = row + 2;
                worksheet.Cells[chartStartRow, 1].Value = "Date";
                worksheet.Cells[chartStartRow, 2].Value = "Total Due";

                using (var range = worksheet.Cells[chartStartRow, 1, chartStartRow, 2])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                for (int i = 0; i < dailyTotals.Count; i++)
                {
                    worksheet.Cells[chartStartRow + i + 1, 1].Value = dailyTotals[i].Date.ToString("dd-MM-yyyy");
                    worksheet.Cells[chartStartRow + i + 1, 2].Value = dailyTotals[i].TotalDue;
                }

                var chart = worksheet.Drawings.AddChart("RevenueChart", eChartType.ColumnClustered);
                chart.Title.Text = "Daily Total Due";
                chart.SetPosition(chartStartRow - 1, 0, 4, 0);
                chart.SetSize(600, 300);

                var dateRange = worksheet.Cells[chartStartRow + 1, 1, chartStartRow + dailyTotals.Count, 1];
                var totalDueRange = worksheet.Cells[chartStartRow + 1, 2, chartStartRow + dailyTotals.Count, 2];
                chart.Series.Add(totalDueRange, dateRange);

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                FileInfo fi = new FileInfo(filePath);
                package.SaveAs(fi);
            }

            TempData["SuccessMessage"] = $"File {baseFileName} has been saved.";
            return RedirectToAction("Export");
        }

        [HttpGet]
        public IActionResult ExportDoctors(DateTime from, DateTime to)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var baseFileName = $"Doctors_{from:ddMMyyyy}_to_{to:ddMMyyyy}.xlsx";

            var fileDirectory = @"D:\User\Desktop\DentalCareResources\Revenue\DoctorsExcel";

            var filePath = Path.Combine(fileDirectory, baseFileName);

            int counter = 1;
            while (System.IO.File.Exists(filePath))
            {
                var newFileName = $"Doctors_{from:ddMMyyyy}_to_{to:ddMMyyyy} ({counter}).xlsx";
                filePath = Path.Combine(fileDirectory, newFileName);
                counter++;
            }

            var invoices = _invoiceService.GetAll().Where(x => x.Date >= from && x.Date <= to).ToList();

            Dictionary<string, int> doctorsRevenue = new Dictionary<string, int>();

            foreach (var doctor in _doctorService.GetAll())
            {
                int sum = 0;
                foreach (var invoice in invoices)
                {
                    var mes = _medicalExamService.Get(invoice.Medicalexaminationid);
                    if (mes != null && mes.Doctorid == doctor.Id)
                    {
                        sum += invoice.Finaltotal;
                    }
                }
                doctorsRevenue.Add(doctor.Id, sum);
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Invoices");

                worksheet.Cells[1, 1].Value = "Doctor ID";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Faculty";
                worksheet.Cells[1, 4].Value = "Years of experience";
                worksheet.Cells[1, 5].Value = "Birthday";
                worksheet.Cells[1, 6].Value = "Total Revenue";

                using (var range = worksheet.Cells["A1:J1"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                int row = 2;
                foreach (var d in doctorsRevenue)
                {
                    var doctor = _doctorService.Get(d.Key);
                    if (doctor != null)
                    {
                        worksheet.Cells[row, 1].Value = doctor.Id;
                        worksheet.Cells[row, 2].Value = doctor.Name;
                        worksheet.Cells[row, 3].Value = doctor.FacultyName;
                        worksheet.Cells[row, 4].Value = (DateTime.Now - doctor.Firstdayofwork).TotalDays / 365;
                        worksheet.Cells[row, 5].Value = doctor.Birthday.ToString("dd-MM-yyyy");
                        worksheet.Cells[row, 6].Value = d.Value;
                    }

                    row++;
                }

                var chart = worksheet.Drawings.AddChart("RevenueChart", eChartType.ColumnClustered);
                chart.Title.Text = "Doctors' Total Revenue Chart";
                chart.SetPosition(1, 0, 7, 0);
                chart.SetSize(600, 300);

                var chartRangeX = worksheet.Cells[2, 1, row - 1, 1]; // Doctor IDs (X-axis)
                var chartRangeY = worksheet.Cells[2, 6, row - 1, 6]; // Total Revenue (Y-axis)

                chart.Series.Add(chartRangeY, chartRangeX);

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                FileInfo fi = new FileInfo(filePath);
                package.SaveAs(fi);
            }

            TempData["SuccessMessage"] = $"File {baseFileName} has been saved.";
            return RedirectToAction("Export");
        }
    }
}
