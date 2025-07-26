using Domain.OrdemServico.Enums;
using Domain.OrdemServico.ValueObjects.OrdemServico;
using Shared.Exceptions;
using System.Net;
using UUIDNext;

namespace Domain.OrdemServico.Aggregates.OrdemServico
{
    public class OrdemServico
    {
        public Guid Id { get; private init; }
        public Guid VeiculoId { get; private set; }
        public Codigo Codigo { get; private set; } = null!;
        public Status Status { get; private set; } = null!;
        public HistoricoTemporal Historico { get; private set; }


        private readonly List<ServicoIncluido> _servicosIncluidos = new();
        private readonly List<ItemIncluido> _itensIncluidos = new();
        public IReadOnlyCollection<ServicoIncluido> ServicosIncluidos => _servicosIncluidos.AsReadOnly();
        public IReadOnlyCollection<ItemIncluido> ItensIncluidos => _itensIncluidos.AsReadOnly();

        public Orcamento? Orcamento { get; private set; } = null;

        // Construtor sem parâmetro para EF Core
        private OrdemServico() 
        {
            Historico = new HistoricoTemporal(DateTime.UtcNow);
        }

        private OrdemServico(Guid id, Guid veiculoId, Codigo codigo, Status status, HistoricoTemporal historico)
        {
            Id = id;
            VeiculoId = veiculoId;
            Codigo = codigo;
            Status = status;
            Historico = historico;
        }

        public static OrdemServico Criar(Guid veiculoId)
        {
            return new OrdemServico(
                Uuid.NewSequential(),
                veiculoId,
                Codigo.GerarNovo(),
                new Status(StatusOrdemServicoEnum.Recebida),
                new HistoricoTemporal(DateTime.UtcNow)
            );
        }

        public bool PermiteAlterarServicosItens()
        {
            var statusPermitidos = new List<string>() { StatusOrdemServicoEnum.Recebida.ToString().ToLower(), StatusOrdemServicoEnum.EmDiagnostico.ToString().ToLower() };
            return statusPermitidos.Contains(Status.Valor);            
        }

        public void AdicionarServico(Guid servicoOriginalId, string nome, decimal preco)
        {
            if (!PermiteAlterarServicosItens())
                throw new DomainException($"Não é possível adicionar serviços a uma Ordem de Serviço com o status '{Status.Valor}'.", HttpStatusCode.UnprocessableContent);

            if (_servicosIncluidos.Any(s => s.ServicoOriginalId == servicoOriginalId))
                throw new DomainException("Este serviço já foi incluído nesta OS.", HttpStatusCode.Conflict);

            var novoServico = ServicoIncluido.Criar(servicoOriginalId, nome, preco);
            _servicosIncluidos.Add(novoServico);
        }

        public void AdicionarItem(Guid itemEstoqueOriginalId, string nome, decimal precoUnitario, int quantidade, TipoItemIncluidoEnum tipo)
        {
            if (!PermiteAlterarServicosItens())
                throw new DomainException($"Não é possível adicionar itens a uma Ordem de Serviço com o status '{Status.Valor}'.", HttpStatusCode.UnprocessableContent);

            if (quantidade <= 0)
                throw new DomainException("A quantidade deve ser maior que zero.", HttpStatusCode.BadRequest);

            var itemExistente = _itensIncluidos.FirstOrDefault(i => i.ItemEstoqueOriginalId == itemEstoqueOriginalId);

            if (itemExistente is not null)
            {
                itemExistente.IncrementarQuantidade(quantidade);
            }
            else
            {
                var novoItem = ItemIncluido.Criar(itemEstoqueOriginalId, nome, precoUnitario, quantidade, tipo);
                _itensIncluidos.Add(novoItem);
            }
        }

        public void RemoverServico(Guid servicoIncluidoId)
        {
            if (!PermiteAlterarServicosItens())
                throw new DomainException($"Não é possível remover serviços de uma Ordem de Serviço com o status '{Status.Valor}'.", HttpStatusCode.UnprocessableContent);

            var servicoParaRemover = _servicosIncluidos.FirstOrDefault(s => s.Id == servicoIncluidoId);
            if (servicoParaRemover == null)
                throw new DomainException("Serviço não encontrado nesta ordem de serviço.", HttpStatusCode.NotFound);

            _servicosIncluidos.Remove(servicoParaRemover);
        }

        public void RemoverItem(Guid itemIncluidoId)
        {
            if (!PermiteAlterarServicosItens())
                throw new DomainException($"Não é possível remover itens de uma Ordem de Serviço com o status '{Status.Valor}'.", HttpStatusCode.UnprocessableContent);

            var itemParaRemover = _itensIncluidos.FirstOrDefault(i => i.Id == itemIncluidoId);
            if (itemParaRemover == null)
                throw new DomainException("Item não encontrado nesta ordem de serviço.", HttpStatusCode.NotFound);

            _itensIncluidos.Remove(itemParaRemover);
        }

        public void Cancelar()
        {
            if (!PodeTransicionarPara(StatusOrdemServicoEnum.Cancelada))
                throw new DomainException($"Não é possível mudar de {Status.Valor} para {StatusOrdemServicoEnum.Cancelada}.", HttpStatusCode.UnprocessableContent);

            Status = new Status(StatusOrdemServicoEnum.Cancelada);
        }

        public void IniciarDiagnostico()
        {
            if (!PodeTransicionarPara(StatusOrdemServicoEnum.EmDiagnostico))
                throw new DomainException($"Não é possível mudar de {Status.Valor} para {StatusOrdemServicoEnum.EmDiagnostico}.", HttpStatusCode.UnprocessableContent);

            Status = new Status(StatusOrdemServicoEnum.EmDiagnostico);
        }
        
        public void GerarOrcamento()
        {
            if (!PodeTransicionarPara(StatusOrdemServicoEnum.AguardandoAprovacao))
                throw new DomainException($"Não é possível mudar de {Status.Valor} para {StatusOrdemServicoEnum.AguardandoAprovacao}.", HttpStatusCode.UnprocessableContent);

            if (!ServicosIncluidos.Any() && !ItensIncluidos.Any())
                throw new DomainException("Não é possível gerar orçamento sem pelo menos um serviço ou item incluído.", HttpStatusCode.UnprocessableContent);

            Status = new Status(StatusOrdemServicoEnum.AguardandoAprovacao);
            Orcamento = Orcamento.GerarOrcamento(ServicosIncluidos, ItensIncluidos);
        }

        public void IniciarExecucao()
        {
            if (!PodeTransicionarPara(StatusOrdemServicoEnum.EmExecucao))
                throw new DomainException($"Não é possível mudar de {Status.Valor} para {StatusOrdemServicoEnum.EmExecucao}.", HttpStatusCode.UnprocessableContent);

            Status = new Status(StatusOrdemServicoEnum.EmExecucao);
            Historico = Historico.MarcarDataInicioExecucao();
        }

        public void FinalizarExecucao()
        {
            if (!PodeTransicionarPara(StatusOrdemServicoEnum.Finalizada))
                throw new DomainException($"Não é possível mudar de {Status.Valor} para {StatusOrdemServicoEnum.Finalizada}.", HttpStatusCode.UnprocessableContent);

            Status = new Status(StatusOrdemServicoEnum.Finalizada);
            Historico = Historico.MarcarDataFinalizadaExecucao();
        }

        public void Entregar()
        {
            if (!PodeTransicionarPara(StatusOrdemServicoEnum.Entregue))
                throw new DomainException($"Não é possível mudar de {Status.Valor} para {StatusOrdemServicoEnum.Entregue}.", HttpStatusCode.UnprocessableContent);

            Status = new Status(StatusOrdemServicoEnum.Entregue);
            Historico = Historico.MarcarDataEntrega();
        }

        private bool PodeTransicionarPara(StatusOrdemServicoEnum novoStatus)
        {
            var statusAtual = Enum.Parse<StatusOrdemServicoEnum>(Status.Valor, ignoreCase: true);

            return novoStatus switch
            {
                // Sempre pode cancelar
                StatusOrdemServicoEnum.Cancelada => true,

                StatusOrdemServicoEnum.EmDiagnostico => statusAtual == StatusOrdemServicoEnum.Recebida,

                StatusOrdemServicoEnum.AguardandoAprovacao => statusAtual == StatusOrdemServicoEnum.EmDiagnostico,

                StatusOrdemServicoEnum.EmExecucao => statusAtual == StatusOrdemServicoEnum.AguardandoAprovacao,

                StatusOrdemServicoEnum.Finalizada => statusAtual == StatusOrdemServicoEnum.EmExecucao,

                StatusOrdemServicoEnum.Entregue => statusAtual == StatusOrdemServicoEnum.Finalizada,

                _ => false
            };
        }
    }
}
