using System.ComponentModel.DataAnnotations;

namespace RamiloAlonsoSaraTarea4.Models
{
	public class Tipo
	{
		[Key]
		public int id_tipo { get; set; }
		public string nombre { get; set; }
	}
}
