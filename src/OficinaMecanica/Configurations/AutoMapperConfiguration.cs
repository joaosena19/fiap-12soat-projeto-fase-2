using Application;
using AutoMapper;

namespace API.Configurations
{
    /// <summary>
    /// Configuração do AutoMapper
    /// </summary>
    public static class AutoMapperConfiguration
    {
        /// <summary>
        /// Configura o AutoMapper para a aplicação
        /// </summary>
        /// <param name="services">Coleção de serviços</param>
        /// <returns>Coleção de serviços configurada</returns>
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            var mapperConfig = AutoMapperConfig.GetConfiguration();
            services.AddSingleton(mapperConfig);
            services.AddSingleton<IMapper>(provider => provider.GetRequiredService<MapperConfiguration>().CreateMapper());

            return services;
        }
    }
}
