using Application.Interfaces;
using Domain.Cadastros.Aggregates;
using Infrastructure.Data;

namespace Infrastructure.Cadastros
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _context;

        public ClienteRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Salvar(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            _context.SaveChanges();
        }

        public Cliente? ObterPorCpf(string cpf)
        {
            return _context.Clientes.FirstOrDefault(c => c.Cpf.Valor == cpf);
        }
    }
}
