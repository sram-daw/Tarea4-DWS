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
			var Data = await _pokemonRepository.GetAllPokemons();
			return View(Data);
		}

		[HttpGet]
		public async Task<IActionResult> DetallePokemon(int id)
		{
			PokemonDetalles detallesPokemon = await _pokemonRepository.GetDetallesPokemon(id);
			return View(detallesPokemon);
		}

	}
}
