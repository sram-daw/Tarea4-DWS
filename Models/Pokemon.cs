namespace RamiloAlonsoSaraTarea4.Models;
using System.ComponentModel.DataAnnotations;

public class Pokemon
{
    [Key]
    public int PokemonId { get; set; }
    public string nombre { get; set; }
    public double peso {  get; set; }
    public double altura { get; set; }
	public string img { get; set; }
}
