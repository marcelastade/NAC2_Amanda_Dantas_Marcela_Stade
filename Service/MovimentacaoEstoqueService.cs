using NAC2.Domain;
using NAC2.Repository;

namespace NAC2.Service
{
    public class MovimentacaoEstoqueService : IMovimentacaoEstoqueService
    {
    
    private readonly MovimentacaoEstoqueRepository _repository;

    public MovimentacaoEstoqueService(MovimentacaoEstoqueRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> AddAllMovimentacoesAsync(MovimentacaoEstoque movimentacao)
    {
        if (movimentacao.Quantidade < 0)
            throw new ArgumentException("A quantidade deve ser positiva.");

        if (!movimentacao.Tipo.Equals("ENTRADA", StringComparison.OrdinalIgnoreCase) ||
            !movimentacao.Tipo.Equals("SAIDA", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("A categoria do produto deve ser PERECIVEL ou NAO_PERECIVEL.");
        }
        return await _repository.IncluiMovimentacaoProdutoAsync(movimentacao);
    }

        Task<MovimentacaoEstoque> IMovimentacaoEstoqueService.AddAllMovimentacoesAsync(MovimentacaoEstoque movimentacao)
        {
            throw new NotImplementedException();
        }
    }
}
