# 9. API

## Autenticação
- **Tipo**: JWT Bearer Token
- **Header**: `Authorization: Bearer {token}`
- **Endpoint para adquirir Token**: `POST /api/authentication/token`
- **Credenciais**:
    ```json
    {
        "clientId": "admin",
        "clientSecret": "admin"
    }
    ```

## Endpoints

Acesse pelo Swagger executando a aplicação, ou pela [documentação Redoc disponível](attachments/oficina_mecanica_api_docs.html) (necessário fazer download do arquivo).

## Códigos de Status HTTP

### Sucesso
- **200 OK**: Operação realizada com sucesso
- **201 Created**: Recurso criado com sucesso
- **204 No Content**: Operação realizada sem retorno de conteúdo

### Erro do Cliente
- **400 Bad Request**: Dados inválidos ou malformados
- **401 Unauthorized**: Token de autenticação ausente ou inválido
- **404 Not Found**: Recurso não encontrado
- **409 Conflict**: Conflito de dados (ex: cliente já cadastrado)
- **422 Unprocessable Content**: Requisição bem formatada, porém impossível de ser processada

### Erro do Servidor
- **500 Internal Server Error**: Erro interno do servidor

## Formato de Resposta

### Sucesso
```json
{
  [Objeto da resposta]
}
```

### Erro
```json
{
  "message": "Mensagem de erro",
  "statusCode": 500
}
```

---
Anterior: [Arquitetura](8_arquitetura.md)  
Próximo: [Testes](10_testes.md)
