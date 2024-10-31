using DentalCare.Models;

namespace DentalCare.Services
{
    public class EquipmentService
    {
        private readonly DentalcareContext _context;

        public EquipmentService(DentalcareContext context)
        {
            _context = context;
        }

        public List<Equipment> GetAll()
        {
            return _context.Equipments.ToList();
        }

        public List<Equipment> GetByType(string id)
        {
            return _context.Equipments.Where(x => x.Equipmenttypeid == id).ToList();
        }

        public Equipment? Get(string id)
        {
            return _context.Equipments.FirstOrDefault(x => x.Id == id);
        }

        public int GetQuantityForEquipment(string medicineId)
        {
            var m = Get(medicineId);
            return m.Quantity;
        }

        public void Add(Equipment equipment)
        {
            _context.Equipments.Add(equipment);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var e = Get(id);
            _context.Equipments.Remove(e);
            _context.SaveChanges();
        }

        public void Update(Equipment equipment)
        {
            _context.Equipments.Update(equipment);
            _context.SaveChanges();
        }

        public void UpdateRange(List<Equipment> list)
        {
            _context.Equipments.UpdateRange(list);
            _context.SaveChanges();
        }

        public string GenerateID()
        {
            var nurseWithHighestID = GetAll()
                .Where(n => n.Id.Length > 1 && long.TryParse(n.Id.Substring(1), out _))
                .OrderByDescending(n => long.Parse(n.Id.Substring(1)))
                .FirstOrDefault();

            if (nurseWithHighestID != null)
            {
                long idNumber = long.Parse(nurseWithHighestID.Id.Substring(1));

                idNumber++;

                string newID = "E" + idNumber.ToString("D9");

                return newID;
            }

            return "E000000001";
        }
    }
}
