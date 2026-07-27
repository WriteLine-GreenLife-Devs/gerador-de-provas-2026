using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Modulos.ModuloMateria;
using GeradorDeProvas.Dominio.Modulos.ModuloProva;
using GeradorDeProvas.Dominio.Modulos.ModuloQuestao;

namespace GeradorDeProvas.Testes.Unidade.Modulos.ModuloProva;

[TestClass]
public sealed class ProvaTests
{
    [TestMethod]
    public void Validar_SemTitulo_DeveRetornar_ErroCorrespondente()
    {
        // Arranjo
        Disciplina disciplina = new Disciplina("Matemática");
        Materia materia = new Materia("Álgebra", 8, disciplina);

        Prova prova = new Prova(string.Empty, disciplina, materia, 8, 1, false);

        // Ação
        List<string> erros = prova.Validar();

        // Asserção
        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Título\" deve ser conter entre 2 e 100 caracteres.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_SemDisciplina_DeveRetornar_ErroCorrespondente()
    {
        // Arranjo
        Materia materia = new Materia("Álgebra", 8, null!);

        Prova prova = new Prova("Prova de Álgebra 8a Serie", null!, materia, 8, 1, false);

        // Ação
        List<string> erros = prova.Validar();

        // Asserção
        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Disciplina\" deve ser preenchido.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_ComSerieZeroOuAbaixo_DeveRetornar_ErroCorrespondente()
    {
        // Arranjo
        Disciplina disciplina = new Disciplina("Matemática");
        Materia materia = new Materia("Álgebra", 0, disciplina);

        Prova prova = new Prova("Prova de Álgebra", disciplina, materia, 0, 1, false);

        // Ação
        List<string> erros = prova.Validar();

        // Asserção
        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Série\" deve ser maior que zero.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_ComSerieEMateria_Diferentes_DeveRetornar_ErroCorrespondente()
    {
        // Arranjo
        Disciplina disciplina = new Disciplina("Matemática");
        Materia materia = new Materia("Álgebra", 8, disciplina);

        Prova prova = new Prova("Prova de Álgebra", disciplina, materia, 5, 1, false);

        // Ação
        List<string> erros = prova.Validar();

        // Asserção
        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Série\" precisa alinhar com a série da \"Matéria\".",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_RecuperacaoComMateria_DeveRetornar_ErroCorrespondente()
    {
        // Arranjo
        Disciplina disciplina = new Disciplina("Matemática");
        Materia materia = new Materia("Álgebra", 8, disciplina);

        Prova prova = new Prova("Prova de Álgebra", disciplina, materia, 8, 1, true);

        // Ação
        List<string> erros = prova.Validar();

        // Asserção
        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Matéria\" não pode ser prenchido em uma prova de recuperação.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_QuantidadeQuestoesAbaixoDeUm_DeveRetornar_ErroCorrespondente()
    {
        // Arranjo
        Disciplina disciplina = new Disciplina("Matemática");
        Materia materia = new Materia("Álgebra", 8, disciplina);

        Prova prova = new Prova("Prova de Álgebra", disciplina, materia, 8, 0, false);

        // Ação
        List<string> erros = prova.Validar();

        // Asserção
        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Quantidade de Questões\" não pode ser zero ou negativo.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_MateriaFora_DaDisciplina_DeveRetornar_ErroCorrespondente()
    {
        // Arranjo
        Disciplina disciplina = new Disciplina("Matemática");
        Materia materia = new Materia("Álgebra", 8, disciplina);

        Disciplina disciplina2 = new Disciplina("Geografia");
        Materia materia2 = new Materia("Relevo", 8, disciplina2);

        Prova prova = new Prova("Prova de Álgebra", disciplina, materia2, 8, 3, false);

        // Ação
        List<string> erros = prova.Validar();

        // Asserção
        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O valor do campo \"Matéria\" deve pertencer à \"Disciplina\" selecionada.",
            erros.First()
        );
    }

    [TestMethod]
    public void Atualizar_AlteraConfiguracaoELimpaQuestoes()
    {
        // Arranjo
        Disciplina disciplina = new Disciplina("Matemática");
        Materia materia = new Materia("Álgebra", 8, disciplina);

        Prova prova = new Prova("Prova de Álgebra", disciplina, materia, 8, 1, false);

        Disciplina disciplina2 = new Disciplina("Geografia");

        // Ação
        prova.Atualizar(new Prova("Prova de Geografia", disciplina2, null, 6, 3, true));

        // Asserção
        Assert.AreEqual("Prova de Geografia", prova.Titulo);
        Assert.AreEqual(6, prova.Serie);
        Assert.AreEqual(3, prova.QuantidadeQuestoes);
        Assert.IsTrue(prova.ProvaRecuperacao);
        Assert.IsNull(prova.Materia);
        Assert.HasCount(0, prova.Questoes);
    }

    [TestMethod]
    public void SortearQuestoes_DeveSelecionar_QuantidadeInformada_SemRepetir()
    {
        // Arranjo
        Disciplina disciplina = new Disciplina("Matemática");
        Materia materia = new Materia("Álgebra", 8, disciplina);

        Prova prova = new Prova("Prova de Álgebra", disciplina, materia, 8, 5, false);

        List<Questao> questoesDisponiveis = Enumerable.Range(1, 5)
            .Select(indice => new Questao($"Questão {indice}", materia, []))
            .ToList();

        // Ação
        List<string> erros = prova.SortearQuestoes(questoesDisponiveis, new Random(70));

        // Asserção
        Assert.IsEmpty(erros);
        Assert.HasCount(5, prova.Questoes);
        Assert.HasCount(5, prova.Questoes.Select(q => q.Id).Distinct());
    }
}
