using Microsoft.AspNetCore.Mvc;
using RamiloAlonsoSaraTarea4.Models;
using RamiloAlonsoSaraTarea4.Models.Repository;

namespace RamiloAlonsoSaraTarea4.Controllers
{
    public class Pokemon : Controller
    {
        private readonly IPokemonRepository _pokemonRepository;

        public Pokemon(IPokemonRepository pokemonRepository)
        {
            _pokemonRepository = pokemonRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var pokemons = await _pokemonRepository.GetAllPokemons();
            return View(pokemons);
        }

        [HttpGet]
        public async Task<IActionResult> DetallePokemon(int id)
        {
            PokemonDetalles detallesPokemon = await _pokemonRepository.GetDetallesPokemon(id);
            return View(detallesPokemon);
        }

        public async Task<IActionResult> FiltrarPokemonPorTipoPesoAltura(int idTipo, double peso, double altura)
        {
            var pokemonsFiltrados = await _pokemonRepository.GetPokemonsByTipoPesoAltura(idTipo,peso,altura);
            if (pokemonsFiltrados != null)
            {                
                return View("Index", pokemonsFiltrados);
            }
            else
            {
				var pokemons = await _pokemonRepository.GetAllPokemons();
				TempData["ErrorMessage"] = "No existe un Pokémon con esas características.";

                return View("Index", pokemons);
            }

        }

		[HttpGet]
		public async Task<IActionResult> Filtrar()
		{
			return View();
		}

	}
}
