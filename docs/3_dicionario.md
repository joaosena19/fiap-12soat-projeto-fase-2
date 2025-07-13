# 3. Dicionário

Aqui está a aplicação da Linguagem Ubíqua do DDD, separada por subdomínios. 

## Termos

### Domínio: Gestão de Ordem de Serviço
**Administrador**: pessoa que interage com o Sistema e realiza todas as atividades, incluindo Gestão de Ordem de Serviço, Gestão de Cadastros e Gestão de Estoque.  
**Cliente**: pessoa que leva seu Veículo até a oficina para solicitar um ou mais Serviços. Pode acompanhar o andamento da Ordem de Serviço, aprovar o Orçamento e buscar o Veículo quando a Ordem de Serviço for Finalizada.  
**Diagnóstico**: atividade onde o Veículo é inspecionado pelo Mecânico para descobrir possíveis problemas.  
**Execução**: atividade feita pelo Mecânico, é a realização de cada um dos Serviços da Ordem de Serviço.  
**Mecânico**: pessoa que Executa a Ordem de Serviço, não interage diretamente com o Sistema.  
**Orçamento**: registro do valor da Ordem de Serviço, de acordo com os Serviços, Peças e Insumos incluídos nela.  
**Ordem de Serviço**: registro do trabalho a ser realizado em um Veículo, solicitado por um Cliente, executado por um Mecânico, e gerenciado por um Administrador.  
**OS**: mesmo que Ordem de Serviço.  
**Sistema**: software desenvolvido para esta solução, responde a comandos dos Usuários, aplica regras de negócio e realiza ações automáticas.  
**Status**: situação possível para uma Ordem de Serviço, altera de acordo com as ações do Sistema, padronizado em:
  - **Recebida**: primeiro Status, atribuído assim que a Ordem de Serviço é criada.
  - **Em Diagnóstico**: segundo Status, representa que a Ordem de Serviço está sendo diagnosticada por um mecânico.
  - **Aguardando Aprovação**: terceiro Status, representa que o Veículo já foi diagnosticado, o Orçamento já foi gerado, e resta que o Cliente indique se aprova ou não que a Execução se inicie.
  - **Em Execução**: quarto Status, indica que o Cliente aprovou o Orçamento, e o Mecânico está executando a Ordem de Serviço.
  - **Finalizada**: quinto Status, indica que o Mecânico terminou a Execução da Ordem de Serviço.
  - **Entregue**: sexto Status, indica que o Cliente retirou o Veículo após a Ordem de Serviço ter sido Finalizada.
  - **Cancelada**: indica que a Ordem de Serviço teve seu processo parado e não irá mais avançar para outros Status. Normalmente, esse Status é atribuído quando o Cliente não aprova o Orçamento.  
**Usuário**: pessoas que interagem diretamente com o sistema, no caso, o Administrador.  

### Subdomínio: Gestão de Cadastros
**Cliente**: registro que representa a pessoa Cliente.  
**Serviço**: tipos de atividades relacionados ao Veículo, realizadas pelo Mecânico, disponíveis para serem incluídas em uma Ordem de Serviço.  
**Veículo**: registro que representa o Veículo de um Cliente.  

### Subdomínio: Gestão de Estoque
**Estoque**: catálogo que lista os Insumos e Peças cadastrados no Sistema, bem como a sua quantidade disponível.  
**Insumo**: itens do estoque que são consumidos durante a Execução da Ordem de Serviço. Para todos os efeitos práticos, é tratado da mesma forma que Peça.  
**Peça**: itens do estoque que substituem/incrementam componentes do Veículo. Para todos os efeitos práticos, é tratado da mesma forma que Insumo.  
**Peça/Insumo**: termo agrupador para Peças e Insumos, pois são frequentemente usados em conjunto.  

---
Anterior: [Domínio](2_dominio.md)  
Próximo: [Domain Storytelling](4_domain_storytelling.md)
