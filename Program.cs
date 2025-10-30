using NAC2.Repository;
using NAC2.Service;
using Service;
using System.ComponentModel.Design;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<ICacheService, CacheService>();

// Registro de repositórios
builder.Services.AddScoped<IProdutoRepository>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                           ?? "Server=localhost;Database=fiap;User=root;Password=123;Port=3306;";
    return new ProdutoRepository(connectionString);
});

builder.Services.AddScoped<IMovimentacaoEstoqueRepository>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                           ?? "Server=localhost;Database=fiap;User=root;Password=123;Port=3306;";
    return new MovimentacaoEstoqueRepository(connectionString);
});

// Registro de services
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IMovimentacaoEstoqueService, MovimentacaoEstoqueService>();
builder.Services.AddScoped<IMovimentacaoEstoqueRepository, MovimentacaoEstoqueRepository>();

// Registro do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Middleware global de tratamento de exceções
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (exception != null)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(exception.Error, "Erro não tratado capturado pelo middleware global");

            var errorResponse = new
            {
                message = "Erro interno do servidor",
                timestamp = DateTime.UtcNow,
                requestId = context.TraceIdentifier
            };

            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(errorResponse));
        }
    });
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
