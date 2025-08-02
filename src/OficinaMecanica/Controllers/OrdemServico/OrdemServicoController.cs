using API.Dtos;
using Application.OrdemServico.Dtos;
using Application.OrdemServico.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        [ProducesResponseType(typeof(IEnumerable<RetornoOrdemServicoCompletaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(typeof(RetornoOrdemServicoCompletaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(typeof(RetornoOrdemServicoCompletaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
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
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(RetornoOrdemServicoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CriarOrdemServicoDto dto)
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
        [ProducesResponseType(typeof(RetornoOrdemServicoComServicosItensDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AdicionarServicos(Guid id, [FromBody] AdicionarServicosDto dto)
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
        [ProducesResponseType(typeof(RetornoOrdemServicoComServicosItensDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AdicionarItem(Guid id, [FromBody] AdicionarItemDto dto)
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
        /// <response code="422">Erros de regra do domínio</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("{id}/servicos/{servicoIncluidoId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
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
        /// <response code="422">Erro de regra do domínio</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("{id}/itens/{itemIncluidoId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
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
        /// <response code="422">Erro de regra do domínio</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("{id}/iniciar-diagnostico")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
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
        /// <response code="409">Orçamento já foi gerado</response>
        /// <response code="422">Erro de regra do domínio</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("{id}/orcamento")]
        [ProducesResponseType(typeof(RetornoOrcamentoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GerarOrcamento(Guid id)
        {
            var result = await _ordemServicoService.GerarOrcamento(id);
            return Created($"/api/ordens-servico/{id}", result);
        }

        /// <summary>
        /// Aprovar orçamento da ordem de serviço, iniciando sua execução
        /// </summary>
        /// <param name="id">ID da ordem de serviço</param>
        /// <returns>Nenhum conteúdo</returns>
        /// <response code="204">Orçamento aprovado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Ordem de serviço não encontrada</response>
        /// <response code="422">Erro de regra do domínio</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("{id}/orcamento/aprovar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AprovarOrcamento(Guid id)
        {
            await _ordemServicoService.AprovarOrcamento(id);
            return NoContent();
        }

        /// <summary>
        /// Desaprovar orçamento ordem de serviço, causando seu cancelamento
        /// </summary>
        /// <param name="id">ID da ordem de serviço</param>
        /// <returns>Nenhum conteúdo</returns>
        /// <response code="204">Orçamento desaprovado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Ordem de serviço não encontrada</response>
        /// <response code="422">Erro de regra do domínio</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("{id}/orcamento/desaprovar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DesaprovarOrcamento(Guid id)
        {
            await _ordemServicoService.DesaprovarOrcamento(id);
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
        /// <response code="422">Erro de regra do domínio</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("{id}/finalizar-execucao")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
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
        /// <response code="422">Erro de regra do domínio</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("{id}/entregar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Entregar(Guid id)
        {
            await _ordemServicoService.Entregar(id);
            return NoContent();
        }

        /// <summary>
        /// Obter tempo médio de execução das ordens de serviço. Considera apenas ordes de serviço entregues, com criação de acordo com a quantidade de dias específicada.
        /// </summary>
        /// <param name="quantidadeDias">Quantidade de dias para análise (1-365). Padrão: 365</param>
        /// <returns>Dados sobre o tempo médio de execução</returns>
        /// <response code="200">Tempo médio calculado com sucesso</response>
        /// <response code="400">Parâmetros inválidos ou nenhuma ordem encontrada</response>
        /// <response code="422">Erro de regra do domínio</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("tempo-medio")]
        [ProducesResponseType(typeof(RetornoTempoMedioDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterTempoMedio([FromQuery] int quantidadeDias = 365)
        {
            var result = await _ordemServicoService.ObterTempoMedio(quantidadeDias);
            return Ok(result);
        }

        /// <summary>
        /// Busca pública de ordem de serviço por código e documento do cliente
        /// </summary>
        /// <param name="dto">Dados para busca: código da ordem de serviço e documento do cliente</param>
        /// <returns>Ordem de serviço encontrada</returns>
        /// <response code="200">Busca realizada com sucesso</response>
        [AllowAnonymous]
        [HttpPost("busca-publica")]
        [ProducesResponseType(typeof(RetornoOrdemServicoCompletaDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> BuscaPublica([FromBody] BuscaPublicaOrdemServicoDto dto)
        {
            var result = await _ordemServicoService.BuscaPublica(dto);

            if(result is null) //Evitar retornar 204 se vier null, sempre deve retornar 200
                return new JsonResult(null) { StatusCode = 200 };

            return Ok(result);
        }
    }
}
