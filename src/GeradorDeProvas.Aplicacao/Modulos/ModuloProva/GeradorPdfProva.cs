using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GeradorDeProvas.Aplicacao.Modulos.ModuloProva;

public sealed class GeradorPdfProva
{
    public byte[] Gerar(DetalhesProvaDto prova, bool incluirGabarito)
    {
        return CriarDocumento(prova, incluirGabarito).GeneratePdf();
    }

    public void GerarEMostrar(DetalhesProvaDto prova, bool incluirGabarito)
    {
        CriarDocumento(prova, incluirGabarito).GeneratePdfAndShow();
    }

    private static IDocument CriarDocumento(DetalhesProvaDto prova, bool incluirGabarito)
    {
        return Document.Create(documento =>
        {
            documento.Page(pagina =>
            {
                pagina.Size(PageSizes.A4);
                pagina.Margin(2, Unit.Centimetre);
                pagina.PageColor(Colors.White);
                pagina.DefaultTextStyle(style => style.FontSize(12));

                // Cabeçalho da Página
                pagina.Header().Column(header =>
                {
                    header.Item()
                        .Text(prova.Titulo)
                        .Bold()
                        .FontSize(18)
                        .FontColor(Colors.Blue.Darken2);

                    header.Item()
                        .PaddingTop(4)
                        .Text(texto =>
                        {
                            texto.Span($"Disciplina: {prova.NomeDisciplina}     ");
                            texto.Span(prova.ProvaRecuperacao ? "Prova de recuperação" : $"Matéria: {prova.NomeMateria}     ");
                            texto.Span($"Série: {prova.Serie}");
                        });

                    header.Item()
                        .PaddingTop(8)
                        .LineHorizontal(1)
                        .LineColor(Colors.Grey.Lighten1);

                });

                // Conteúdo da Página
                pagina.Content().PaddingVertical(15).Column(conteudo =>
                {
                    conteudo.Spacing(12);

                    for (int indice = 0; indice < prova.Questoes.Count; indice++)
                    {
                        QuestaoProvaDto questaoDto = prova.Questoes[indice];

                        conteudo.Item()
                            .PreventPageBreak()
                            .Column(questao =>
                            {
                                questao.Spacing(5);

                                questao.Item().Text(texto =>
                                {
                                    texto.Span($"{indice + 1}. ").Bold();
                                    texto.Span(questaoDto.Enunciado);
                                });

                                foreach (AlternativaProvaDto alternativaDto in questaoDto.Alternativas)
                                {
                                    string marcador = incluirGabarito && alternativaDto.Correta
                                        ? "[X]"
                                        : "[  ]";

                                    questao.Item()
                                        .PaddingLeft(15)
                                        .Text($"{marcador} {alternativaDto.Texto}");
                                }
                            });
                    }
                });

                // Rodapé da Página
                pagina.Footer()
                    .AlignCenter()
                    .Text(texto =>
                    {
                        texto.Span("Página ");
                        texto.CurrentPageNumber();
                        texto.Span(" de ");
                        texto.TotalPages();
                    });
            });
        });
    }
}
