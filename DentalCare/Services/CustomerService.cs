using DentalCare.Models;

namespace DentalCare.Services
{
    public class CustomerService
    {
        private readonly DentalcareContext _context;

        public CustomerService(DentalcareContext context)
        {
            _context = context;
        }
        public List<Customer> GetAll()
        {

            return _context.Customers.ToList();
        }

        public Customer? Get(String id)
        {

            return _context.Customers.FirstOrDefault(x => x.Id.Equals(id));
        }

        public void Add(Customer customer)
        {

            _context.Customers.Add(customer);
            _context.SaveChanges();
        }

        public void Delete(String id)
        {

            var customer = Get(id);
            _context.Customers.Remove(customer);
            _context.SaveChanges();
        }

        public void Update(Customer customer)
        {

            _context.Customers.Update(customer);
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

                string newID = "C" + idNumber.ToString("D9");

                return newID;
            }

            return "C000000001";
        }
    }
}
