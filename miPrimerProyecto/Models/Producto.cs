namespace miPrimerProyecto.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Precio { get; set; }

        //relacion con catergoria 
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }

        //Relaciones con Proveedor 
        public int ProveedorId { get; set; }

        public Proveedor Proveedor { get; set; }
        //inventario 

        public Inventario Inventario { get; set; }

}
}
