using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Infra.Compartilhado.Orm;

namespace GeradorDeProvas.Infra.Modulos.ModuloDisciplina;

public sealed class RepositorioDisciplinaEmOrm(
    GeradorDeProvasDbContext dbContext
) : RepositorioBaseEmOrm<Disciplina>(dbContext), IRepositorioDisciplina;
