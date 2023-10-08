using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Data;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class PokemonController : Controller
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly DataContext context;
        private readonly IMapper _mapper;

        public PokemonController(IPokemonRepository pokemonRepository, DataContext context,IMapper mapper)
        {
            _pokemonRepository = pokemonRepository;
            this.context = context;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemons()
        {
            var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(pokemons);
        }
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonId(int id)
        {
            if (!_pokemonRepository.PokemonExist(id))
                return NotFound();
            var pokemonid = _pokemonRepository.GetPokemonId(id);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(pokemonid);
        }

        [HttpGet("{name}")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonName(string name)
        {
            var pokemonname = _pokemonRepository.GetPokemonName(name);
            //if (!_pokemonRepository.PokemonExist(id))
            //    return NotFound();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(pokemonname);
        }

        [HttpGet("{id}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int id)
        {
            if (!_pokemonRepository.PokemonExist(id))
                return NotFound();
            var pokemonrate = _pokemonRepository.GetPokemonRating(id);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(pokemonrate);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int categoryId,[FromBody] PokemonDto pokemonCreate)
        {
            if (pokemonCreate == null)
                return BadRequest(ModelState);
            var pokemon = _pokemonRepository.GetPokemons()
                            .Where(p => p.Name.Trim().ToUpper() == pokemonCreate.Name.TrimEnd().ToUpper())
                            .FirstOrDefault();
            if (pokemon != null)
            {
                ModelState.AddModelError("", "ce pokemon existe deja");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var pokmap = _mapper.Map<Pokemon>((pokemonCreate));
            if (!_pokemonRepository.CreatePokemon(ownerId,categoryId,pokmap))
            {
                ModelState.AddModelError("", "Erreur de Sauvegarde");
                return StatusCode(500, ModelState);
            }
            return Ok("Creé avec Succes");
        }
    }
}
