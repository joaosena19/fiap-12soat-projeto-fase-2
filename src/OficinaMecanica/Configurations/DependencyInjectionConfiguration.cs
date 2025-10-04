using Application.Authentication.Interfaces;
using Application.Authentication.Services;
using Application.Contracts.Gateways;
using Application.OrdemServico.Interfaces;
using Application.OrdemServico.Interfaces.External;
using Application.OrdemServico.Services;
using Infrastructure.AntiCorruptionLayer.OrdemServico;
using Infrastructure.Authentication;
using Infrastructure.Repositories.Cadastros;
using Infrastructure.Repositories.Estoque;
using Infrastructure.Repositories.OrdemServico;

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

            // Serviços de ordem de serviço
            services.AddScoped<IOrdemServicoService, OrdemServicoService>();

            // Gateways de cadastros 
            services.AddScoped<IClienteGateway, ClienteRepository>();
            services.AddScoped<IVeiculoGateway, VeiculoRepository>();
            services.AddScoped<IServicoGateway, ServicoRepository>();

            // Gateways de estoque
            services.AddScoped<IItemEstoqueGateway, ItemEstoqueRepository>();

            // Repositórios de ordem de serviço
            services.AddScoped<IOrdemServicoRepository, OrdemServicoRepository>();

            // Camada anti-corrupção
            services.AddScoped<IServicoExternalService, ServicoExternalService>();
            services.AddScoped<IEstoqueExternalService, EstoqueExternalService>();
            services.AddScoped<IVeiculoExternalService, VeiculoExternalService>();
            services.AddScoped<IClienteExternalService, ClienteExternalService>();

            return services;
        }
    }
}
