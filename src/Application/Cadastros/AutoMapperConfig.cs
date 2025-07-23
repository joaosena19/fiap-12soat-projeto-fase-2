using AutoMapper;
using Application.Cadastros.Mappings;

namespace Application.Cadastros
{
    public static class AutoMapperConfig
    {
        public static MapperConfiguration GetConfiguration()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ClienteProfile>();
                cfg.AddProfile<VeiculoProfile>();
                cfg.AddProfile<ServicoProfile>();
            });
        }

        public static IMapper CreateMapper()
        {
            var config = GetConfiguration();
            return config.CreateMapper();
        }
    }
}
