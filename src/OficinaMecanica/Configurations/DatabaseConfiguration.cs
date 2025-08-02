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
            var connectionString = configuration.GetConnectionString("LocalhostConnection");
            
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            return services;
        }
    }
}
