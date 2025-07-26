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
            Status = Status.TransicionarPara(StatusOrdemServicoEnum.Cancelada);
        }

        public void IniciarDiagnostico()
        {
            Status = Status.TransicionarPara(StatusOrdemServicoEnum.EmDiagnostico);
        }
        public void GerarOrcamento()
        {
            Status = Status.TransicionarPara(StatusOrdemServicoEnum.AguardandoAprovacao);
            Orcamento = Orcamento.GerarOrcamento(ServicosIncluidos, ItensIncluidos);
        }

        public void IniciarExecucao()
        {
            Status = Status.TransicionarPara(StatusOrdemServicoEnum.EmExecucao);
            Historico = Historico.MarcarDataInicioExecucao();
        }

        public void FinalizarExecucao()
        {
            Status = Status.TransicionarPara(StatusOrdemServicoEnum.Finalizada);
            Historico = Historico.MarcarDataFinalizadaExecucao();
        }

        public void Entregar()
        {
            Status = Status.TransicionarPara(StatusOrdemServicoEnum.Entregue);
            Historico = Historico.MarcarDataEntrega();
        }
    }
}
