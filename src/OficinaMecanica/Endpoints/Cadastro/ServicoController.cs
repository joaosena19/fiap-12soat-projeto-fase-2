using API.Dtos;
using Application.Cadastros.Dtos;
using Application.Cadastros.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Cadastro
{
    /// <summary>
    /// Controller para gerenciamento de cadastro de serviços
    /// </summary>
    [Route("api/cadastros/servicos")]
    [ApiController]
    [Produces("application/json")]
    public class ServicoController : ControllerBase
    {
        private readonly IServicoService _servicoService;

        public ServicoController(IServicoService servicoService)
        {
            _servicoService = servicoService;
        }

        /// <summary>
        /// Buscar todos os serviços
        /// </summary>
        /// <returns>Lista de serviços</returns>
        /// <response code="200">Lista de serviços retornada com sucesso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RetornoServicoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var result = await _servicoService.Buscar();
            return Ok(result);
        }

        /// <summary>
        /// Buscar serviço por ID
        /// </summary>
        /// <param name="id">ID do serviço</param>
        /// <returns>Serviço encontrado</returns>
        /// <response code="200">Serviço encontrado com sucesso</response>
        /// <response code="404">Serviço não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RetornoServicoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _servicoService.BuscarPorId(id);
            return Ok(result);
        }

        /// <summary>
        /// Criar um novo serviço
        /// </summary>
        /// <param name="dto">Dados do serviço a ser criado</param>
        /// <returns>Serviço criado com sucesso</returns>
        /// <response code="201">Serviço criado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="409">Conflito - Serviço já existe</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(RetornoServicoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CriarServicoDto dto)
        {
            var result = await _servicoService.CriarServico(dto.Nome, dto.Preco);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Atualizar um serviço existente
        /// </summary>
        /// <param name="id">ID do serviço a ser atualizado</param>
        /// <param name="dto">Dados do serviço a ser atualizado</param>
        /// <returns>Serviço atualizado com sucesso</returns>
        /// <response code="200">Serviço atualizado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Serviço não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RetornoServicoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(Guid id, [FromBody] AtualizarServicoDto dto)
        {
            var result = await _servicoService.AtualizarServico(id, dto.Nome, dto.Preco);
            return Ok(result);
        }
    }
}
