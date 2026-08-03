using GeradorDeProvas.Aplicacao.Modulos.ModuloProva;
using QuestPDF.Infrastructure;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace GeradorDeProvas.Testes.Unidade.Modulos.ModuloProva;

[TestClass]
public sealed class GeradorPdfProvaTests
{
    [TestInitialize]
    public void ConfigurarQuestPdf()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        QuestPDF.Settings.UseEnvironmentFonts = false;
    }

    [TestMethod]
    public void Gerar_ProvaNormal_ContemDadosETextoAcentuado()
    {
        // Arrange
        GeradorPdfProva gerador = new();

        // Act
        byte[] pdf = gerador.Gerar(CriarProva(), incluirGabarito: false);

        (int quantidadePaginas, string texto) = LerDocumento(pdf);

        // Assert
        Assert.AreEqual(1, quantidadePaginas);
        Assert.Contains("Avaliação de Matemática", texto);
        Assert.Contains("Disciplina: Matemática", texto);
        Assert.Contains("Matéria: Álgebra", texto);
        Assert.Contains("Questão sobre educação", texto);
        Assert.Contains("Alternativa correta com acentuação", texto);
    }

    [TestMethod]
    public void Gerar_Gabarito_MarcaApenasAlternativaCorreta()
    {
        // Arrange
        GeradorPdfProva gerador = new();
        DetalhesProvaDto prova = CriarProva();

        // Act
        byte[] pdfProva = gerador.Gerar(prova, incluirGabarito: false);
        byte[] pdfGabarito = gerador.Gerar(prova, incluirGabarito: true);

        string textoProva = LerTexto(pdfProva);
        string textoGabarito = LerTexto(pdfGabarito);

        // Assert
        Assert.Contains("[  ] Alternativa correta com acentuação", textoProva);
        Assert.Contains("[  ] Alternativa incorreta", textoProva);
        Assert.Contains("[X] Alternativa correta com acentuação", textoGabarito);
        Assert.Contains("[  ] Alternativa incorreta", textoGabarito);
    }

    [TestMethod]
    public void Gerar_MuitasQuestoes_CriaMultiplasPaginasEConservaTexto()
    {
        // Arrange
        GeradorPdfProva gerador = new();
        DetalhesProvaDto prova = CriarProva(quantidadeQuestoes: 60, textoLongo: true);

        // Act
        byte[] pdf = gerador.Gerar(prova, incluirGabarito: true);

        (int quantidadePaginas, string texto) = LerDocumento(pdf);

        // Assert
        Assert.IsGreaterThan(1, quantidadePaginas);
        Assert.Contains("Questão 1:", texto);
        Assert.Contains("Questão 60:", texto);
        Assert.Contains("fórmula de Bhaskara", texto);
    }

    private static (int QuantidadePaginas, string Texto) LerDocumento(byte[] pdf)
    {
        using PdfDocument documento = PdfDocument.Open(pdf);

        string texto = string.Join(
            Environment.NewLine,
            documento.GetPages()
            .Select(pagina => ContentOrderTextExtractor.GetText(pagina, true))
        );

        return (documento.NumberOfPages, texto);
    }

    private static string LerTexto(byte[] pdf) => LerDocumento(pdf).Texto;

    private static DetalhesProvaDto CriarProva(
        int quantidadeQuestoes = 1,
        bool textoLongo = false
    )
    {
        List<QuestaoProvaDto> questoes = Enumerable.Range(1, quantidadeQuestoes)
            .Select(indice => new QuestaoProvaDto(
                Guid.CreateVersion7(),
                textoLongo
                    ? $"Questão {indice}: explique detalhadamente a aplicação da fórmula de Bhaskara em situações práticas de matemática financeira, considerando diferentes cenários e justificando cada etapa do raciocínio."
                    : "Questão sobre educação e matemática.",
                [
                    new AlternativaProvaDto(
                        Guid.CreateVersion7(),
                        "Alternativa correta com acentuação",
                        true
                    ),
                    new AlternativaProvaDto(Guid.CreateVersion7(), "Alternativa incorreta", false)
                ]
            ))
            .ToList();

        return new DetalhesProvaDto(
            Guid.CreateVersion7(),
            "Avaliação de Matemática",
            Guid.CreateVersion7(),
            "Matemática",
            Guid.CreateVersion7(),
            "Álgebra",
            7,
            quantidadeQuestoes,
            false,
            questoes
        );
    }
}
