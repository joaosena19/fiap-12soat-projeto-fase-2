using Application.Interfaces;
using Application.Cadastros.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
            return StatusCode((int)HttpStatusCode.Created, result);
        }
    }
}