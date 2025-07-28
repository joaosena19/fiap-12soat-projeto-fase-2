using Application.OrdemServico.Interfaces;
using Domain.OrdemServico.Enums;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.OrdemServico
{
    public class OrdemServicoRepository : IOrdemServicoRepository
    {
        private readonly AppDbContext _context;

        public OrdemServicoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico> SalvarAsync(Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico ordemServico)
        {
            await _context.OrdensServico.AddAsync(ordemServico);
            await _context.SaveChangesAsync();

            return ordemServico;
        }

        public async Task<Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico?> ObterPorIdAsync(Guid id)
        {
            return await _context.OrdensServico
                .Include(os => os.ServicosIncluidos)
                .Include(os => os.ItensIncluidos)
                .Include(os => os.Orcamento)
                .FirstOrDefaultAsync(os => os.Id == id);
        }

        public async Task<Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico?> ObterPorCodigoAsync(string codigo)
        {
            return await _context.OrdensServico
                .Include(os => os.ServicosIncluidos)
                .Include(os => os.ItensIncluidos)
                .Include(os => os.Orcamento)
                .FirstOrDefaultAsync(os => os.Codigo.Valor.ToUpper() == codigo.ToUpper());
        }

        public async Task<Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico> AtualizarAsync(Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico ordemServico)
        {
            // Busca os dados atuais para ver se as entidades filhas de OrdemServico devem ser adicionadas
            var existingOrdemServico = await _context.OrdensServico.AsNoTracking()
                .Include(os => os.ServicosIncluidos)
                .Include(os => os.ItensIncluidos)
                .Include(os => os.Orcamento)
                .FirstAsync(os => os.Id == ordemServico.Id);

            var existingServicoIds = existingOrdemServico.ServicosIncluidos.Select(s => s.Id).ToHashSet();
            var existingItensIds = existingOrdemServico.ItensIncluidos.Select(s => s.Id).ToHashSet();
            var existingOrcamento = existingOrdemServico.Orcamento?.Id;

            await _context.ServicosIncluidos.AddRangeAsync(ordemServico.ServicosIncluidos.Where(si => !existingServicoIds.Contains(si.Id)));
            await _context.ItensIncluidos.AddRangeAsync(ordemServico.ItensIncluidos.Where(si => !existingItensIds.Contains(si.Id)));

            if(existingOrcamento is null && ordemServico.Orcamento is not null)
                await _context.Orcamentos.AddAsync(ordemServico.Orcamento);

            await _context.SaveChangesAsync();

            return ordemServico;
        }

        public async Task<IEnumerable<Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico>> ObterTodosAsync()
        {
            return await _context.OrdensServico
                .Include(os => os.ServicosIncluidos)
                .Include(os => os.ItensIncluidos)
                .Include(os => os.Orcamento)
                .ToListAsync();
        }

        public async Task<IEnumerable<Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico>> ObterEntreguesUltimosDiasAsync(int quantidadeDias)
        {
            var dataLimite = DateTime.UtcNow.AddDays(-quantidadeDias);
            
            return await _context.OrdensServico
                .Where(os => os.Status.Valor == StatusOrdemServicoEnum.Entregue && 
                            os.Historico.DataCriacao.Date >= dataLimite.Date)
                .ToListAsync();
        }

        public async Task RemoverAsync(Guid id)
        {
            var ordemServico = await _context.OrdensServico.FindAsync(id);
            if (ordemServico != null)
            {
                _context.OrdensServico.Remove(ordemServico);
                await _context.SaveChangesAsync();
            }
        }
    }
}
