using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Admin.Preguntas;

public class EliminarPreguntaCommandHandler(
    IPreguntaRepository preguntaRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<EliminarPreguntaCommand>
{
    public async Task Handle(EliminarPreguntaCommand request, CancellationToken cancellationToken)
    {
        var pregunta = await preguntaRepository.ObtenerPorIdAsync(request.PreguntaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Pregunta), request.PreguntaId);

        preguntaRepository.Eliminar(pregunta);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
