using FluentResults;
using GeradorDeProvas.Aplicacao.Modulos.ModuloProva;
using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Modulos.ModuloMateria;
using GeradorDeProvas.Dominio.Modulos.ModuloProva;
using GeradorDeProvas.Dominio.Modulos.ModuloQuestao;
using Moq;

namespace GeradorDeProvas.Testes.Unidade.Modulos.ModuloProva;

[TestClass]
public sealed class ServicoProvaTests
{
    [TestMethod]
    public void Cadastrar_ConfiguracaoValida_CadastraProvaComQuestoesSelecionadas()
    {
        // Arrange
        Disciplina disciplina = new("Matemática");
        Materia materia = new("Álgebra", 7, disciplina);

        Questao primeiraQuestao = CriarQuestao(materia, "Quanto é 2 + 2?");
        Questao segundaQuestao = CriarQuestao(materia, "Quanto é 3 + 3?");

        Mock<IRepositorioProva> repositorioProva = new();
        Mock<IRepositorioDisciplina> repositorioDisciplina = new();
        Mock<IRepositorioMateria> repositorioMateria = new();
        Mock<IRepositorioQuestao> repositorioQuestao = new();

        repositorioProva.Setup(r => r.SelecionarTodos()).Returns([]);

        repositorioDisciplina.Setup(r => r.SelecionarPorId(disciplina.Id)).Returns(disciplina);

        repositorioMateria.Setup(r => r.SelecionarPorId(materia.Id)).Returns(materia);
        repositorioMateria.Setup(r => r.SelecionarTodos()).Returns([materia]);

        repositorioQuestao.Setup(r => r.SelecionarTodos()).Returns([primeiraQuestao, segundaQuestao]);

        Prova? provaCadastrada = null;

        repositorioProva
            .Setup(r => r.Cadastrar(It.IsAny<Prova>()))
            .Callback<Prova>(prova => provaCadastrada = prova);

        ServicoProva servico = new(
            repositorioProva.Object,
            repositorioDisciplina.Object,
            repositorioMateria.Object,
            repositorioQuestao.Object
        );

        // Act
        CadastrarProvaDto dto = new("Avaliação", disciplina.Id, materia.Id, 7, 2, false);

        Result resultado = servico.Cadastrar(dto, [primeiraQuestao.Id, segundaQuestao.Id]);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(provaCadastrada);
        Assert.AreEqual("Avaliação", provaCadastrada.Titulo);
        Assert.AreSame(disciplina, provaCadastrada.Disciplina);
        Assert.AreSame(materia, provaCadastrada.Materia);
        Assert.HasCount(2, provaCadastrada.Questoes);

        Assert.AreEqual(primeiraQuestao.Id, provaCadastrada.Questoes[0].Id);
        Assert.AreEqual(segundaQuestao.Id, provaCadastrada.Questoes[1].Id);

        repositorioProva.Verify(r => r.Cadastrar(It.IsAny<Prova>()), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_TituloDuplicado_RetornaFalha()
    {
        // Arrange
        Disciplina disciplina = new("Matemática");

        Mock<IRepositorioProva> repositorioProva = new();
        Mock<IRepositorioDisciplina> repositorioDisciplina = new();
        Mock<IRepositorioMateria> repositorioMateria = new();
        Mock<IRepositorioQuestao> repositorioQuestao = new();

        repositorioProva.Setup(r => r.SelecionarTodos()).Returns([
            new Prova("Avaliação", disciplina, null, 7, 1, true)
        ]);

        ServicoProva servico = new(
            repositorioProva.Object,
            repositorioDisciplina.Object,
            repositorioMateria.Object,
            repositorioQuestao.Object
        );

        // Act
        Result resultado = servico.Cadastrar(
            new CadastrarProvaDto(" avaliação ", disciplina.Id, null, 7, 1, true)
        );

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Titulo", resultado.Errors.Single().Metadata["Campo"]);
        repositorioProva.Verify(r => r.Cadastrar(It.IsAny<Prova>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_MateriaDeOutraDisciplina_RetornaFalha()
    {
        // Arrange
        Disciplina disciplina = new("Matemática");
        Disciplina outraDisciplina = new("História");

        Materia materia = new("Álgebra", 7, outraDisciplina);

        Mock<IRepositorioProva> repositorioProva = new();
        Mock<IRepositorioDisciplina> repositorioDisciplina = new();
        Mock<IRepositorioMateria> repositorioMateria = new();
        Mock<IRepositorioQuestao> repositorioQuestao = new();

        repositorioProva.Setup(r => r.SelecionarTodos()).Returns([]);

        repositorioDisciplina.Setup(r => r.SelecionarPorId(disciplina.Id)).Returns(disciplina);
        repositorioMateria.Setup(r => r.SelecionarPorId(materia.Id)).Returns(materia);

        ServicoProva servico = new(
            repositorioProva.Object,
            repositorioDisciplina.Object,
            repositorioMateria.Object,
            repositorioQuestao.Object
        );

        // Act
        Result resultado = servico.Cadastrar(
            new CadastrarProvaDto("Avaliação", disciplina.Id, materia.Id, 7, 1, false)
        );

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.Contains("não pertence à disciplina", resultado.Errors.Single().Message);
        repositorioProva.Verify(r => r.Cadastrar(It.IsAny<Prova>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_QuestoesForaDaConfiguracao_RetornaFalha()
    {
        // Arrange
        Disciplina disciplina = new("Matemática");

        Materia materia = new("Álgebra", 7, disciplina);
        Materia outraMateria = new("Geometria", 8, disciplina);

        Questao questaoDeOutraMateria = CriarQuestao(outraMateria, "Questão fora da matéria");

        Mock<IRepositorioProva> repositorioProva = new();
        Mock<IRepositorioDisciplina> repositorioDisciplina = new();
        Mock<IRepositorioMateria> repositorioMateria = new();
        Mock<IRepositorioQuestao> repositorioQuestao = new();

        ServicoProva servico = new(
            repositorioProva.Object,
            repositorioDisciplina.Object,
            repositorioMateria.Object,
            repositorioQuestao.Object
        );

        repositorioProva.Setup(r => r.SelecionarTodos()).Returns([]);

        repositorioDisciplina.Setup(r => r.SelecionarPorId(disciplina.Id)).Returns(disciplina);

        repositorioMateria.Setup(r => r.SelecionarPorId(materia.Id)).Returns(materia);
        repositorioMateria.Setup(r => r.SelecionarTodos()).Returns([materia]);

        repositorioQuestao.Setup(r => r.SelecionarTodos()).Returns([questaoDeOutraMateria]);

        // Act
        Result resultado = servico.Cadastrar(
            new CadastrarProvaDto("Avaliação", disciplina.Id, materia.Id, 7, 1, false),
            [questaoDeOutraMateria.Id]
        );

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.Contains("não pertencem à configuração", resultado.Errors.Single().Message);
        repositorioProva.Verify(r => r.Cadastrar(It.IsAny<Prova>()), Times.Never);
    }

    [TestMethod]
    public void Duplicar_ProvaExistente_CadastraCopiaComNovoTitulo()
    {
        // Arrange
        Disciplina disciplina = new("Matemática");
        Materia materia = new("Álgebra", 7, disciplina);

        Prova provaOriginal = new("Avaliação original", disciplina, materia, 7, 2, false);

        Mock<IRepositorioProva> repositorioProva = new();
        Mock<IRepositorioDisciplina> repositorioDisciplina = new();
        Mock<IRepositorioMateria> repositorioMateria = new();
        Mock<IRepositorioQuestao> repositorioQuestao = new();

        repositorioProva.Setup(r => r.SelecionarTodos()).Returns([]);
        repositorioProva.Setup(r => r.SelecionarPorId(provaOriginal.Id)).Returns(provaOriginal);

        Prova? provaDuplicada = null;

        repositorioProva
            .Setup(r => r.Cadastrar(It.IsAny<Prova>()))
            .Callback<Prova>(prova => provaDuplicada = prova);

        ServicoProva servico = new(
            repositorioProva.Object,
            repositorioDisciplina.Object,
            repositorioMateria.Object,
            repositorioQuestao.Object
        );

        // Act
        Result resultado = servico.Duplicar(new DuplicarProvaDto(provaOriginal.Id, "Avaliação cópia"));

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(provaDuplicada);
        Assert.AreEqual("Avaliação cópia", provaDuplicada.Titulo);
        Assert.AreSame(disciplina, provaDuplicada.Disciplina);
        Assert.AreSame(materia, provaDuplicada.Materia);
        Assert.AreEqual(provaOriginal.Serie, provaDuplicada.Serie);
        Assert.AreEqual(provaOriginal.QuantidadeQuestoes, provaDuplicada.QuantidadeQuestoes);
        repositorioProva.Verify(r => r.Cadastrar(It.IsAny<Prova>()), Times.Once);
    }

    [TestMethod]
    public void Excluir_ProvaInexistente_RetornaFalha()
    {
        // Arrange
        Guid provaId = Guid.CreateVersion7();

        Mock<IRepositorioProva> repositorioProva = new();
        Mock<IRepositorioDisciplina> repositorioDisciplina = new();
        Mock<IRepositorioMateria> repositorioMateria = new();
        Mock<IRepositorioQuestao> repositorioQuestao = new();

        ServicoProva servico = new(
            repositorioProva.Object,
            repositorioDisciplina.Object,
            repositorioMateria.Object,
            repositorioQuestao.Object
        );

        // Act
        Result resultado = servico.Excluir(provaId);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.Contains("Prova não encontrada", resultado.Errors.Single().Message);

        repositorioProva.Verify(r => r.Excluir(It.IsAny<Guid>()), Times.Once);
    }

    private static Questao CriarQuestao(Materia materia, string enunciado)
    {
        return new Questao(
            enunciado,
            materia,
            [new Alternativa("Resposta correta", true), new Alternativa("Resposta errada", false)]
        );
    }
}
