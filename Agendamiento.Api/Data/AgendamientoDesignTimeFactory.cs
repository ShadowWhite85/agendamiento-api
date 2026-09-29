using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Agendamiento.Api.Data;

// Permite generar migraciones sin construir el host completo
// (Swashbuckle 10 no es instantiable en design-time)
public class AgendamientoDesignTimeFactory : IDesignTimeDbContextFactory<AgendamientoDbContext>
{
    public AgendamientoDbContext CreateDbContext(string[] args) =>
        new(new DbContextOptionsBuilder<AgendamientoDbContext>()
            .UseSqlite("Data Source=agendamiento.db")
            .Options);
}
