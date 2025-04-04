namespace miPrimerProyecto.Models
{
    public class Proveedor
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public List<Producto> Productos { get; set; } = new List<Producto>();
    }
}
