# 2. Instalação e uso

## Instalação

### Localmente com Docker

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

### Publicação na AWS com Terraform

Para publicar a infraestrutura na AWS e fazer o deploy da aplicação em um cluster Kubernetes, siga os passos abaixo:

#### Pré-requisitos

1. **AWS CLI instalado e configurado**
   ```bash
   aws --version
   aws configure
   ```

2. **Terraform instalado**
   ```bash
   terraform --version
   ```

3. **kubectl instalado**
   ```bash
   kubectl version --client
   ```

4. **Docker instalado** (para construir e publicar a imagem)
   ```bash
   docker --version
   ```

#### Configuração do Terraform

1. **Preparar o arquivo de backend**

   Edite o arquivo `infra/backend.tf` e substitua o bucket pelo seu:
   ```terraform
   terraform {
     backend "s3" {
       bucket = "SEU-BUCKET-AQUI"  # Substitua pelo seu bucket S3
       key    = "infra/terraform/terraform.tfstate"
       region = "us-east-1"  # Mude se necessário
     }
   }
   ```

2. **Configurar variáveis**

   Crie o arquivo de variáveis a partir do exemplo:
   ```bash
   cd infra
   cp terraform.tfvars.example terraform.tfvars
   ```

   Edite o arquivo `terraform.tfvars` com seus valores:
   ```
   # Variáveis Obrigatórias
   bucket_name = "seu-bucket-s3"
   eks_iam_user_name = "seu-usuario-iam"
   eks_cluster_name = "seu-cluster-eks"
   ```

#### Provisionamento da Infraestrutura

1. **Criar o bucket S3 para o estado do Terraform**
   
   Este passo é necessário antes de inicializar o Terraform:
   ```bash
   aws s3 mb s3://seu-bucket-s3 --region us-east-1
   ```

2. **Inicializar o Terraform**
   ```bash
   terraform init
   ```

3. **Verificar o plano de execução**
   ```bash
   terraform plan
   ```

4. **Aplicar as alterações**
   ```bash
   terraform apply
   ```
   
   Confirme digitando `yes` quando solicitado. Este processo pode demorar cerca de 15-20 minutos para completar.

#### Configuração do Kubernetes

1. **Configurar acesso ao cluster EKS**
   ```bash
   aws eks update-kubeconfig --region us-east-1 --name seu-cluster-eks
   ```

2. **Verificar conexão com o cluster**
   ```bash
   kubectl cluster-info
   kubectl get nodes
   ```

#### Deploy da Aplicação no Kubernetes

1. **Aplicar configurações do Kubernetes**
   
   Com o cluster EKS já configurado, aplique os manifestos Kubernetes:
   ```bash
   kubectl apply -f k8s/oficina-mecanica-config.yaml
   kubectl apply -f k8s/oficina-mecanica-secret.yaml
   kubectl apply -f k8s/postgres/
   kubectl apply -f k8s/api/
   ```

2. **Verificar status do deployment**
   ```bash
   kubectl get pods
   kubectl get services
   ```

3. **Obter o endpoint da aplicação**
   ```bash
   kubectl get service oficina-mecanica-service
   ```
   
   O endereço estará no campo EXTERNAL-IP e poderá demorar alguns minutos para ficar disponível.

#### Limpeza de Recursos

```bash
kubectl delete -f k8s/
terraform destroy
```

## Autenticação

1. **Autenticação comum**
    Obtenha o token de autentição no endpoint `/api/authentication/token`, passando as credenciais:
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