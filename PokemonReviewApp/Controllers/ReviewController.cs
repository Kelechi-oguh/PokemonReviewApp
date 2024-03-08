using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Models;
using PokemonReviewApp.Services.OwnerService;
using PokemonReviewApp.Services.PokemonService;
using PokemonReviewApp.Services.ReviewerService;
using PokemonReviewApp.Services.ReviewService;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepo;
        private readonly IPokemonRepository _pokeRepo;
        private readonly IReviewerRepository _reviewerRepo;
        private readonly IMapper _mapper;

        public ReviewController(IReviewRepository reviewRepo, IPokemonRepository pokeRepo, IReviewerRepository reviewerRepo, IMapper mapper)
        {
            _reviewRepo = reviewRepo;
            _pokeRepo = pokeRepo;
            _reviewerRepo = reviewerRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetReviews()
        {
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepo.GetReviews());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviews);
        }

        [HttpGet("{reviewId}")]
        public IActionResult GetReview(int reviewId)
        {
            if (!_reviewRepo.ReviewExists(reviewId))
                return NotFound();

            var review = _mapper.Map<ReviewDto>(_reviewRepo.GetReview(reviewId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(review);
        }

        [HttpGet("{pokeId}/reviews")]
        public IActionResult GetReviewsOfAPokemon(int pokeId)
        {
            if (!_pokeRepo.PokemonExists(pokeId))
                return NotFound();

            var pokeReviews = _mapper.Map<List<ReviewDto>>(_reviewRepo.GetReviewsOfAPokemon(pokeId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(pokeReviews);
        }


        [HttpPost]
        public IActionResult CreateReview([FromQuery] int reviewerId, [FromQuery] string pokemonName, [FromBody] ReviewDto reviewNew)
        {
            if (reviewNew == null)
            {
                ModelState.AddModelError("", "Empty review cannot be created");
                return BadRequest(ModelState);
            }

            var reviewExists = _reviewRepo.GetReviews()
                .Where(r => r.Title.Trim().ToUpper() == reviewNew.Title.Trim().ToUpper())
                .FirstOrDefault();

            if (reviewExists != null)
            {
                ModelState.AddModelError("", "Review title already used");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewMapper = _mapper.Map<Review>(reviewNew);

            reviewMapper.Reviewer = _reviewerRepo.GetReviewer(reviewerId);
            reviewMapper.Pokemon = _pokeRepo.GetPokemonByName(pokemonName);

            var reviewCreated = _reviewRepo.CreateReview(reviewMapper);

            if (!reviewCreated)
            {
                ModelState.AddModelError("", "Something wrong happened");
                return StatusCode(500, ModelState);
            }

            return Ok(reviewNew);
        }


        [HttpPut("{reviewId}")]
        public IActionResult UpdateReview(int reviewId, [FromBody] ReviewDto reviewUpdate)
        {
            if (reviewUpdate == null)
                return BadRequest(ModelState);

            if (reviewId != reviewUpdate.Id)
                return BadRequest(ModelState);

            var reviewExists = _reviewRepo.ReviewExists(reviewId);

            if (!reviewExists)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewMap = _mapper.Map<Review>(reviewUpdate);

            var updatedReview = _reviewRepo.UpdateReview(reviewMap);

            if (!updatedReview)
            {
                ModelState.AddModelError("", "Something went wrong!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        [HttpDelete("{reviewId}")]
        public IActionResult DeleteReview(int reviewId)
        {
            if (!_reviewRepo.ReviewExists(reviewId))
                return NotFound();

            var reviewToDelete = _reviewRepo.GetReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewDeleted = _reviewRepo.DeleteReview(reviewToDelete);

            if (!reviewDeleted)
            {
                ModelState.AddModelError("", "Something went wrong deleting a review");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
