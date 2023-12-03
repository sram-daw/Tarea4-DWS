namespace RamiloAlonsoSaraTarea4.Models
{
    public class Combate
    {
        public EquipoAleatorio equipo1 { get; set; }
        public EquipoAleatorio equipo2 { get; set; }
		public bool isRandom { get; set; }
        public int equipoGanador { get; set; }
	}
}
