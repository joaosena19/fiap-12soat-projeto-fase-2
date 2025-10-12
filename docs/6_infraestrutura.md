# 6. Infraestrutura

## Terraform com AWS

**Pasta**: `infra/`

### Componentes principais

- **Provider**: AWS (v6.0), região us-east-1
- **Backend**: Estado armazenado em bucket S3
- **Rede**: VPC (CIDR 10.0.0.0/16), subnets públicas em 3 AZs, Internet Gateway
- **EKS**: Cluster v1.33 com node group t3.small (min: 1, max: 3, desejado: 2)
- **IAM**: Roles para cluster EKS e nós de trabalho, políticas de acesso
- **TFSTATE**: Bucket S3

## Kubernetes

**Pasta:** `k8s/`

### API
- **Deployment**:
  - Réplicas iniciais: 1
  - Recursos: 100m-250m CPU, 128Mi-256Mi memória
  - Health Probes: liveness, readiness, startup

- **Service**: LoadBalancer (porta externa 80 → interna 8080)

- **HPA**:
  - Réplicas: min 1, max 5
  - Gatilho: 70% CPU

### PostgreSQL
- **Deployment**:
  - Réplicas: 1
  - Recursos: 100m-300m CPU, 256Mi-384Mi memória
  - Health: Probe com `pg_isready`

- **Volume**: 1Gi
> Utilizei **emptyDir** para o volume devido à limitações do Free Tier da AWS. Em um cenário real, invés de emptyDir seria usado um banco persistido.

- **Service**: ClusterIP

### Configurações

- **ConfigMap**: Parâmetros de aplicação não-sensíveis
- **Secrets**: Parâmetros de aplicação sensíveis

---
Anterior: [Endpoints](5_endpoints.md)  
Próximo: [CI/CD](7_ci_cd.md)