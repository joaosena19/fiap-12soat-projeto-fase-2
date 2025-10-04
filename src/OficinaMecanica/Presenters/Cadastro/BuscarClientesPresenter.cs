using Application.Cadastros.Dtos;
using Application.Contracts.Presenters;
using Domain.Cadastros.Aggregates;

namespace API.Presenters.Cadastro
{
    public class BuscarClientesPresenter : BasePresenter, IBuscarClientesPresenter
    {
        public void ApresentarSucesso(IEnumerable<Cliente> clientes)
        {
            var dto = clientes.Select(cliente => new RetornoClienteDto
            {
                Id = cliente.Id,
                Nome = cliente.Nome.Valor,
                DocumentoIdentificador = cliente.DocumentoIdentificador.Valor,
                TipoDocumentoIdentificador = cliente.DocumentoIdentificador.TipoDocumento.ToString()
            });
            
            DefinirSucesso(dto);
        }
    }
}