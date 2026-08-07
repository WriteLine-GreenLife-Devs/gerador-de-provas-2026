using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Modulos.ModuloMateria;
using GeradorDeProvas.Dominio.Modulos.ModuloQuestao;

namespace GeradorDeProvas.Testes.Unidade.Modulos.ModuloQuestao;

[TestClass]
public sealed class QuestaoTests
{
    [TestMethod]
    public void Construtor_DeveVincular_CadaAlternativa_AQuestao()
    {
        // Arranjo
        Materia materia = new Materia("Álgebra", 8, new Disciplina("Matemática"));
        List<Alternativa> alternativas = [new Alternativa("3", false), new Alternativa("4", true),];

        // Ação
        Questao questao = new Questao("Quanto é 2 + 2?", materia, alternativas);

        bool alternativasVinculadas = questao.Alternativas
            .All(a => ReferenceEquals(questao, a.Questao));

        // Asserção
        Assert.IsTrue(alternativasVinculadas);
    }

    [TestMethod]
    public void Validar_SemEnunciado_DeveRetornar_ErroCorrespondente()
    {
        // Arranjo
        Materia materia = new Materia("Álgebra", 8, new Disciplina("Matemática"));
        List<Alternativa> alternativas = [new Alternativa("3", false), new Alternativa("4", true),];

        Questao questao = new Questao(string.Empty, materia, alternativas);

        // Ação
        List<string> erros = questao.Validar();

        // Asserção
        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Enunciado\" deve ser preenchido e conter no máximo 2000 caracteres.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_SemMateria_DeveRetornar_ErroCorrespondente()
    {
        // Arranjo
        List<Alternativa> alternativas = [new Alternativa("3", false), new Alternativa("4", true),];
        Questao questao = new Questao("Quanto é 2 + 2?", null!, alternativas);

        // Ação
        List<string> erros = questao.Validar();

        // Asserção
        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Matéria\" deve ser preenchido.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_SemAlternativas_DeveRetornar_ErroCorrespondente()
    {
        // Arranjo
        Materia materia = new Materia("Álgebra", 8, new Disciplina("Matemática"));

        Questao questao = new Questao("Quanto é 2 + 2?", materia, []);

        // Ação
        List<string> erros = questao.Validar();

        // Asserção
        List<string> errosEsperados = [
            "A questão deve possuir no mínimo duas alternativas.",
            "A questão deve possuir uma alternativa correta."
        ];

        Assert.HasCount(2, erros);
        CollectionAssert.AreEqual(
            errosEsperados,
            erros
        );
    }

    [TestMethod]
    public void Validar_ComPoucasAlternativas_DeveRetornar_ErroCorrespondente()
    {
        // Arranjo
        Materia materia = new Materia("Álgebra", 8, new Disciplina("Matemática"));

        List<Alternativa> alternativas = [new Alternativa("4", true)];
        Questao questao = new Questao("Quanto é 2 + 2?", materia, alternativas);

        // Ação
        List<string> erros = questao.Validar();

        // Asserção
        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "A questão deve possuir no mínimo duas alternativas.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_ComMuitasAlternativas_DeveRetornar_ErroCorrespondente()
    {
        // Arranjo
        Materia materia = new Materia("Álgebra", 8, new Disciplina("Matemática"));

        List<Alternativa> alternativas = [
            new Alternativa("4", true),
            new Alternativa("12", false),
            new Alternativa("5", false),
            new Alternativa("7", false),
            new Alternativa("18", false)
        ];

        Questao questao = new Questao("Quanto é 2 + 2?", materia, alternativas);

        // Ação
        List<string> erros = questao.Validar();

        // Asserção
        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "A questão deve possuir no máximo quatro alternativas.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_SemAlternativaCorreta_DeveRetornar_ErroCorrespondente()
    {
        // Arranjo
        Materia materia = new Materia("Álgebra", 8, new Disciplina("Matemática"));

        List<Alternativa> alternativas = [
            new Alternativa("2", false),
            new Alternativa("12", false),
            new Alternativa("5", false),
            new Alternativa("7", false)
        ];

        Questao questao = new Questao("Quanto é 2 + 2?", materia, alternativas);

        // Ação
        List<string> erros = questao.Validar();

        // Asserção
        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "A questão deve possuir uma alternativa correta.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_ComMuitasAlternativasCorretas_DeveRetornar_ErroCorrespondente()
    {
        // Arranjo
        Materia materia = new Materia("Álgebra", 8, new Disciplina("Matemática"));

        List<Alternativa> alternativas = [
            new Alternativa("4", true),
            new Alternativa("12", false),
            new Alternativa("5", false),
            new Alternativa("4", true)
        ];

        Questao questao = new Questao("Quanto é 2 + 2?", materia, alternativas);

        // Ação
        List<string> erros = questao.Validar();

        // Asserção
        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "A questão deve possuir apenas uma alternativa correta.",
            erros.First()
        );
    }
}
