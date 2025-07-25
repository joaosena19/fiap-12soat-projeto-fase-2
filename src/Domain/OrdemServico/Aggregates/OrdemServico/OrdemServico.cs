using Domain.OrdemServico.Enums;
using Domain.OrdemServico.ValueObjects.OrdemServico;
using Shared.Exceptions;
using UUIDNext;

namespace Domain.OrdemServico.Aggregates.OrdemServico
{
    public class OrdemServico
    {
        public Guid Id { get; private init; }
        public Codigo Codigo { get; private set; } = null!;
        public Status Status { get; private set; } = null!;
        public HistoricoTemporal Historico { get; private set; }


        private readonly List<ServicoIncluido> _servicosIncluidos = new();
        private readonly List<ItemIncluido> _itensIncluidos = new();
        public IReadOnlyCollection<ServicoIncluido> ServicosIncluidos => _servicosIncluidos.AsReadOnly();
        public IReadOnlyCollection<ItemIncluido> ItensIncluidos => _itensIncluidos.AsReadOnly();

        public Orcamento? Orcamento { get; private set; } = null;

        // Construtor sem parâmetro para EF Core
        private OrdemServico() { }

        private OrdemServico(Guid id, Codigo codigo, Status status, HistoricoTemporal historico)
        {
            Id = id;
            Codigo = codigo;
            Status = status;
            Historico = historico;
        }

        public static OrdemServico Criar()
        {
            return new OrdemServico(
                Uuid.NewSequential(),
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
                throw new DomainException($"Não é possível adicionar serviços a uma Ordem de Serviço com o status '{Status.Valor}'.");

            if (_servicosIncluidos.Any(s => s.ServicoOriginalId == servicoOriginalId))
                throw new DomainException("Este serviço já foi incluído nesta OS.");

            var novoServico = ServicoIncluido.Criar(servicoOriginalId, nome, preco);
            _servicosIncluidos.Add(novoServico);
        }

        public void AdicionarItem(Guid itemEstoqueOriginalId, string nome, decimal precoUnitario, int quantidade, TipoItemIncluidoEnum tipo)
        {
            if (!PermiteAlterarServicosItens())
                throw new DomainException($"Não é possível adicionar itens a uma Ordem de Serviço com o status '{Status.Valor}'.");

            if (quantidade <= 0)
                throw new DomainException("A quantidade deve ser maior que zero.");

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
