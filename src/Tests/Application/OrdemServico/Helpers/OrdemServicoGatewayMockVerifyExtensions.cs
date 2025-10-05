
using Moq;
using Application.OrdemServico.Interfaces.External;
using Application.Contracts.Gateways;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Tests.Application.OrdemServico.Helpers
{
    public static class OrdemServicoGatewayMockVerifyExtensions
    {
        public static void DeveTerVerificadoExistenciaVeiculo(this Mock<IVeiculoExternalService> mock, Guid veiculoId)
        {
            mock.Verify(v => v.VerificarExistenciaVeiculo(veiculoId), Times.Once);
        }

        public static void DeveTerVerificadoCodigoExistente(this Mock<IOrdemServicoGateway> mock)
        {
            mock.Verify(g => g.ObterPorCodigoAsync(It.IsAny<string>()), Times.AtLeastOnce);
        }

        public static void DeveTerSalvoOrdemServico(this Mock<IOrdemServicoGateway> mock)
        {
            mock.Verify(g => g.SalvarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Once);
        }
    }
}
