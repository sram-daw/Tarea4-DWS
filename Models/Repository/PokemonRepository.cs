using Dapper;
using RamiloAlonsoSaraTarea4.Controllers;

namespace RamiloAlonsoSaraTarea4.Models.Repository
{
	public class PokemonRepository : IPokemonRepository
	{
		private readonly Conexion _conexion;

		public PokemonRepository(Conexion conexion)
		{
			_conexion = conexion;
		}


		public async Task<IEnumerable<Pokemon>> GetAllPokemons()
		{
			var query = "SELECT * FROM pokemon";
			using (var conexion = _conexion.ObtenerConexion())
			{
				var pokemons = await conexion.QueryAsync<Pokemon>(query);
				return pokemons.ToList();
			}
		}

		public async Task<string> GetPokemonName(int id)
		{
			var query = "SELECT nombre FROM pokemon WHERE PokemonId = @id";
			using (var conexion = _conexion.ObtenerConexion())
			{
				var pokemonName = await conexion.QuerySingleOrDefaultAsync<string>(query, new { id });
				return pokemonName.ToString();
			}
		}

		public async Task<IEnumerable<string>> GetEvolucion(int idPokemon)
		{
			var queryIdEvolucion1 = "SELECT pokemon_evolucionado FROM evoluciona_de WHERE pokemon_origen= @idPokemon";
			var queryNombre = "SELECT nombre FROM pokemon WHERE PokemonId= @idEvolucion";
			var queryIdEvolucion2 = "SELECT pokemon_evolucionado FROM evoluciona_de WHERE pokemon_origen= @idEvolucion";
			var queryNombre2 = "SELECT nombre FROM pokemon WHERE PokemonId= @idEvolucion2";

			List<string> listaEvoluciones = new List<string>();
			using (var conexion = _conexion.ObtenerConexion())
			{
				var idEvolucion = await conexion.QuerySingleOrDefaultAsync<int>(queryIdEvolucion1, new { idPokemon });
				if (idEvolucion != 0)
				{
					var nombre = await conexion.QuerySingleOrDefaultAsync<string>(queryNombre, new { idEvolucion });
					listaEvoluciones.Add(nombre.ToString());

					var idEvolucion2 = await conexion.QuerySingleOrDefaultAsync<int>(queryIdEvolucion2, new { idEvolucion });
					if (idEvolucion2 != 0)
					{
						var nombre2 = await conexion.QuerySingleOrDefaultAsync<string>(queryNombre2, new { idEvolucion2 });
						listaEvoluciones.Add(nombre2.ToString());
					}
				}
				else
				{
					listaEvoluciones.Add("Sin evolución");
				}
			}
			return listaEvoluciones;
		}

		public async Task<IEnumerable<string>> GetInvolucion(int idPokemon)
		{
			var queryIdInvolucion1 = "SELECT pokemon_origen FROM evoluciona_de WHERE pokemon_evolucionado= @idPokemon";
			var queryNombre1 = "SELECT nombre FROM pokemon WHERE PokemonId= @idInvolucion";

			var queryIdInvolucion2 = "SELECT pokemon_origen FROM evoluciona_de WHERE pokemon_evolucionado= @idInvolucion";
			var queryNombre2 = "SELECT nombre FROM pokemon WHERE PokemonId= @idInvolucion2";

			List<string> listaInvoluciones = new List<string>();
			using (var conexion = _conexion.ObtenerConexion())
			{
				var idInvolucion = await conexion.QuerySingleOrDefaultAsync<int>(queryIdInvolucion1, new { idPokemon });
				if (idInvolucion != 0) //QuerySingleOrDefaultAsync devuelve 0 si no obtiene resultados
				{
					var nombre = await conexion.QuerySingleOrDefaultAsync<string>(queryNombre1, new { idInvolucion });
					listaInvoluciones.Add(nombre);

					var idInvolucion2 = await conexion.QuerySingleOrDefaultAsync<int>(queryIdInvolucion2, new { idInvolucion });
					if (idInvolucion2 != 0)
					{
						var nombre2 = await conexion.QuerySingleOrDefaultAsync<string>(queryNombre2, new { idInvolucion2 });
						listaInvoluciones.Add(nombre2);
					}
				}
				else
				{
					listaInvoluciones.Add("Sin involución");
				}
			}
			return listaInvoluciones;
		}

		public async Task<IEnumerable<string>> GetMovimientos(int idPokemon)
		{
			var queryMovimientosIds = "SELECT id_movimiento FROM pokemon_movimiento_forma WHERE numero_pokedex = @idPokemon";
			var queryNombreMovimientos = "SELECT nombre FROM movimiento WHERE id_movimiento IN @idsMovimientos";
			using (var conexion = _conexion.ObtenerConexion())
			{
				var idsMovimientos = await conexion.QueryAsync<int>(queryMovimientosIds, new { idPokemon });
				var nombresMovimientos = await conexion.QueryAsync<string>(queryNombreMovimientos, new { idsMovimientos });

				return nombresMovimientos.ToList();
			}
		}

		public async Task<IEnumerable<string>> GetTipos(int idPokemon)
		{
			var queryTiposIds = "SELECT id_tipo FROM pokemon_tipo WHERE numero_pokedex= @idPokemon";
			var queryTiposNombres = "SELECT nombre FROM tipo WHERE id_tipo IN @idsTipos";

			using (var conexion = _conexion.ObtenerConexion())
			{
				var idsTipos = await conexion.QueryAsync<int>(queryTiposIds, new { idPokemon });
				var nombresTipos = await conexion.QueryAsync<string>(queryTiposNombres, new { idsTipos });

				return nombresTipos.ToList();
			}
		}


		public async Task<PokemonDetalles> GetDetallesPokemon(int id)
		{
			PokemonDetalles pokemonDetalles = new PokemonDetalles();

			pokemonDetalles.nombre = (await GetPokemonName(id)).ToString();
			pokemonDetalles.evoluciones = (await GetEvolucion(id)).ToList();
			pokemonDetalles.involuciones = (await GetInvolucion(id)).ToList();
			pokemonDetalles.movimientos = (await GetMovimientos(id)).ToList();
			pokemonDetalles.img = "https://img.pokemondb.net/artwork/large/" + pokemonDetalles.nombre.ToLower() + ".jpg";

			return pokemonDetalles;

		}

		public async Task<IEnumerable<Pokemon>> GetPokemonsByTipoPesoAltura(int idTipo, string rangoPeso, string rangoAltura)
		{
			// Función auxiliar para convertir el rango en un array de dos elementos. El primero será el límite inferior y el segundo el mayor
			Func<string, double[]> parseRange = range =>
			{
				var parts = range.Split('-');
				return parts.Select(double.Parse).ToArray();
			};

			var limitesPeso = parseRange(rangoPeso);
			var limitesAltura = parseRange(rangoAltura);

			var queryIdsPesoAltura = "SELECT PokemonId FROM pokemon WHERE peso BETWEEN @pesoInferior AND @pesoSuperior AND altura BETWEEN @alturaInferior AND @alturaSuperior";
			var queryIdsPorTipo = "SELECT numero_pokedex FROM pokemon_tipo WHERE numero_pokedex IN @idsPesoAltura AND id_tipo = @idTipo";
			var queryPokemons = "SELECT * FROM pokemon WHERE PokemonId IN @idsPorTipo";


			using (var conexion = _conexion.ObtenerConexion())
			{
				var idsPesoAltura = await conexion.QueryAsync<int>(queryIdsPesoAltura, new
				{
					pesoInferior = limitesPeso[0],
					pesoSuperior = limitesPeso[1],
					alturaInferior = limitesAltura[0],
					alturaSuperior = limitesAltura[1]
				});
				if (idsPesoAltura.Any())
				{
					var idsPorTipo = await conexion.QueryAsync<int>(queryIdsPorTipo, new { idsPesoAltura, idTipo });
					if (idsPorTipo.Any())
					{
						var pokemons = await conexion.QueryAsync<Pokemon>(queryPokemons, new { idsPorTipo });
						return pokemons.ToList();
					}
					else return null;

				}
				else return null;
			}
		}

		public async Task<IEnumerable<Tipo>> GetAllTipos()
		{
			var query = "SELECT id_tipo, nombre FROM tipo";
			using (var conexion = _conexion.ObtenerConexion())
			{
				var tipos = await conexion.QueryAsync<Tipo>(query);
				return tipos.ToList();
			}
		}

		public async Task<IEnumerable<Pokemon>> GetMyTeam(List<int> listaIdsPokemons) {

			var query = "SELECT * FROM pokemon";
			if (listaIdsPokemons.Any())
			{
				using (var conexion = _conexion.ObtenerConexion())
				{
					var pokemons = await conexion.QueryAsync<Pokemon>(query);
					return pokemons.Where(x => listaIdsPokemons.Contains(x.PokemonId));
				}
			}
			else return null;
			
		}

	}
}
