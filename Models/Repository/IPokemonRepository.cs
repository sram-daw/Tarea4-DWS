namespace RamiloAlonsoSaraTarea4.Models.Repository
{
    public interface IPokemonRepository
    {
        Task<IEnumerable<Pokemon>> GetAllPokemons();
        Task<string> GetEvolucion(int idPokemon);
        Task<string> GetInvolucion(int idPokemon);
        Task<IEnumerable<string>> GetMovimientos(int idPokemon);
        Task<IEnumerable<string>> GetTipos(int idPokemon);
        Task<PokemonDetalles> GetDetallesPokemon(int id);
        Task<IEnumerable<Pokemon>> GetPokemonsByTipo(int idTipo);
        Task<IEnumerable<Tipo>> GetAllTipos();



	}
}
