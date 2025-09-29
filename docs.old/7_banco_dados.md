# 7. Banco de Dados

## Tecnologia
- **Sistema**: PostgreSQL 16
- **ORM**: Entity Framework Core 9.0.7

O motivo da escolha do banco PostgreSQL é por ser um banco relacional, que se encaixa bem com esta aplicação, já que temos diversos relacionamentos entre Cliente, Veículo, Ordem de Serviço etc. e especificamente o Postgre pois é gratuito e facilmente configurável via Docker.

Foi adotada uma abordagem code-first, mapeando as entidades e delegando para o Entity Framework Core a criação das tabelas, definição de campos e relacionamentos.

## Entidades

### Módulo Cadastros
- **Cliente**: Informações dos clientes
- **Servico**: Catálogo de serviços oferecidos
- **Veiculo**: Dados dos veículos dos clientes

### Módulo Estoque
- **ItemEstoque**: Controle de peças e insumos

### Módulo Ordem de Serviço
- **OrdemServico**: Ordens de serviço
- **ServicoIncluido**: Serviços incluídos em uma OS
- **ItemIncluido**: Itens de estoque utilizados em uma OS
- **Orcamento**: Orçamentos das ordens de serviço

## Configurações
- **EF Migrations**: Controle de versionamento do schema
- **SeedData**: Dados iniciais para desenvolvimento

## Esquema do banco de dados

![Esquema de banco de dados](attachments/oficina_mecanica_db.png)

---
Anterior: [Tecnologias Utilizadas](6_tecnologias.md)  
Próximo: [Arquitetura](8_arquitetura.md)
