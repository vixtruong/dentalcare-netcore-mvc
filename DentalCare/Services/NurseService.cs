using DentalCare.Models;

namespace DentalCare.Services
{
    public class NurseService
    {
        private readonly DentalcareContext _context;

        public NurseService(DentalcareContext context)
        {
            _context = context;
        }

        public List<Nurse> GetAll()
        {
            var list = _context.Nurses.ToList();
            SortFiredList(list);
            return list;
        }

        public Nurse? Get(string id)
        {
            return _context.Nurses.FirstOrDefault(x => x.Id == id);
        }

        public void Add(Nurse nurse)
        {
            _context.Nurses.Add(nurse);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var nurse = Get(id);
            if (nurse != null)
            {
                _context.Nurses.Remove(nurse);
                _context.SaveChanges();
            }
        }

        public void Update(Nurse nurse)
        {
            _context.Nurses.Update(nurse);
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

                string newID = "N" + idNumber.ToString("D9");

                return newID;
            }

            return "N000000001";
        }

        public void SortFiredList(List<Nurse> list)
        {
            list.Sort((x, y) => x.Fired == y.Fired ? 0 : ((bool)x.Fired ? 1 : -1));
        }
    }
}