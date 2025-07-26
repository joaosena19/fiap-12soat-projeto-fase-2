using Domain.OrdemServico.Enums;
using Shared.Exceptions;
using System.Net;

namespace Domain.OrdemServico.ValueObjects.OrdemServico
{
    public record Status
    {
        private readonly string _valor = string.Empty;

        // Construtor sem parâmetro para o EF Core
        private Status() { }

        public Status(StatusOrdemServicoEnum statusOrdemServicoEnum)
        {
            if (!Enum.IsDefined(typeof(StatusOrdemServicoEnum), statusOrdemServicoEnum))
            {
                var valores = string.Join(", ", Enum.GetNames(typeof(StatusOrdemServicoEnum)));
                throw new DomainException($"Status da Ordem de Serviço '{statusOrdemServicoEnum}' não é válido. Valores aceitos: {valores}.", HttpStatusCode.UnprocessableContent);
            }

            _valor = statusOrdemServicoEnum.ToString().ToLower();
        }

        public string Valor => _valor;

        public Status TransicionarPara(StatusOrdemServicoEnum novoStatus)
        {
            if (!PodeTransicionarPara(novoStatus))
            {
                throw new DomainException($"Não é possível mudar de {Valor} para {novoStatus}.", HttpStatusCode.UnprocessableContent);
            }

            return new Status(novoStatus);
        }

        private bool PodeTransicionarPara(StatusOrdemServicoEnum novoStatus)
        {
            var statusAtual = Enum.Parse<StatusOrdemServicoEnum>(_valor, ignoreCase: true);

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
