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
            throw new ArgumentNullException(nameof(movimentacao), "Movimentação inválido.");
        await _connection.OpenAsync();
        string sql = @"
                INSERT INTO Movimentacao (Tipo, Quantidade, DataMoviementacao, Lote, DataValidade, CodSKU)
                VALUES (@Tipo, @Quantidade, @DataMoviementacao, @Lote, @DataValidade, @CodSKU);
                SELECT LAST_INSERT_ID();
            ";
        var id = await _connection.ExecuteScalarAsync<int>(sql, movimentacao);
        await _connection.CloseAsync();
        return id;
    }

    //public async Task<IEnumerable<Produto>> GetProdutosAbaixoEstoqueMinimoAsync()
    //{
    //    await _connection.OpenAsync();
    //    string sql = "SELECT p.CodSKU, p.Nome, p.Categoria, p.PrecoUnitario, p.QuantMinima, p.DataCriacao FROM Produto p " +
    //        "JOIN MovimentacaoEstoque m ON p.CodSKU = m.CodSKU WHERE m.Quantidade < p.QuantMinima;";
    //    var produtos = await _connection.QueryAsync<Produto>(sql);
    //    await _connection.CloseAsync();
    //    return produtos;
    //}
}
}
