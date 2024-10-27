using DentalCare.Models;

namespace DentalCare.Services
{
    public class BillService
    {
        private readonly DentalcareContext _context;

        public BillService(DentalcareContext context)
        {
            _context = context;
        }

        public List<Bill> GetAll()
        {
            return _context.Bills.ToList();
        }

        public Bill? Get(string id)
        {
            return _context.Bills.FirstOrDefault(b => b.Id == id);
        }

        public void Add(Bill bill)
        {
            _context.Bills.Add(bill);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var bill = Get(id);
            _context.Bills.Remove(bill);
            _context.SaveChanges();
        }

        public void Update(Bill bill)
        {
            _context.Bills.Add(bill);
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

                string newID = "B" + idNumber.ToString("D9");

                return newID;
            }

            return "B000000001";
        }
    }
}
