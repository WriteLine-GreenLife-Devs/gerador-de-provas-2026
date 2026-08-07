using FizzWare.NBuilder;
using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Modulos.ModuloMateria;
using GeradorDeProvas.Testes.Integracao.Compartilhado.Orm;

namespace GeradorDeProvas.Testes.Integracao.Modulos;

[TestClass]
public sealed class RepositorioMateriaEmOrmTests : RepositorioBaseEmOrmTests
{
    [TestMethod]
    public void CadastrarESelecionarPorId_CarregaRegistro_ComRelacionamentos()
    {
        // Arranjo
        Disciplina disciplina = Builder<Disciplina>
            .CreateNew()
            .With(d => d.UserId = Guid.Empty)
            .Persist();

        Materia materia = Builder<Materia>
            .CreateNew()
            .With(m => m.Disciplina = disciplina)
            .With(m => m.UserId = Guid.Empty)
            .Build();

        // Ação
        repositorioMateria.Cadastrar(materia);
        dbContext.ChangeTracker.Clear();

        Materia? materiaSelecionada = repositorioMateria.SelecionarPorId(materia.Id);

        // Asserção
        Assert.IsNotNull(materiaSelecionada);
        Assert.AreEqual("Nome1", materiaSelecionada.Nome);
    }

    [TestMethod]
    public void Editar_AtualizaRegistroExistente()
    {
        // Arranjo
        Disciplina disciplina = Builder<Disciplina>
            .CreateNew()
            .With(d => d.UserId = Guid.Empty)
            .Persist();

        Materia materia = Builder<Materia>
            .CreateNew()
            .With(m => m.Disciplina = disciplina)
            .With(m => m.UserId = Guid.Empty)
            .Persist();

        Disciplina disciplinaAtualizada = Builder<Disciplina>
            .CreateNew()
            .With(d => d.UserId = Guid.Empty)
            .Persist();

        Materia materiaAtualizada = Builder<Materia>
            .CreateNew()
            .With(m => m.Nome = "NomeAtualizado")
            .With(m => m.Serie = 2)
            .With(m => m.Disciplina = disciplinaAtualizada)
            .With(m => m.UserId = Guid.Empty)
            .Build();

        // Ação
        bool conseguiuEditar = repositorioMateria.Editar(materia.Id, materiaAtualizada);
        dbContext.ChangeTracker.Clear();

        Materia? materiaSelecionada = repositorioMateria.SelecionarPorId(materia.Id);

        // Asserção
        Assert.IsTrue(conseguiuEditar);
        Assert.IsNotNull(materiaSelecionada);
        Assert.AreEqual("NomeAtualizado", materiaSelecionada.Nome);
        Assert.AreEqual(2, materiaSelecionada.Serie);
        Assert.AreEqual(disciplinaAtualizada.Nome, materiaSelecionada.Disciplina.Nome);
    }

    [TestMethod]
    public void Excluir_RemoveRegistroExistente()
    {
        // Arranjo
        Disciplina disciplina = Builder<Disciplina>
            .CreateNew()
            .With(d => d.UserId = Guid.Empty)
            .Persist();

        Materia materia = Builder<Materia>
            .CreateNew()
            .With(m => m.Disciplina = disciplina)
            .With(m => m.UserId = Guid.Empty)
            .Persist();

        // Ação
        bool conseguiuExcluir = repositorioMateria.Excluir(materia.Id);
        dbContext.ChangeTracker.Clear();

        Materia? materiaSelecionada = repositorioMateria.SelecionarPorId(materia.Id);

        // Asserção
        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(materiaSelecionada);
    }

    [TestMethod]
    public void SelecionarTodos_CarregaRegistros_ComRelacionamentos()
    {
        // Arranjo
        Disciplina disciplina = Builder<Disciplina>
            .CreateNew()
            .With(d => d.UserId = Guid.Empty)
            .Persist();

        IList<Materia> materias = Builder<Materia>
            .CreateListOfSize(3)
            .All()
            .With(m => m.Disciplina = disciplina)
            .With(m => m.UserId = Guid.Empty)
            .Persist();

        // Ação
        List<Materia> materiasSelecionadas = repositorioMateria.SelecionarTodos();

        // Asserção
        Assert.HasCount(3, materiasSelecionadas);
        CollectionAssert.AreEquivalent(materias.ToList(), materiasSelecionadas);
    }
}
