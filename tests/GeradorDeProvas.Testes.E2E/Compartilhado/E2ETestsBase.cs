using GeradorDeProvas.Testes.E2E.Modulos.ModuloAutenticacao;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace GeradorDeProvas.Testes.E2E.Compartilhado;

public abstract class E2ETestsBase : PageTest
{
    private TestApplicationFactory aplicacao = null!;

    protected string UrlBase { get; set; } = string.Empty;

    [TestInitialize]
    public async Task InicializarAplicacao()
    {
        aplicacao = new TestApplicationFactory();

        UrlBase = aplicacao.UrlBase;
    }

    [TestCleanup]
    public async Task EncerrarAplicacao()
    {
        try
        {
            if (aplicacao is not null)
                await aplicacao.DisposeAsync();
        }
        finally
        {
            aplicacao = null!;
        }
    }

    protected async Task RegistrarUsuarioAsync(string email, string senha)
    {
        using IServiceScope scope = aplicacao.Services.CreateScope();

        UserManager<IdentityUser<Guid>> userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser<Guid>>>();

        IdentityUser<Guid> user = new IdentityUser<Guid>()
        {
            Id = Guid.CreateVersion7(),
            UserName = email,
            Email = email
        };

        IdentityResult resultado = await userManager.CreateAsync(user, senha);

        Assert.IsTrue(
            resultado.Succeeded,
            string.Join("; ", resultado.Errors.Select(erro => erro.Description))
        );
    }

    protected async Task RegistrarEEntrarAsync(string email, string senha)
    {
        await RegistrarUsuarioAsync(email, senha);

        EntrarPage entrarPage = new(Page, UrlBase);

        await entrarPage.IrParaAsync();
        await entrarPage.PreencherAsync(email, senha);
        await entrarPage.ConfirmarAsync();
    }
}
