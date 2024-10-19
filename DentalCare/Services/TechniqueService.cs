using DentalCare.Models;

namespace DentalCare.Services
{
    public class TechniqueService
    {
        private readonly DentalcareContext _context;

        public TechniqueService(DentalcareContext context)
        {
            _context = context;
        }

        public List<Technique> GetAll()
        {
            return _context.Techniques.ToList();
        }

        public Technique? Get(string id)
        {
            return _context.Techniques.FirstOrDefault(x => x.Id == id);
        }

        public void Add(Technique tech)
        {
            _context.Techniques.Add(tech);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var tech = Get(id);
            _context.Techniques.Remove(tech);
            _context.SaveChanges();
        }

        public void Update(Technique tech)
        {
            _context.Techniques.Update(tech);
            _context.SaveChanges();
        }

        public string GenerateID()
        {
            var nurseWithHighestID = GetAll()
                .Where(n => n.Id.StartsWith("TE") && n.Id.Length == 10 && long.TryParse(n.Id.Substring(2), out _))
                .OrderByDescending(n => long.Parse(n.Id.Substring(2)))
                .FirstOrDefault();

            if (nurseWithHighestID != null)
            {
                long idNumber = long.Parse(nurseWithHighestID.Id.Substring(2));
                idNumber++;

                if (idNumber.ToString("D8").Length <= 8)
                {
                    return "TE" + idNumber.ToString("D8");
                }
                else
                {
                    throw new InvalidOperationException("ID exceeds maximum length after incrementing.");
                }
            }

            return "TE00000001";
        }
    }
}
