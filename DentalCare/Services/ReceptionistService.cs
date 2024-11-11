using DentalCare.Models;

namespace DentalCare.Services
{
    public class ReceptionistService
    {
        private readonly DentalcareContext _context;

        public ReceptionistService(DentalcareContext context)
        {
            _context = context;
        }

        public Receptionist? Get(string id)
        {
            
            return _context.Receptionists.FirstOrDefault(x => x.Id.Equals(id));
        }

        public List<Receptionist> GetAll()
        {
            var list = _context.Receptionists.ToList();
            SortFiredList(list);
            return list;
        }

        public void Add(Receptionist receptionist)
        {
            
            _context.Receptionists.Add(receptionist);
            _context.SaveChanges();
        }

        public void Delete(String id)
        {
            
            var receptionist = Get(id);

            if (receptionist != null)
            {
                _context.Receptionists.Remove(receptionist);
                _context.SaveChanges();
            }
        }

        public void Update(Receptionist receptionist)
        {
            
            _context.Receptionists.Update(receptionist);
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

                string newID = "R" + idNumber.ToString("D9");

                return newID;
            }

            return "R000000001";
        }

        public void SortFiredList(List<Receptionist> list)
        {
            list.Sort((x, y) => x.Fired == y.Fired ? 0 : ((bool) x.Fired ? 1 : -1));
        }
    }
}
