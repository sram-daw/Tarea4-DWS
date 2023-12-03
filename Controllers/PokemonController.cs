using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using RamiloAlonsoSaraTarea4.Models;
using RamiloAlonsoSaraTarea4.Models.Repository;
using System.Text;
using System.Text.Json;

namespace RamiloAlonsoSaraTarea4.Controllers
{
	public class PokemonController : Controller
	{
		private Combate equiposCombate = new Combate(); //se guardarán los equipos que se van a enfrentar en combate para ser usados en distintas acciones

		private readonly IPokemonRepository _pokemonRepository;

		public PokemonController(IPokemonRepository pokemonRepository)
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
		[Route("Pokemon/Detalle/{id:int}")]
		public async Task<IActionResult> DetallePokemon(int id)
		{
			PokemonDetalles detallesPokemon = await _pokemonRepository.GetDetallesPokemon(id);
			return View(detallesPokemon);
		}

		[HttpGet]
		public async Task<IActionResult> Filtrar()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> FiltrarPorParams(int idTipo, string peso, string altura)
		{
			var pokemonsFiltrados = await _pokemonRepository.GetPokemonsByTipoPesoAltura(idTipo, peso, altura);
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

		public async Task<IActionResult> AddToEquipo(int idPokemon)
		{

			byte[] equipoSession = HttpContext.Session.Get("Equipo");
			List<int> listaEquipo = new List<int>();

			// Si el array de equipo no existe, se crea uno nuevo
			if (equipoSession == null)
			{
				listaEquipo.Add(idPokemon);
				string listaSerializada = JsonSerializer.Serialize(listaEquipo);
				HttpContext.Session.Set("Equipo", Encoding.UTF8.GetBytes(listaSerializada));
				TempData["SuccessMessage"] = "Pokémon añadido al equipo.";
				var pokemons = await _pokemonRepository.GetAllPokemons();
				return View("Index", pokemons);
			}
			else
			{
				//si existe ya una lista, la deserializamos para añadir el nuevo id del pokemon añadido
				string listaSerializada = Encoding.UTF8.GetString(equipoSession);
				listaEquipo = JsonSerializer.Deserialize<List<int>>(listaSerializada);
				int numPokemonEquipo = listaEquipo.Count();
				if (numPokemonEquipo < 6)
				{
					listaEquipo.Add(idPokemon);
					//se vuelve a serializar para guardar la lista
					listaSerializada = JsonSerializer.Serialize(listaEquipo);
					HttpContext.Session.Set("Equipo", Encoding.UTF8.GetBytes(listaSerializada));
					TempData["SuccessMessage"] = "Pokémon añadido al equipo.";
					var pokemons = await _pokemonRepository.GetAllPokemons();
					return View("Index", pokemons);
				}
				else
				{
					TempData["ErrorMessage"] = "El equipo está lleno. Para añadir otro Pokémon, borra antes alguno.";
					var pokemons = await _pokemonRepository.GetAllPokemons();
					return View("Index", pokemons);
				}
			}
		}

		public async Task<IActionResult> BorrarDeEquipo(int idPokemon)
		{
			byte[] equipoSession = HttpContext.Session.Get("Equipo");
			List<int> listaEquipo = new List<int>();
			//se deserializa la lista de la variable de sesión
			string listaSerializada = Encoding.UTF8.GetString(equipoSession);
			listaEquipo = JsonSerializer.Deserialize<List<int>>(listaSerializada);
			listaEquipo.Remove(idPokemon);
			if (!listaEquipo.Any())
			{
				HttpContext.Session.Remove("Equipo"); //si la lista está vacía, se borra la variable de sesión para que el método CombateVsMiEquipoCrear pueda comprobar correctamente si existe un equipo guardado
				return View("VerMiEquipo");
			}
			else
			{
				//se vuelve a serializar para guardar la lista
				listaSerializada = JsonSerializer.Serialize(listaEquipo);
				HttpContext.Session.Set("Equipo", Encoding.UTF8.GetBytes(listaSerializada));

				TempData["SuccessMessage"] = "Pokémon eliminado del equipo.";
				var pokemons = await _pokemonRepository.GetMyTeam(listaEquipo);
				return View("VerMiEquipo", pokemons);
			}

		}

		[HttpGet]
		public async Task<IActionResult> VerMiEquipo()
		{
			byte[] equipoSession = HttpContext.Session.Get("Equipo");
			List<int> listaEquipo = new List<int>();
			if (equipoSession == null)
			{
				return View();
			}
			else
			{
				string listaSerializada = Encoding.UTF8.GetString(equipoSession);
				listaEquipo = JsonSerializer.Deserialize<List<int>>(listaSerializada);
				var pokemons = await _pokemonRepository.GetMyTeam(listaEquipo);
				if (pokemons != null)
				{
					return View("VerMiEquipo", pokemons);
				}
				else return View("VerMiEquipo");
			}

		}

		[HttpGet]
		public async Task<IActionResult> GenerarEquipoAleatorio()
		{
			EquipoAleatorio equipoAleatorio = await _pokemonRepository.GetRandomTeam();
			return View(equipoAleatorio);

		}

		[HttpGet]
		public async Task<IActionResult> Combate()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> CombateVsMiEquipoCrear()
		{
			byte[] equipoSession = HttpContext.Session.Get("Equipo");
			List<int> listaEquipo = new List<int>();
			if (equipoSession == null)
			{
				TempData["ErrorMessage"] = "No tienes ningún equipo. Crea uno desde \"Lista de Pokémon\" para combatir.";
				return View("Combate");
			}
			else
			{
				string listaSerializada = Encoding.UTF8.GetString(equipoSession);
				listaEquipo = JsonSerializer.Deserialize<List<int>>(listaSerializada);
				var pokemons = await _pokemonRepository.GetMyTeam(listaEquipo);
				Combate combate = new Combate();
				combate.equipo1 = new EquipoAleatorio();
				combate.equipo1.pokemons = pokemons.ToList(); //el equipo 1 será el propio
				combate.isRandom = false;
				foreach (var pokemon in combate.equipo1.pokemons)
				{
					pokemon.img = _pokemonRepository.GetImgPokemon(pokemon.PokemonId);
				}
				combate.equipo2 = await _pokemonRepository.GetRandomTeam();
				foreach (var pokemon in combate.equipo2.pokemons)
				{
					pokemon.img = _pokemonRepository.GetImgPokemon(pokemon.PokemonId);
				}

				//se guarda el objeto combate en la variable global
				var equiposCombateBytes = JsonSerializer.SerializeToUtf8Bytes(combate);
				HttpContext.Session.Set("EquiposCombate", equiposCombateBytes);
				return View("Combate", combate);
			}
		}

		[HttpGet]
		public async Task<IActionResult> CombateEquiposRandomCrear()
		{
			Combate combate = new Combate();
			combate.equipo1 = await _pokemonRepository.GetRandomTeam();
			foreach (var pokemon in combate.equipo1.pokemons)
			{
				pokemon.img = _pokemonRepository.GetImgPokemon(pokemon.PokemonId);
			}
			combate.equipo2 = await _pokemonRepository.GetRandomTeam();
			foreach (var pokemon in combate.equipo2.pokemons)
			{
				pokemon.img = _pokemonRepository.GetImgPokemon(pokemon.PokemonId);
			}
			combate.isRandom = true;

			//se guarda el objeto combate en la variable global
			var equiposCombateBytes = JsonSerializer.SerializeToUtf8Bytes(combate);
			HttpContext.Session.Set("EquiposCombate", equiposCombateBytes);
			return View("Combate", combate);
		}

		[HttpGet]
		public async Task<IActionResult> Combatir()
		{
			List<int> idsequipo1 = new List<int>();
			List<int> idsequipo2 = new List<int>();

			Combate combateSession = new Combate();
			//se obtiene el objeto combate guardado en la session storage
			byte[] combateSessionBytes = HttpContext.Session.Get("EquiposCombate");
			string combateSerializado = Encoding.UTF8.GetString(combateSessionBytes);
			combateSession = JsonSerializer.Deserialize<Combate>(combateSerializado);

			foreach (var pokemon in combateSession.equipo1.pokemons) {
				idsequipo1.Add(pokemon.PokemonId);
			}

			foreach (var pokemon in combateSession.equipo2.pokemons)
			{
				idsequipo2.Add(pokemon.PokemonId);
			}

			combateSession.equipoGanador = await _pokemonRepository.Combatir(idsequipo1, idsequipo2);

			return View("Combate", combateSession);

		}
	}
}
