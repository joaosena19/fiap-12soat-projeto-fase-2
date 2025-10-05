using Application.Contracts.Presenters;
using Shared.Enums;
using Tests.Application.Cadastros.Cliente.Helpers;
using ClienteAggregate = Domain.Cadastros.Aggregates.Cliente;

namespace Tests.Application.Cadastros.Cliente
{
    public class CriarClienteUseCaseTest
    {
        private readonly ClienteTestFixture _fixture;

        public CriarClienteUseCaseTest()
        {
            _fixture = new ClienteTestFixture();
        }

        [Fact(DisplayName = "Deve criar cliente com sucesso")]
        public async Task ExecutarAsync_DeveCriarClienteComSucesso()
        {
            // Arrange
            var cliente = new ClienteBuilder().Build();
            _fixture.ClienteGatewayMock.AoSalvar().Retorna(cliente);

            // Act
            await _fixture.CriarClienteUseCase.ExecutarAsync(cliente.Nome.Valor, cliente.DocumentoIdentificador.Valor, _fixture.ClienteGatewayMock.Object, _fixture.CriarClientePresenterMock.Object);

            // Assert
            _fixture.CriarClientePresenterMock.DeveTerApresentadoSucessoComQualquerObjeto<ICriarClientePresenter, ClienteAggregate>();
            _fixture.CriarClientePresenterMock.NaoDeveTerApresentadoErro<ICriarClientePresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando já existe cliente com documento")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteJaExiste()
        {
            // Arrange
            var cliente = new ClienteBuilder().Build();
            _fixture.ClienteGatewayMock.AoObterPorDocumento(cliente.DocumentoIdentificador.Valor).Retorna(cliente);

            // Act
            await _fixture.CriarClienteUseCase.ExecutarAsync(cliente.Nome.Valor, cliente.DocumentoIdentificador.Valor, _fixture.ClienteGatewayMock.Object, _fixture.CriarClientePresenterMock.Object);

            // Assert
            _fixture.CriarClientePresenterMock.DeveTerApresentadoErro<ICriarClientePresenter, ClienteAggregate>("Já existe um cliente cadastrado com este documento.", ErrorType.Conflict);
            _fixture.CriarClientePresenterMock.NaoDeveTerApresentadoSucesso<ICriarClientePresenter, ClienteAggregate>();
        }
    }
}
