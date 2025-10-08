using API.Dtos;
using API.Presenters.OrdemServico;
using Application.OrdemServico.Dtos;
using Infrastructure.Database;
using Infrastructure.Handlers.OrdemServico;
using Infrastructure.Repositories.OrdemServico;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.OrdemServico
{
    /// <summary>
    /// Controller para gerenciamento de ordens de serviço
    /// </summary>
    [Route("api/ordens-servico")]
    [ApiController]
    [Produces("application/json")]
    public class OrdemServicoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdemServicoController(AppDbContext context)
        {
            _context = context;
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
            var gateway = new OrdemServicoRepository(_context);
            var presenter = new BuscarOrdensServicoPresenter();
            var handler = new OrdemServicoHandler();
            
            await handler.BuscarOrdensServicoAsync(gateway, presenter);
            return presenter.ObterResultado();
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
            var gateway = new OrdemServicoRepository(_context);
            var presenter = new BuscarOrdemServicoPorIdPresenter();
            var handler = new OrdemServicoHandler();
            
            await handler.BuscarOrdemServicoPorIdAsync(id, gateway, presenter);
            return presenter.ObterResultado();
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
            var gateway = new OrdemServicoRepository(_context);
            var presenter = new BuscarOrdemServicoPorCodigoPresenter();
            var handler = new OrdemServicoHandler();
            
            await handler.BuscarOrdemServicoPorCodigoAsync(codigo, gateway, presenter);
            return presenter.ObterResultado();
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
            var gateway = new OrdemServicoRepository(_context);
            var presenter = new CriarOrdemServicoPresenter();
            var handler = new OrdemServicoHandler();
            var veiculoExternalService = new Infrastructure.AntiCorruptionLayer.OrdemServico.VeiculoExternalService(new Infrastructure.Repositories.Cadastros.VeiculoRepository(_context));
            
            await handler.CriarOrdemServicoAsync(dto.VeiculoId, gateway, veiculoExternalService, presenter);
            return presenter.ObterResultado();
        }

        /// <summary>
        /// Criar uma nova ordem de serviço completa com cliente, veículo, serviços e itens
        /// </summary>
        /// <param name="dto">Dados completos para criação da ordem de serviço</param>
        /// <returns>Ordem de serviço criada com sucesso</returns>
        /// <response code="201">Ordem de serviço criada com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="422">Erro de validação ou regra de negócio</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("completa")]
        [ProducesResponseType(typeof(RetornoOrdemServicoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CriarCompleta([FromBody] CriarOrdemServicoCompletaDto dto)
        {
            var ordemServicoGateway = new OrdemServicoRepository(_context);
            var clienteGateway = new Infrastructure.Repositories.Cadastros.ClienteRepository(_context);
            var veiculoGateway = new Infrastructure.Repositories.Cadastros.VeiculoRepository(_context);
            var servicoGateway = new Infrastructure.Repositories.Cadastros.ServicoRepository(_context);
            var itemEstoqueGateway = new Infrastructure.Repositories.Estoque.ItemEstoqueRepository(_context);
            
            var presenter = new CriarOrdemServicoCompletaPresenter();
            var useCase = new Application.OrdemServico.UseCases.CriarOrdemServicoCompletaUseCase();
            
            await useCase.ExecutarAsync(dto, ordemServicoGateway, clienteGateway, veiculoGateway, servicoGateway, itemEstoqueGateway, presenter);
            return presenter.ObterResultado();
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
            var gateway = new OrdemServicoRepository(_context);
            var presenter = new AdicionarServicosPresenter();
            var handler = new OrdemServicoHandler();

            var servicoExternalService = new Infrastructure.AntiCorruptionLayer.OrdemServico.ServicoExternalService(new Infrastructure.Repositories.Cadastros.ServicoRepository(_context));
            await handler.AdicionarServicosAsync(id, dto.ServicosOriginaisIds, gateway, servicoExternalService, presenter);
            return presenter.ObterResultado();
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
            var gateway = new OrdemServicoRepository(_context);
            var presenter = new AdicionarItemPresenter();
            var handler = new OrdemServicoHandler();

            var estoqueExternalService = new Infrastructure.AntiCorruptionLayer.OrdemServico.EstoqueExternalService(new Infrastructure.Repositories.Estoque.ItemEstoqueRepository(_context));
            await handler.AdicionarItemAsync(id, dto.ItemEstoqueOriginalId, dto.Quantidade, gateway, estoqueExternalService, presenter);
            return presenter.ObterResultado();
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
            var gateway = new OrdemServicoRepository(_context);
            var presenter = new OperacaoOrdemServicoPresenter();
            var handler = new OrdemServicoHandler();
            
            await handler.RemoverServicoAsync(id, servicoIncluidoId, gateway, presenter);
            return presenter.ObterResultado();
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
            var gateway = new OrdemServicoRepository(_context);
            var presenter = new OperacaoOrdemServicoPresenter();
            var handler = new OrdemServicoHandler();
            
            await handler.RemoverItemAsync(id, itemIncluidoId, gateway, presenter);
            return presenter.ObterResultado();
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
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Cancelar(Guid id)
        {
            var gateway = new OrdemServicoRepository(_context);
            var presenter = new OperacaoOrdemServicoPresenter();
            var handler = new OrdemServicoHandler();
            
            await handler.CancelarAsync(id, gateway, presenter);
            return presenter.ObterResultado();
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
            var gateway = new OrdemServicoRepository(_context);
            var presenter = new OperacaoOrdemServicoPresenter();
            var handler = new OrdemServicoHandler();

            await handler.IniciarDiagnosticoAsync(id, gateway, presenter);
            return presenter.ObterResultado();
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
            var gateway = new OrdemServicoRepository(_context);
            var presenter = new GerarOrcamentoPresenter();
            var handler = new OrdemServicoHandler();

            await handler.GerarOrcamentoAsync(id, gateway, presenter);
            return presenter.ObterResultado();
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
            var gateway = new OrdemServicoRepository(_context);
            var presenter = new OperacaoOrdemServicoPresenter();
            var handler = new OrdemServicoHandler();

            var estoqueExternalService = new Infrastructure.AntiCorruptionLayer.OrdemServico.EstoqueExternalService(new Infrastructure.Repositories.Estoque.ItemEstoqueRepository(_context));
            await handler.AprovarOrcamentoAsync(id, gateway, estoqueExternalService, presenter);
            return presenter.ObterResultado();
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
            var gateway = new OrdemServicoRepository(_context);
            var presenter = new OperacaoOrdemServicoPresenter();
            var handler = new OrdemServicoHandler();

            await handler.DesaprovarOrcamentoAsync(id, gateway, presenter);
            return presenter.ObterResultado();
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
            var gateway = new OrdemServicoRepository(_context);
            var presenter = new OperacaoOrdemServicoPresenter();
            var handler = new OrdemServicoHandler();

            await handler.FinalizarExecucaoAsync(id, gateway, presenter);
            return presenter.ObterResultado();
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
            var gateway = new OrdemServicoRepository(_context);
            var presenter = new OperacaoOrdemServicoPresenter();
            var handler = new OrdemServicoHandler();

            await handler.EntregarAsync(id, gateway, presenter);
            return presenter.ObterResultado();
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
            var gateway = new OrdemServicoRepository(_context);
            var presenter = new ObterTempoMedioPresenter();
            var handler = new OrdemServicoHandler();

            await handler.ObterTempoMedioAsync(quantidadeDias, gateway, presenter);
            return presenter.ObterResultado();
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
            var gateway = new OrdemServicoRepository(_context);
            var presenter = new BuscaPublicaOrdemServicoPresenter();
            var handler = new OrdemServicoHandler();

            var clienteExternalService = new Infrastructure.AntiCorruptionLayer.OrdemServico.ClienteExternalService(new Infrastructure.Repositories.Cadastros.VeiculoRepository(_context), new Infrastructure.Repositories.Cadastros.ClienteRepository(_context));
            await handler.BuscaPublicaAsync(dto.CodigoOrdemServico, dto.DocumentoIdentificadorCliente, gateway, clienteExternalService, presenter);
            return presenter.ObterResultado();
        }
    }
}
