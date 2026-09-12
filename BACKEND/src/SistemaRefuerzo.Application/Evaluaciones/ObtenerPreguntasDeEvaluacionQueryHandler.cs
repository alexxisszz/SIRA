using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Evaluaciones;

public class ObtenerPreguntasDeEvaluacionQueryHandler(
    IUsuarioRepository usuarioRepository,
    IEvaluacionRepository evaluacionRepository,
    IPreguntaRepository preguntaRepository) : IRequestHandler<ObtenerPreguntasDeEvaluacionQuery, List<PreguntaDto>>
{
    public async Task<List<PreguntaDto>> Handle(ObtenerPreguntasDeEvaluacionQuery request, CancellationToken cancellationToken)
    {
        var alumno = await usuarioRepository.ObtenerAlumnoPorUsuarioIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new NotFoundException(nameof(Alumno), request.UsuarioId);

        var evaluacion = await evaluacionRepository.ObtenerPorIdAsync(request.EvaluacionId, cancellationToken)
            ?? throw new NotFoundException(nameof(Evaluacion), request.EvaluacionId);

        if (evaluacion.AlumnoId != alumno.Id)
            throw new NotFoundException(nameof(Evaluacion), request.EvaluacionId);

        var preguntas = new List<PreguntaDto>();
        foreach (var preguntaId in evaluacion.PreguntasAsignadas)
        {
            var pregunta = await preguntaRepository.ObtenerPorIdAsync(preguntaId, cancellationToken);
            if (pregunta is not null)
                preguntas.Add(new PreguntaDto(
                    pregunta.Id,
                    pregunta.Enunciado,
                    pregunta.Subtema,
                    pregunta.Opciones.Select(o => new OpcionDto(o.Id, o.Texto)).ToList()));
        }

        return preguntas;
    }
}
