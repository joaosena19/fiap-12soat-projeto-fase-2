using Application.Authentication.Interfaces;
using Application.Authentication.Services;
using Infrastructure.Authentication;

namespace API.Configurations
{
    /// <summary>
    /// Configuração de injeção de dependências para serviços e repositórios
    /// </summary>
    public static class DependencyInjectionConfiguration
    {
        /// <summary>
        /// Registra todos os serviços e repositórios da aplicação
        /// </summary>
        /// <param name="services">Coleção de serviços</param>
        /// <returns>Coleção de serviços configurada</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Serviços de autenticação
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}
