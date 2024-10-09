using DentalCare.Models;

namespace DentalCare.Services
{
    public class FacultyService
    {
        private readonly DentalcareContext _context;

        public FacultyService(DentalcareContext context)
        {
            _context = context;
        }

        public List<Faculty> GetAll()
        {
            return _context.Faculties.ToList();
        }

        public Faculty? GetByName(string name)
        {
            return _context.Faculties.FirstOrDefault(x => x.Name.Equals(name));
        }

        public Faculty? Get(string id)
        {
            return _context.Faculties.FirstOrDefault(x => x.Id.Equals(id));
        }

        public void Add(Faculty falcuty)
        {
            _context.Faculties.Add(falcuty);
            _context.SaveChanges();
        }

        public void Update(Faculty faculty)
        {
            _context.Faculties.Update(faculty);
            _context.SaveChanges();
        }

        public string GenerateID()
        {
            var existingFaculties = _context.Faculties.Select(f => f.Id).ToList();

            int facultyId;

            // Check if there are no existing faculties
            if (existingFaculties == null || existingFaculties.Count == 0)
            {
                facultyId = 1;
            }
            else
            {
                var highestId = int.Parse(existingFaculties.Max());

                facultyId = highestId + 1;
            }

            return facultyId.ToString().Trim();
        }
    }
}
