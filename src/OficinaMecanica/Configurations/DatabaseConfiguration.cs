using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace API.Configurations
{
    /// <summary>
    /// Configuração do banco de dados
    /// </summary>
    public static class DatabaseConfiguration
    {
        /// <summary>
        /// Configura o Entity Framework Core com PostgreSQL
        /// </summary>
        /// <param name="services">Coleção de serviços</param>
        /// <param name="configuration">Configuração da aplicação</param>
        /// <returns>Coleção de serviços configurada</returns>
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            return services;
        }

        /// <summary>
        /// Remove o DbContext para evitar problemas com múltiplas instâncias (usado em testes)
        /// </summary>
        /// <param name="services">Coleção de serviços</param>
        public static void RemoveDbContext(this IServiceCollection services)
        {
            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(AppDbContext));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            
            var optionsDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (optionsDescriptor != null)
            {
                services.Remove(optionsDescriptor);
            }
        }
    }
}
