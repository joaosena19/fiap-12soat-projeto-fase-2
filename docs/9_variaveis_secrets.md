# 9. Variáveis e secrets

Abaixo está uma comparação das variáveis e secrets que o sistema utiliza

| appsettings.json                | .env Docker / config-map Kubernetes |
| ------------------------------- | ----------------------------------- |
| DatabaseConnection.Host         | DatabaseConnection__Host            |
| DatabaseConnection.Port         | DatabaseConnection__Port            |
| DatabaseConnection.DatabaseName | DatabaseConnection__DatabaseName    |
| DatabaseConnection.User         | DatabaseConnection__User            |
| DatabaseConnection.Password     | DatabaseConnection__Password        |
| Jwt.Key                         | Jwt__Key                            |
| Jwt.Issuer                      | Jwt__Issuer                         |
| Jwt.Audience                    | Jwt__Audience                       |
| ApiCredentials.ClientId         | ApiCredentials__ClientId            |
| ApiCredentials.ClientSecret     | ApiCredentials__ClientSecret        |
| Webhook.HmacSecret              | Webhook__HmacSecret                 |
| N/A (var. ambiente)             | ASPNETCORE_ENVIRONMENT              |
| N/A (var. ambiente)             | ASPNETCORE_URLS                     |

## Secrets da Infraestrutura (AWS/Terraform)

Os seguintes secrets são utilizados **apenas** na infraestrutura e pipelines de CI/CD:

| Secret | Descrição | Onde é usado |
|--------|-----------|--------------|
| `AWS_ACCESS_KEY_ID` | Chave de acesso da AWS | Pipeline CI/CD |
| `AWS_SECRET_ACCESS_KEY` | Chave secreta da AWS | Pipeline CI/CD |
| `AWS_REGION` | Região da AWS | Pipeline CI/CD |
| `DOCKERHUB_USERNAME` | Username do Docker Hub | Pipeline CI/CD |
| `DOCKERHUB_TOKEN` | Token de acesso ao Docker Hub | Pipeline CI/CD |
| `TF_VAR_BUCKET_NAME` | Nome do bucket S3 | Terraform |
| `TF_VAR_EKS_IAM_USER_NAME` | Nome do usuário IAM | Terraform |
| `TF_VAR_EKS_NODE_SCALING_DESIRED_SIZE` | Tamanho do cluster | Terraform |
| `TF_VAR_EKS_NODE_SCALING_MAX_SIZE` | Tamanho máximo do cluster | Terraform |
| `TF_VAR_EKS_NODE_SCALING_MIN_SIZE` | Tamanho mínimo do cluster | Terraform |
| `TF_VAR_EKS_CLUSTER_NAME` | Nome do cluster EKS | Terraform |

## Observações

Para facilitar a avaliação do projeto, os arquivos contendo secrets (`appsettings.Development.json`, `.env`, `oficina-mecanica-secret.yaml`) estão incluídos no repositório. O objetivo é apenas agilizar a avaliação, e em um cenário real, esses valores não estariam expostos.

Ainda assim, os secrets realmente críticos utilizados pela infraestrutura AWS estão armazenados de forma segura como GitHub Actions Secrets e não estão visíveis no repositório.