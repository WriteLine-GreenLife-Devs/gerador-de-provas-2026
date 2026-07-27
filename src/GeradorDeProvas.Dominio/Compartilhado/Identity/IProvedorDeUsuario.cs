namespace GeradorDeProvas.Dominio.Compartilhado.Identity;

public interface IProvedorDeUsuario
{
    Guid? Id { get; }
    bool EstaAutenticado { get; }
}
