using AutoMapper;
using Application.Estoque.DTO;
using Domain.Estoque.Aggregates;
using Domain.Estoque.Enums;

namespace Application.Estoque.Mappings
{
    public class ItemEstoqueProfile : Profile
    {
        public ItemEstoqueProfile()
        {
            CreateMap<ItemEstoque, RetornoItemEstoqueDTO>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome.Valor))
                .ForMember(dest => dest.Quantidade, opt => opt.MapFrom(src => src.Quantidade.Valor))
                .ForMember(dest => dest.TipoItemEstoque, opt => opt.MapFrom(src => Enum.Parse<TipoItemEstoqueEnum>(src.TipoItemEstoque.Valor, true)));

            CreateMap<CriarItemEstoqueDTO, ItemEstoque>()
                .ConstructUsing(src => ItemEstoque.Criar(src.Nome, src.Quantidade, src.TipoItemEstoque));

            // Note: For update operations, we'll use domain methods directly
            // as they maintain business rules
        }
    }
}
