using Microsoft.AspNetCore.Mvc;
using PlayerManagementSystem.Models;
using PlayerManagementSystem.Repositories;

namespace PlayerManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly PersonRepository _personRepository;

        public PersonController(PersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        [HttpGet]
        public IEnumerable<PersonalDetails> Get()
        {
            return _personRepository.GetAll();
        }

        [HttpGet("{id}")]
        public PersonalDetails Get(int id)
        {
            return _personRepository.GetById(id);
        }

        [HttpPost]
        public void Post([FromBody] PersonalDetails personalDetails)
        {
            _personRepository.Add(personalDetails);
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] PersonalDetails personalDetails)
        {
            _personRepository.Update(personalDetails);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _personRepository.Delete(id);
        }
    }
}