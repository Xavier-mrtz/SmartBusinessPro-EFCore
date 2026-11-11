using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartBusinessPro.Models
{
    /// <summary>
    /// Entidad que representa la tabla Productos en la base de datos SmartBusinessPro.
    /// Relacionada con Categorias mediante CategoriaId (clave foránea).
    /// </summary>
    [Table("Productos")]
    public class Producto
    {
        [Key]
        public int ProductoId { get; set; }

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        public int Existencia { get; set; }

        // Clave foránea
        public int CategoriaId { get; set; }

        // Navegación hacia la categoría
        [ForeignKey("CategoriaId")]
        public virtual Categoria Categoria { get; set; }
    }
}
