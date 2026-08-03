using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Testes.Integracao.Compartilhado.Orm;
using FizzWare.NBuilder;

namespace GeradorDeProvas.Testes.Integracao.ModuloDisciplina;

[TestClass]
public sealed class RepositorioDisciplinaEmOrmTests : RepositorioBaseEmOrmTests
{
    [TestMethod]
    public void CadastrarESelecionarPorId_CarregaRegistro()
    {
        // Arranjo
        Disciplina disciplina = Builder<Disciplina>
            .CreateNew()
            .With(d => d.UserId = Guid.Empty)
            .Build();

        // Ação
        repositorioDisciplina.Cadastrar(disciplina);
        dbContext.ChangeTracker.Clear();

        Disciplina? disciplinaSelecionada = repositorioDisciplina.SelecionarPorId(disciplina.Id);

        // Asserção
        Assert.IsNotNull(disciplinaSelecionada);
        Assert.AreEqual("Nome1", disciplinaSelecionada.Nome);
    }

    [TestMethod]
    public void Editar_AtualizaRegistroExistente()
    {
        // Arranjo
        Disciplina disciplina = Builder<Disciplina>
            .CreateNew()
            .With(d => d.UserId = Guid.Empty)
            .Persist();

        Disciplina disciplinaAtualizada = Builder<Disciplina>
            .CreateNew()
            .With(d => d.Nome = "NomeAtualizado")
            .With(d => d.UserId = Guid.Empty)
            .Build();

        // Ação
        bool conseguiuEditar = repositorioDisciplina.Editar(disciplina.Id, disciplinaAtualizada);
        dbContext.ChangeTracker.Clear();

        Disciplina? disciplinaSelecionada = repositorioDisciplina.SelecionarPorId(disciplina.Id);

        // Asserção
        Assert.IsTrue(conseguiuEditar);
        Assert.IsNotNull(disciplinaSelecionada);
        Assert.AreEqual("NomeAtualizado", disciplinaSelecionada.Nome);
    }

    [TestMethod]
    public void Excluir_RemoveRegistroExistente()
    {
        // Arranjo
        Disciplina disciplina = Builder<Disciplina>
            .CreateNew()
            .With(d => d.UserId = Guid.Empty)
            .Persist();

        // Ação
        bool conseguiuExcluir = repositorioDisciplina.Excluir(disciplina.Id);
        dbContext.ChangeTracker.Clear();

        // Asserção
        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(repositorioDisciplina.SelecionarPorId(disciplina.Id));
    }

    [TestMethod]
    public void SelecionarTodos_CarregaRegistros()
    {
        // Arranjo / Ação
        IList<Disciplina> disciplina = Builder<Disciplina>
            .CreateListOfSize(3)
            .All()
            .With(d => d.UserId = Guid.Empty)
            .Persist();

        dbContext.ChangeTracker.Clear();

        // Asserção
        Assert.HasCount(3, repositorioDisciplina.SelecionarTodos());
    }
}
