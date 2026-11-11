# SmartBusinessPro - Contexto de Datos con Entity Framework Core

Implementación de un contexto centralizado (`SmartBusinessContext`) para una aplicación Windows Forms que consulta productos y categorías desde SQL Server usando Entity Framework Core.

## Objetivo
Configurar la conexión a la base de datos `SmartBusinessPro` de forma centralizada, evitando repetir la configuración en cada formulario, y demostrar la recuperación de datos en un `DataGridView`.

## Estructura del proyecto

```
SmartBusinessPro-EFCore/
├── Models/
│   ├── Categoria.cs
│   └── Producto.cs
├── Data/
│   └── SmartBusinessContext.cs
├── Forms/
│   └── FormProductos.cs
├── App.config.example
└── README.md
```

## Requisitos

- Visual Studio 2022 o superior
- .NET Framework 4.8 o .NET 6/8 (Windows Forms)
- SQL Server (Express, Developer o LocalDB)
- Paquetes NuGet:
  - `Microsoft.EntityFrameworkCore.SqlServer`
  - `Microsoft.EntityFrameworkCore`
  - `System.Configuration.ConfigurationManager` (si se usa .NET Core / .NET 5+)

### Instalación de paquetes (Package Manager Console)

```powershell
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package System.Configuration.ConfigurationManager
```

## Configuración de la cadena de conexión

La cadena de conexión se define en `App.config` (recomendado por Microsoft para Windows Forms):

```xml
<connectionStrings>
  <add name="SmartBusinessProConnection"
       connectionString="Server=.\SQLEXPRESS;Database=SmartBusinessPro;Trusted_Connection=True;TrustServerCertificate=True;"
       providerName="Microsoft.EntityFrameworkCore.SqlServer" />
</connectionStrings>
```

### Componentes de la cadena

| Componente | Descripción |
|------------|-------------|
| **Server** | Nombre de la instancia de SQL Server (ej: `.\SQLEXPRESS`, `(localdb)\mssqllocaldb`, `localhost`) |
| **Database** | Nombre de la base de datos (`SmartBusinessPro`) |
| **Trusted_Connection=True** | Autenticación de Windows (no requiere usuario/contraseña) |
| **TrustServerCertificate=True** | Útil en desarrollo cuando el certificado no está confiado |

> **Nota**: Ajusta el valor de `Server` según tu entorno. Si usas autenticación SQL, reemplaza `Trusted_Connection=True` por `User ID=sa;Password=TuPassword;`.

## Clase SmartBusinessContext

```csharp
public class SmartBusinessContext : DbContext
{
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Categoria> Categorias { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string connectionString = ConfigurationManager
                .ConnectionStrings["SmartBusinessProConnection"]?.ConnectionString;

            // Fallback de desarrollo
            if (string.IsNullOrEmpty(connectionString))
                connectionString = "Server=.\SQLEXPRESS;Database=SmartBusinessPro;Trusted_Connection=True;TrustServerCertificate=True;";

            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
```

## Entidades

### Categoria
- `CategoriaId` (PK)
- `Nombre`
- Colección de navegación `Productos`

### Producto
- `ProductoId` (PK)
- `Nombre`
- `Precio`
- `Existencia`
- `CategoriaId` (FK)
- Navegación `Categoria`

## Cómo usar en un formulario

```csharp
using (var context = new SmartBusinessContext())
{
    var productos = context.Productos
        .Include(p => p.Categoria)
        .Select(p => new
        {
            p.Nombre,
            p.Precio,
            p.Existencia,
            Categoria = p.Categoria.Nombre
        })
        .ToList();

    dataGridView1.DataSource = productos;
}
```

## Pruebas realizadas

### 1. Conexión válida
- Se configuró correctamente la cadena apuntando a una instancia local de SQL Server.
- Se verificó con `context.Database.CanConnect()`.
- Se recuperaron los registros de `Productos` y se mostraron en el `DataGridView` junto con el nombre de la categoría.

### 2. Nombre de servidor incorrecto
- Al cambiar `Server` a un valor inexistente (ej: `Server=ServidorInexistente`), se produce una `SqlException`.
- El mensaje de error indica que no se puede conectar al servidor.
- Se captura y se muestra un mensaje amigable al usuario.

### 3. Nombre de base de datos incorrecto
- Al cambiar `Database=SmartBusinessPro` por un nombre inexistente, SQL Server responde que la base de datos no existe o no se puede abrir.
- El manejo de excepciones permite informar claramente el problema.

## Errores comunes y soluciones

| Error | Causa probable | Solución |
|-------|----------------|----------|
| `A network-related or instance-specific error occurred` | SQL Server no está en ejecución o el nombre de instancia es incorrecto | Verificar el servicio SQL Server y el nombre de la instancia |
| `Cannot open database "SmartBusinessPro"` | La base de datos no existe | Crear la base de datos o corregir el nombre |
| `Login failed` | Credenciales incorrectas | Usar `Trusted_Connection=True` o verificar usuario/contraseña |
| `The ConnectionString property has not been initialized` | No se encontró la entrada en App.config | Verificar el nombre `SmartBusinessProConnection` |

## Conclusión

Centralizar el acceso a datos mediante un único `DbContext` (`SmartBusinessContext`) ofrece las siguientes ventajas:

1. **Una sola configuración**: La cadena de conexión y el proveedor se definen una sola vez.
2. **Mantenibilidad**: Cambios en la conexión o en el modelo se realizan en un solo lugar.
3. **Reutilización**: Cualquier formulario o capa de la aplicación puede instanciar el contexto y consultar datos de forma consistente.
4. **Separación de responsabilidades**: Los formularios se enfocan en la UI; el contexto se encarga de la persistencia.

Para aplicaciones Windows Forms, Microsoft recomienda (y documenta) el uso de `App.config` junto con `ConfigurationManager` para almacenar la cadena de conexión de forma externa al código fuente, facilitando el cambio de entornos (desarrollo, pruebas, producción) sin recompilar.

---

**Repositorio**: https://github.com/Xavier-mrtz/SmartBusinessPro-EFCore
