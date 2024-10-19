using DentalCare.Models;

namespace DentalCare.Services
{
    public class MedicineTypeService
    {
        private readonly DentalcareContext _context;

        public MedicineTypeService(DentalcareContext context)
        {
            _context = context;
        }

        public List<Medicinetype> GetAll()
        {
            return _context.Medicinetypes.ToList();
        }

        public Medicinetype? Get(string id) { 
            return _context.Medicinetypes.FirstOrDefault(x => x.Id == id);
        }

        public void Add(Medicinetype type) { 
            _context.Medicinetypes.Add(type);
            _context.SaveChanges();
        }

        public void Delete(string id) {
            _context.Medicinetypes.Remove(Get(id));
            _context.SaveChanges();
        }

        public void Update(Medicinetype type) { 
            _context.Medicinetypes.Update(type);
            _context.SaveChanges();
        }

        public string GenerateID()
        {
            var nurseWithHighestID = GetAll()
                .Where(n => n.Id.StartsWith("MT") && n.Id.Length == 10 && long.TryParse(n.Id.Substring(2), out _))
                .OrderByDescending(n => long.Parse(n.Id.Substring(2)))
                .FirstOrDefault();

            if (nurseWithHighestID != null)
            {
                long idNumber = long.Parse(nurseWithHighestID.Id.Substring(2));
                idNumber++;

                if (idNumber.ToString("D8").Length <= 8)
                {
                    return "MT" + idNumber.ToString("D8");
                }
                else
                {
                    throw new InvalidOperationException("ID exceeds maximum length after incrementing.");
                }
            }

            return "MT00000001";
        }
    }
}
