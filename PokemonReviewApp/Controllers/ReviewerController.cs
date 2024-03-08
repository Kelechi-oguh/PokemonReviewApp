using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Models;
using PokemonReviewApp.Services.ReviewerService;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewerController : ControllerBase
    {
        private readonly IReviewerRepository _reviewerRepo;
        private readonly IMapper _mapper;

        public ReviewerController(IReviewerRepository reviewerRepo, IMapper mapper)
        {
            _reviewerRepo = reviewerRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetReviewers()
        {
            var reviewers = _mapper.Map<List<ReviewerDto>>(_reviewerRepo.GetReviewers());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviewers);
        }

        [HttpGet("{reviewerId}")]
        public IActionResult GetReviewer(int reviewerId)
        {
            if (!_reviewerRepo.ReviewerExists(reviewerId))
                return NotFound();

            var reviewer = _mapper.Map<ReviewerDto>(_reviewerRepo.GetReviewer(reviewerId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviewer);
        }

        [HttpGet("{reviewerId}/reviews")]
        public IActionResult GetReviewsByReviewers(int reviewerId)
        {
            if (!_reviewerRepo.ReviewerExists(reviewerId))
                return NotFound();

            var reviews = _mapper.Map<List<ReviewDto>>(_reviewerRepo.GetReviewsByReviewers(reviewerId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviews);
        }


        [HttpPost]
        public IActionResult CreateReviewer([FromBody] ReviewerPostDto reviewerNew)
        {
            if (reviewerNew == null)
            {
                ModelState.AddModelError("", "Reviewer cannot be empty");
                return BadRequest(ModelState);
            }

            var reviewerExists = _reviewerRepo.GetReviewers()
                .Where(r => r.FirstName.Trim().ToUpper() == reviewerNew.FirstName.Trim().ToUpper()
                & r.LastName.Trim().ToUpper() == reviewerNew.LastName.Trim().ToUpper())
                .FirstOrDefault();

            if (reviewerExists != null)
            {
                ModelState.AddModelError("", "Reviewer already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            

            var reviewerMap = _mapper.Map<Reviewer>(reviewerNew);

            var reviewerCreated = _reviewerRepo.CreateReviewer(reviewerMap);

            if (!reviewerCreated)
            {
                ModelState.AddModelError("", "Something wrong happened");
                return StatusCode(500, ModelState);
            }

            return Ok(reviewerNew);
        }


        [HttpPut("{reviewerId}")]
        public IActionResult UpdateReviewer(int reviewerId, [FromBody] ReviewerPostDto reviewerUpdate)
        {
            if (reviewerUpdate == null)
                return BadRequest(ModelState);

            if (reviewerId != reviewerUpdate.Id)
                return BadRequest(ModelState);

            var reviewerExists = _reviewerRepo.ReviewerExists(reviewerId);

            if (!reviewerExists)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerMap = _mapper.Map<Reviewer>(reviewerUpdate);

            var updatedReviewer = _reviewerRepo.UpdateReviewer(reviewerMap);

            if (!updatedReviewer)
            {
                ModelState.AddModelError("", "Something went wrong!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        [HttpDelete("{reviewerId}")]
        public IActionResult DeleteReviewer(int reviewerId)
        {
            if (!_reviewerRepo.ReviewerExists(reviewerId))
                return NotFound();

            var reviewerToDelete = _reviewerRepo.GetReviewer(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerDeleted = _reviewerRepo.DeleteReviewer(reviewerToDelete);

            if (!reviewerDeleted)
            {
                ModelState.AddModelError("", "Something went wrong when deleting a reviewer");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
