using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;

namespace GeradorDeProvas.Testes.Unidade.Modulos.ModuloDisciplina;

[TestClass]
public sealed class DisciplinaTests
{
    #region Testes da Validação de Disciplina

    [TestMethod]
    public void Validar_ComNomeVazio_DeveRetornarErro()
    {
        // Arranjo [Configura os dados do teste]
        Disciplina disciplina = new Disciplina(string.Empty);

        // Ação [Executa a ação sob teste]
        List<string> erros = disciplina.Validar();

        // Asserção [Checa o resultado comparando com o esperado]
        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Nome\" deve ser preenchido.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_ComNomeCurto_DeveRetornarErro()
    {
        // Arranjo [Configura os dados do teste]
        Disciplina disciplina = new Disciplina(new string('A', 1));

        // Ação [Executa a ação sob teste]
        List<string> erros = disciplina.Validar();

        // Asserção [Checa o resultado comparando com o esperado]
        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Nome\" deve conter no mínimo 2 caracteres.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_ComNomeLongo_DeveRetornarErro()
    {
        // Arranjo [Configura os dados do teste]
        Disciplina disciplina = new Disciplina(new string('A', 101));

        // Ação [Executa a ação sob teste]
        List<string> erros = disciplina.Validar();

        // Asserção [Checa o resultado comparando com o esperado]
        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Nome\" deve conter no máximo 100 caracteres.",
            erros.First()
        );
    }
    #endregion
}
