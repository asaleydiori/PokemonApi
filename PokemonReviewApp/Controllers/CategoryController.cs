using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Data;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository,IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        public IActionResult GetCategories()
        {
            var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(categories);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetCategoryId(int id)
        {
            if (!_categoryRepository.CategoryExist(id))
                return NotFound();
            var catid = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(id));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(catid);
        }

        [HttpGet("pokemon/{categoryid}")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonsByCategory(int categoryid)
        {
            //if (!_categoryRepository.CategoryExist(categoryid))
            //    return NotFound();
            var pokms = _mapper.Map<List<PokemonDto>>(_categoryRepository.GetPokemonByCategory(categoryid));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(pokms);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCategory([FromBody]CategoryDto categoryCreate)
        {
            if (categoryCreate == null)
                return BadRequest(ModelState);
            var category = _categoryRepository.GetCategories()
                .Where(c => c.Name.Trim().ToUpper() == categoryCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();
            if(category != null)
            {
                ModelState.AddModelError("", "Cette Categorie existe deja");
                return StatusCode(422,ModelState);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var categorymap = _mapper.Map<Category>((categoryCreate));
            if (!_categoryRepository.CreateCategory(categorymap))
            {
                ModelState.AddModelError("", "Erreur de Sauvegarde");
                return StatusCode(500, ModelState);
            }

            return Ok("Categorie Creer avec Succes");
        }

        //update category
        [HttpPut("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryDto updatedCategory)
        {
            if(updatedCategory == null) return BadRequest(ModelState);
            if (updatedCategory.Id != categoryId) return BadRequest(ModelState);
            if (!_categoryRepository.CategoryExist(categoryId)) return NotFound();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var categoryMap = _mapper.Map<Category>(updatedCategory);
            if(!_categoryRepository.UpdateCategory(categoryMap))
            {
                ModelState.AddModelError("", "Erreur lors de la mise a jour de la categorie");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]

        public IActionResult DeleteCategory(int categoryId)
        {
            if(!_categoryRepository.CategoryExist(categoryId))
            {
                return NotFound();
                
            }

            var categoryToDelete = _categoryRepository.GetCategory(categoryId);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.DeleteCategory(categoryToDelete))
            {
                ModelState.AddModelError("", "Erreur lors de la suppression de la categorie");
                return StatusCode(500, ModelState);
            }
            return Ok("supprime avec succes");
        }
    }
}
