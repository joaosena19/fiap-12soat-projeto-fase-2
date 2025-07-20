using Microsoft.AspNetCore.Mvc;
using API.DTO;
using Application.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OficinaMecanica.Controllers
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
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CriarClienteDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _clienteService.CriarCliente(dto.Nome, dto.Cpf);
                return Created();
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar cliente: {ex.Message}");
            }
        }
    }
}