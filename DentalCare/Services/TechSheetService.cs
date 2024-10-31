using DentalCare.Models;

namespace DentalCare.Services
{
    public class TechSheetService
    {
        private readonly DentalcareContext _context;

        public TechSheetService(DentalcareContext context)
        {
            _context = context;
        }

        public List<Techsheet> GetAll()
        {
            return _context.Techsheets.ToList();
        }

        public Techsheet? Get(string id)
        {
            return _context.Techsheets.FirstOrDefault(x => x.Id == id);
        }

        public bool IsExistMes(string mesId)
        {
            return _context.Techsheets.Any(x => x.MedicalexaminationId == mesId);
        }

        public void Add(Techsheet sheet)
        {
            _context.Techsheets.Add(sheet);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var sheet = Get(id);
            _context.Techsheets.Remove(sheet);
            _context.SaveChanges();
        }

        public void Update(Techsheet sheet)
        {
            _context.Techsheets.Update(sheet);
            _context.SaveChanges();
        }

        public string GenerateID()
        {
            var nurseWithHighestID = GetAll()
                .Where(n => n.Id.StartsWith("TS") && n.Id.Length == 10 && long.TryParse(n.Id.Substring(2), out _))
                .OrderByDescending(n => long.Parse(n.Id.Substring(2)))
                .FirstOrDefault();

            if (nurseWithHighestID != null)
            {
                long idNumber = long.Parse(nurseWithHighestID.Id.Substring(2));
                idNumber++;

                if (idNumber.ToString("D8").Length <= 8)
                {
                    return "TS" + idNumber.ToString("D8");
                }
                else
                {
                    throw new InvalidOperationException("ID exceeds maximum length after incrementing.");
                }
            }

            return "TS00000001";
        }
    }
}
