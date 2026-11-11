using Microsoft.EntityFrameworkCore;
using SmartBusinessPro.Models;
using System.Configuration;

namespace SmartBusinessPro.Data
{
    /// <summary>
    /// Contexto centralizado de Entity Framework Core para la base de datos SmartBusinessPro.
    /// Gestiona la conexión y expone los DbSet de las entidades.
    /// </summary>
    public class SmartBusinessContext : DbContext
    {
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        public SmartBusinessContext()
        {
        }

        public SmartBusinessContext(DbContextOptions<SmartBusinessContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Lee la cadena de conexión desde App.config
                // Nombre del connectionString: "SmartBusinessProConnection"
                string connectionString = ConfigurationManager.ConnectionStrings["SmartBusinessProConnection"]?.ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    // Cadena de respaldo para desarrollo (ajustar según el entorno)
                    // Formato típico: Server=NOMBRE_SERVIDOR;Database=SmartBusinessPro;Trusted_Connection=True;TrustServerCertificate=True;
                    connectionString = "Server=.\\SQLEXPRESS;Database=SmartBusinessPro;Trusted_Connection=True;TrustServerCertificate=True;";
                }

                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de relación uno a muchos
            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuraciones adicionales si es necesario (nombres de tablas, etc.)
            modelBuilder.Entity<Categoria>().ToTable("Categorias");
            modelBuilder.Entity<Producto>().ToTable("Productos");
        }
    }
}
