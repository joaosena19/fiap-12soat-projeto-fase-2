using AutoMapper;
using Application.OrdemServico.DTO;
using Domain.OrdemServico.Aggregates.OrdemServico;

namespace Application.OrdemServico.Mappings
{
    public class OrdemServicoProfile : Profile
    {
        public OrdemServicoProfile()
        {
            // Mapeamento completo (com serviços, itens e orçamento)
            CreateMap<Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico, RetornoOrdemServicoCompletaDTO>()
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Codigo.Valor))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Valor))
                .ForMember(dest => dest.DataCriacao, opt => opt.MapFrom(src => src.Historico.DataCriacao))
                .ForMember(dest => dest.DataInicioExecucao, opt => opt.MapFrom(src => src.Historico.DataInicioExecucao))
                .ForMember(dest => dest.DataFinalizacao, opt => opt.MapFrom(src => src.Historico.DataFinalizacao))
                .ForMember(dest => dest.DataEntrega, opt => opt.MapFrom(src => src.Historico.DataEntrega))
                .ForMember(dest => dest.ServicosIncluidos, opt => opt.MapFrom(src => src.ServicosIncluidos))
                .ForMember(dest => dest.ItensIncluidos, opt => opt.MapFrom(src => src.ItensIncluidos))
                .ForMember(dest => dest.Orcamento, opt => opt.MapFrom(src => src.Orcamento));

            // Mapeamento básico (sem serviços, itens e orçamento)
            CreateMap<Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico, RetornoOrdemServicoDTO>()
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Codigo.Valor))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Valor))
                .ForMember(dest => dest.DataCriacao, opt => opt.MapFrom(src => src.Historico.DataCriacao))
                .ForMember(dest => dest.DataInicioExecucao, opt => opt.MapFrom(src => src.Historico.DataInicioExecucao))
                .ForMember(dest => dest.DataFinalizacao, opt => opt.MapFrom(src => src.Historico.DataFinalizacao))
                .ForMember(dest => dest.DataEntrega, opt => opt.MapFrom(src => src.Historico.DataEntrega));

            // Mapeamento com serviços e itens (sem orçamento)
            CreateMap<Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico, RetornoOrdemServicoComServicosItensDTO>()
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Codigo.Valor))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Valor))
                .ForMember(dest => dest.DataCriacao, opt => opt.MapFrom(src => src.Historico.DataCriacao))
                .ForMember(dest => dest.DataInicioExecucao, opt => opt.MapFrom(src => src.Historico.DataInicioExecucao))
                .ForMember(dest => dest.DataFinalizacao, opt => opt.MapFrom(src => src.Historico.DataFinalizacao))
                .ForMember(dest => dest.DataEntrega, opt => opt.MapFrom(src => src.Historico.DataEntrega))
                .ForMember(dest => dest.ServicosIncluidos, opt => opt.MapFrom(src => src.ServicosIncluidos))
                .ForMember(dest => dest.ItensIncluidos, opt => opt.MapFrom(src => src.ItensIncluidos));

            CreateMap<ServicoIncluido, RetornoServicoIncluidoDTO>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome.Valor))
                .ForMember(dest => dest.Preco, opt => opt.MapFrom(src => src.Preco.Valor));

            CreateMap<ItemIncluido, RetornoItemIncluidoDTO>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome.Valor))
                .ForMember(dest => dest.Quantidade, opt => opt.MapFrom(src => src.Quantidade.Valor))
                .ForMember(dest => dest.TipoItemIncluido, opt => opt.MapFrom(src => src.TipoItemIncluido.Valor))
                .ForMember(dest => dest.Preco, opt => opt.MapFrom(src => src.Preco.Valor));

            CreateMap<Domain.OrdemServico.Aggregates.OrdemServico.Orcamento, RetornoOrcamentoDTO>()
                .ForMember(dest => dest.DataCriacao, opt => opt.MapFrom(src => src.DataCriacao.Valor))
                .ForMember(dest => dest.Preco, opt => opt.MapFrom(src => src.Preco.Valor));
        }
    }
}
