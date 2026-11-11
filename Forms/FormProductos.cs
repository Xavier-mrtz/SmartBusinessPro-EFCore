using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using SmartBusinessPro.Data;
using SmartBusinessPro.Models;

namespace SmartBusinessPro.Forms
{
    public partial class FormProductos : Form
    {
        public FormProductos()
        {
            InitializeComponent();
        }

        private void FormProductos_Load(object sender, EventArgs e)
        {
            CargarProductos();
        }

        /// <summary>
        /// Carga los productos desde la base de datos y los muestra en el DataGridView.
        /// Incluye el nombre de la categoría mediante Include (eager loading).
        /// </summary>
        private void CargarProductos()
        {
            try
            {
                using (var context = new SmartBusinessContext())
                {
                    // Verifica si la base de datos existe y es accesible
                    if (!context.Database.CanConnect())
                    {
                        MessageBox.Show(
                            "No se pudo conectar a la base de datos. Verifique que SQL Server esté en ejecución y que la cadena de conexión sea correcta.",
                            "Error de conexión",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    // Consulta con Include para traer la categoría relacionada
                    var productos = context.Productos
                        .Include(p => p.Categoria)
                        .Select(p => new
                        {
                            p.ProductoId,
                            NombreProducto = p.Nombre,
                            p.Precio,
                            p.Existencia,
                            Categoria = p.Categoria != null ? p.Categoria.Nombre : "Sin categoría"
                        })
                        .ToList();

                    dgvProductos.DataSource = productos;

                    // Ajustes visuales opcionales
                    if (dgvProductos.Columns.Count > 0)
                    {
                        dgvProductos.Columns["ProductoId"].HeaderText = "ID";
                        dgvProductos.Columns["NombreProducto"].HeaderText = "Nombre del Producto";
                        dgvProductos.Columns["Precio"].HeaderText = "Precio";
                        dgvProductos.Columns["Existencia"].HeaderText = "Existencia";
                        dgvProductos.Columns["Categoria"].HeaderText = "Categoría";

                        dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }

                    lblEstado.Text = $"Productos cargados: {productos.Count}";
                }
            }
            catch (Microsoft.Data.SqlClient.SqlException sqlEx)
            {
                MessageBox.Show(
                    $"Error de SQL Server:\n{sqlEx.Message}\n\n" +
                    "Posibles causas:\n" +
                    "- Nombre del servidor incorrecto\n" +
                    "- Base de datos 'SmartBusinessPro' no existe\n" +
                    "- SQL Server no está en ejecución\n" +
                    "- Credenciales incorrectas",
                    "Error de base de datos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblEstado.Text = "Error de conexión";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error inesperado:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblEstado.Text = "Error al cargar datos";
            }
        }

        private void btnRecargar_Click(object sender, EventArgs e)
        {
            CargarProductos();
        }
    }
}
