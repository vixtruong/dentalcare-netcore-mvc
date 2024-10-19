using DentalCare.Models;

namespace DentalCare.Services
{
    public class EquipmentTypeService
    {
        private readonly DentalcareContext _context;

        public EquipmentTypeService(DentalcareContext context)
        {
            _context = context;
        }

        public List<Equipmenttype> GetAll()
        {
            return _context.Equipmenttypes.ToList();
        }

        public Equipmenttype? Get(string id)
        {
            return _context.Equipmenttypes.FirstOrDefault(x => x.Id == id);
        }

        public void Add(Equipmenttype type)
        {
            _context.Equipmenttypes.Add(type);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var e = Get(id);
            _context.Equipmenttypes.Remove(e);
            _context.SaveChanges();
        }

        public void Update(Equipmenttype type)
        {
            _context.Equipmenttypes.Update(type);
            _context.SaveChanges();
        }

        public string GenerateID()
        {
            var nurseWithHighestID = GetAll()
                .Where(n => n.Id.StartsWith("ET") && n.Id.Length == 10 && long.TryParse(n.Id.Substring(2), out _))
                .OrderByDescending(n => long.Parse(n.Id.Substring(2)))
                .FirstOrDefault();

            if (nurseWithHighestID != null)
            {
                long idNumber = long.Parse(nurseWithHighestID.Id.Substring(2));
                idNumber++;

                if (idNumber.ToString("D8").Length <= 8)
                {
                    return "ET" + idNumber.ToString("D8");
                }
                else
                {
                    throw new InvalidOperationException("ID exceeds maximum length after incrementing.");
                }
            }

            return "ET00000001";
        }
    }
}
