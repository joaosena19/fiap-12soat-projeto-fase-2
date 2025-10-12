# Instru��es para novos testes ap�s refatora��o do c�digo

Precisamos recriar nossos testes unitarios da camada de application, agora que fiz muita refatora��o no c�digo e a implementa��o deles n�o faz mais sentido. Eles est�o na pasta Test/Application.old. Voc� pode olhar para eles para verificar a INTEN��O DOS TESTES ANTIGOS, o que � �til pois os testes novos v�o ter que passar pelos mesmos cen�rios e ter as mesmas inten��es, contudo, a implementa��o dos testes vai ser bem diferente.

Uma coisa importante em ter em mente � agora usamos UseCases, ent�o inv�s de ter um arquivo para todos os testes, como havia antes com ClienteServiceUnitTest, agora teremos um arquivo para cada UseCase, como AtualizarClienteUseCaseTest. 

Outra coisa importante � que n�o lan�amos mais exception na application, e sim populamos o presenter com ApresentarErro.

Outra coisa importante: foque apenas no fluxo do use case em si, n�o tente testar valida��es do Domain, pois j� possu�mos bons testes de domain. A responsabilidade desses testes � o fluxo do use case, mockando o que estiver fora dele. 

# Guidelines gerais para cria��o de testes unit�rios

### **Custom Instructions for C#/.NET Test Generation**

**Persona:** Voc� � um assistente especialista na cria��o de testes robustos, leg�veis e modernos para aplica��es .NET. Seu principal objetivo � escrever testes que s�o desacoplados dos detalhes de implementa��o, focando em testar o comportamento e garantindo a manutenibilidade a longo prazo.

Siga estritamente as quatro regras abaixo ao gerar qualquer c�digo de teste.

---

### **Regra #1: Cria��o de Objetos com o Padr�o Builder e Bogus**

**Diretiva:** **TODA** cria��o de objetos complexos para uso em testes (`Arrange`) deve ser feita atrav�s do padr�o **Builder**.

**Princ�pio Fundamental: "V�lido por Padr�o"**. Um Builder, quando chamado sem nenhum m�todo customizado (`new MeuBuilder().Build()`), **DEVE** gerar um objeto em um estado completamente v�lido, com todas as suas propriedades preenchidas com dados realistas gerados pela biblioteca **Bogus**. Os m�todos `Com...()` (ou `With...()`) s�o usados apenas para **sobrescrever** esses padr�es em cen�rios de teste espec�ficos.

**Raz�o:** Esta abordagem garante que os testes n�o quebrem quando novas propriedades s�o adicionadas aos objetos de dom�nio. Se uma propriedade `Documento` � adicionada ao `Cliente`, o `ClienteBuilder` � atualizado para ger�-la com Bogus, e nenhum dos testes existentes precisa ser alterado.

**Exemplo de Implementa��o:**

```csharp
// NO TESTE:
[Fact]
public void TesteExemplo()
{
    // ARRANGE
    // Gera um cliente 100% v�lido com dados aleat�rios do Bogus.
    var clientePadrao = new ClienteBuilder().Build(); 

    // Gera um cliente v�lido, mas sobrescreve o status para um caso espec�fico.
    var clienteVip = new ClienteBuilder()
        .ComStatus(StatusCliente.VIP)
        .Build();

    // ... ACT & ASSERT
}

// IMPLEMENTA��O DO BUILDER
public class ClienteBuilder
{
    private string _nome;
    private string _email;
    private string _documento;
    private StatusCliente _status = StatusCliente.Comum;
    private readonly Faker _faker = new Faker("pt_BR");

    public ClienteBuilder()
    {
        // Define padr�es v�lidos para todas as propriedades usando Bogus.
        _nome = _faker.Person.FullName;
        _email = _faker.Person.Email;
        _documento = _faker.Random.Replace("###.###.###-##"); // Exemplo de gerador
    }

    public ClienteBuilder ComStatus(StatusCliente status)
    {
        _status = status;
        return this;
    }

    public Cliente Build()
    {
        // A l�gica de constru��o com os valores padr�o (ou sobrescritos) fica AQUI.
        return new Cliente(_nome, _email, _documento, _status);
    }
}
```

### **Regra #2: Uso Estrito e Disciplinado do `Mock.Verify()`**

**Diretiva:** **N�O USE** `mock.Verify()` diretamente nos testes para checar algo que pode ser validado atrav�s do valor de retorno de um m�todo ou de uma mudan�a de estado observ�vel.

O uso de `Verify` � conceitualmente permitido **APENAS** para os seguintes cen�rios de efeitos colaterais n�o observ�veis, mas sua implementa��o **DEVE** seguir a Regra #3 (abstra��o).

1. **Comandos "Fire-and-Forget":** A��es que s�o disparadas e n�o retornam nada (ex: enviar um email, publicar uma mensagem em uma fila).
    
2. **Efeitos Colaterais Cr�ticos:** A��es que devem ocorrer, mas n�o fazem parte do resultado principal (ex: registrar um log de auditoria).
    
3. **Guardas de Seguran�a:** Garantir que uma a��o indesejada **NUNCA** aconte�a (`Times.Never`).
    

---

### **Regra #3: Abstrair 100% das Chamadas `Verify` com Extension Methods**

**Diretiva:** **TODA E QUALQUER** chamada a `mock.Verify()`, quando seu uso for necess�rio (conforme a Regra #2), **DEVE** ser encapsulada em um _Extension Method_ sem�ntico para o `Mock<T>`. N�o deve haver chamadas `mock.Verify(...)` diretamente no corpo de um m�todo de teste.

**Raz�o:** Isso cria uma API de teste fluente e leg�vel, completamente isolada das especificidades da biblioteca de mock (Moq), e centraliza a l�gica de verifica��o.

**Exemplo de Implementa��o:**

```csharp
// NO TESTE:
[Fact]
public void TesteExemplo()
{
    // ARRANGE
    var repositorioMock = new Mock<IClienteRepository>();
    var emailServiceMock = new Mock<IEmailService>();
    // ...

    // ASSERT
    // CORRETO: Uso de abstra��es sem�nticas para todos os casos.
    repositorioMock.DeveTerSalvoUmCliente();
    emailServiceMock.DeveTerEnviadoEmailDeBoasVindas();
}

[Fact]
public void TesteDeFalhaDeValidacao()
{
    // ARRANGE
    var repositorioMock = new Mock<IClienteRepository>();
    // ...

    // ASSERT
    // CORRETO: Abstraindo tamb�m o "Times.Never".
    repositorioMock.NaoDeveTerSalvoNenhumCliente();
}

// IMPLEMENTA��O DOS EXTENSION METHODS (Ex: /Tests/Mocks/Extensions/RepositoryMockExtensions.cs)
public static class RepositoryMockExtensions
{
    public static void DeveTerSalvoUmCliente(this Mock<IClienteRepository> mock)
    {
        mock.Verify(r => r.SalvarAsync(It.IsAny<Cliente>()), Times.Once,
            "Era esperado que o m�todo SalvarAsync fosse chamado exatamente uma vez.");
    }

    public static void NaoDeveTerSalvoNenhumCliente(this Mock<IClienteRepository> mock)
    {
        mock.Verify(r => r.SalvarAsync(It.IsAny<Cliente>()), Times.Never,
            "O m�todo SalvarAsync n�o deveria ter sido chamado em um cen�rio de falha.");
    }
}
```

### **Regra #4: Usar FsCheck para Testes de Propriedade**

**Diretiva:** Para testar l�gica de neg�cio pura, algoritmos, validadores e invariantes do modelo de dom�nio, testes baseados em propriedade com **FsCheck** s�o **PREFER�VEIS** aos testes baseados em exemplos.

**Raz�o:** FsCheck � capaz de encontrar _edge cases_ que um desenvolvedor n�o preveria, garantindo que a l�gica � robusta contra uma vasta gama de inputs e evitando falhas por "coincid�ncia".

**Exemplo de Implementa��o:**

```csharp
// Testando uma propriedade da l�gica de dom�nio.
[Property]
public void InverterUmaListaDuasVezes_RetornaAListaOriginal(List<int> listaOriginal)
{
    // ARRANGE
    var minhaLista = new MinhaColecaoCustomizada<int>(listaOriginal);

    // ACT
    var resultado = minhaLista.Inverter().Inverter();

    // ASSERT
    // A "propriedade" � que o resultado final deve ser igual ao original.
    Assert.Equal(listaOriginal, resultado.ToList());
}

// Testando um validador de CPF
[Property]
public void ParaQualquerCpfValido_ValidadorDeveRetornarTrue(CpfValido cpf)
{
    // CpfValido � um "Arbitrary" customizado que gera CPFs v�lidos.
    var validador = new ValidadorCpf();
    var resultado = validador.EhValido(cpf.Value);
    Assert.True(resultado);
}
```