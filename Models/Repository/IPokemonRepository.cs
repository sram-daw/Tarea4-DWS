namespace RamiloAlonsoSaraTarea4.Models.Repository
{
    public interface IPokemonRepository
    {
        Task<IEnumerable<Pokemon>> GetAllPokemons();
        Task<IEnumerable<string>> GetEvolucion(int idPokemon);
        Task<IEnumerable<string>> GetInvolucion(int idPokemon);
        Task<IEnumerable<string>> GetMovimientos(int idPokemon);
        Task<IEnumerable<string>> GetTiposPorIdPokemon(int idPokemon);
        Task<PokemonDetalles> GetDetallesPokemon(int id);
        Task<IEnumerable<Pokemon>> GetPokemonsByTipoPesoAltura(int idTipo, string peso, string altura);
        Task<IEnumerable<Pokemon>> GetMyTeam(List<int> listaIdsPokemons);
        Task<EquipoAleatorio> GetRandomTeam();
        Task<double> GetAlturaMedia(List<int> listaIds);
        Task<double> GetPesoMedio(List<int> listaIds);
        Task<List<(string Tipo, int Cantidad)>> GetListaTiposMasRepetidos(List<int> listaIds);
        public string GetImgPokemon(int idPokemon);
        Task<int> Combatir(List<int> equipo1, List<int> equipo2);
        Task<int> ObtenerIdTipoEnCombate(int idPokemon);
        Task<double> GetAlturaById(int idPokemon);
        Task<double> GetPesoById(int idPokemon);








    }
}
