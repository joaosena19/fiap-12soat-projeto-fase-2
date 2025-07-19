using Application.Interfaces;
using Domain.Cadastros.Aggregates;
using Domain.Cadastros.ValueObjects.Cliente;

namespace Application.Cadastros
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public void CriarCliente(string nome, string cpf)
        {
            var clienteExistente = _clienteRepository.ObterPorCpf(cpf);
            if (clienteExistente != null)
                throw new InvalidOperationException("Já existe um cliente cadastrado com este CPF.");

            var novoCliente = Cliente.Criar(nome, new Cpf(cpf));
            _clienteRepository.Salvar(novoCliente);
        }
    }
}
