# Deploy automático no Azure

O arquivo [`.github/examples/deploy-azure.yml`](../.github/examples/deploy-azure.yml) contém um exemplo completo de integração contínua e deploy do Gerador de Provas. Ele está fora de `.github/workflows` para servir apenas como referência e, nessa localização, não é executado pelo GitHub Actions.

## O que o workflow faz

Quando ativado, o workflow executa as etapas abaixo em ordem:

1. Restaura e compila a solução em `Release`.
2. Instala o Chromium utilizado pelo Playwright.
3. Executa testes unitários, de integração e E2E.
4. Publica a aplicação e gera um artefato.
5. Aplica as migrations do Entity Framework no Azure SQL.
6. Autentica no Azure utilizando OIDC.
7. Configura as variáveis da aplicação no Azure App Service.
8. Publica o artefato no slot `Production`.

As etapas `migrate` e `deploy` dependem da conclusão bem-sucedida de `build-and-test`. Portanto, uma falha na compilação ou nos testes impede a alteração do banco e a publicação.

## Secrets necessários

Os valores sensíveis devem ser cadastrados em **Settings → Secrets and variables → Actions → Secrets** no repositório do GitHub.

| Secret | Finalidade |
| --- | --- |
| `AZURE_CLIENT_ID` | Identificador da aplicação ou identidade utilizada pelo GitHub Actions. |
| `AZURE_TENANT_ID` | Identificador do tenant do Microsoft Entra ID. |
| `AZURE_SUBSCRIPTION_ID` | Identificador da assinatura do Azure. |
| `AZURESQL_CONNECTION_STRING` | Connection string do banco Azure SQL. |
| `NEWRELIC_LICENSE_KEY` | Chave de licença do New Relic utilizada pela aplicação. |
| `AUTOMAPPER_LICENSE_KEY` | Chave de licença do AutoMapper utilizada pela aplicação. |

Os três primeiros nomes são genéricos e substituem os nomes extensos gerados automaticamente pelo Azure. O conteúdo dos secrets não deve ser adicionado ao código, ao README ou ao histórico do Git.

## Variables necessárias

Os valores não sensíveis devem ser cadastrados em **Settings → Secrets and variables → Actions → Variables**.

| Variable | Exemplo | Finalidade |
| --- | --- | --- |
| `AZURE_RESOURCE_GROUP` | `gerador-de-provas` | Resource group do App Service. |
| `AZURE_WEBAPP_NAME` | `gerador-de-provas-webapp` | Nome do Azure Web App. |

## Configuração do acesso OIDC

O exemplo utiliza OpenID Connect e não depende de um client secret permanente.

1. Crie ou selecione uma aplicação no Microsoft Entra ID.
2. Conceda à identidade acesso somente aos recursos necessários, normalmente no resource group da aplicação.
3. Adicione uma credencial federada para GitHub Actions.
4. Configure a organização, o repositório e a branch `main` na credencial federada.
5. Cadastre no GitHub os valores `AZURE_CLIENT_ID`, `AZURE_TENANT_ID` e `AZURE_SUBSCRIPTION_ID`.

O job `deploy` declara `id-token: write`, permissão necessária para o login com OIDC.

## Preparação do Azure SQL

A connection string deve utilizar um usuário com permissão para aplicar as migrations necessárias. O Azure SQL também deve aceitar a conexão do runner do GitHub Actions. A regra de firewall deve ser definida conforme a política do ambiente; não é recomendado armazenar usuário ou senha diretamente no workflow.

Antes de ativar o deploy automático, valide as migrations localmente em um banco de desenvolvimento e confira se não há operação destrutiva inesperada.

## Como ativar

Somente faça esta etapa quando os recursos do Azure, secrets e variables estiverem configurados.

1. Copie `.github/examples/deploy-azure.yml` para `.github/workflows/deploy-azure.yml`.
2. Revise os caminhos dos projetos e o nome do slot.
3. Valide o YAML e execute inicialmente pelo botão **Run workflow**, fornecido por `workflow_dispatch`.
4. Confira os jobs `build-and-test`, `migrate` e `deploy` no GitHub Actions.
5. Depois da validação manual, descomente o gatilho de `push` na `main` somente se o deploy automático fizer parte do processo do projeto.

O exemplo vem configurado apenas com o gatilho manual. Assim, adicionar o arquivo em `.github/workflows` não inicia uma publicação automaticamente no mesmo `push` que introduz o workflow.

Enquanto o arquivo permanecer em `.github/examples`, nenhum push ou execução manual iniciará esse workflow.

## Como desativar

Para interromper novos deploys automáticos, retire o arquivo de `.github/workflows` e mantenha a versão de referência em `.github/examples`. Uma execução que já esteja em andamento deve ser cancelada separadamente na interface do GitHub Actions.

## Cuidados antes da primeira execução

- Confirme que todos os testes passam em `Release`.
- Confirme que o artifact contém somente a publicação da aplicação web.
- Verifique os nomes do App Service, resource group e slot.
- Verifique as permissões mínimas da identidade do Azure.
- Confirme que a connection string aponta para o banco correto.
- Nunca use secrets reais em arquivos versionados.
- Faça a primeira execução manualmente e acompanhe cada job.
