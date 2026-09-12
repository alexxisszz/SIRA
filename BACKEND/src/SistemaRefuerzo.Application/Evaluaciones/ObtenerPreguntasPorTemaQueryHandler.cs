using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Evaluaciones;

public class ObtenerPreguntasPorTemaQueryHandler(
    ITemaRepository temaRepository,
    IPreguntaRepository preguntaRepository) : IRequestHandler<ObtenerPreguntasPorTemaQuery, List<PreguntaDto>>
{
    private const int CantidadPreguntasPorEvaluacion = 10;

    public async Task<List<PreguntaDto>> Handle(ObtenerPreguntasPorTemaQuery request, CancellationToken cancellationToken)
    {
        var tema = await temaRepository.ObtenerPorIdAsync(request.TemaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Tema), request.TemaId);

        var preguntas = request.Nivel is NivelDesempeno nivel
            ? await preguntaRepository.ObtenerPorTemaYNivelAsync(tema.Id, nivel, cancellationToken)
            : await preguntaRepository.ObtenerPorTemaAsync(tema.Id, cancellationToken);

        if (request.Subtema is string subtema)
            preguntas = preguntas.Where(p => p.Subtema == subtema).ToList();

        if (request.ExcluirSubtemas is { Count: > 0 } excluirSubtemas)
            preguntas = preguntas.Where(p => !excluirSubtemas.Contains(p.Subtema)).ToList();

        return preguntas
            .OrderBy(_ => Guid.NewGuid())
            .Take(CantidadPreguntasPorEvaluacion)
            .Select(p => new PreguntaDto(
                p.Id,
                p.Enunciado,
                p.Subtema,
                p.Opciones.Select(o => new OpcionDto(o.Id, o.Texto)).ToList()))
            .ToList();
    }
}