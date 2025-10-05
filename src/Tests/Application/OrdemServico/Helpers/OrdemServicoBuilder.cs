using Bogus;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Tests.Application.OrdemServico.Helpers
{
    public class OrdemServicoBuilder
    {
        private Guid _veiculoId;
        private readonly Faker _faker = new Faker("pt_BR");

        public OrdemServicoBuilder()
        {
            _veiculoId = Guid.NewGuid();
        }

        public OrdemServicoBuilder ComVeiculoId(Guid veiculoId)
        {
            _veiculoId = veiculoId;
            return this;
        }

        public OrdemServicoAggregate Build()
        {
            return OrdemServicoAggregate.Criar(_veiculoId);
        }
    }
}