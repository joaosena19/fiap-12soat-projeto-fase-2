using API.Dtos;
using Application.Cadastros.Dtos;
using Application.Cadastros.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Cadastro
{
    /// <summary>
    /// Controller para gerenciamento de cadastro de veículos
    /// </summary>
    [Route("api/cadastros/veiculos")]
    [ApiController]
    [Produces("application/json")]
    public class VeiculoController : ControllerBase
    {
        private readonly IVeiculoService _veiculoService;

        public VeiculoController(IVeiculoService veiculoService)
        {
            _veiculoService = veiculoService;
        }

        /// <summary>
        /// Buscar todos os veículos
        /// </summary>
        /// <returns>Lista de veículos</returns>
        /// <response code="200">Lista de veículos retornada com sucesso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RetornoVeiculoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var result = await _veiculoService.Buscar();
            return Ok(result);
        }

        /// <summary>
        /// Buscar veículo por ID
        /// </summary>
        /// <param name="id">ID do veículo</param>
        /// <returns>Veículo encontrado</returns>
        /// <response code="200">Veículo encontrado com sucesso</response>
        /// <response code="404">Veículo não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RetornoVeiculoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _veiculoService.BuscarPorId(id);
            return Ok(result);
        }

        /// <summary>
        /// Buscar veículo por placa
        /// </summary>
        /// <param name="placa">Placa do veículo</param>
        /// <returns>Veículo encontrado</returns>
        /// <response code="200">Veículo encontrado com sucesso</response>
        /// <response code="404">Veículo não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("placa/{placa}")]
        [ProducesResponseType(typeof(RetornoVeiculoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByPlaca(string placa)
        {
            var result = await _veiculoService.BuscarPorPlaca(placa);
            return Ok(result);
        }

        /// <summary>
        /// Buscar veículos por ID do cliente
        /// </summary>
        /// <param name="clienteId">ID do cliente</param>
        /// <returns>Lista de veículos do cliente</returns>
        /// <response code="200">Veículos encontrados com sucesso</response>
        /// <response code="422">Cliente não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("cliente/{clienteId}")]
        [ProducesResponseType(typeof(IEnumerable<RetornoVeiculoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByClienteId(Guid clienteId)
        {
            var result = await _veiculoService.BuscarPorClienteId(clienteId);
            return Ok(result);
        }

        /// <summary>
        /// Criar um novo veículo
        /// </summary>
        /// <param name="dto">Dados do veículo a ser criado</param>
        /// <returns>Veículo criado com sucesso</returns>
        /// <response code="201">Veículo criado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="409">Placa já cadastrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(RetornoVeiculoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CriarVeiculoDto dto)
        {
            var result = await _veiculoService.CriarVeiculo(dto.ClienteId, dto.Placa, dto.Modelo, dto.Marca, dto.Cor, dto.Ano, dto.TipoVeiculo);
            return Created($"/api/cadastros/veiculos/{result.Id}", result);
        }

        /// <summary>
        /// Atualizar um veículo existente
        /// </summary>
        /// <param name="id">ID do veículo</param>
        /// <param name="dto">Dados do veículo para atualização</param>
        /// <returns>Veículo atualizado com sucesso</returns>
        /// <response code="200">Veículo atualizado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Veículo não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RetornoVeiculoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(Guid id, [FromBody] AtualizarVeiculoDto dto)
        {
            var result = await _veiculoService.AtualizarVeiculo(id, dto.Modelo, dto.Marca, dto.Cor, dto.Ano, dto.TipoVeiculo);
            return Ok(result);
        }
    }
}
