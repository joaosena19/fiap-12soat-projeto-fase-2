using AutoMapper;
using Application.Cadastros.Dtos;
using Domain.Cadastros.Aggregates;

namespace Application.Cadastros.Mappings
{
    public class ServicoProfile : Profile
    {
        public ServicoProfile()
        {
            CreateMap<Servico, RetornoServicoDto>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome.Valor))
                .ForMember(dest => dest.Preco, opt => opt.MapFrom(src => src.Preco.Valor));

            CreateMap<CriarServicoDto, Servico>()
                .ConstructUsing(src => Servico.Criar(src.Nome, src.Preco));

            // Note: For update operations, we'll use domain methods directly
            // as they maintain business rules
        }
    }
}
