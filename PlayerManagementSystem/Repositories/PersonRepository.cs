using PlayerManagementSystem.EfContext;
using PlayerManagementSystem.Models;

namespace PlayerManagementSystem.Repositories
{
    public class PersonRepository
    {
        private readonly EfDbContext _context;

        public PersonRepository(EfDbContext context)
        {
            _context = context;
        }

        public void Add(PersonalDetails personalDetails)
        {
            _context.PersonalDetails.Add(personalDetails);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var personalDetails = _context.PersonalDetails.Find(id);
            _context.PersonalDetails.Remove(personalDetails!);
            _context.SaveChanges();
        }

        public IEnumerable<PersonalDetails> GetAll()
        {
            return _context.PersonalDetails.ToList();
        }

        public PersonalDetails GetById(int id)
        {
            return _context.PersonalDetails.Find(id)!;
        }

        public void Update(PersonalDetails personalDetails)
        {
            _context.PersonalDetails.Update(personalDetails);
            _context.SaveChanges();
        }
    }
}