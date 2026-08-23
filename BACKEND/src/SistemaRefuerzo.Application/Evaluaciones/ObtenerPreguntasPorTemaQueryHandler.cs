using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Evaluaciones;

public class ObtenerPreguntasPorTemaQueryHandler(
    ITemaRepository temaRepository,
    IPreguntaRepository preguntaRepository) : IRequestHandler<ObtenerPreguntasPorTemaQuery, List<PreguntaDto>>
{
    public async Task<List<PreguntaDto>> Handle(ObtenerPreguntasPorTemaQuery request, CancellationToken cancellationToken)
    {
        var tema = await temaRepository.ObtenerPorIdAsync(request.TemaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Tema), request.TemaId);

        var preguntas = await preguntaRepository.ObtenerPorTemaAsync(tema.Id, cancellationToken);

        return preguntas
            .Select(p => new PreguntaDto(
                p.Id,
                p.Enunciado,
                p.Opciones.Select(o => new OpcionDto(o.Id, o.Texto)).ToList()))
            .ToList();
    }
}