# 🚗 Meu Combustível - API Backend

Este repositório contém o backend da aplicação **"Meu Combustível"**, um sistema de gestão de frotas desenvolvido em **ASP.NET Core Web API**. A API é responsável por gerenciar dados de **empresas, usuários, motoristas, veículos, abastecimentos e manutenções**, utilizando o **Supabase** como banco de dados e serviço de autenticação.

---

## 🚀 Tecnologias Utilizadas

* **Backend:** ASP.NET Core 8.0 Web API (C#)
* **ORM/Acesso a Dados:** Supabase.Postgrest Client
* **Mapeamento de Objetos:** AutoMapper
* **Autenticação/Autorização:** JWT (com Supabase Auth)
* **Banco de Dados:** PostgreSQL (via Supabase)
* **Documentação:** Swagger / OpenAPI
* **Gerenciamento de Pacotes:** NuGet
* **Implantação:** Render

---

## 🧪 Como Testar a API em Produção

Acesse a API em produção:

🔗 [Swagger UI - API em Produção](https://gestaodecombustivelapp.onrender.com/)

### 1. Autentique-se (Obtenha o Token)

No Swagger UI, encontre o endpoint:
`POST /api/Usuario/autenticar`

Clique em "Try it out" e use as credenciais de teste:

```json
{
  "email": "admin@gmail.com",
  "senha": "Admin1020"
}
```

Clique em **"Execute"** e copie o valor completo do `accessToken`.

### 2. Autorize as Requisições

No topo do Swagger, clique em **"Authorize"**.

Cole o token no seguinte formato:

```
Bearer SEU_ACCESS_TOKEN_COMPLETO_AQUI
```

Clique em **"Authorize"** e depois em **"Close"**.

### 3. Teste um Endpoint Protegido

Exemplo:
`GET /api/Usuario`
Clique em "Try it out" e depois em "Execute" para ver os usuários cadastrados.

---

## ⚙️ Como Começar (Ambiente Local)

### Pré-requisitos

* [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download)
* Visual Studio 2022 (ou Visual Studio Code)
* Conta no [Supabase](https://supabase.com/) com projeto e tabelas configuradas

### 📅 Clonando o Repositório

```bash
git clone https://github.com/marcosviniciusdeveloper/GestaoDeCombustivelApp.git
cd GestaoDeCombustivelApp/WebApplication1
```

---

## 👈️ Configuração do Banco de Dados (Supabase)

* Crie as seguintes tabelas no Supabase: `empresas`, `usuarios`, `motoristas`, `veiculos`, `abastecimentos`, `manutencoes`.
* Garanta que:

  * A coluna `empresa_id` em `usuarios` permite `NULL`.
  * A coluna `usuario_id` em `motoristas` seja `PRIMARY KEY` e `FOREIGN KEY` para `usuarios.id`.
  * As políticas de RLS (Row Level Security) estejam desativadas ou configuradas corretamente para desenvolvimento.

---

## 🔐 Configuração do Ambiente (Variáveis)

As variáveis sensíveis da aplicação (como URL do Supabase, JWT Secret, etc.) devem ser definidas via **variáveis de ambiente** para maior segurança.

Exemplo (no Render, .env, ou launchSettings):

```env
ASPNETCORE_ENVIRONMENT=Production
Supabase__Url=<sua-url-supabase>
Supabase__AnonKey=<sua-anon-key>
Jwt__Issuer=<url-da-api>
Jwt__Audience=authenticated
Jwt__Key=<seu-jwt-secret>
```

> 🚨 Nunca exponha suas chaves diretamente no código-fonte ou em arquivos versionados (como appsettings.json).

---

## ▶️ Executando a API Localmente

Instale as dependências:

```bash
dotnet restore
```

Rode a aplicação:

```bash
dotnet run
```

A API ficará disponível em: `https://localhost:7105`

---

## ☁️ Implantação no Render

1. Crie um novo Web Service no painel do Render.
2. Selecione o repositório e configure:

   * **Root Directory:** `/WebApplication1`
   * **Runtime:** .NET
   * **Build Command:** `dotnet publish -c Release -o ./publish`
   * **Start Command:** `dotnet ./publish/WebApplication1.dll`
3. Adicione suas variáveis de ambiente no Render conforme descrito acima.

---

## 🤝 Contribuição

Contribuições são bem-vindas!

1. Faça um fork do repositório
2. Crie uma branch: `git checkout -b minha-feature`
3. Commit suas alterações: `git commit -m 'Minha nova feature'`
4. Push na branch: `git push origin minha-feature`
5. Abra um Pull Request

---

## 📄 Licença

Este projeto está sob a licença MIT.

---
