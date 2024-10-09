using DentalCare.Models;
using Microsoft.EntityFrameworkCore;

namespace DentalCare.Services
{
    public class DoctorService
    {
        private readonly DentalcareContext _context;

        public DoctorService(DentalcareContext context)
        {
            _context = context;
        }

        public List<Doctor> GetAll()
        {
            var list = _context.Doctors.ToList();
            SortFiredList(list);
            return list;
        }

        public List<Doctor> GetByFacultyId(string id)
        {
            return _context.Doctors.Where(x => x.Falcutyid == id).ToList();
        }
        public Doctor? Get(string id)
        {
            return _context.Doctors.FirstOrDefault(x => x.Id == id);
        }

        public void Add(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var doctor = Get(id);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
                _context.SaveChanges();
            }
        }

        public void Update(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
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

                string newID = "D" + idNumber.ToString("D9");

                return newID;
            }

            return "D000000001";
        }

        public void SortFiredList(List<Doctor> list)
        {
            list.Sort((x, y) => x.Fired == y.Fired ? 0 : ((bool)x.Fired ? 1 : -1));
        }
    }
}
