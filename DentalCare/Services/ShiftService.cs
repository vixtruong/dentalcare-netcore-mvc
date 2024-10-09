using DentalCare.Models;

namespace DentalCare.Services
{
    public class ShiftService
    {
        private readonly DentalcareContext _context;

        public ShiftService(DentalcareContext context)
        {
            _context = context;
        }

        public List<Shift> GetAll()
        {
            return _context.Shifts.ToList();
        }

        public Shift? Get(string id)
        {
            return _context.Shifts.FirstOrDefault(x => x.Id == id);
        }

        public void Add(Shift shift)
        {
            _context.Shifts.Add(shift);
            _context.SaveChanges();
        }

        public void Update(Shift shift)
        {
            _context.Shifts.Update(shift);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var shift = Get(id);
            _context.Shifts.Remove(shift);
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

                string newID = "S" + idNumber.ToString("D9");

                return newID;
            }

            return "S000000001";
        }
    }
}
