using NAC2.Domain;

namespace NAC2.Repository
{
    public interface IMovimentacaoEstoqueRepository
    {
        Task<int> IncluiMovimentacaoProdutoAsync(MovimentacaoEstoque movimentacao);
    }
}
