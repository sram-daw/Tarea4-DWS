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
			var tipos = await _pokemonRepository.GetAllTipos();
			PokemonViewModel pokemonWithTipos = new PokemonViewModel();
			pokemonWithTipos.Pokemons = pokemons.ToList();
			pokemonWithTipos.Tipos = tipos.ToList();

			return View(pokemonWithTipos);
		}

		[HttpGet]
		public async Task<IActionResult> DetallePokemon(int id)
		{
			PokemonDetalles detallesPokemon = await _pokemonRepository.GetDetallesPokemon(id);
			return View(detallesPokemon);
		}

		public async Task<IActionResult> FiltrarPokemonPorTipo(int id)
		{
			var pokemons = await _pokemonRepository.GetPokemonsByTipo(id);
			var tipos = await _pokemonRepository.GetAllTipos();
			PokemonViewModel pokemonWithTipos = new PokemonViewModel();
			pokemonWithTipos.Pokemons = pokemons.ToList();
			pokemonWithTipos.Tipos = tipos.ToList();
			if (pokemons != null)
			{
				return View("Index", pokemonWithTipos);
			}
			else
			{
				TempData["ErrorMessage"] = "No existe un tipo con ese ID asociado.";

				return View("Index");
			}

		}

	}
}
