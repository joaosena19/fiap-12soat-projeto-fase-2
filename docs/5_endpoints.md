# 5. Endpoints

## Endpoints novos/modificados

Foram criados/modificados os seguintes endpoints para atender aos requisitos da Fase 2:

### Abertura de Ordem de Serviço (OS)

Antes havia apenas o endpoints `/api/ordens-servico`, que recebia o Id do veículo somente, e era necessário adicionar as outras informações em outras requisições.

Agora, além dele também existe o endpoint `/api/ordens-servico/completa`, que recebe todas as informações necessárias para criar a OS.

![[os_completa.png]]

### Consulta de status da OS

Esses endpoints não tiveram alterações. É possível consultar o status da OS pelo Id ou Código, estando autenticado:

![[os_status_consulta.png]]

E também é possível na busca pública, que não tem autenticação JWT e verifica através da combinação do código da OS e do documento do Cliente.

![[os_busca_publica.png]]

### Aprovação de orçamento

Antes havia apenas os endpoints `/api/ordens-servico/{id}/orcamento/aprovar` e `/api/ordens-servico/{id}/orcamento/desaprovar`. Além deles, foram criados versões webhooks, que usam autenticação HMAC.

![[os_aprovacao_webhook.png]]

### Listagem de ordens de serviço

O endpoint `/api/ordens-servico` foi alterado para ter a ordenação necessária e filtrar status inválidos. Além dos status requisitados, também removi da listagem o status *Cancelado*, que é um status a mais que havia criado, e que faz sentido também ser removido.

![[os_listagem.png]]

### Atualização de status da OS

Antes não havia nenhum endpoint para atualização direta do status da OS, sendo atualizado somente como parte de outras ações como *Aprovar Orçamento* ou *Iniciar Diagnóstico*. Agora, existe um endpoint webhook específico para atualização de status. Ele direciona para os mesmos métodos originais da entidade, aproveitando as mesmas regras de negócio que já existiam.

![[os_atualizar_status.png]]

## Documentação de todos os endpoints

Acesse pelo Swagger executando a aplicação, ou pela [documentação Redoc disponível](attachments/api_endpoint_doc.html) (necessário fazer download do arquivo).

## Autenticação

1. **Autenticação comum**
    Obtenha o token de autenticação no endpoint `/api/authentication/token`, passando as credenciais:
    ```json
    {
        "clientId": "admin",
        "clientSecret": "admin"
    }
    ```
    Use como Bearer token para todos os endpoints, exceto webhooks.

2. **Autenticação para webhooks**
    Gere uma assinatura HMAC para o payload específico da requisição, usando algum gerador como [Free Formater GMAC Generator](https://www.freeformatter.com/hmac-generator.html), usando o secret **63f4945d921d599f27ae4fdf5bada3f1**
    
    Envie a assinatura no header **X-Signature**

---
Anterior: [Arquitetura](4_arquitetura.md)  
Próximo: [Infraestrutura](6_infraestrutura.md)