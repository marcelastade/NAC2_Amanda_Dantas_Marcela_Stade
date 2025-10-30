using NAC2.Domain;

namespace NAC2.Service
{
    public interface IMovimentacaoEstoqueService { 
        
        Task<MovimentacaoEstoque> AddAllMovimentacoesAsync(MovimentacaoEstoque movimentacao);
    }
}
