using AutoMapper;
using Application.Cadastros.DTO;
using Domain.Cadastros.Aggregates;

namespace Application.Cadastros.Mappings
{
    public class VeiculoProfile : Profile
    {
        public VeiculoProfile()
        {
            CreateMap<Veiculo, RetornoVeiculoDTO>()
                .ForMember(dest => dest.Placa, opt => opt.MapFrom(src => src.Placa.Valor))
                .ForMember(dest => dest.Modelo, opt => opt.MapFrom(src => src.Modelo.Valor))
                .ForMember(dest => dest.Marca, opt => opt.MapFrom(src => src.Marca.Valor))
                .ForMember(dest => dest.Cor, opt => opt.MapFrom(src => src.Cor.Valor))
                .ForMember(dest => dest.Ano, opt => opt.MapFrom(src => src.Ano.Valor))
                .ForMember(dest => dest.TipoVeiculo, opt => opt.MapFrom(src => src.TipoVeiculo.Valor));

            CreateMap<CriarVeiculoDTO, Veiculo>()
                .ConstructUsing(src => Veiculo.Criar(src.ClienteId, src.Placa, src.Modelo, src.Marca, src.Cor, src.Ano, src.TipoVeiculo));

            // Note: For update operations, we'll use domain methods directly
            // as they maintain business rules
        }
    }
}
