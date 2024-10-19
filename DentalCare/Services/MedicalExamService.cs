using DentalCare.Models;

namespace DentalCare.Services
{
    public class MedicalExamService
    {
        private readonly DentalcareContext _context;

        public MedicalExamService(DentalcareContext context)
        {
            _context = context;
        }

        public List<Medicalexamination> GetAll()
        {
            return _context.Medicalexaminations.ToList();
        }

        public Medicalexamination? Get(string id)
        {
            return GetAll().FirstOrDefault(x => x.Id == id);
        }

        public void Add(Medicalexamination model)
        {
            _context.Medicalexaminations.Add(model);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            _context.Medicalexaminations.Remove(Get(id));
            _context.SaveChanges();
        }

        public string GenerateID()
        {
            var nurseWithHighestID = GetAll()
                .Where(n => n.Id.StartsWith("ME") && n.Id.Length == 10 && long.TryParse(n.Id.Substring(2), out _))
                .OrderByDescending(n => long.Parse(n.Id.Substring(2)))
                .FirstOrDefault();

            if (nurseWithHighestID != null)
            {
                long idNumber = long.Parse(nurseWithHighestID.Id.Substring(2));
                idNumber++;

                if (idNumber.ToString("D8").Length <= 8)
                {
                    return "ME" + idNumber.ToString("D8");
                }
                else
                {
                    throw new InvalidOperationException("ID exceeds maximum length after incrementing.");
                }
            }

            return "ME00000001";
        }
    }
}
