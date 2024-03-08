using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Models;
using PokemonReviewApp.Services.CountryService;
using PokemonReviewApp.Services.OwnerService;
using PokemonReviewApp.Services.PokemonService;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly IOwnerRepository _ownerRepo;
        private readonly ICountryRepository _countryRepo;
        private readonly IPokemonRepository _pokeRepo;
        private readonly IMapper _mapper;

        public OwnerController(IOwnerRepository ownerRepo, ICountryRepository countryRepo, IPokemonRepository pokeRepo, IMapper mapper)
        {
            _ownerRepo = ownerRepo;
            _countryRepo = countryRepo;
            _pokeRepo = pokeRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetOwners()
        {
            var owners = _mapper.Map<List<OwnerDto>>(_ownerRepo.GetOwners());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(owners);
        }

        [HttpGet("{ownerId}")]
        public IActionResult GetOwner(int ownerId)
        {
            if (!_ownerRepo.OwnerExists(ownerId))
                return NotFound();

            var owner = _mapper.Map<OwnerDto>(_ownerRepo.GetOwner(ownerId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(owner);
        }

        [HttpGet("owned-by/{pokeId}")]
        public IActionResult GetOwnersOfPokemon(int pokeId)
        {
            if (!_pokeRepo.PokemonExists(pokeId))
                return NotFound();

            var owner = _mapper.Map<List<OwnerDto>>(_ownerRepo.GetOwnerofPokemon(pokeId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(owner);
        }

        [HttpGet("{ownerId}/pokemon")]
        public IActionResult GetPokemonByOwner(int ownerId)
        {
            if (!_ownerRepo.OwnerExists(ownerId))
                return NotFound();

            var pokemon = _mapper.Map<List<PokemonDto>>(_ownerRepo.GetPokemonByOwner(ownerId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(pokemon);
        }


        [HttpPost]
        public IActionResult CreateOwner([FromQuery] int countryId, [FromBody] OwnerDto ownerNew)
        {
            if (ownerNew == null)
            {
                ModelState.AddModelError("", "Owner name cannot be left blank");
                return BadRequest(ModelState);
            }

            var ownerExists = _ownerRepo.GetOwners()
                .Where(o => o.FirstName.Trim().ToUpper() == ownerNew.FirstName.Trim().ToUpper()
                & o.LastName.Trim().ToUpper() == ownerNew.LastName.Trim().ToUpper())
                .FirstOrDefault();

            if (ownerExists != null)
            {
                ModelState.AddModelError("", "Owner already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ownerMap = _mapper.Map<Owner>(ownerNew);

            // The code below is used to link the owner to a countryId
            // before creating it
            // It resolves an error from entity framework 
            // because of the relationship between Country and Owners.
            ownerMap.Country = _countryRepo.GetCountry(countryId);

            var ownerCreated = _ownerRepo.CreateOwner(ownerMap);


            if (!ownerCreated)
            {
                ModelState.AddModelError("", "Something wrong happened");
                return StatusCode(500, ModelState);
            }

            return Ok(ownerNew);
        }


        [HttpPut("{ownerId}")]
        public IActionResult UpdateOwner(int ownerId, [FromBody] OwnerDto ownerUpdate)
        {
            if (ownerUpdate == null)
                return BadRequest(ModelState);

            if (ownerId != ownerUpdate.Id)
                return BadRequest(ModelState);

            var ownerExists = _ownerRepo.OwnerExists(ownerId);

            if (!ownerExists)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ownerMap = _mapper.Map<Owner>(ownerUpdate);

            var updatedOwner = _ownerRepo.UpdateOwner(ownerMap);

            if (!updatedOwner)
            {
                ModelState.AddModelError("", "Something went wrong!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        [HttpDelete("{ownerId}")]
        public IActionResult DeleteOwner(int ownerId)
        {
            if (!_ownerRepo.OwnerExists(ownerId))
                return NotFound();

            var ownerToDelete = _ownerRepo.GetOwner(ownerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_ownerRepo.DeleteOwner(ownerToDelete))
            {
                ModelState.AddModelError("", "Something went wrong!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
