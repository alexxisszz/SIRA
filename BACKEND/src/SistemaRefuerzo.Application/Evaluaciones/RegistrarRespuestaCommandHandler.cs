using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Evaluaciones;

public class RegistrarRespuestaCommandHandler(
    IEvaluacionRepository evaluacionRepository,
    IPreguntaRepository preguntaRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<RegistrarRespuestaCommand>
{
    public async Task Handle(RegistrarRespuestaCommand request, CancellationToken cancellationToken)
    {
        var evaluacion = await evaluacionRepository.ObtenerPorIdAsync(request.EvaluacionId, cancellationToken)
            ?? throw new NotFoundException(nameof(Evaluacion), request.EvaluacionId);

        var pregunta = await preguntaRepository.ObtenerPorIdAsync(request.PreguntaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Pregunta), request.PreguntaId);

        var opcionSeleccionada = pregunta.Opciones.FirstOrDefault(o => o.Id == request.OpcionSeleccionadaId)
            ?? throw new NotFoundException(nameof(OpcionPregunta), request.OpcionSeleccionadaId);

        evaluacion.RegistrarRespuesta(pregunta.Id, opcionSeleccionada.Id, opcionSeleccionada.EsCorrecta);

        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}