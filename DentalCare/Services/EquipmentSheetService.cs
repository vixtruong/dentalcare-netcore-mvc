using DentalCare.Models;

namespace DentalCare.Services
{
    public class EquipmentSheetService
    {
        private readonly DentalcareContext _context;

        public EquipmentSheetService(DentalcareContext context)
        {
            _context = context;
        }

        public List<EquipmentSheet> GetAll()
        {
            return _context.EquipmentSheets.ToList();
        }

        public bool IsExistMes(string mesId)
        {
            return _context.EquipmentSheets.Any(x => x.MedicalexaminationId == mesId);
        }

        public EquipmentSheet? Get(string id)
        {
            return _context.EquipmentSheets.FirstOrDefault(p => p.Id == id);
        }

        public void Add(EquipmentSheet equipmentSheet)
        {
            _context.EquipmentSheets.Add(equipmentSheet);
            _context.SaveChanges();
        }

        public void Update(EquipmentSheet equipmentSheet)
        {
            _context.EquipmentSheets.Update(equipmentSheet);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var p = Get(id);
            _context.EquipmentSheets.Remove(p);
            _context.SaveChanges();
        }

        public string GenerateID()
        {
            var nurseWithHighestID = GetAll()
                .Where(n => n.Id.StartsWith("ES") && n.Id.Length == 10 && long.TryParse(n.Id.Substring(2), out _))
                .OrderByDescending(n => long.Parse(n.Id.Substring(2)))
                .FirstOrDefault();

            if (nurseWithHighestID != null)
            {
                long idNumber = long.Parse(nurseWithHighestID.Id.Substring(2));
                idNumber++;

                if (idNumber.ToString("D8").Length <= 8)
                {
                    return "ES" + idNumber.ToString("D8");
                }
                else
                {
                    throw new InvalidOperationException("ID exceeds maximum length after incrementing.");
                }
            }

            return "ES00000001";
        }
    }
}
