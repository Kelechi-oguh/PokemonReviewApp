using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Models;
using PokemonReviewApp.Services.CountryService;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryRepository _countryRepo;
        private readonly IMapper _mapper;

        public CountryController(ICountryRepository countryRepo, IMapper mapper)
        {
            _countryRepo = countryRepo;
            _mapper = mapper;
        }


        [HttpGet("all-countries")]
        public IActionResult GetCountries() 
        {
            var countries = _mapper.Map<List<CountryDto>>(_countryRepo.GetCountries());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(countries);
        }

        [HttpGet("{countryId}")]
        public IActionResult GetCountry(int countryId)
        {
            if (!_countryRepo.CountryExists(countryId))
                return NotFound();

            var country = _mapper.Map<CountryDto>(_countryRepo.GetCountry(countryId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(country);
        }


        [HttpGet("/Country-by-owner/{ownerId}")]
        public IActionResult GetCountryOfOwner(int ownerId)
        {
            var country = _mapper.Map<CountryDto>(_countryRepo.GetCountryByOwner(ownerId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(country);
        }


        [HttpPost]
        public IActionResult CreateCountry([FromBody] CountryDto countryNew)
        {
            if (countryNew == null | countryNew.Name == string.Empty)
            {
                ModelState.AddModelError("", "New country name cannot be empty");
                return BadRequest(ModelState);
            }

            var countryExists = _countryRepo.GetCountries()
                .Where(c=> c.Name.Trim().ToUpper() == countryNew.Name.Trim().ToUpper())
                .FirstOrDefault();

            if (countryExists != null)
            {
                ModelState.AddModelError("", "Country name already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryMap = _mapper.Map<Country>(countryNew);
            var countryCreated = _countryRepo.CreateCountry(countryMap);

            if (!countryCreated)
            {
                ModelState.AddModelError("", "Something went wrong!");
                return StatusCode(500, ModelState);
            }

            return Ok(countryNew);
        }


        [HttpPut("{countryId}")]
        public IActionResult UpdateCountry(int countryId, [FromBody] CountryDto countryUpdate)
        {
            if (countryUpdate == null)
                return BadRequest(ModelState);

            if (countryId != countryUpdate.Id)
                return BadRequest(ModelState);

            var countryExists = _countryRepo.CountryExists(countryId);

            if (!countryExists)
                return NotFound();

            var countryMap = _mapper.Map<Country>(countryUpdate);

            var updatedCountry = _countryRepo.UpdateCountry(countryMap);

            if (!updatedCountry)
            {
                ModelState.AddModelError("", "Something went wrong!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        [HttpDelete("{countryId}")]
        public IActionResult DeleteCountry(int countryId)
        {
            if (!_countryRepo.CountryExists(countryId))
                return NotFound();

            var countryToDelete = _countryRepo.GetCountry(countryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryDeleted = _countryRepo.DeleteCountry(countryToDelete);

            if (!countryDeleted)
            {
                ModelState.AddModelError("", "Something went wrong!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
