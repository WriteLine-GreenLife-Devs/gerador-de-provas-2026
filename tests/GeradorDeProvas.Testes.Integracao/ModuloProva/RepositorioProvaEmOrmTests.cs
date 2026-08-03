using FizzWare.NBuilder;
using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Modulos.ModuloMateria;
using GeradorDeProvas.Dominio.Modulos.ModuloProva;
using GeradorDeProvas.Dominio.Modulos.ModuloQuestao;
using GeradorDeProvas.Testes.Integracao.Compartilhado.Orm;

namespace GeradorDeProvas.Testes.Integracao.ModuloProva;

[TestClass]
public sealed class RepositorioProvaEmOrmTests : RepositorioBaseEmOrmTests
{
    [TestMethod]
    public void CadastrarESelecionarPorId_CarregaRelacionamentosDaProva()
    {
        // Arranjo
        Disciplina disciplina = Builder<Disciplina>
            .CreateNew()
            .With(d => d.Nome = "Matemática")
            .With(d => d.UserId = Guid.Empty)
            .Persist();

        Materia materia = Builder<Materia>
            .CreateNew()
            .With(m => m.Nome = "Álgebra")
            .With(m => m.Serie = 8)
            .With(m => m.Disciplina = disciplina)
            .With(m => m.UserId = Guid.Empty)
            .Persist();

        IList<Questao> questoesDisponiveis = Builder<Questao>
            .CreateListOfSize(5)
            .All()
            .With(q => q.Materia = materia)
            .With(q => q.Alternativas =
                Enumerable
                    .Range(1, 2)
                    .Select(i => new Alternativa($"Alternativa {i}", i % 2 == 0))
                    .ToList()
            )
            .With(q => q.UserId = Guid.Empty)
            .Persist();

        Prova prova = Builder<Prova>
            .CreateNew()
            .With(p => p.Titulo = "Prova de Álgebra")
            .With(p => p.Disciplina = disciplina)
            .With(p => p.Materia = materia)
            .With(p => p.Serie = 8)
            .With(p => p.QuantidadeQuestoes = 5)
            .With(p => p.ProvaRecuperacao = false)
            .With(p => p.UserId = Guid.Empty)
            .Do(p => p.SortearQuestoes(questoesDisponiveis, new Random(70)))
            .Build();

        // Ação
        repositorioProva.Cadastrar(prova);
        dbContext.ChangeTracker.Clear();

        Prova? provaSelecionada = repositorioProva.SelecionarPorId(prova.Id);

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
        Disciplina disciplina = Builder<Disciplina>
            .CreateNew()
            .With(d => d.Nome = "Matemática")
            .With(d => d.UserId = Guid.Empty)
            .Persist();

        Materia materia = Builder<Materia>
            .CreateNew()
            .With(m => m.Nome = "Álgebra")
            .With(m => m.Serie = 8)
            .With(m => m.Disciplina = disciplina)
            .With(m => m.UserId = Guid.Empty)
            .Persist();

        IList<Questao> questoesDisponiveis = Builder<Questao>
            .CreateListOfSize(5)
            .All()
            .With(q => q.Materia = materia)
            .With(q => q.Alternativas =
                Enumerable
                    .Range(1, 2)
                    .Select(i => new Alternativa($"Alternativa {i}", i % 2 == 0))
                    .ToList()
            )
            .With(q => q.UserId = Guid.Empty)
            .Persist();

        Prova prova = Builder<Prova>
            .CreateNew()
            .With(p => p.Titulo = "Prova de Álgebra")
            .With(p => p.Disciplina = disciplina)
            .With(p => p.Materia = materia)
            .With(p => p.Serie = 8)
            .With(p => p.QuantidadeQuestoes = 5)
            .With(p => p.ProvaRecuperacao = false)
            .With(p => p.UserId = Guid.Empty)
            .Do(p => p.SortearQuestoes(questoesDisponiveis, new Random(70)))
            .Persist();

        Prova provaAtualizada = Builder<Prova>
            .CreateNew()
            .With(p => p.Titulo = "Prova Final")
            .With(p => p.Disciplina = disciplina)
            .With(p => p.Materia = null)
            .With(p => p.Serie = 8)
            .With(p => p.QuantidadeQuestoes = 5)
            .With(p => p.ProvaRecuperacao = true)
            .With(p => p.UserId = Guid.Empty)
            .Do(p => p.SortearQuestoes(questoesDisponiveis, new Random(70)))
            .Build();

        // Ação
        bool conseguiuEditar = repositorioProva.Editar(prova.Id, provaAtualizada);
        dbContext.ChangeTracker.Clear();

        // Asserção
        Assert.IsTrue(conseguiuEditar);
        Assert.AreEqual(
            "Prova Final",
            repositorioProva.SelecionarPorId(prova.Id)!.Titulo
        );
    }

    [TestMethod]
    public void Excluir_RemoveProvaExistente()
    {
        // Arranjo
        Disciplina disciplina = Builder<Disciplina>
            .CreateNew()
            .With(d => d.Nome = "Matemática")
            .With(d => d.UserId = Guid.Empty)
            .Persist();

        Materia materia = Builder<Materia>
            .CreateNew()
            .With(m => m.Nome = "Álgebra")
            .With(m => m.Serie = 8)
            .With(m => m.Disciplina = disciplina)
            .With(m => m.UserId = Guid.Empty)
            .Persist();

        IList<Questao> questoesDisponiveis = Builder<Questao>
            .CreateListOfSize(5)
            .All()
            .With(q => q.Materia = materia)
            .With(q => q.Alternativas =
                Enumerable
                    .Range(1, 2)
                    .Select(i => new Alternativa($"Alternativa {i}", i % 2 == 0))
                    .ToList()
            )
            .With(q => q.UserId = Guid.Empty)
            .Persist();

        Prova prova = Builder<Prova>
            .CreateNew()
            .With(p => p.Titulo = "Prova de Álgebra")
            .With(p => p.Disciplina = disciplina)
            .With(p => p.Materia = materia)
            .With(p => p.Serie = 8)
            .With(p => p.QuantidadeQuestoes = 5)
            .With(p => p.ProvaRecuperacao = false)
            .With(p => p.UserId = Guid.Empty)
            .Do(p => p.SortearQuestoes(questoesDisponiveis, new Random(70)))
            .Persist();

        dbContext.ChangeTracker.Clear();

        // Ação
        bool conseguiuExcluir = repositorioProva.Excluir(prova.Id);
        dbContext.ChangeTracker.Clear();

        // Asserção
        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(repositorioProva.SelecionarPorId(prova.Id));
    }

    [TestMethod]
    public void SelecionarTodos_RetornaProvasComRelacionamentos()
    {
        // Arranjo
        Disciplina disciplina = Builder<Disciplina>
            .CreateNew()
            .With(d => d.Nome = "Matemática")
            .With(d => d.UserId = Guid.Empty)
            .Persist();

        Materia materia = Builder<Materia>
            .CreateNew()
            .With(m => m.Nome = "Álgebra")
            .With(m => m.Serie = 8)
            .With(m => m.Disciplina = disciplina)
            .With(m => m.UserId = Guid.Empty)
            .Persist();

        Prova prova = Builder<Prova>
            .CreateNew()
            .With(p => p.Titulo = "Prova de Álgebra")
            .With(p => p.Disciplina = disciplina)
            .With(p => p.Materia = materia)
            .With(p => p.Serie = 8)
            .With(p => p.QuantidadeQuestoes = 5)
            .With(p => p.ProvaRecuperacao = false)
            .With(p => p.UserId = Guid.Empty)
            .Build();

        List<Questao> questoesDisponiveis = Enumerable.Range(1, 5)
            .Select(indice => new Questao($"Questão {indice}", materia, [new Alternativa("4", false), new Alternativa("7", true)]))
            .ToList();

        prova.SortearQuestoes(questoesDisponiveis, new Random(70));

        repositorioProva.Cadastrar(prova);

        dbContext.ChangeTracker.Clear();

        // Ação
        List<Prova> provas = repositorioProva.SelecionarTodos();

        // Asserção
        Assert.HasCount(1, provas);
        Assert.AreEqual("Matemática", provas.First().Disciplina.Nome);
        Assert.AreEqual("Álgebra", provas.First().Materia!.Nome);
        Assert.HasCount(5, provas.First().Questoes);
    }
}
