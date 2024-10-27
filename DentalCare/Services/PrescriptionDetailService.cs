using DentalCare.Models;

namespace DentalCare.Services
{
    public class PrescriptionDetailService
    {
        private readonly DentalcareContext _context;

        public PrescriptionDetailService(DentalcareContext context)
        {
            _context = context;
        }

        public List<Prescriptiondetail> GetAll()
        {
            return _context.Prescriptiondetails.ToList();
        }

        public void AddRange(List<Prescriptiondetail> list)
        {
            foreach (var detail in list)
            {
                _context.Prescriptiondetails.Add(detail);
            }
            _context.SaveChanges();
        }

        public void Add(Prescriptiondetail detail)
        {
            _context.Prescriptiondetails.Add(detail);
            _context.SaveChanges();
        }

        public void DeleteByPrescriptionId(string prescriptionId)
        {
            var detailsToDelete = GetAll().Where(detail => detail.Prescriptionid == prescriptionId).ToList();
            if (detailsToDelete.Any())
            {
                _context.Prescriptiondetails.RemoveRange(detailsToDelete);
                _context.SaveChanges();
            }
        }
    }
}
