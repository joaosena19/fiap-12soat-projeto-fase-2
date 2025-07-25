using Application.Cadastros.DTO;
using Application.Cadastros.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Cadastro
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
        [ProducesResponseType(typeof(IEnumerable<RetornoServicoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(typeof(RetornoServicoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(typeof(RetornoServicoDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CriarServicoDTO dto)
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
        [ProducesResponseType(typeof(RetornoServicoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(Guid id, [FromBody] AtualizarServicoDTO dto)
        {
            var result = await _servicoService.AtualizarServico(id, dto.Nome, dto.Preco);
            return Ok(result);
        }
    }
}
