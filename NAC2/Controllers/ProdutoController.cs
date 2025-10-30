using Microsoft.AspNetCore.Mvc;
using NAC2.Domain;
using NAC2.Repository;
using NAC2.Service;
using Newtonsoft.Json;
using Service;
using System.Net;

namespace NAC2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoService produtoService;
        private readonly IProdutoRepository produtoRepository;
        private readonly ICacheService cacheService;
        private readonly ILogger<ProdutoController> logger;
        private const string cacheKey = "produtos-cache";

        public ProdutoController(
            IProdutoService produtoService,
            IProdutoRepository produtoRepository,
            ICacheService cacheService,
            ILogger<ProdutoController> logger)
        {
            this.produtoService = produtoService;
            this.produtoRepository = produtoRepository;
            this.cacheService = cacheService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                logger.LogInformation("Iniciando busca de produtos");

                try
                {
                    await cacheService.SetExpiryAsync(cacheKey, TimeSpan.FromMinutes(20));
                    string? cachedProdutos = await cacheService.GetAsync(cacheKey);

                    if (!string.IsNullOrEmpty(cachedProdutos))
                    {
                        logger.LogInformation("Produtos encontrados no cache Redis");
                        var produtos = JsonConvert.DeserializeObject<IEnumerable<Produto>>(cachedProdutos);
                        return Ok(produtos);
                    }
                }
                catch (Exception redisEx)
                {
                    logger.LogWarning(redisEx, "Erro ao acessar cache Redis, continuando sem cache");
                }

                var produtoList = await produtoRepository.GetProdutosAbaixoEstoqueMinimoAsync();

                if (produtoList == null || !produtoList.Any())
                {
                    logger.LogInformation("Nenhum produto encontrado no banco de dados");
                    return Ok(new List<Produto>());
                }

                try
                {
                    var produtoListJson = JsonConvert.SerializeObject(produtoList);
                    await cacheService.SetAsync(cacheKey, produtoListJson, TimeSpan.FromMinutes(20));
                    logger.LogInformation("Produtos salvos no cache Redis");
                }
                catch (Exception cacheEx)
                {
                    logger.LogWarning(cacheEx, "Erro ao salvar no cache Redis, mas dados foram retornados");
                }

                logger.LogInformation("Retornando {Count} produtos", produtoList.Count());
                return Ok(produtoList);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro interno ao buscar produtos");
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "Erro interno do servidor ao buscar produtos", timestamp = DateTime.UtcNow });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Produto produto)
        {
            try
            {
                if (produto == null)
                {
                    logger.LogWarning("Tentativa de cadastrar produto nulo");
                    return BadRequest(new { message = "Os dados do produto são obrigatórios.", timestamp = DateTime.UtcNow });
                }

                logger.LogInformation("Cadastrando novo produto: {Nome}", produto.Nome);

                int novoProdutoId = await produtoRepository.AddProdutoAsync(produto);

                if (novoProdutoId <= 0)
                {
                    logger.LogWarning("Falha ao cadastrar produto {Nome}", produto.Nome);
                    return StatusCode((int)HttpStatusCode.InternalServerError,
                        new { message = "Falha ao cadastrar produto.", timestamp = DateTime.UtcNow });
                }

                await InvalidateCache();

                logger.LogInformation("Produto {Nome} cadastrado com sucesso", produto.Nome);
                return CreatedAtAction(nameof(Get), new { id = novoProdutoId }, produto);
            }
            catch (ArgumentException argEx)
            {

                logger.LogWarning(argEx, "Erro de validação ao cadastrar produto");
                return BadRequest(new { message = argEx.Message, timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro interno ao cadastrar produto");
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "Erro interno do servidor ao cadastrar produto", timestamp = DateTime.UtcNow });
            }
        }


        private async Task InvalidateCache()
        {
            try
            {
                await cacheService.DeleteAsync(cacheKey);
                logger.LogInformation("Cache de produtos invalidado com sucesso");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Erro ao invalidar cache Redis, mas operação continuará");
            }
        }
    }
}
