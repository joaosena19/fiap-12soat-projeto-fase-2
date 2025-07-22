using Application.Cadastros.Interfaces;
using Domain.Cadastros.Aggregates;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Cadastros
{
    public class VeiculoRepository : IVeiculoRepository
    {
        private readonly AppDbContext _context;

        public VeiculoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Veiculo> SalvarAsync(Veiculo veiculo)
        {
            _context.Veiculos.Add(veiculo);
            await _context.SaveChangesAsync();
            return veiculo;
        }

        public async Task<Veiculo?> ObterPorPlacaAsync(string placa)
        {
            return await _context.Veiculos.FirstOrDefaultAsync(v => v.Placa.Valor == placa);
        }

        public async Task<Veiculo?> ObterPorIdAsync(Guid id)
        {
            return await _context.Veiculos
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Veiculo> AtualizarAsync(Veiculo veiculo)
        {
            _context.Veiculos.Update(veiculo);
            await _context.SaveChangesAsync();
            return veiculo;
        }

        public async Task<IEnumerable<Veiculo>> ObterTodosAsync()
        {
            return await _context.Veiculos.ToListAsync();
        }
    }
}
