using NAC2.Domain;

namespace NAC2.Repository
{
    public interface IProdutoRepository
    {
        Task<IEnumerable<Produto>> GetProdutosAbaixoEstoqueMinimoAsync();
        Task<int> AddProdutoAsync(Produto produto);
    }

}
