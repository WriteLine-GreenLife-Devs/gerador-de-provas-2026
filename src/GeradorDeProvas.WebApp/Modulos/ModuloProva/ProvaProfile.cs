using AutoMapper;
using GeradorDeProvas.Aplicacao.Modulos.ModuloProva;

namespace GeradorDeProvas.WebApp.Modulos.ModuloProva;

public class ProvaProfile : Profile
{
    public ProvaProfile()
    {
        CreateMap<ListarProvaDto, ListarProvaViewModel>();
        CreateMap<DetalhesProvaDto, DetalhesProvaViewModel>();
        CreateMap<DetalhesProvaDto, DuplicarProvaViewModel>();
        CreateMap<DetalhesProvaDto, ExcluirProvaViewModel>();
        CreateMap<QuestaoProvaDto, QuestaoProvaViewModel>();
        CreateMap<AlternativaProvaDto, AlternativaProvaViewModel>();
    }
}
