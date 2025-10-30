using NAC2.Domain;
using NAC2.Repository;
using System;
using System.Threading.Tasks;

namespace NAC2.Service
{
    public class MovimentacaoEstoqueService : IMovimentacaoEstoqueService
    {
        private readonly IMovimentacaoEstoqueRepository _repository;

        public MovimentacaoEstoqueService(IMovimentacaoEstoqueRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<int> AddAllMovimentacoesAsync(MovimentacaoEstoque movimentacao)
        {
            if (movimentacao == null)
                throw new ArgumentNullException(nameof(movimentacao), "A movimentação não pode ser nula.");

            if (movimentacao.Quantidade <= 0)
                throw new ArgumentException("A quantidade deve ser positiva.");

            if (!movimentacao.Tipo.Equals("ENTRADA", StringComparison.OrdinalIgnoreCase) &&
                !movimentacao.Tipo.Equals("SAIDA", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("O tipo da movimentação deve ser ENTRADA ou SAIDA.");

            if (movimentacao.Tipo.Equals("ENTRADA", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(movimentacao.Lote) && movimentacao.DataValidade < DateTime.UtcNow)
                    throw new ArgumentException("Data de validade inválida para produto perecível.");
            }

            if(movimentacao.Tipo.Equals("SAIDA", StringComparison.OrdinalIgnoreCase))
{
                var estoqueAtual = await _repository.GetEstoqueAtualAsync(movimentacao.CodSKU);
                if (estoqueAtual < movimentacao.Quantidade)
                    throw new InvalidOperationException("Estoque insuficiente para a saída.");

                if (movimentacao.DataValidade < DateTime.UtcNow)
                    throw new InvalidOperationException("Não é permitido movimentar produto perecível vencido.");
            }

            return await _repository.IncluiMovimentacaoProdutoAsync(movimentacao);
        }
    }
}
