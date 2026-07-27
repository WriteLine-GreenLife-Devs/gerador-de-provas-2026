using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Modulos.ModuloMateria;
using GeradorDeProvas.Dominio.Modulos.ModuloProva;
using GeradorDeProvas.Dominio.Modulos.ModuloQuestao;
using GeradorDeProvas.Infra.Compartilhado.Orm;
using GeradorDeProvas.Infra.Modulos.ModuloProva;
using GeradorDeProvas.Testes.Integracao.Identity;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeProvas.Testes.Integracao.ModuloProva;

[TestClass]
public sealed class RepositorioProvaEmOrmTests
{
    private GeradorDeProvasDbContext dbContext = null!;
    private RepositorioProvaEmOrm repositorio = null!;

    [TestInitialize]
    public void InicializarRepositorio()
    {
        dbContext = CriarDbContext(Guid.NewGuid());

        repositorio = new RepositorioProvaEmOrm(dbContext);
    }

    [TestMethod]
    public void CadastrarESelecionarPorId_CarregaRelacionamentosDaProva()
    {
        // Arranjo
        Disciplina disciplina = new Disciplina("Matemática");
        Materia materia = new Materia("Álgebra", 8, disciplina);

        Prova prova = new Prova("Prova de Álgebra", disciplina, materia, 8, 5, false);

        List<Questao> questoesDisponiveis = Enumerable.Range(1, 5)
            .Select(indice => new Questao($"Questão {indice}", materia, [new Alternativa("4", false), new Alternativa("7", true)]))
            .ToList();

        prova.SortearQuestoes(questoesDisponiveis, new Random(70));

        // Ação
        repositorio.Cadastrar(prova);
        dbContext.ChangeTracker.Clear();

        Prova? provaSelecionada = repositorio.SelecionarPorId(prova.Id);

        // Asserção
        Assert.IsNotNull(provaSelecionada);
        Assert.AreEqual("Prova de Álgebra", provaSelecionada.Titulo);
        Assert.AreEqual(disciplina.Id, provaSelecionada.Disciplina.Id);
        Assert.AreEqual(materia.Id, provaSelecionada.Materia!.Id);
        Assert.HasCount(5, provaSelecionada.Questoes);
        Assert.HasCount(2, provaSelecionada.Questoes[0].Alternativas);
    }

    [TestMethod]
    public void Editar_AtualizaProvaExistente()
    {
        // Arranjo
        Disciplina disciplina = new Disciplina("Matemática");
        Materia materia = new Materia("Álgebra", 8, disciplina);

        Prova prova = new Prova("Prova de Álgebra", disciplina, materia, 8, 5, false);

        List<Questao> questoesDisponiveis = Enumerable.Range(1, 5)
            .Select(indice => new Questao($"Questão {indice}", materia, [new Alternativa("4", false), new Alternativa("7", true)]))
            .ToList();

        prova.SortearQuestoes(questoesDisponiveis, new Random(70));

        repositorio.Cadastrar(prova);

        Prova provaAtualizada = new Prova("Prova Final", disciplina, null!, 8, 5, true);

        // Ação
        bool conseguiuEditar = repositorio.Editar(prova.Id, provaAtualizada);
        dbContext.ChangeTracker.Clear();

        // Asserção
        Assert.IsTrue(conseguiuEditar);
        Assert.AreEqual(
            "Prova Final",
            repositorio.SelecionarPorId(prova.Id)!.Titulo
        );
    }

    [TestMethod]
    public void Excluir_RemoveProvaExistente()
    {
        // Arranjo
        Disciplina disciplina = new Disciplina("Matemática");
        Materia materia = new Materia("Álgebra", 8, disciplina);

        Prova prova = new Prova("Prova de Álgebra", disciplina, materia, 8, 5, false);

        List<Questao> questoesDisponiveis = Enumerable.Range(1, 5)
            .Select(indice => new Questao($"Questão {indice}", materia, [new Alternativa("4", false), new Alternativa("7", true)]))
            .ToList();

        prova.SortearQuestoes(questoesDisponiveis, new Random(70));

        repositorio.Cadastrar(prova);

        // Ação
        bool conseguiuExcluir = repositorio.Excluir(prova.Id);
        dbContext.ChangeTracker.Clear();

        // Asserção
        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(repositorio.SelecionarPorId(prova.Id));
    }

    [TestMethod]
    public void SelecionarTodos_RetornaProvasComRelacionamentos()
    {
        // Arranjo
        Disciplina disciplina = new Disciplina("Matemática");
        Materia materia = new Materia("Álgebra", 8, disciplina);

        Prova prova = new Prova("Prova de Álgebra", disciplina, materia, 8, 5, false);

        List<Questao> questoesDisponiveis = Enumerable.Range(1, 5)
            .Select(indice => new Questao($"Questão {indice}", materia, [new Alternativa("4", false), new Alternativa("7", true)]))
            .ToList();

        prova.SortearQuestoes(questoesDisponiveis, new Random(70));

        repositorio.Cadastrar(prova);
        dbContext.ChangeTracker.Clear();

        // Ação
        List<Prova> provas = repositorio.SelecionarTodos();

        // Asserção
        Assert.HasCount(1, provas);
        Assert.AreEqual("Matemática", provas.First().Disciplina.Nome);
        Assert.AreEqual("Álgebra", provas.First().Materia!.Nome);
        Assert.HasCount(5, provas.First().Questoes);
    }

    private GeradorDeProvasDbContext CriarDbContext(Guid userId)
    {
        DbContextOptions<GeradorDeProvasDbContext> options =
            new DbContextOptionsBuilder<GeradorDeProvasDbContext>()
                .UseInMemoryDatabase("GeradorDeProvasTestDB_Memory")
                .Options;

        return new GeradorDeProvasDbContext(options, new ProvedorDeUsuarioFake(userId));
    }
}
