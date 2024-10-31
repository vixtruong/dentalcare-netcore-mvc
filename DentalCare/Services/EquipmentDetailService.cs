using DentalCare.Models;

namespace DentalCare.Services
{
    public class EquipmentDetailService
    {
        private readonly DentalcareContext _context;

        public EquipmentDetailService(DentalcareContext context)
        {
            _context = context;
        }

        public List<Equipmentdetail> GetAll()
        {
            return _context.Equipmentdetails.ToList();
        }

        public void AddRange(List<Equipmentdetail> list)
        {
            foreach (var detail in list)
            {
                _context.Equipmentdetails.Add(detail);
            }
            _context.SaveChanges();
        }

        public void Add(Equipmentdetail detail)
        {
            _context.Equipmentdetails.Add(detail);
            _context.SaveChanges();
        }

        public void DeleteByEquipmentSheetId(string prescriptionId)
        {
            var detailsToDelete = GetAll().Where(detail => detail.EquipmentsheetId == prescriptionId).ToList();
            if (detailsToDelete.Any())
            {
                _context.Equipmentdetails.RemoveRange(detailsToDelete);
                _context.SaveChanges();
            }
        }
    }
}
