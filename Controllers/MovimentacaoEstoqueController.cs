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
    public class MovimentacaoEstoqueController : ControllerBase
    {
        private readonly MovimentacaoEstoqueService movimentacaoService;
        private readonly IMovimentacaoEstoqueRepository movimentacaoRepository;
        private readonly ICacheService cacheService;
        private readonly ILogger<MovimentacaoEstoqueController> logger;
        private const string cacheKey = "produtos-cache";

        public MovimentacaoEstoqueController(
            MovimentacaoEstoqueService movimentacaoService,
            IMovimentacaoEstoqueRepository movimentacaoRepository,
            ICacheService cacheService,
            ILogger<MovimentacaoEstoqueController> logger)
        {
            this.movimentacaoService = movimentacaoService;
            this.movimentacaoRepository = movimentacaoRepository;
            this.cacheService = cacheService;
            this.logger = logger;
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MovimentacaoEstoque movimentacao)
        {
            try
            {
                if (movimentacao == null)
                {
                    logger.LogWarning("Tentativa de cadastrar movimentação nula");
                    return BadRequest(new { message = "Os dados da movimentação são obrigatórios.", timestamp = DateTime.UtcNow });
                }

                logger.LogInformation("Registrando movimentação do produto");

                int movimentacaoId = await movimentacaoService.AddAllMovimentacoesAsync(movimentacao);

                if (movimentacaoId <= 0)
                {
                    logger.LogWarning("Falha ao registrar movimentação do produto");
                    return StatusCode((int)HttpStatusCode.InternalServerError,
                        new { message = "Falha ao registrar movimentação.", timestamp = DateTime.UtcNow });
                }

                await InvalidateCache();
                logger.LogInformation("Movimentação registrada com sucesso");

                return CreatedAtAction(nameof(Post), new { id = movimentacaoId }, movimentacao);
            }
            catch (ArgumentException argEx)
            {
                logger.LogWarning(argEx, "Erro de validação ao registrar movimentação");
                return BadRequest(new { message = argEx.Message, timestamp = DateTime.UtcNow });
            }
            catch (InvalidOperationException invEx)
            {
                logger.LogWarning(invEx, "Erro de operação ao registrar movimentação");
                return BadRequest(new { message = invEx.Message, timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro interno ao registrar movimentação do produto");
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "Erro interno do servidor ao registrar movimentação", timestamp = DateTime.UtcNow });
            }
        }


        private async Task InvalidateCache()
        {
            try
            {
                await cacheService.DeleteAsync(cacheKey);
                logger.LogInformation("Cache de movimentações invalidado com sucesso");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Erro ao invalidar cache Redis, mas operação continuará");
            }
        }
    }
}
