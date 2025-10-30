using Dapper;
using NAC2.Domain;
using MySqlConnector;

namespace NAC2.Repository
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly MySqlConnection _connection;
        public ProdutoRepository(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
        }

        public async Task<int> AddProdutoAsync(Produto produto)
        {
            if (produto == null)
                throw new ArgumentNullException(nameof(produto), "Produto inválido.");
            await _connection.OpenAsync();
            string sql = @"
                INSERT INTO Produto (CodSKU, Nome, Categoria, PrecoUnitario, QuantMinima, DataCriacao)
                VALUES (@CodSKU, @Nome, @Categoria, @PrecoUnitario, @QuantMinima, @DataCriacao);
                SELECT LAST_INSERT_ID();
            ";
            var id = await _connection.ExecuteScalarAsync<int>(sql, produto);
            await _connection.CloseAsync();
            return id;
        }

        public async Task<IEnumerable<Produto>> GetProdutosAbaixoEstoqueMinimoAsync()
        {
            await _connection.OpenAsync();
            string sql = "SELECT p.CodSKU, p.Nome, p.Categoria, p.PrecoUnitario, p.QuantMinima, p.DataCriacao FROM Produto p " +
                "JOIN MovimentacaoEstoque m ON p.CodSKU = m.CodSKU WHERE m.Quantidade < p.QuantMinima;";
            var produtos = await _connection.QueryAsync<Produto>(sql);
            await _connection.CloseAsync();
            return produtos;
        }
    }
}
