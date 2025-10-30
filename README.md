🧾 #NAC2 - Sistema de Controle de Estoque e Movimentações

O NAC2 é uma API REST desenvolvida em .NET 8, utilizando Dapper, MySQL e Redis para controle de cache.
O sistema permite o cadastro de produtos, controle de movimentações de estoque (entrada e saída), além de validações de regras de negócio relacionadas à validade de produtos perecíveis e alertas de estoque mínimo.

⚙️ #Regras de Negócio Implementadas
🧩 #Produto

O nome, categoria, preço unitário e quantidade mínima são obrigatórios.

A categoria deve ser obrigatoriamente "PERECIVEL" ou "NAO_PERECIVEL".

Produtos perecíveis não podem ter movimentações após a data de validade.

Produtos abaixo da quantidade mínima geram log de alerta no sistema.

📦 #Movimentação de Estoque

Tipos permitidos: ENTRADA e SAIDA.

Se a movimentação for SAIDA, o sistema valida o estoque atual para evitar quantidades negativas.

Caso o produto ainda não tenha movimentações, ele é incluído automaticamente com a quantidade informada.

Movimentações de produtos perecíveis vencidos são bloqueadas.

🧱 #Diagrama Simples das Entidades
+-------------------+
|     Produto       |
+-------------------+
| CodSKU (PK)       |
| Nome              |
| Categoria          |
| PrecoUnitario      |
| QuantMinima        |
| DataCriacao        |
+-------------------+
           |
           | 1:N
           |
+---------------------------+
|    MovimentacaoEstoque    |
+---------------------------+
| Id (PK)       |
| Tipo (Entrada/Saída)      |
| Quantidade                |
| DataMovimentacao          |
| Lote                      |
| DataValidade              |
| CodSKU (FK -> Produto)    |
+---------------------------+

🔧 #Como Executar o Projeto
✅ #Pré-requisitos

.NET 8 SDK

MySQL Server

Redis

Visual Studio 2022 ou VS Code

🚀 Passos para executar

Clone o repositório:

git clone https://github.com/seuusuario/NAC2.git
cd NAC2


Configure a string de conexão no appsettings.json ou no Program.cs:

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=fiap;User=root;Password=123;Port=3306;"
}


Execute o projeto:

dotnet run


Acesse a API no navegador:

https://localhost:7080/swagger

📬 #Exemplos de Requisições API
🔹 #Cadastrar Produto

POST /api/Produto

{
  "CodSKU": 101,
  "Nome": "Leite Integral",
  "Categoria": "PERECIVEL",
  "PrecoUnitario": 5.99,
  "QuantMinima": 10,
  "DataCriacao": "2025-10-30T00:00:00Z"
}

🔹 #Registrar Movimentação de Estoque

POST /api/MovimentacaoEstoque

{
  "Tipo": "ENTRADA",
  "Quantidade": 50,
  "DataMovimentacao": "2025-10-30T00:00:00Z",
  "Lote": "L001",
  "DataValidade": "2025-12-01T00:00:00Z",
  "CodSKU": 101
}

🔹 Buscar Produtos com Estoque Baixo

GET /api/Produto

