using API.DTO;
using Application.OrdemServico.DTO;
using Application.OrdemServico.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.OrdemServico
{
    /// <summary>
    /// Controller para gerenciamento de ordens de serviço
    /// </summary>
    [Route("api/ordens-servico")]
    [ApiController]
    [Produces("application/json")]
    public class OrdemServicoController : ControllerBase
    {
        private readonly IOrdemServicoService _ordemServicoService;

        public OrdemServicoController(IOrdemServicoService ordemServicoService)
        {
            _ordemServicoService = ordemServicoService;
        }

        /// <summary>
        /// Buscar todas as ordens de serviço
        /// </summary>
        /// <returns>Lista de ordens de serviço</returns>
        /// <response code="200">Lista de ordens de serviço retornada com sucesso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RetornoOrdemServicoCompletaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var result = await _ordemServicoService.Buscar();
            return Ok(result);
        }

        /// <summary>
        /// Buscar ordem de serviço por ID
        /// </summary>
        /// <param name="id">ID da ordem de serviço</param>
        /// <returns>Ordem de serviço encontrada</returns>
        /// <response code="200">Ordem de serviço encontrada com sucesso</response>
        /// <response code="404">Ordem de serviço não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RetornoOrdemServicoCompletaDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _ordemServicoService.BuscarPorId(id);
            return Ok(result);
        }

        /// <summary>
        /// Buscar ordem de serviço por código
        /// </summary>
        /// <param name="codigo">Código da ordem de serviço</param>
        /// <returns>Ordem de serviço encontrada</returns>
        /// <response code="200">Ordem de serviço encontrada com sucesso</response>
        /// <response code="404">Ordem de serviço não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("codigo/{codigo}")]
        [ProducesResponseType(typeof(RetornoOrdemServicoCompletaDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByCodigo(string codigo)
        {
            var result = await _ordemServicoService.BuscarPorCodigo(codigo);
            return Ok(result);
        }

        /// <summary>
        /// Criar uma nova ordem de serviço
        /// </summary>
        /// <param name="dto">Dados da ordem de serviço a ser criada</param>
        /// <returns>Ordem de serviço criada com sucesso</returns>
        /// <response code="201">Ordem de serviço criada com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="422">Veículo não encontrado</response>
        /// <response code="409">Conflito - Ordem de serviço já existe</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(RetornoOrdemServicoDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CriarOrdemServicoDTO dto)
        {
            var result = await _ordemServicoService.CriarOrdemServico(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Adicionar serviços à ordem de serviço
        /// </summary>
        /// <param name="id">ID da ordem de serviço</param>
        /// <param name="dto">Lista de serviços a serem adicionados</param>
        /// <returns>Ordem de serviço atualizada</returns>
        /// <response code="200">Serviços adicionados com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Ordem de serviço não encontrada</response>
        /// <response code="422">Serviço não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("{id}/servicos")]
        [ProducesResponseType(typeof(RetornoOrdemServicoComServicosItensDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AdicionarServicos(Guid id, [FromBody] AdicionarServicosDTO dto)
        {
            var result = await _ordemServicoService.AdicionarServicos(id, dto);
            return Ok(result);
        }

        /// <summary>
        /// Adicionar item à ordem de serviço
        /// </summary>
        /// <param name="id">ID da ordem de serviço</param>
        /// <param name="dto">Dados do item a ser adicionado</param>
        /// <returns>Ordem de serviço atualizada</returns>
        /// <response code="200">Item adicionado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Ordem de serviço não encontrada</response>
        /// <response code="422">Item de estoque não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("{id}/itens")]
        [ProducesResponseType(typeof(RetornoOrdemServicoComServicosItensDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AdicionarItem(Guid id, [FromBody] AdicionarItemDTO dto)
        {
            var result = await _ordemServicoService.AdicionarItem(id, dto);
            return Ok(result);
        }

        /// <summary>
        /// Remover serviço da ordem de serviço
        /// </summary>
        /// <param name="id">ID da ordem de serviço</param>
        /// <param name="servicoIncluidoId">ID do serviço incluído a ser removido</param>
        /// <returns>Nenhum conteúdo</returns>
        /// <response code="204">Serviço removido com sucesso</response>
        /// <response code="404">Ordem de serviço ou serviço não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("{id}/servicos/{servicoIncluidoId}")]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoverServico(Guid id, Guid servicoIncluidoId)
        {
            await _ordemServicoService.RemoverServico(id, servicoIncluidoId);
            return NoContent();
        }

        /// <summary>
        /// Remover item da ordem de serviço
        /// </summary>
        /// <param name="id">ID da ordem de serviço</param>
        /// <param name="itemIncluidoId">ID do item incluído a ser removido</param>
        /// <returns>Nenhum conteúdo</returns>
        /// <response code="204">Item removido com sucesso</response>
        /// <response code="404">Ordem de serviço ou item não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("{id}/itens/{itemIncluidoId}")]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoverItem(Guid id, Guid itemIncluidoId)
        {
            await _ordemServicoService.RemoverItem(id, itemIncluidoId);
            return NoContent();
        }

        /// <summary>
        /// Cancelar ordem de serviço
        /// </summary>
        /// <param name="id">ID da ordem de serviço</param>
        /// <returns>Nenhum conteúdo</returns>
        /// <response code="204">Ordem de serviço cancelada com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Ordem de serviço não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("{id}/cancelar")]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Cancelar(Guid id)
        {
            await _ordemServicoService.Cancelar(id);
            return NoContent();
        }

        /// <summary>
        /// Iniciar diagnóstico da ordem de serviço
        /// </summary>
        /// <param name="id">ID da ordem de serviço</param>
        /// <returns>Nenhum conteúdo</returns>
        /// <response code="204">Diagnóstico iniciado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Ordem de serviço não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("{id}/iniciar-diagnostico")]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> IniciarDiagnostico(Guid id)
        {
            await _ordemServicoService.IniciarDiagnostico(id);
            return NoContent();
        }

        /// <summary>
        /// Gerar orçamento da ordem de serviço
        /// </summary>
        /// <param name="id">ID da ordem de serviço</param>
        /// <returns>Orçamento gerado</returns>
        /// <response code="201">Orçamento gerado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Ordem de serviço não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("{id}/gerar-orcamento")]
        [ProducesResponseType(typeof(RetornoOrcamentoDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GerarOrcamento(Guid id)
        {
            var result = await _ordemServicoService.GerarOrcamento(id);
            return Created($"/api/ordens-servico/{id}", result);
        }

        /// <summary>
        /// Iniciar execução da ordem de serviço
        /// </summary>
        /// <param name="id">ID da ordem de serviço</param>
        /// <returns>Nenhum conteúdo</returns>
        /// <response code="204">Execução iniciada com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Ordem de serviço não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("{id}/iniciar-execucao")]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> IniciarExecucao(Guid id)
        {
            await _ordemServicoService.IniciarExecucao(id);
            return NoContent();
        }

        /// <summary>
        /// Finalizar execução da ordem de serviço
        /// </summary>
        /// <param name="id">ID da ordem de serviço</param>
        /// <returns>Nenhum conteúdo</returns>
        /// <response code="204">Execução finalizada com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Ordem de serviço não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("{id}/finalizar-execucao")]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FinalizarExecucao(Guid id)
        {
            await _ordemServicoService.FinalizarExecucao(id);
            return NoContent();
        }

        /// <summary>
        /// Entregar ordem de serviço
        /// </summary>
        /// <param name="id">ID da ordem de serviço</param>
        /// <returns>Nenhum conteúdo</returns>
        /// <response code="204">Ordem de serviço entregue com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Ordem de serviço não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("{id}/entregar")]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Entregar(Guid id)
        {
            await _ordemServicoService.Entregar(id);
            return NoContent();
        }
    }
}
