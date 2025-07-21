using Application.Cadastros.DTO;
using Application.Cadastros.Interfaces;
using Domain.Cadastros.Aggregates;
using Shared.Exceptions;
using System.Net;

namespace Application.Cadastros.Services
{
    public class ServicoService : IServicoService
    {
        private readonly IServicoRepository _servicoRepository;

        public ServicoService(IServicoRepository servicoRepository)
        {
            _servicoRepository = servicoRepository;
        }

        public async Task<RetornoServicoDTO> CriarServico(string nome, decimal preco)
        {
            var servicoExistente = await _servicoRepository.ObterPorNomeAsync(nome);
            if (servicoExistente != null)
                throw new DomainException("Já existe um serviço cadastrado com este nome.", HttpStatusCode.Conflict);

            var novoServico = Servico.Criar(nome, preco);
            var result = await _servicoRepository.SalvarAsync(novoServico);

            return new RetornoServicoDTO() { Id = result.Id, Nome = result.Nome.Valor, Preco = result.Preco.Valor };
        }

        public async Task<RetornoServicoDTO> AtualizarServico(Guid id, string nome, decimal preco)
        {
            var servico = await _servicoRepository.ObterPorIdAsync(id);
            if (servico == null)
                throw new DomainException("Serviço não encontrado.", HttpStatusCode.NotFound);

            servico.Atualizar(nome, preco);
            var result = await _servicoRepository.AtualizarAsync(servico);

            return new RetornoServicoDTO() { Id = result.Id, Nome = result.Nome.Valor, Preco = result.Preco.Valor };
        }

        public async Task<IEnumerable<RetornoServicoDTO>> Buscar()
        {
            var servicos = await _servicoRepository.ObterTodosAsync();
            return servicos.Select(c => new RetornoServicoDTO 
            { 
                Id = c.Id, 
                Nome = c.Nome.Valor, 
                Preco = c.Preco.Valor 
            });
        }

        public async Task<RetornoServicoDTO> BuscarPorId(Guid id)
        {
            var servico = await _servicoRepository.ObterPorIdAsync(id);
            if (servico == null)
                throw new DomainException("Serviço não encontrado.", HttpStatusCode.NotFound);

            return new RetornoServicoDTO() { Id = servico.Id, Nome = servico.Nome.Valor, Preco = servico.Preco.Valor };
        }
    }
}
