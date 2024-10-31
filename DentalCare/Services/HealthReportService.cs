using DentalCare.Models;

namespace DentalCare.Services
{
    public class HealthReportService
    {
        private readonly DentalcareContext _context;

        public HealthReportService(DentalcareContext context)
        {
            _context = context;
        }

        public List<Healthreport> GetAll()
        {
            return _context.Healthreports.ToList();
        }

        public Healthreport? Get(string id)
        {
            return _context.Healthreports.FirstOrDefault(x => x.Id == id);
        }

        public void Add(Healthreport report)
        {
            _context.Healthreports.Add(report);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var report = Get(id);
            _context.Healthreports.Remove(report);
            _context.SaveChanges();
        }

        public void Update(Healthreport report)
        {
            _context.Healthreports.Update(report);
            _context.SaveChanges();
        }

        public string GenerateID()
        {
            var nurseWithHighestID = GetAll()
                .Where(n => n.Id.StartsWith("HR") && n.Id.Length == 10 && long.TryParse(n.Id.Substring(2), out _))
                .OrderByDescending(n => long.Parse(n.Id.Substring(2)))
                .FirstOrDefault();

            if (nurseWithHighestID != null)
            {
                long idNumber = long.Parse(nurseWithHighestID.Id.Substring(2));
                idNumber++;

                if (idNumber.ToString("D8").Length <= 8)
                {
                    return "HR" + idNumber.ToString("D8");
                }
                else
                {
                    throw new InvalidOperationException("ID exceeds maximum length after incrementing.");
                }
            }

            return "HR00000001";
        }
    }
}
