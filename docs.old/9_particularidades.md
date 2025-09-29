# 9. Particularidades

Este documento descreve algumas das implementações e decisões específicas do projeto.

## 1. Tipo CPF ou CNPJ definidos automaticamente

Ao criar um novo Cliente, o sistema identifica automaticamente se o valor fornecido é um CPF ou um CNPJ válido. A validação e a definição do tipo são feitas no Value Object `DocumentoIdentificador`, que é uma propriedade do aggregate `Cliente`.

```csharp
// extraído de src/Domain/Cadastros/ValueObjects/Cliente/DocumentoIdentificador.cs

public DocumentoIdentificador(string documento)
{
    var documentoLimpo = LimparDocumento(documento);
    
    if (ValidarCpf(documentoLimpo))
    {
        _valor = documentoLimpo;
        _tipoDocumento = TipoDocumentoEnum.CPF;
    }
    else if (ValidarCnpj(documentoLimpo))
    {
        _valor = documentoLimpo;
        _tipoDocumento = TipoDocumentoEnum.CNPJ;
    }
    else
    {
        throw new DomainException("Documento de identificação inválido", ErrorType.InvalidInput);
    }
}
```

## 2. Validações de Placa do Veículo

O sistema valida se a placa do veículo atende o padrão antigo do Brasil (`AAA-1234`) ou o novo padrão Mercosul (`AAA1A23`). A validação é realizada no Value Object `Placa`, que é uma propriedade do aggregate `Veiculo`.

```csharp
// extraído de src/Domain/Cadastros/ValueObjects/Veiculo/Placa.cs

public Placa(string placa)
{
    // ...
    if (!Regex.IsMatch(placa, @"^([A-Z]{3}[0-9]{4}|[A-Z]{3}[0-9]{1}[A-Z]{1}[0-9]{2})$", RegexOptions.None, TimeSpan.FromMilliseconds(100)))
        throw new DomainException("Formato de placa inválido", ErrorType.InvalidInput);

    _valor = placa;
}
```

## 3. Código da Ordem de Serviço

O código da Ordem de Serviço é gerado automaticamente com o objetivo de ser um texto único e legível. O formato é `OS-YYYYMMDD-XXXXXX`, onde `YYYYMMDD` é a data de criação e `XXXXXX` são os 6 primeiros caracteres de um UUID aleatório.

```csharp
// extraído de src/Domain/OrdemServico/ValueObjects/OrdemServico/Codigo.cs

public static Codigo GerarNovo()
{
    var data = DateTime.UtcNow.ToString("yyyyMMdd");
    var sufixo = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant(); // 6 primeiros chars
    var codigo = $"OS-{data}-{sufixo}";
    return new Codigo(codigo);
}
```

A chance de repetição de código é mínima, mas ainda assim, a criação da Ordem de Serviço usa uma lógica de repetição para validar se o código já está sendo usado.

```csharp
// extraído de src/Application/OrdemServico/Services/OrdemServicoService.cs

// ...
do
{
    novaOrdemServico = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico.Criar(dto.VeiculoId);
    ordemServicoExistente = await _ordemServicoRepository.ObterPorCodigoAsync(novaOrdemServico.Codigo.Valor);
} while (ordemServicoExistente != null);
// ...
```

## 4. Enums para Clareza e Consistência

Utilizamos `enums` para representar valores bem definidos no domínio, como `TipoDocumentoEnum` e `TipoVeiculoEnum`. Isso garante que apenas valores válidos sejam utilizados na lógica de negócio.

Normalmente, `enums` seriam salvos no banco de dados com seu valor númerico, mas isso gera um acoplamento, pois os valores no banco de dados só tem significado com a aplicação, já que `1` ou `2` não significam nada por si só. Para que o banco de dados seja expressivo, o EF Core é configurado para salvar os `enums` como strings em vez de seus valores numéricos.

```csharp
// extraído de src/Infrastructure/Database/Configurations/ClienteConfiguration.cs

// ...
doc.Property(p => p.TipoDocumento)
   .HasColumnName("tipo_documento_identificador")
   .IsRequired()
   .HasMaxLength(4)
   .HasConversion(
       v => v.ToString().ToLower(),
       v => Enum.Parse<Domain.Cadastros.Enums.TipoDocumentoEnum>(v, true)
   );
// ...
```

Da mesma forma, a API é configurada para aceitar e retornar os `enums` como strings.

```csharp
// extraído de src/OficinaMecanica/Configurations/ControllersConfiguration.cs

// ...
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
// ...
```

## 5. UUID v7 para Chaves Primárias

Todas as entidades do sistema utilizam UUID v7 como chave primária. Esta versão do UUID é sequencial e baseada em timestamp, o que traz benefícios significativos:

*   **Desempenho de Banco de Dados:** Ao contrário de UUIDs v4 (totalmente aleatórios), os UUIDs v7 reservam seus primeiros 48bits para um timestamp, tornando-os naturalmente sequenciais. Isso evita a fragmentação da tabela e índices, mantendo a performance de escrita similar à de um `BIGINT` auto-incrementado.
*   **Geração Descentralizada:** Os IDs podem ser gerados pela aplicação, eliminando a necessidade de uma ida e volta ao banco de dados para obter o ID de uma nova entidade.
*   **Unicidade:** Globalmente único, impedindo erros de consultas ou `JOIN` com tabelas erradas por engano, e previnindo que um atacante possa inferir informações como quantidade de registros.

Utilizamos a biblioteca `UUIDNext` para a geração desses identificadores.

```csharp
// extraído de src/Domain/Cadastros/Aggregates/Cliente.cs

using UUIDNext;

namespace Domain.Cadastros.Aggregates
{
    public class Cliente
    {
        public Guid Id { get; private set; }
        // ...

        public Cliente(Nome nome, DocumentoIdentificador documentoIdentificador)
        {
            Id = Uuid.NewV7();
            // ...
        }
    }
}
```

## 6. Tratamento de Erros e HTTP Status Code

O sistema utiliza uma exception customizada, chamada `DomainException`, para violações de regras de negócio ou problemas de validação.

```csharp
// extraído de src/Shared/Exceptions/DomainException.cs

public class DomainException : Exception
{
    public ErrorType ErrorType { get; }

    public DomainException(string message = "Invalid input", ErrorType errorType = ErrorType.InvalidInput) 
        : base(message)
    {
        ErrorType = errorType;
    }
}
```

A exceção carrega um `ErrorType`, um enum que classifica o erro (ex: `InvalidInput`, `ResourceNotFound`).

```csharp
// extraído de src/Shared/Enums/ErrorType.cs

public enum ErrorType
{
    InvalidInput,
    ResourceNotFound,
    ReferenceNotFound,
    DomainRuleBroken,
    Conflict,
    Unauthorized,
    UnexpectedError
}
```

Na camada de API, um middleware intercepta essas exceções e converte o `ErrorType` no `HttpStatusCode` apropriado.

Fazemos uma distinção importante no tratamento de recursos não encontrados:
*   **`404 Not Found` (`ResourceNotFound`):** Usado quando o recurso principal da rota não é encontrado. Ex: `GET /clientes/{id}` com um ID que não existe.
*   **`422 Unprocessable Entity` (`ReferenceNotFound`):** Usado quando um recurso referenciado no corpo da requisição não é encontrado. Ex: `POST /ordens-servico` referenciando um `veiculoId` que não existe.

```csharp
// extraído de src/OficinaMecanica/Extensions/ErrorTypeExtensions.cs

public static HttpStatusCode ToHttpStatusCode(this ErrorType errorType)
{
    return errorType switch
    {
        ErrorType.InvalidInput => HttpStatusCode.BadRequest,
        ErrorType.ResourceNotFound => HttpStatusCode.NotFound,
        ErrorType.ReferenceNotFound => HttpStatusCode.UnprocessableEntity,
        ErrorType.DomainRuleBroken => HttpStatusCode.UnprocessableEntity,
        ErrorType.Conflict => HttpStatusCode.Conflict,
        ErrorType.Unauthorized => HttpStatusCode.Unauthorized,
        ErrorType.UnexpectedError => HttpStatusCode.InternalServerError,
        _ => HttpStatusCode.InternalServerError
    };
}
```

## 7. Anti-Corruption Layer (ACL)

Utilizamos uma camada Anti-Corruption Layer para a comunicação entre diferentes Bounded Contexts. 

Um ótimo exemplo é a adição de um item de estoque a uma Ordem de Serviço. O `OrdemServicoService` (contexto de Ordem de Serviço) precisa de informações sobre um `ItemEstoque` (contexto de Estoque). Em vez de interagir diretamente com o domínio de Estoque, ele utiliza uma interface externa, `IEstoqueExternalService`.

```csharp
// extraído de src/Application/OrdemServico/Services/OrdemServicoService.cs

public async Task<RetornoOrdemServicoComServicosItensDto> AdicionarItem(Guid ordemServicoId, AdicionarItemDto dto)
{
    var ordemServico = await ObterOrdemServicoPorId(ordemServicoId);

    var itemEstoque = await _estoqueExternalService.ObterItemEstoquePorIdAsync(dto.ItemEstoqueOriginalId);
    if (itemEstoque == null)
        throw new DomainException($"Item de estoque com ID {dto.ItemEstoqueOriginalId} não encontrado.", ErrorType.ReferenceNotFound);

    ordemServico.AdicionarItem(
        itemEstoque.Id,
        itemEstoque.Nome,
        itemEstoque.Preco,
        dto.Quantidade,
        itemEstoque.TipoItemIncluido);

    var result = await _ordemServicoRepository.AtualizarAsync(ordemServico);
    return _mapper.Map<RetornoOrdemServicoComServicosItensDto>(result);
}
```

A implementação dessa interface, `EstoqueExternalService`, reside na camada de infraestrutura e é a responsável por buscar os dados no contexto de Estoque e traduzi-los para o contexto de Ordem de Serviço. O `enum` `TipoItemEstoqueEnum` (do contexto de Estoque) é convertido para `TipoItemIncluidoEnum` (do contexto de Ordem de Serviço).

```csharp
// extraído de src/Infrastructure/AntiCorruptionLayer/OrdemServico/EstoqueExternalService.cs

public class EstoqueExternalService : IEstoqueExternalService
{
    // ...

    public async Task<ItemEstoqueExternalDto?> ObterItemEstoquePorIdAsync(Guid itemId)
    {
        var item = await _itemEstoqueRepository.ObterPorIdAsync(itemId);

        // ...

        return new ItemEstoqueExternalDto
        {
            // ...
            TipoItemIncluido = ConverterTipoItemEstoqueParaTipoItemIncluido(item.TipoItemEstoque.Valor)
        };
    }

    private static TipoItemIncluidoEnum ConverterTipoItemEstoqueParaTipoItemIncluido(TipoItemEstoqueEnum tipoItemEstoque)
    {
        return tipoItemEstoque switch
        {
            TipoItemEstoqueEnum.Peca => TipoItemIncluidoEnum.Peca,
            TipoItemEstoqueEnum.Insumo => TipoItemIncluidoEnum.Insumo,
            _ => throw new DomainException($"Tipo de item de estoque '{tipoItemEstoque}' não é válido.", ErrorType.InvalidInput)
        };
    }
}
```

## 8. Marker Attributes

Na camada de Domain, utilizamos atributos customizados como `[AggregateRoot]`, `[AggregateMember]` e `[ValueObject]`. Eles são Marker Attributes, ou seja, não possuem nenhuma lógica ou comportamento. Seu único propósito é declarar a intenção de uma classe dentro dos padrões do DDD.

```csharp
// extraído de src/Shared/Attributes/MarkerAttributes.cs

namespace Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AggregateRootAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class)]
    public class AggregateMemberAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class)]
    public class ValueObjectAttribute : Attribute { }
}
```

```csharp
// extraído de src/Domain/Cadastros/Aggregates/Cliente.cs

using Shared.Attributes;
// ...

namespace Domain.Cadastros.Aggregates
{
    [AggregateRoot]
    public class Cliente
    {
        // ...
    }
}
```

Essa prática serve como uma forma de documentação viva no código, alinhando a implementação com a linguagem e os conceitos do DDD.

## 9. Uso Exclusivo de Value Objects em Aggregates

Na camada de Domain, todos os Aggregates utilizam exclusivamente Value Objects para suas propriedades (com exceção do Id), evitando o uso de tipos como `string` ou `int`. Essa abordagem garante maior consistência e validação automática dos dados.

Por exemplo, o agregado `Cliente` utiliza os Value Objects `NomeCliente` e `DocumentoIdentificador` para representar suas propriedades de nome e documento, respectivamente. Isso assegura que os dados estejam sempre em um estado válido.

```csharp
// extraído de src/Domain/Cadastros/Aggregates/Cliente.cs

// ...
public class Cliente
{
    public Guid Id { get; private set; }
    public NomeCliente Nome { get; private set; } = null!;
    public DocumentoIdentificador DocumentoIdentificador { get; private set; } = null!;

    // ...
}

//...
public NomeCliente(string nome)
{
    if (string.IsNullOrWhiteSpace(nome))
        throw new DomainException("Nome não pode ser vazio", ErrorType.InvalidInput);

    if (nome.Length > 200)
        throw new DomainException("Nome não pode ter mais de 200 caracteres", ErrorType.InvalidInput);

    _valor = nome;
}
// ...

public DocumentoIdentificador(string documento)
{
    var documentoLimpo = LimparDocumento(documento);
    
    if (ValidarCpf(documentoLimpo))
    {
        _valor = documentoLimpo;
        _tipoDocumento = TipoDocumentoEnum.CPF;
    }
    else if (ValidarCnpj(documentoLimpo))
    {
        _valor = documentoLimpo;
        _tipoDocumento = TipoDocumentoEnum.CNPJ;
    }
    else
    {
        throw new DomainException("Documento de identificação inválido", ErrorType.InvalidInput);
    }
}
// ...
```

Essa abordagem reforça os princípios do DDD, garantindo um domínio rico e expressivo.

---
Anterior: [Arquitetura](8_arquitetura.md)  
Próximo: [API](10_api.md)
