using AutoMapper;
using Application.Cadastros.Dtos;
using Domain.Cadastros.Aggregates;

namespace Application.Cadastros.Mappings
{
    public class ClienteProfile : Profile
    {
        public ClienteProfile()
        {
            CreateMap<Cliente, RetornoClienteDto>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome.Valor))
                .ForMember(dest => dest.DocumentoIdentificador, opt => opt.MapFrom(src => src.DocumentoIdentificador.Valor))
                .ForMember(dest => dest.TipoDocumentoIdentificador, opt => opt.MapFrom(src => src.DocumentoIdentificador.TipoDocumento.ToString()));

            CreateMap<CriarClienteDto, Cliente>()
                .ConstructUsing(src => Cliente.Criar(src.Nome, src.DocumentoIdentificador));

            // Note: For update operations, we'll use domain methods directly
            // as they maintain business rules
        }
    }
}
