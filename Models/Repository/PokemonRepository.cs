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

        public async Task<string> GetEvolucion(int idPokemon)
        {
            var queryId = "SELECT pokemon_evolucionado FROM evoluciona_de WHERE pokemon_origen= @idPokemon";
            var queryNombre = "SELECT nombre FROM pokemon WHERE PokemonId= @idEvolucion";
            using (var conexion = _conexion.ObtenerConexion())
            {
                var idEvolucion = await conexion.QuerySingleOrDefaultAsync<int>(queryId, new { idPokemon });
                if (idEvolucion != 0)
                {
                    var nombre = await conexion.QuerySingleOrDefaultAsync<string>(queryNombre, new { idEvolucion });
                    return nombre.ToString();
                }
                else return "Sin evolución";
            }
        }

        public async Task<string> GetInvolucion(int idPokemon)
        {
            var queryId = "SELECT pokemon_origen FROM evoluciona_de WHERE pokemon_evolucionado= @idPokemon";
            var queryNombre = "SELECT nombre FROM pokemon WHERE PokemonId= @idInvolucion";
            using (var conexion = _conexion.ObtenerConexion())
            {
                var idInvolucion = await conexion.QuerySingleOrDefaultAsync<int>(queryId, new { idPokemon });
                if (idInvolucion != 0) //QuerySingleOrDefaultAsync devuelve 0 si no obtiene resultados
                {
                    var nombre = await conexion.QuerySingleOrDefaultAsync<string>(queryNombre, new { idInvolucion });
                    return nombre.ToString();
                }
                else return "Sin involución";
            }
        }

        public async Task<IEnumerable<string>> GetMovimientos(int idPokemon)
        {
            var queryTiposIds = "SELECT id_tipo FROM pokemon_tipo WHERE numero_pokedex= @idPokemon";
            var queryMovimientosIds = "SELECT id_tipo_ataque FROM tipo WHERE id_tipo IN @idsTipos";
            var queryNombreMovimientos = "SELECT nombre FROM movimiento WHERE id_movimiento IN @idsMovimientos";
            using (var conexion = _conexion.ObtenerConexion())
            {
                var idsTipos = await conexion.QueryAsync<int>(queryTiposIds, new { idPokemon });
                var idsMovimientos = await conexion.QueryAsync<int>(queryMovimientosIds, new { idsTipos });
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
            pokemonDetalles.evolucion = (await GetEvolucion(id)).ToString();
            pokemonDetalles.involucion = (await GetInvolucion(id)).ToString();
            pokemonDetalles.movimientos = (await GetMovimientos(id)).ToList();

            return pokemonDetalles;

        }

        public async Task<IEnumerable<Pokemon>> GetPokemonsByTipo(int idTipo)
        {
            var queryIds = "SELECT numero_pokedex FROM pokemon_tipo WHERE id_tipo= @idTipo";
            var queryPokemons = "SELECT * FROM pokemon WHERE PokemonId IN @idsPokemons";

            using (var conexion = _conexion.ObtenerConexion())
            {
                var idsPokemons = await conexion.QueryAsync<int>(queryIds, new { idTipo });
                if (idsPokemons.Any())
                {
                    var pokemons = await conexion.QueryAsync<Pokemon>(queryPokemons, new { idsPokemons });
                    return pokemons.ToList();
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
	}
}
