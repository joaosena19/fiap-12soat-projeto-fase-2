using Application.Interfaces;
using Domain.Cadastros.Aggregates;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Cadastros
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _context;

        public ClienteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cliente> SalvarAsync(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();

            return cliente;
        }

        public async Task<Cliente?> ObterPorCpfAsync(string cpf)
        {
            return await _context.Clientes.FirstOrDefaultAsync(c => c.Cpf.Valor == cpf);
        }
    }
}
