using DentalCare.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace DentalCare.Services
{
    public class PrescriptionService
    {
        private readonly DentalcareContext _context;

        public PrescriptionService(DentalcareContext context)
        {
            _context = context;
        }

        public bool IsExistMes(string mesId)
        {
            return _context.Prescriptions.Any(x => x.Medicalexaminationid == mesId);
        }

        public List<Prescription> GetAll()
        {
            return _context.Prescriptions.ToList();
        }

        public Prescription? Get(string id)
        {
            return _context.Prescriptions.FirstOrDefault(p => p.Id == id);
        }

        public Prescription? GetByMesId(string mesId)
        {
            return _context.Prescriptions.FirstOrDefault(p => p.Medicalexaminationid.Equals(mesId));
        }

        public void Add(Prescription prescription)
        {
            _context.Prescriptions.Add(prescription);
            _context.SaveChanges();
        }

        public void Update(Prescription prescription)
        {
            _context.Prescriptions.Update(prescription);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var p = Get(id);
            _context.Prescriptions.Remove(p);
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

                string newID = "P" + idNumber.ToString("D9");

                return newID;
            }

            return "P000000001";
        }
    }
}
