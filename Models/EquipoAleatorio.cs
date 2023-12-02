namespace RamiloAlonsoSaraTarea4.Models
{
    public class EquipoAleatorio
    {
        public List<Pokemon> pokemons { get; set; }
        public int cantidad { get; set; }
        public List<(string Tipo, int Cantidad)> listaTiposMasRepetidos { get; set; }
        public double pesoMedio { get; set; }
        public double alturaMedia { get; set; }
    }
}
