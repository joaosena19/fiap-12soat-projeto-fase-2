using Application.Estoque.Interfaces;
using Application.OrdemServico.DTO.External;
using Application.OrdemServico.Interfaces.External;
using Domain.OrdemServico.Enums;
using Shared.Exceptions;
using System.Net;

namespace Infrastructure.AntiCorruptionLayer.OrdemServico
{
    /// <summary>
    /// Anti-corruption layer para acessar itens de estoque do bounded context de Estoque
    /// </summary>
    public class EstoqueExternalService : IEstoqueExternalService
    {
        private readonly IItemEstoqueRepository _itemEstoqueRepository;

        public EstoqueExternalService(IItemEstoqueRepository itemEstoqueRepository)
        {
            _itemEstoqueRepository = itemEstoqueRepository;
        }

        public async Task<ItemEstoqueExternalDTO?> ObterItemEstoquePorIdAsync(Guid itemId)
        {
            var item = await _itemEstoqueRepository.ObterPorIdAsync(itemId);
            
            if (item == null)
                return null;

            return new ItemEstoqueExternalDTO
            {
                Id = item.Id,
                Nome = item.Nome.Valor,
                Preco = item.Preco.Valor,
                Quantidade = item.Quantidade.Valor,
                TipoItemIncluido = ConverterTipoItemEstoqueParaTipoItemIncluido(item.TipoItemEstoque.Valor)
            };
        }

        /// <summary>
        /// Converte o tipo de item de estoque (do bounded context de Estoque) 
        /// para o tipo de item incluído (do bounded context de OrdemServico)
        /// </summary>
        private TipoItemIncluidoEnum ConverterTipoItemEstoqueParaTipoItemIncluido(string tipoItemEstoque)
        {
            return tipoItemEstoque.ToLower() switch
            {
                "peca" => TipoItemIncluidoEnum.Peca,
                "insumo" => TipoItemIncluidoEnum.Insumo,
                _ => throw new DomainException($"Tipo de item de estoque '{tipoItemEstoque}' não é válido.", HttpStatusCode.BadRequest)
            };
        }
    }
}
