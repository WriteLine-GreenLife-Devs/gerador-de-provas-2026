using FluentResults;
using GeradorDeProvas.Aplicacao.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Compartilhado;
using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Modulos.ModuloMateria;
using Moq;

namespace GeradorDeProvas.Testes.Unidade.Modulos.ModuloDisciplina;

[TestClass]
public sealed class ServicoDisciplinaTests
{
    [TestMethod]
    public void Cadastrar_DadosValidos_PersisteDisciplina()
    {
        // Arrange
        Mock<IRepositorioDisciplina> repositorioDisciplina = new Mock<IRepositorioDisciplina>();
        Mock<IRepositorioMateria> repositorioMateria = new Mock<IRepositorioMateria>();

        repositorioDisciplina.Setup(r => r.SelecionarTodos()).Returns([]);

        Disciplina? disciplinaCadastrada = null;

        repositorioDisciplina
            .Setup(r => r.Cadastrar(It.IsAny<Disciplina>()))
            .Callback<Disciplina>(
                disciplina => disciplinaCadastrada = disciplina
            );

        ServicoDisciplina servicoDisciplina = new ServicoDisciplina(
            repositorioDisciplina.Object,
            repositorioMateria.Object
        );

        // Act
        Result resultado = servicoDisciplina.Cadastrar(new CadastrarDisciplinaDto("Matemática"));

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(disciplinaCadastrada);
        Assert.AreEqual("Matemática", disciplinaCadastrada.Nome);

        repositorioDisciplina.Verify(r => r.Cadastrar(It.IsAny<Disciplina>()), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_NomeDuplicado_RetornaFalha()
    {
        // Arrange
        Mock<IRepositorioDisciplina> repositorioDisciplina = new Mock<IRepositorioDisciplina>();
        Mock<IRepositorioMateria> repositorioMateria = new Mock<IRepositorioMateria>();

        repositorioDisciplina
            .Setup(r => r.SelecionarTodos())
            .Returns([new Disciplina("Matemática")]);

        ServicoDisciplina servicoDisciplina = new ServicoDisciplina(
            repositorioDisciplina.Object,
            repositorioMateria.Object
        );

        // Act
        Result resultado = servicoDisciplina
            .Cadastrar(new CadastrarDisciplinaDto(" MATEMÁTICA "));

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Nome", resultado.Errors.Single().Metadata["Campo"]);
        Assert.Contains("Já existe", resultado.Errors.Single().Message);

        repositorioDisciplina.Verify(r => r.Cadastrar(It.IsAny<Disciplina>()), Times.Never);
    }

    [TestMethod]
    public void Excluir_DisciplinaSemVinculos_ExcluiDisciplina()
    {
        // Arrange
        Disciplina disciplina = new Disciplina("Matemática");

        Mock<IRepositorioDisciplina> repositorioDisciplina = new();
        Mock<IRepositorioMateria> repositorioMateria = new();

        repositorioDisciplina
            .Setup(r => r.SelecionarPorId(disciplina.Id))
            .Returns(disciplina);

        repositorioMateria
            .Setup(r => r.SelecionarTodos())
            .Returns([]);

        ServicoDisciplina servicoDisciplina = new ServicoDisciplina(
            repositorioDisciplina.Object,
            repositorioMateria.Object
        );

        // Act
        Result resultado = servicoDisciplina.Excluir(disciplina.Id);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);

        repositorioDisciplina.Verify(r => r.Excluir(disciplina.Id), Times.Once);
    }

    [TestMethod]
    public void Excluir_DisciplinaComMateriasVinculadas_RetornaFalha()
    {
        // Arrange
        Disciplina disciplina = new Disciplina("Matemática");

        Mock<IRepositorioDisciplina> repositorioDisciplina = new();
        Mock<IRepositorioMateria> repositorioMateria = new();

        repositorioDisciplina
            .Setup(r => r.SelecionarPorId(disciplina.Id))
            .Returns(disciplina);

        repositorioMateria
            .Setup(r => r.SelecionarTodos())
            .Returns([new Materia("Álgebra", 7, disciplina)]);

        ServicoDisciplina servicoDisciplina = new ServicoDisciplina(
            repositorioDisciplina.Object,
            repositorioMateria.Object
        );

        // Act
        Result resultado = servicoDisciplina.Excluir(disciplina.Id);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.Contains("matérias vinculadas", resultado.Errors.Single().Message);

        repositorioDisciplina.Verify(r => r.Excluir(disciplina.Id), Times.Never);
    }
}
