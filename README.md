# Identificação

Aluno: João Pedro Sena Dainese  
Registro FIAP: RM365182  


Turma 12SOAT - Software Architecure  
Grupo individual  
Grupo 120  

Discord: joaodainese  
Email: joaosenadainese@gmail.com  

# Introdução

Este projeto foi realizado como Tech Challange da primeira fase do curso de Pós-Graduação em Software Architecture, ministrado pela FIAP. O objetivo dessa fase é aplicar os conceitos do Domain-Drive Design (DDD), Segurança e Qualidade de Software.

# Documentação

Disponível na pasta `docs`. Acesse diretamente [clicando aqui](./docs/1_introducao.md).

# Instalação

1.  **Clonar o Repositório:**
    ```bash
    git clone [https://github.com/joaosena19/fiap-12soat-projeto-fase-1](https://github.com/joaosena19/fiap-12soat-projeto-fase-1)
    cd fiap-12soat-projeto-fase-1/src
    ```
2.  **Iniciar a Aplicação:**
    Na pasta `src`, execute o comando:
    ```bash
    docker-compose up --build
    ```
    Este comando irá construir a imagem da API, baixar a do PostgreSQL, iniciar ambos os containers, aplicar as migrations do banco de dados e popular o banco com dados de teste para o ambiente de desenvolvimento.

    A aplicação estará disponível em `http://localhost:5001`
3. **Autenticação**
    Obtenha o token de autentição no endpoint `/api/authentication/token`, passando as credenciais:
    ```json
    {
        "clientId": "admin",
        "clientSecret": "admin"
    }
    ```
    Use como Bearer token para todos os endpoints.