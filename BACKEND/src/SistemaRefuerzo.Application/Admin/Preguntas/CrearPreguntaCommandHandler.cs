using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Admin.Preguntas;

public class CrearPreguntaCommandHandler(
    ITemaRepository temaRepository,
    IPreguntaRepository preguntaRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CrearPreguntaCommand, Guid>
{
    public async Task<Guid> Handle(CrearPreguntaCommand request, CancellationToken cancellationToken)
    {
        ValidadorDeOpciones.Validar(request.Opciones);

        var tema = await temaRepository.ObtenerPorIdAsync(request.TemaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Tema), request.TemaId);

        var pregunta = new Pregunta(tema.Id, request.Enunciado, request.NivelDificultad);
        foreach (var opcion in request.Opciones)
            pregunta.AgregarOpcion(opcion.Texto, opcion.EsCorrecta);

        preguntaRepository.Agregar(pregunta);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);

        return pregunta.Id;
    }
}
