using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NAC2.Domain;
using NAC2.Repository;
using StackExchange.Redis;
using System.ComponentModel.Design;
using System.Runtime.ConstrainedExecution;

namespace NAC2.Service
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _repository;

        public ProdutoService(IProdutoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Produto>> GetAllProdutosAbaixoQuantidadeMinimaAsync() =>
            await _repository.GetProdutosAbaixoEstoqueMinimoAsync();

        public async Task<int> AddProdutoAsync(Produto produto)
        {
            if (produto == null)
                throw new ArgumentNullException(nameof(produto));

            if (string.IsNullOrWhiteSpace(produto.Nome))
                throw new ArgumentException("O nome do produto é obrigatório.");

            if (produto.PrecoUnitario <= 0)
                throw new ArgumentException("O preço do produto deve ser maior que zero.");

            if (produto.QuantMinima < 0)
                throw new ArgumentException("O estoque mínimo não pode ser negativo.");

            if (string.IsNullOrWhiteSpace(produto.Categoria))
                throw new ArgumentException("A categoria do produto é obrigatória.");

            if (!produto.Categoria.Equals("PERECIVEL", StringComparison.OrdinalIgnoreCase) ||
                !produto.Categoria.Equals("NAO_PERECIVEL", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("A categoria do produto deve ser PERECIVEL ou NAO_PERECIVEL.");
            }
            return await _repository.AddProdutoAsync(produto);
        }

        Task<Produto> IProdutoService.AddProdutoAsync(Produto produto)
        {
            throw new NotImplementedException();
        }
    }
}

