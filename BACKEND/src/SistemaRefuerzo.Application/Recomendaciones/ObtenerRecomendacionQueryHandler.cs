using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Recomendaciones;

public class ObtenerRecomendacionQueryHandler(
    IRecomendacionRepository recomendacionRepository,
    IPreguntaRepository preguntaRepository) : IRequestHandler<ObtenerRecomendacionQuery, RecomendacionDto>
{
    public async Task<RecomendacionDto> Handle(ObtenerRecomendacionQuery request, CancellationToken cancellationToken)
    {
        var recomendacion = await recomendacionRepository.ObtenerPorIdAsync(request.RecomendacionId, cancellationToken)
            ?? throw new NotFoundException(nameof(Recomendacion), request.RecomendacionId);

        var ejercicios = new List<EjercicioSugeridoDto>();
        foreach (var ejercicio in recomendacion.EjerciciosRecomendados)
        {
            var pregunta = await preguntaRepository.ObtenerPorIdAsync(ejercicio.PreguntaId, cancellationToken);
            if (pregunta is not null)
                ejercicios.Add(new EjercicioSugeridoDto(pregunta.Id, pregunta.Enunciado));
        }

        return new RecomendacionDto(
            recomendacion.Id,
            recomendacion.Nivel,
            recomendacion.TemasPorReforzar.ToList(),
            ejercicios,
            recomendacion.Retroalimentacion);
    }
}