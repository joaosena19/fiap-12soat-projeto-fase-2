using Application.Cadastros.Interfaces;
using Domain.Cadastros.Aggregates;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Cadastros
{
    public class ServicoRepository : IServicoRepository
    {
        private readonly AppDbContext _context;

        public ServicoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Servico> SalvarAsync(Servico servico)
        {
            await _context.Servicos.AddAsync(servico);
            await _context.SaveChangesAsync();

            return servico;
        }

        public async Task<Servico?> ObterPorIdAsync(Guid id)
        {
            return await _context.Servicos.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Servico?> ObterPorNomeAsync(string nome)
        {
            return await _context.Servicos.FirstOrDefaultAsync(s => s.Nome.Valor == nome);
        }

        public async Task<Servico> AtualizarAsync(Servico servico)
        {
            _context.Servicos.Update(servico);
            await _context.SaveChangesAsync();

            return servico;
        }

        public async Task<IEnumerable<Servico>> ObterTodosAsync()
        {
            return await _context.Servicos.ToListAsync();
        }
    }
}
