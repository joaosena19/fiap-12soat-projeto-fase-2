# 3. Clean Architecture
## Camadas

### Entities / Enterprise Business Rules

É o projeto Domain como um todo, onde estão as entidades e as regras de negócio. Ele praticamente não teve alteração em relação à primeira fase. 

![Estrutura do projeto Domain](attachments/domain.png)

### Use Cases / Application Business Rules

Vivem no projeto Application. Na fase 1, havia um Service para cada entidade, com todos os métodos de casos de uso. Agora, Cada caso de uso é uma classe.

![Estrutura do projeto Application](attachments/application.png)

### Interface Adapters

A interface dos Presenters e Gateways vive no projeto de Application.

![Contratos do projeto Application](attachments/application_contracts.png)

A implementação dos Presenters vive no projeto de API, que é o único chamador externo do sistema no momento.

![Implementação dos Presenters](attachments/presenters.png)

A implementação dos Gateways é através de Repositories, que ficam no projeto de Infrastructure. Os Controllers da Clean Architecture nomeei de "Handlers", para não ter confusão com os Controllers nativos do .NET para definição de endpoints. Eles não tem interface, e vivem também no projeto de Infrastructure.

![Handlers e Repositories](attachments/handlers_repositories.png)

### Frameworks & Drivers

A camada mais externa da Clean Architecture. São os Endpoints da API, que ficam no projeto de API, e os Repositories, no projeto de Infrastructure.

![Endpoints da API](attachments/endpoints.png)
![Repositories de Infrastructure](attachments/repositories.png)

## Inversão de dependência

Agora é feita diretamente, sem uso de Container de DI. Cada camada instância os componentes necessário e fornece para as camada inferiores, que dependem de Interfaces apenas.

Exemplo de endpoint (camada 4), criando as dependências necessárias e passando para o handler (Controller da Clean Architecture, camada 3).

![Exemplo de endpoint GetById](attachments/get_by_id_endpoint.png)

## Fluxo completo

O chamador externo (que no caso hoje temos apenas a API, mas poderia ter outros como uma CLI, desktop etc) define como quer sua apresentação, atendendo a interface `I[UseCase]Presenter`, e também instancia o Gateway, que é definido no projeto de Infrastructure (no momento, sendo um repositório de acesso à banco de dados, mas poderia ser uma outra API externa, I/O etc.). 

Definição da API sobre o Presenter
![Definição do Presenter para criar cliente](attachments/api_presenter_criar_cliente.png)

O chamador externo chama o Controller da CA (que nomeamos como Handler para evitar confusões com o Controller de API do .NET), passando o Gateway e o Presenter.

![Endpoint POST para criar cliente](attachments/api_post_criar_cliente.png)

O Handler tem a responsabilidade de chamar o UseCase, pegando os dados que recebeu do chamador externo e tratando de uma forma que o UseCase entenda. No momento, não há necessidade de converter os dados pois o projeto é pequeno e sempre fala a mesma língua, mas quando houver necessidade, será aí.

![Handler para criar cliente](attachments/handler_criar_cliente.png)

O UseCase, recebendo o Gateway e o Presenter, tem a responsabilidade de chamar a Entidade, mandar a Entidade realizar a ação necessária, atualizar os dados no Gateway que foi recebido, e popular o Presenter com o resultado. O UseCase nunca retorna nada, ele apenas popula o Presenter. 

![UseCase para criar cliente](attachments/usecase_criar_cliente.png)

A Entidade realiza a ação, garantindo um estado válido através de suas regras de negócio

![Entidade Cliente - método criar](attachments/entidade_cliente_criar.png)

O Presenter populado então é retornado camada por camada até voltar para o chamador externo, este que tem definido como quer que seja a apresentação.

![Retorno do endpoint criar cliente](attachments/api_post_criar_cliente_retorno.png)

---
Anterior: [Instalação e Uso](2_instalacao_uso.md)  
Próximo: [Arquitetura](4_arquitetura.md)
