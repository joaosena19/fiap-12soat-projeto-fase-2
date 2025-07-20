using Application.Cadastros.DTO;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de cadastro de clientes
    /// </summary>
    [Route("api/cadastros/clientes")]
    [ApiController]
    [Produces("application/json")]
    public class CadastroClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public CadastroClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        /// <summary>
        /// Criar um novo cliente
        /// </summary>
        /// <param name="dto">Dados do cliente a ser criado</param>
        /// <returns>Cliente criado com sucesso</returns>
        /// <response code="201">Cliente criado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="409">Conflito - Cliente já existe</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(RetornoClienteDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CriarClienteDTO dto)
        {
            var result = await _clienteService.CriarCliente(dto.Nome, dto.Cpf);
            return Created(); //todo: alterar para CreatedAtAction quando o endpoint de busca for implementado
        }

        /// <summary>
        /// Atualizar um cliente existente
        /// </summary>
        /// <param name="id">ID do cliente a ser atualizado</param>
        /// <param name="dto">Dados do cliente a ser atualizado</param>
        /// <returns>Cliente atualizado com sucesso</returns>
        /// <response code="200">Cliente atualizado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Cliente não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RetornoClienteDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(Guid id, [FromBody] AtualizarClienteDTO dto)
        {
            var result = await _clienteService.AtualizarCliente(id, dto.Nome);
            return Ok(result);
        }
    }
}