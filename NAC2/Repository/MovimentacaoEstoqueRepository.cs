using MySqlConnector;
using Dapper;
using NAC2.Domain;
using System.Data.Common;

namespace NAC2.Repository
{
    public class MovimentacaoEstoqueRepository : IMovimentacaoEstoqueRepository
    {
        private readonly MySqlConnection _connection;
        public MovimentacaoEstoqueRepository(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
        }

        public async Task<int> IncluiMovimentacaoProdutoAsync(MovimentacaoEstoque movimentacao)
        {
            if (movimentacao == null)
                throw new ArgumentNullException(nameof(movimentacao), "Movimentação inválida.");

            await _connection.OpenAsync();

            string verificaSql = "SELECT COUNT(*) FROM MovimentacaoEstoque WHERE CodSKU = @CodSKU";
            int existe = await _connection.ExecuteScalarAsync<int>(verificaSql, new { movimentacao.CodSKU });

            int id = 0;

            if (existe > 0)
            {
                string atualizaSql = movimentacao.Tipo.ToUpper() == "ENTRADA"
                    ? @"UPDATE MovimentacaoEstoque 
                       SET Quantidade = Quantidade + @Quantidade, DataMovimentacao = @DataMovimentacao 
                       WHERE CodSKU = @CodSKU"
                    : @"UPDATE MovimentacaoEstoque 
                       SET Quantidade = Quantidade - @Quantidade, DataMovimentacao = @DataMovimentacao 
                       WHERE CodSKU = @CodSKU";

                await _connection.ExecuteAsync(atualizaSql, movimentacao);

                id = movimentacao.CodSKU;
            }
            else
            {
                string insertSql = @"
                    INSERT INTO MovimentacaoEstoque 
                    (Tipo, Quantidade, DataMovimentacao, Lote, DataValidade, CodSKU)
                    VALUES (@Tipo, @Quantidade, @DataMovimentacao, @Lote, @DataValidade, @CodSKU);
                    SELECT LAST_INSERT_ID();
                ";

                id = await _connection.ExecuteScalarAsync<int>(insertSql, movimentacao);
            }

            await _connection.CloseAsync();
            return id;
        }

        public async Task<int> GetEstoqueAtualAsync(int codSKU)
        {
            var sql = "SELECT Quantidade FROM MovimentacaoEstoque WHERE CodSKU = @CodSKU";
            return await _connection.ExecuteScalarAsync<int>(sql, new { CodSKU = codSKU });
        }

        public async Task AtualizaEstoqueProdutoAsync(int codSKU, int quantidade, string tipo)
        {
            string sql = tipo.ToUpper() == "ENTRADA"
                ? "UPDATE MovimentacaoEstoque SET Quantidade = Quantidade + @quantidade WHERE CodSKU = @codSKU"
                : "UPDATE MovimentacaoEstoque SET Quantidade = Quantidade - @quantidade WHERE CodSKU = @codSKU";

            await _connection.ExecuteAsync(sql, new { codSKU, quantidade });
        }
    }
}
