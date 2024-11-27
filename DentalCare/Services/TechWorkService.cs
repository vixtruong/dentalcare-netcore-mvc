using DentalCare.Models;

namespace DentalCare.Services
{
    public class TechWorkService
    {
        private readonly DentalcareContext _context;

        public TechWorkService(DentalcareContext context)
        {
            _context = context;
        }

        public List<Techposition> GetAll()
        {
            return _context.Techpositions.ToList();
        }

        public Techposition? Get(string id)
        {
            return _context.Techpositions.FirstOrDefault(x => x.Id == id);
        }

        public List<Techposition> GetByType(string typeId)
        {
            return _context.Techpositions.Where(x => x.Techniqueid == typeId).ToList();
        }

        public void Add(Techposition tech)
        {
            _context.Techpositions.Add(tech);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var tech = Get(id);
            _context.Techpositions.Remove(tech);
            _context.SaveChanges();
        }

        public void Update(Techposition tech)
        {
            _context.Techpositions.Update(tech);
            _context.SaveChanges();
        }

        public string GenerateID()
        {
            var nurseWithHighestID = GetAll()
                .Where(n => n.Id.StartsWith("TP") && n.Id.Length == 6 && long.TryParse(n.Id.Substring(2), out _))
                .OrderByDescending(n => long.Parse(n.Id.Substring(2)))
                .FirstOrDefault();

            if (nurseWithHighestID != null)
            {
                long idNumber = long.Parse(nurseWithHighestID.Id.Substring(2));
                idNumber++;

                if (idNumber.ToString("D4").Length <= 4)
                {
                    return "TP" + idNumber.ToString("D4");
                }
                else
                {
                    throw new InvalidOperationException("ID exceeds maximum length after incrementing.");
                }
            }

            return "TP0001";
        }
    }
}
