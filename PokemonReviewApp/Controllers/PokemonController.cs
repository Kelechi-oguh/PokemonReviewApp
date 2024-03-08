using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Models;
using PokemonReviewApp.Services.OwnerService;
using PokemonReviewApp.Services.PokemonService;
using PokemonReviewApp.Services.ReviewerService;
using PokemonReviewApp.Services.ReviewService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PokemonReviewApp.Controllers
{
    [Route("api/pokemon")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonRepository _pokemonRepo;
        private readonly IReviewRepository _reviewRepo;
        private readonly IMapper _mapper;

        public PokemonController(IPokemonRepository pokemonRepo, IReviewRepository reviewRepo, IMapper mapper)
        {
            _pokemonRepo = pokemonRepo;
            _reviewRepo = reviewRepo;
            _mapper = mapper;
        }

        // GET: api/<PokemonController>
        [HttpGet("get-pokemons")]
        public IActionResult GetPokemons()
        {
            var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepo.GetPokemons());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(pokemons);
        }

        [HttpGet("{pokeId}")]
        public IActionResult GetPokemon(int pokeId)
        {
            var pokemon = _mapper.Map<PokemonDto>(_pokemonRepo.GetPokemonById(pokeId));
            if (pokemon == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(pokemon);
        }


        [HttpGet("name/{pokeName}")]
        public IActionResult GetPokemonByName(string pokeName)
        {
            var pokemon = _mapper.Map<PokemonDto>(_pokemonRepo.GetPokemonByName(pokeName));
            if (pokemon == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(pokemon);
        }


        [HttpGet("{pokeId}/rating")]
        public IActionResult GetPokemonRating(int pokeId)
        {
            if (!_pokemonRepo.PokemonExists(pokeId))
                return NotFound();

            var ratingAvg = _pokemonRepo.GetPokemonRating(pokeId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(ratingAvg);
        }

        [HttpGet("{pokeId}/all-pokemon-rating")]
        public IActionResult AllPokemonRatings(int pokeId)
        {
            if (!_pokemonRepo.PokemonExists(pokeId))
                return NotFound();

            var ratings = _pokemonRepo.GetAllPokemonsRatingById(pokeId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(ratings);
        }


        [HttpPost]
        public IActionResult CreatePokemon([FromQuery] int categoryId, [FromQuery] int ownerId, [FromBody] PokemonDto pokemonNew)
        {
            if (pokemonNew == null)
            {
                ModelState.AddModelError("", "Pokemon cannot be left blank");
                return BadRequest(ModelState);
            }

            var pokemonExists = _pokemonRepo.GetPokemons()
                .Where(p=> p.Name.Trim().ToUpper() == pokemonNew.Name.Trim().ToUpper())
                .FirstOrDefault();

            if (pokemonExists != null)
            {
                ModelState.AddModelError("", "Pokemon already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pokemonMapper = _mapper.Map<Pokemon>(pokemonNew);

            var pokemonCreated = _pokemonRepo.CreatePokemon(ownerId, categoryId, pokemonMapper);

            if (!pokemonCreated)
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }

            return Ok(pokemonNew);
        }

        [HttpPut("{pokeId}")]
        public IActionResult UpdatePokemon(int pokeId, [FromBody] PokemonDto pokemonUpdate)
        {
            if (pokemonUpdate == null)
                return BadRequest(ModelState);

            if (pokeId != pokemonUpdate.Id)
                return BadRequest(ModelState);

            var pokeExist = _pokemonRepo.PokemonExists(pokeId);
            
            if (!pokeExist)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pokemonMap = _mapper.Map<Pokemon>(pokemonUpdate);

            var updatedPokemon = _pokemonRepo.UpdatePokemon(pokemonMap);

            if (!updatedPokemon)
            {
                ModelState.AddModelError("", "Something went wrong!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        [HttpDelete("{pokeId}")]
        public IActionResult DeleteCategory(int pokeId)
        {
            if (!_pokemonRepo.PokemonExists(pokeId))
                return NotFound();

            var pokemonToDelete = _pokemonRepo.GetPokemonById(pokeId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pokemonDeleted = _pokemonRepo.DeletePokemon(pokemonToDelete);

            if (!pokemonDeleted)
            {
                ModelState.AddModelError("", "Something went wrong when deleting Pokemon");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
