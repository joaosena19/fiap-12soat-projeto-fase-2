using AutoMapper;
using Application.Cadastros.Mappings;
using Application.Estoque.Mappings;
using Application.OrdemServico.Mappings;

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
                cfg.AddProfile<OrdemServicoProfile>();
            });
        }

        public static IMapper CreateMapper()
        {
            var config = GetConfiguration();
            return config.CreateMapper();
        }
    }
}
