using DentalCare.Models;

namespace DentalCare.Services
{
    public class MedicineService
    {
        private readonly DentalcareContext _context;

        public MedicineService(DentalcareContext context)
        {
            _context = context;
        }

        public List<Medicine> GetAll()
        {
            return _context.Medicines.ToList();
        }

        public List<Medicine> GetByType(string id)
        {
            return _context.Medicines.Where(x => x.Medicinetypeid == id).ToList();
        }

        public Medicine? Get(string id)
        {
            return _context.Medicines.FirstOrDefault(x => x.Id.Equals(id));
        }

        public string? GetType(string typeId)
        {
            return _context.Medicinetypes.FirstOrDefault(x => x.Id.Equals(typeId))?.Name;
        }

        public void Add(Medicine medicine)
        {
            _context.Medicines.Add(medicine);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var m = Get(id);
            _context.Medicines.Remove(m);
            _context.SaveChanges();
        }

        public void Update(Medicine medicine) { 
            _context.Medicines.Update(medicine);
            _context.SaveChanges();
        }

        public void UpdateRange(List<Medicine> list)
        {
            _context.Medicines.UpdateRange(list);
            _context.SaveChanges();
        }

        public int GetQuantityForMedicine(string medicineId)
        {
            var m = Get(medicineId);
            return m.Quantity;
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

                string newID = "M" + idNumber.ToString("D9");

                return newID;
            }

            return "M000000001";
        }
    }
}
