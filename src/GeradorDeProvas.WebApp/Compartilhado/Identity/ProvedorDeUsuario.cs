using System.Security.Claims;
using GeradorDeProvas.Dominio.Compartilhado.Identity;

namespace GeradorDeProvas.WebApp.Compartilhado.Identity;

public sealed class ProvedorDeUsuario(IHttpContextAccessor httpContextAccessor) : IProvedorDeUsuario
{
    public Guid? Id
    {
        get
        {
            ClaimsPrincipal? user = httpContextAccessor.HttpContext?.User;

            if (user?.Identity is null || !user.Identity.IsAuthenticated)
                return null;

            string? claim = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (claim is null || !Guid.TryParse(claim, out Guid id))
                return null;

            return id;
        }
    }

    public bool EstaAutenticado => Id != null;
}
