using DentalCare.Models;

namespace DentalCare.Services
{
    public class AppointmentService
    {
        private readonly DentalcareContext _context;

        public AppointmentService(DentalcareContext context)
        {
            _context = context;
        }

        public List<Appointment> GetAll()
        {
            return _context.Appointments.ToList();
        }

        public Appointment? Get(string id)
        {
            return _context.Appointments.FirstOrDefault(x => x.Id.Equals(id));
        }

        public void Add(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var appointment = Get(id);

            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                _context.SaveChanges();
            }
        }

        public void Update(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
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

                string newID = "A" + idNumber.ToString("D9");

                return newID;
            }

            return "A000000001";
        }
    }
}
