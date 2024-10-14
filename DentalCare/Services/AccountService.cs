using DentalCare.Models;

namespace DentalCare.Services
{
    public class AccountService
    {
        private readonly DentalcareContext _context;

        public AccountService(DentalcareContext context)
        {
            _context = context;
        }

        public List<Account> GetAll()
        {
            return _context.Accounts.ToList();
        }

        public Account? Get(String username)
        {
            return _context.Accounts.FirstOrDefault(x => x.Phone.Equals(username) || x.Email.Equals(username));
        }

        public Account? GetByDoctorId(string doctorId)
        {
            return _context.Accounts.FirstOrDefault(x => x.DoctorId == doctorId);
        }

        public Account? GetByReceptionistId(string receptionistId)
        {
            return _context.Accounts.FirstOrDefault(x => x.ReceptionistId == receptionistId);
        }

        public void Add(Account account)
        {
            _context.Accounts.Add(account);
            _context.SaveChanges();
        }

        public void Update(Account account)
        {
            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

        public void Delete(String username)
        {
            var account = Get(username);
            _context.Accounts.Remove(account);
            _context.SaveChanges();
        }
    }
}
