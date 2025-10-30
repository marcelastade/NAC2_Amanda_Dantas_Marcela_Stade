using NAC2.Domain;
using System.Runtime.ConstrainedExecution;

namespace NAC2.Service
{
    public interface IProdutoService
    {
        Task<Produto> AddProdutoAsync(Produto produto);
        Task<IEnumerable<Produto>> GetAllProdutosAbaixoQuantidadeMinimaAsync();
    }
}
