using Application.Cadastros.Dtos;
using Application.Contracts.Presenters;
using Domain.Cadastros.Aggregates;

namespace API.Presenters.Cadastro
{
    public class CriarClientePresenter : BasePresenter, ICriarClientePresenter
    {
        public void ApresentarSucesso(Cliente cliente)
        {
            var dto = new RetornoClienteDto
            {
                Id = cliente.Id,
                Nome = cliente.Nome.Valor,
                DocumentoIdentificador = cliente.DocumentoIdentificador.Valor,
                TipoDocumentoIdentificador = cliente.DocumentoIdentificador.TipoDocumento.ToString()
            };
            
            DefinirSucessoComLocalizacao("GetById", "Cliente", new { id = cliente.Id }, dto);
        }
    }
}