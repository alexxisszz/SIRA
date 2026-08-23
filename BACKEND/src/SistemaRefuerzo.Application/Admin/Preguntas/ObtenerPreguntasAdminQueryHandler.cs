using MediatR;
using SistemaRefuerzo.Application.Common.Interfaces;

namespace SistemaRefuerzo.Application.Admin.Preguntas;

public class ObtenerPreguntasAdminQueryHandler(IPreguntaRepository preguntaRepository)
    : IRequestHandler<ObtenerPreguntasAdminQuery, List<AdminPreguntaDto>>
{
    public async Task<List<AdminPreguntaDto>> Handle(ObtenerPreguntasAdminQuery request, CancellationToken cancellationToken)
    {
        var preguntas = await preguntaRepository.ObtenerPorTemaAsync(request.TemaId, cancellationToken);

        return preguntas
            .Select(p => new AdminPreguntaDto(
                p.Id,
                p.TemaId,
                p.Enunciado,
                p.NivelDificultad,
                p.Opciones.Select(o => new AdminOpcionDto(o.Id, o.Texto, o.EsCorrecta)).ToList()))
            .ToList();
    }
}
