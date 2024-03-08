using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Models;
using PokemonReviewApp.Services.CategoryService;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet("all-categories")]
        public IActionResult GetCategories()
        {
            var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(categories);
        }


        [HttpGet("{categoryId}")]
        public IActionResult GetCategory(int categoryId)
        {
            if (!_categoryRepository.CategoriesExists(categoryId))
                return NotFound();

            var category = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(categoryId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(category);
        }


        [HttpGet("pokemon/{categoryId}")]
        public IActionResult GetPokemonByCategoryId(int categoryId)
        {
            if (!_categoryRepository.CategoriesExists(categoryId))
                return NotFound();

            var pokemons = _mapper.Map<List<PokemonDto>>(_categoryRepository.GetPokemonByCategory(categoryId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(pokemons);
        }

        [HttpPost]
        public IActionResult CreateCategory([FromBody] CategoryDto categoryNew)
        {
            if (categoryNew == null | categoryNew.Name == string.Empty)
            {
                ModelState.AddModelError("", "New category name cannot be empty");
                return BadRequest(ModelState);
            }

            var categoryExists = _categoryRepository.GetCategories()
                .Where(c=> c.Name.Trim().ToUpper() == categoryNew.Name.Trim().ToUpper())
                .FirstOrDefault();

            if (categoryExists != null)
            {
                ModelState.AddModelError("", "Category already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryMap = _mapper.Map<Category>(categoryNew);
            var categoryCreated = _categoryRepository.CreateCategory(categoryMap);

            if (!categoryCreated)
            {
                ModelState.AddModelError("", "Something went wrong!");
                return StatusCode(500, ModelState);
            }

            return Ok(categoryNew);
        }


        [HttpPut("{categoryId}")]
        public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryDto categoryUpdate)
        {
            if (categoryUpdate == null)
                return BadRequest(ModelState);

            if (categoryId != categoryUpdate.Id)
                return BadRequest(ModelState);

            var categoryExists = _categoryRepository.CategoriesExists(categoryId);

            if (!categoryExists)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryMap = _mapper.Map<Category>(categoryUpdate);

            var updatedCategory = _categoryRepository.UpdateCategory(categoryMap);

            if (!updatedCategory)
            {
                ModelState.AddModelError("", "Something went wrong!");
                return StatusCode(500, ModelState);
            }

            // update(put) methods normally return NoContent.
            return NoContent();
        }


        [HttpDelete("{categoryId}")]
        public IActionResult DeleteCategory(int categoryId)
        {
            if (!_categoryRepository.CategoriesExists(categoryId))
                return NotFound();

            var categoryToDelete = _categoryRepository.GetCategory(categoryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryDeleted = _categoryRepository.DeleteCategory(categoryToDelete);
            if (!categoryDeleted)
            {
                ModelState.AddModelError("", "Something went wrong!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
