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

            }
            else
            {
                //si existe ya una lista, la deserializamos para añadir el nuevo id del pokemon añadido
                string listaSerializada = Encoding.UTF8.GetString(equipoSession);
                listaEquipo = JsonSerializer.Deserialize<List<int>>(listaSerializada);
                listaEquipo.Add(idPokemon);
                //se vuelve a serializar para guardar la lista
                listaSerializada = JsonSerializer.Serialize(listaEquipo);
                HttpContext.Session.Set("Equipo", Encoding.UTF8.GetBytes(listaSerializada));
            }
            TempData["SuccessMessage"] = "Pokémon añadido al equipo.";
            var pokemons = await _pokemonRepository.GetAllPokemons();
            return View("Index", pokemons);

        }

        public async Task<IActionResult> BorrarDeEquipo(int idPokemon)
        {

            byte[] equipoSession = HttpContext.Session.Get("Equipo");
            List<int> listaEquipo = new List<int>();
            //se deserializa la lista de la variable de sesión
            string listaSerializada = Encoding.UTF8.GetString(equipoSession);
            listaEquipo = JsonSerializer.Deserialize<List<int>>(listaSerializada);
            listaEquipo.Remove(idPokemon);
            //se vuelve a serializar para guardar la lista
            listaSerializada = JsonSerializer.Serialize(listaEquipo);
            HttpContext.Session.Set("Equipo", Encoding.UTF8.GetBytes(listaSerializada));

            TempData["SuccessMessage"] = "Pokémon eliminado del equipo.";
            var pokemons = await _pokemonRepository.GetMyTeam(listaEquipo);
            return View("VerMiEquipo", pokemons);

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
    }
}
