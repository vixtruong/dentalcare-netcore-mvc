using DentalCare.Models;

namespace DentalCare.Services
{
    public class TechDetailService
    {
        private readonly DentalcareContext _context;

        public TechDetailService(DentalcareContext context)
        {
            _context = context;
        }

        public List<Techdetail> GetAll()
        {
            return _context.Techdetails.ToList();
        }

        public Techdetail? GetByTechSheetIdAndTechWorkId(string techsheetId, string techworkId)
        {
            return _context.Techdetails.FirstOrDefault(x => x.TechsheetId == techsheetId && x.Techpositionid == techworkId);
        }

        public bool IsExistTechWork(string techsheetId, string techworkId)
        {
            return _context.Techdetails.Any(x => x.Techpositionid == techworkId && x.TechsheetId == techsheetId);
        }

        public void Add(Techdetail techdetail)
        {
            _context.Add(techdetail);
            _context.SaveChanges();
        }

        public void AddRange(List<Techdetail> list)
        {
            _context.Techdetails.AddRange(list);
            _context.SaveChanges();
        }

        public void Update(Techdetail techdetail)
        {
            _context.Techdetails.Update(techdetail);
            _context.SaveChanges();
        }

        public void UpdateRange(List<Techdetail> list)
        {
            _context.Techdetails.UpdateRange(list);
            _context.SaveChanges();
        }

        public void DeleteRangeByTechsheetId(string techSheetId)
        {
            var list = _context.Techdetails.Where(x => x.TechsheetId == techSheetId).ToList();
            _context.Techdetails.RemoveRange(list);
            _context.SaveChanges();
        }
    }
}
