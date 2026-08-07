using System.Text.RegularExpressions;
using GeradorDeProvas.Testes.E2E.Compartilhado;

namespace GeradorDeProvas.Testes.E2E.Modulos.ModuloDisciplina;

[TestClass]
public sealed class DisciplinaE2ETests : E2ETestsBase
{
    [TestMethod]
    public async Task DeveExibir_ListagemVazia_ParaUsuario_SemDisciplinas()
    {
        // Arrange
        await RegistrarEEntrarAsync("disciplina.listagem@teste.local", "Senha123!");
        DisciplinaListarPage listarPage = new(Page, UrlBase);

        // Act
        await listarPage.IrParaAsync();

        // Assert
        await Expect(Page).ToHaveURLAsync(listarPage.Url);
        await Expect(listarPage.Titulo).ToBeVisibleAsync();
        await Expect(listarPage.CadastarNova).ToBeVisibleAsync();
        await Expect(listarPage.EstadoVazio).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task DeveCadastrar_Disciplina_ComDadosValidos()
    {
        // Arrange
        await RegistrarEEntrarAsync("disciplina.cadastro@teste.local", "Senha123!");

        DisciplinaFormPage formPage = new(Page, UrlBase);
        DisciplinaListarPage listarPage = new(Page, UrlBase);

        // Act
        await formPage.IrParaCadastroAsync();
        await formPage.PreencherNomeAsync("Matemática");
        await formPage.ConfirmarAsync();

        // Assert
        await Expect(Page).ToHaveURLAsync(listarPage.Url);
        await Expect(listarPage.NomeDaDisciplina("Matemática")).ToBeVisibleAsync();
        await Expect(listarPage.EstadoVazio).Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task DeveEditar_Disciplina_ComDadosValidos()
    {
        // Arrange
        await RegistrarEEntrarAsync("disciplina.edicao@teste.local", "Senha123!");
        await CadastrarDisciplinaAsync("Matemática");

        DisciplinaFormPage formPage = new(Page, UrlBase);
        DisciplinaListarPage listarPage = new(Page, UrlBase);

        await listarPage.EditarAsync("Matemática");

        // Act
        await formPage.PreencherNomeAsync("História do Brasil");
        await formPage.ConfirmarAsync();

        // Assert
        await Expect(Page).ToHaveURLAsync(listarPage.Url);
        await Expect(listarPage.NomeDaDisciplina("História do Brasil")).ToBeVisibleAsync();
        await Expect(listarPage.NomeDaDisciplina("Matemática")).Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task DeveExcluir_Disciplina_SemVinculos()
    {
        // Arrange
        await RegistrarEEntrarAsync("disciplina.exclusao@teste.local", "Senha123!");
        await CadastrarDisciplinaAsync("Matemática");

        DisciplinaListarPage listarPage = new(Page, UrlBase);
        DisciplinaExcluirPage excluirPage = new(Page);

        await listarPage.ExcluirAsync("Matemática");

        // Act
        await Expect(Page).ToHaveURLAsync(
            new Regex($"{Regex.Escape(UrlBase)}/Disciplina/Excluir/.*")
        );

        await Expect(excluirPage.MensagemConfirmacao).ToBeVisibleAsync();
        await excluirPage.ConfirmarAsync();

        // Assert
        await Expect(Page).ToHaveURLAsync(listarPage.Url);
        await Expect(listarPage.NomeDaDisciplina("Matemática")).Not.ToBeVisibleAsync();
        await Expect(listarPage.EstadoVazio).ToBeVisibleAsync();
    }

    private async Task CadastrarDisciplinaAsync(string nome)
    {
        DisciplinaFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherNomeAsync(nome);
        await formPage.ConfirmarAsync();

        DisciplinaListarPage listarPage = new(Page, UrlBase);

        await Expect(Page).ToHaveURLAsync(listarPage.Url);
    }
}
