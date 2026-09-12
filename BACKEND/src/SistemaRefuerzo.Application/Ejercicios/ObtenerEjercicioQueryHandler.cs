using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Application.Evaluaciones;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Ejercicios;

public class ObtenerEjercicioQueryHandler(IPreguntaRepository preguntaRepository) : IRequestHandler<ObtenerEjercicioQuery, PreguntaDto>
{
    public async Task<PreguntaDto> Handle(ObtenerEjercicioQuery request, CancellationToken cancellationToken)
    {
        var pregunta = await preguntaRepository.ObtenerPorIdAsync(request.PreguntaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Pregunta), request.PreguntaId);

        return new PreguntaDto(
            pregunta.Id,
            pregunta.Enunciado,
            pregunta.Subtema,
            pregunta.Opciones.Select(o => new OpcionDto(o.Id, o.Texto)).ToList());
    }
}
