using AutoMapper;
using Application.Cadastros.Mappings;
using Application.Estoque.Mappings;

namespace Application
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
                cfg.AddProfile<ItemEstoqueProfile>();
            });
        }

        public static IMapper CreateMapper()
        {
            var config = GetConfiguration();
            return config.CreateMapper();
        }
    }
}
