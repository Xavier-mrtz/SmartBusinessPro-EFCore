using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartBusinessPro.Models
{
    /// <summary>
    /// Entidad que representa la tabla Categorias en la base de datos SmartBusinessPro.
    /// </summary>
    [Table("Categorias")]
    public class Categoria
    {
        [Key]
        public int CategoriaId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        // Navegación: una categoría tiene muchos productos
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
