namespace RamiloAlonsoSaraTarea4.Models.Repository
{
    public interface IPokemonRepository
    {
        Task<IEnumerable<Pokemon>> GetAllPokemons();
        Task<IEnumerable<string>> GetEvolucion(int idPokemon);
        Task<IEnumerable<string>> GetInvolucion(int idPokemon);
        Task<IEnumerable<string>> GetMovimientos(int idPokemon);
        Task<IEnumerable<string>> GetTipos(int idPokemon);
        Task<PokemonDetalles> GetDetallesPokemon(int id);
        Task<IEnumerable<Pokemon>> GetPokemonsByTipoPesoAltura(int idTipo, string peso, string altura);
        Task<IEnumerable<Tipo>> GetAllTipos();
        Task<IEnumerable<Pokemon>> GetMyTeam(List<int> listaIdsPokemons);





	}
}
