using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Admin.Preguntas;

public class ActualizarPreguntaCommandHandler(
    IPreguntaRepository preguntaRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ActualizarPreguntaCommand>
{
    public async Task Handle(ActualizarPreguntaCommand request, CancellationToken cancellationToken)
    {
        ValidadorDeOpciones.Validar(request.Opciones);

        var pregunta = await preguntaRepository.ObtenerPorIdAsync(request.PreguntaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Pregunta), request.PreguntaId);

        pregunta.ActualizarContenido(
            request.Enunciado,
            request.NivelDificultad,
            request.Opciones.Select(o => (o.Texto, o.EsCorrecta)));

        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
