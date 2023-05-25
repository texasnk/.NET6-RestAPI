using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReviewAPI.Dto;
using ReviewAPI.Interface;
using ReviewAPI.Models;
using ReviewAPI.Repository;

namespace ReviewAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : Controller
    {
        public readonly IPokemonRepository _pokemonRepository;
        public readonly IOwnerRepository _ownerRepository;
        public readonly ICategoryRepository _categoryRepository;
        public readonly IMapper _mapper;

        public PokemonController(IPokemonRepository pokemonRepository, 
            IMapper mapper, 
            IOwnerRepository ownerRepository, 
            ICategoryRepository categoryRepository)
        {
            _pokemonRepository = pokemonRepository;
            _ownerRepository = ownerRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type= typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemons()
        {
            var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            else
                return Ok(pokemons);
        }

        [HttpGet("{pokeId}")]
        [ProducesResponseType(200, Type=typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int pokeId)
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();

            var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokeId));
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(pokemon);
        }

        [HttpGet("{pokeId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int pokeId)
        {
            if(!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();

            var rating = _pokemonRepository.GetPokemonRating(pokeId);
            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(rating);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreatePokemon([FromQuery] int ownerId,[FromQuery] int catId, [FromBody] PokemonDto pokemonCreate)
        {
            if (pokemonCreate == null)
                return BadRequest(ModelState);

            var pokemons = _pokemonRepository.GetPokemons()
                .Where(c => c.Name.Trim().ToUpper() == pokemonCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (pokemons != null)
            {
                ModelState.AddModelError("", "Pokemon already exists!");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);

            if (!_pokemonRepository.CreatePokemon(ownerId, catId, pokemonMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving!");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created!");
        }


        [HttpPut("pokemonId")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePokemon(
            [FromQuery] int ownerId, 
            [FromQuery] int catId, 
            int pokeId, 
            [FromBody] PokemonDto pokemonUpdate)
        {
            if (pokemonUpdate == null || ownerId == 0 || catId == 0)
                return BadRequest(ModelState);

            else if (pokeId != pokemonUpdate.Id)
                return BadRequest(ModelState);

            else if (!_pokemonRepository.PokemonExists(pokeId))
                return NotFound("Pokemon not found!");
            else if (!_ownerRepository.OwnerExists(ownerId))
                return NotFound("Owner not found!");
            else if (!_categoryRepository.CategoryExists(catId))
                return NotFound("Category not found!");
            else if (!ModelState.IsValid)
                return BadRequest(ModelState);

            else
            {
                var pokeMap = _mapper.Map<Pokemon>(pokemonUpdate);
                if (!_pokemonRepository.UpdatePokemon(ownerId, catId, pokeMap))
                {
                    ModelState.AddModelError("", "Something went wrong while updating!");
                    return StatusCode(500, ModelState);
                }

                return Ok("Successfully updated!");
            }

        }

        [HttpDelete("pokemonId")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeletePokemon(int pokemonId)
        {

            if (!_pokemonRepository.PokemonExists(pokemonId))
                return NotFound();

            else if (!ModelState.IsValid)
                return BadRequest(ModelState);

            else
            {
                var pokemonDelete = _pokemonRepository.GetPokemon(pokemonId);
                if (!_pokemonRepository.DeletePokemon(pokemonDelete))
                {
                    ModelState.AddModelError("", "Something went wrong while deleting!");
                    return StatusCode(500, ModelState);
                }

                return Ok("Successfully updated!");
            }
        }

    }
}
