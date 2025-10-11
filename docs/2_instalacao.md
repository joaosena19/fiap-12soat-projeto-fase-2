- Falar como executar localmente com docker
- Como fazer deploy com kubernetes
- Como provisionar infraestrutura com terraform

# 2. Instalação

## Localmente com Docker

1.  **Clonar o Repositório:**
    ```bash
    git clone https://github.com/joaosena19/fiap-12soat-projeto-fase-2
    cd fiap-12soat-projeto-fase-2/src
    ```
2.  **Iniciar a Aplicação:**
    Na pasta `src`, execute o comando:
    ```bash
    docker-compose up --build
    ```
    Este comando irá construir a imagem da API, baixar a do PostgreSQL, iniciar ambos os containers, aplicar as migrations do banco de dados e popular o banco com dados de teste para o ambiente de desenvolvimento.

    A aplicação estará disponível em `http://localhost:5001`
3. **Autenticação comum**
    Obtenha o token de autentição no endpoint `/api/authentication/token`, passando as credenciais:
    ```json
    {
        "clientId": "admin",
        "clientSecret": "admin"
    }
    ```
    Use como Bearer token para todos os endpoints.
4. **Autenticação para webhooks**
    Gere uma assinatura HMAC para o payload específico da requisição, usando algum gerador como [Free Formater GMAC Generator](https://www.freeformatter.com/hmac-generator.html), usando o secret **63f4945d921d599f27ae4fdf5bada3f1**

    Envia a assinatura no header **X-Signature**

## 