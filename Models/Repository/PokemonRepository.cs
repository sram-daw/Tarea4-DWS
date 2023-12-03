using Dapper;
using RamiloAlonsoSaraTarea4.Controllers;
using System.ComponentModel.DataAnnotations;

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
                foreach (var pokemon in pokemons)
                {
                    pokemon.img = GetImgPokemon(pokemon.PokemonId);
                }
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

		public async Task<IEnumerable<string>> GetTiposPorIdPokemon(int idPokemon)
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
			pokemonDetalles.img = GetImgPokemon(id);

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

		public async Task<IEnumerable<Pokemon>> GetMyTeam(List<int> listaIdsPokemons)
		{
			if (listaIdsPokemons.Any())
			{
				var pokemons = await GetAllPokemons();
				var pokemonsTeam = pokemons.Where(x => listaIdsPokemons.Contains(x.PokemonId));
                foreach (var pokemon in pokemons)
                {
                    pokemon.img = GetImgPokemon(pokemon.PokemonId);
                }
                return pokemonsTeam;
			}
			else return null;

		}

		public async Task<EquipoAleatorio> GetRandomTeam()
		{
			EquipoAleatorio equipoAleatorio = new EquipoAleatorio();
			var pokemons = await GetAllPokemons();
			List<int> randomIds = new List<int>();
			Random random = new Random();
			while (randomIds.Count() < 6)
			{
				randomIds.Add(random.Next(1, 152));
			}
			var pokemonsEquipo = pokemons.Where(x => randomIds.Contains(x.PokemonId));
			equipoAleatorio.pokemons = pokemonsEquipo.ToList();
			equipoAleatorio.cantidad = equipoAleatorio.pokemons.Count();
			equipoAleatorio.alturaMedia = await GetAlturaMedia(randomIds);
			equipoAleatorio.pesoMedio = await GetPesoMedio(randomIds);
			equipoAleatorio.listaTiposMasRepetidos = await GetListaTiposMasRepetidos(randomIds);
			foreach (var pokemon in pokemons)
			{
				pokemon.img = GetImgPokemon(pokemon.PokemonId);
			}
			return equipoAleatorio;

		}

		public async Task<double> GetAlturaMedia(List<int> listaIds)
		{
			var queryAlturas = "SELECT altura FROM pokemon WHERE PokemonId IN @listaIds";
			using (var conexion = _conexion.ObtenerConexion())
			{
				var alturas = await conexion.QueryAsync<double>(queryAlturas, new { listaIds });
				return alturas.Average();
			}
		}

		public async Task<double> GetPesoMedio(List<int> listaIds)
		{
			var queryPesos = "SELECT peso FROM pokemon WHERE PokemonId IN @listaIds";
			using (var conexion = _conexion.ObtenerConexion())
			{
				var alturas = await conexion.QueryAsync<double>(queryPesos, new { listaIds });
				return alturas.Average();
			}
		}

		public async Task<double> GetPesoById(int idPokemon)
		{
			var queryPeso = "SELECT peso FROM pokemon WHERE PokemonId= @idPokemon";
			double pesoPokemon;
			using (var conexion = _conexion.ObtenerConexion())
			{
				pesoPokemon = await conexion.QuerySingleOrDefaultAsync<double>(queryPeso, new { idPokemon });
			}
			return pesoPokemon;
		}

		public async Task<double> GetAlturaById(int idPokemon)
		{
			var queryAltura = "SELECT altura FROM pokemon WHERE PokemonId= @idPokemon";
			double alturaPokemon;
			using (var conexion = _conexion.ObtenerConexion())
			{
				alturaPokemon = await conexion.QuerySingleOrDefaultAsync<double>(queryAltura, new { idPokemon });
			}
			return alturaPokemon;
		}

		public async Task<List<(string Tipo, int Cantidad)>> GetListaTiposMasRepetidos(List<int> listaIds)
		{
			List<int> listaIdsTipos = new List<int>();
			var queryListaTiposIds = "SELECT id_tipo FROM pokemon_tipo WHERE numero_pokedex IN @listaIds";

			List<(string Tipo, int Cantidad)> conteoPorTipo = new List<(string Tipo, int Cantidad)>();

			using (var conexion = _conexion.ObtenerConexion())
			{
				//se obtienen los ids de los tipos asociados a los id de cada pokemon de la lista
				var idsTipos = await conexion.QueryAsync<int>(queryListaTiposIds, new { listaIds });
				listaIdsTipos = idsTipos.ToList();
				//se crea una lista en la que cada item tiene dos ints asociados: uno será el id del tipo y otro el número de veces que aparece en la lista (cuantos pokemon de ese tipo hay)
				List<(int IdTipo, int Cantidad)> conteoPorIdTipo = listaIdsTipos
					.GroupBy(numero => numero) //se agrupan los resultados iguales
					.Select(grupo => (IdTipo: grupo.Key, Cantidad: grupo.Count())) //se transforma cada grupo obtenido en el paso anterior en una tupla con dos propiedades: IdTipo y Cantidad (nº de veces que aparece ese id)
					.OrderByDescending(resultado => resultado.Cantidad)
					.ToList();

				//se transforma la lista anterior en otra de tipo (string Tipo, int Cantidad) para asociar el nombre del tipo con la cantidad de veces que aparece
				for (int i = 0; i < conteoPorIdTipo.Count(); i++)
				{
					string nombreTipo = await GetNombreTipoPorId(conteoPorIdTipo[i].IdTipo);
					conteoPorTipo.Add((nombreTipo, conteoPorIdTipo[i].Cantidad));
				}
				conteoPorTipo = conteoPorTipo.OrderByDescending(tupla => tupla.Cantidad).ToList();
			}
			return conteoPorTipo;
		}

		public async Task<string> GetNombreTipoPorId(int idTipo)
		{
			var queryNombreTipo = "SELECT nombre FROM tipo WHERE id_tipo=@idTipo";

			using (var conexion = _conexion.ObtenerConexion())
			{
				var nombreTipo = await conexion.QuerySingleOrDefaultAsync<string>(queryNombreTipo, new { idTipo });
				return nombreTipo.ToString() ?? string.Empty; //si el resultado es null, devuelve un string vacío.
			}
		}
		public string GetImgPokemon(int idPokemon)
		{
			return "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/" + idPokemon + ".png";
		}

		public async Task<int> Combatir(List<int> idsEquipo1, List<int> idsEquipo2)
		{
			int contadorVictoriasEquipo1 = 0;
			int contadorVictoriasEquipo2 = 0;

			int idPokemon1;
			int idPokemon2;

			int idTipoPokemon1;
			int idTipoPokemon2;

			double pesoPokemon1;
			double pesoPokemon2;

			double alturaPokemon1;
			double alturaPokemon2;

			Random rnd = new Random();

			//Si ambos equipos tienen 6 pokemon, habrá 6 enfrentamientos: los primeros pokemon de cada equipo entre sí, luego los segundos, etc. Gana el tipo con el id menor.
			//Si el equipo propio tiene menos de 6 pokemon, el ganador se elige aleatoriamente

			if (idsEquipo1.Count < 6)
			{
				return rnd.Next(1, 3);
			}
			else
			{
				for (int i = 0; i < idsEquipo1.Count; i++)
				{
					idTipoPokemon1 = await ObtenerIdTipoEnCombate(idsEquipo1[i]);
					idTipoPokemon2 = await ObtenerIdTipoEnCombate(idsEquipo2[i]);

					//se decide el ganador
					if (idTipoPokemon1 < idTipoPokemon2)
					{
						contadorVictoriasEquipo1 += 1;
					}
					else if (idTipoPokemon2 < idTipoPokemon1)
					{
						contadorVictoriasEquipo2 += 1;

					}
					else if (idTipoPokemon1 == idTipoPokemon2) //en caso de empate gana el más pesado. 
					{
						idPokemon1 = idsEquipo1[i];
						idPokemon2 = idsEquipo2[i];

						pesoPokemon1 = await GetPesoById(idPokemon1);
						pesoPokemon2 = await GetPesoById(idPokemon2);

						if (pesoPokemon1 > pesoPokemon2)
						{
							contadorVictoriasEquipo1 += 1;
						}
						else if (pesoPokemon2 > pesoPokemon1)
						{
							contadorVictoriasEquipo2 += 1;

						}
						else if (pesoPokemon1 == pesoPokemon2) //Si pesan lo mismo, gana el más alto
						{
							alturaPokemon1 = await GetAlturaById(idPokemon1);
							alturaPokemon2 = await GetAlturaById(idPokemon2);

							if (alturaPokemon1 > alturaPokemon2)
							{
								contadorVictoriasEquipo1 += 1;
							}
							else if (alturaPokemon2 > alturaPokemon1)
							{
								contadorVictoriasEquipo2 += 1;
							}
							else //si son de la misma altura, el ganador se escoge aleatoriamente 
							{
								int ganador = rnd.Next(1, 3);
								if (ganador == 1)
								{
									contadorVictoriasEquipo1 += 1;
								}
								else
								{
									contadorVictoriasEquipo2 += 1;
								}
							}
						}
					}
				}
			}

			if (contadorVictoriasEquipo1 == contadorVictoriasEquipo2) //si ambos equipos han ganado las mismas veces, gana el equipo con mayor peso medio
			{
				double pesoMedioEquipo1 = await GetPesoMedio(idsEquipo1);
				double pesoMedioEquipo2 = await GetPesoMedio(idsEquipo2);
				if (pesoMedioEquipo1 > pesoMedioEquipo2)
				{
					return 1; //gana el equipo 1
				}
				else return 2; //gana el equipo2
			}
			else if (contadorVictoriasEquipo1 > contadorVictoriasEquipo2)
			{
				return 1; //gana el equipo 1
			}
			else return 2; //gana el equipo2

		}

		public async Task<int> ObtenerIdTipoEnCombate(int idPokemon)
		{

			var queryTiposIds = "SELECT id_tipo FROM pokemon_tipo WHERE numero_pokedex= @idPokemon";

			List<int> idsTipos = new List<int>();
			int idTipo;

			using (var conexion = _conexion.ObtenerConexion())

			{
				var idTiposPokemon = await conexion.QueryAsync<int>(queryTiposIds, new { idPokemon });

				idsTipos = idTiposPokemon.ToList();
			}
			//se asigna el id del tipo (Si el pokemon es de dos tipos, se tiene en cuenta el más favorable.)
			if (idsTipos.Count() > 1)
			{
				idTipo = idsTipos.Min();
			}
			else
			{
				idTipo = idsTipos[0];
			}
			return idTipo;
		}
	}
}

