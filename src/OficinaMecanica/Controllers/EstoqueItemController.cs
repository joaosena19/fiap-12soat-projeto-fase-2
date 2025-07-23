using Application.Estoque.DTO;
using Application.Estoque.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de itens de estoque
    /// </summary>
    [Route("api/estoque/itens")]
    [ApiController]
    [Produces("application/json")]
    public class EstoqueItemController : ControllerBase
    {
        private readonly IItemEstoqueService _itemEstoqueService;

        public EstoqueItemController(IItemEstoqueService itemEstoqueService)
        {
            _itemEstoqueService = itemEstoqueService;
        }

        /// <summary>
        /// Buscar todos os itens de estoque
        /// </summary>
        /// <returns>Lista de itens de estoque</returns>
        /// <response code="200">Lista de itens de estoque retornada com sucesso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RetornoItemEstoqueDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var result = await _itemEstoqueService.Buscar();
            return Ok(result);
        }

        /// <summary>
        /// Buscar item de estoque por ID
        /// </summary>
        /// <param name="id">ID do item de estoque</param>
        /// <returns>Item de estoque encontrado</returns>
        /// <response code="200">Item de estoque encontrado com sucesso</response>
        /// <response code="404">Item de estoque não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RetornoItemEstoqueDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _itemEstoqueService.BuscarPorId(id);
            return Ok(result);
        }

        /// <summary>
        /// Criar um novo item de estoque
        /// </summary>
        /// <param name="dto">Dados do item de estoque a ser criado</param>
        /// <returns>Item de estoque criado com sucesso</returns>
        /// <response code="201">Item de estoque criado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(RetornoItemEstoqueDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CriarItemEstoqueDTO dto)
        {
            var result = await _itemEstoqueService.CriarItemEstoque(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Atualizar um item de estoque existente
        /// </summary>
        /// <param name="id">ID do item de estoque a ser atualizado</param>
        /// <param name="dto">Dados do item de estoque a ser atualizado</param>
        /// <returns>Item de estoque atualizado com sucesso</returns>
        /// <response code="200">Item de estoque atualizado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Item de estoque não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RetornoItemEstoqueDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(Guid id, [FromBody] AtualizarItemEstoqueDTO dto)
        {
            var result = await _itemEstoqueService.AtualizarItemEstoque(id, dto);
            return Ok(result);
        }

        /// <summary>
        /// Atualizar apenas a quantidade de um item de estoque
        /// </summary>
        /// <param name="id">ID do item de estoque</param>
        /// <param name="dto">Nova quantidade do item de estoque</param>
        /// <returns>Item de estoque com quantidade atualizada</returns>
        /// <response code="200">Quantidade atualizada com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Item de estoque não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPatch("{id}/quantidade")]
        [ProducesResponseType(typeof(RetornoItemEstoqueDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateQuantidade(Guid id, [FromBody] AtualizarQuantidadeDTO dto)
        {
            var result = await _itemEstoqueService.AtualizarQuantidade(id, dto);
            return Ok(result);
        }

        /// <summary>
        /// Verificar disponibilidade de um item de estoque
        /// </summary>
        /// <param name="id">ID do item de estoque</param>
        /// <param name="quantidadeRequisitada">Quantidade necessária para verificação</param>
        /// <returns>Informações sobre a disponibilidade do item</returns>
        /// <response code="200">Verificação realizada com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Item de estoque não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id}/disponibilidade")]
        [ProducesResponseType(typeof(RetornoDisponibilidadeDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VerificarDisponibilidade(Guid id, int quantidadeRequisitada)
        {
            var result = await _itemEstoqueService.VerificarDisponibilidade(id, quantidadeRequisitada);
            return Ok(result);
        }
    }
}
