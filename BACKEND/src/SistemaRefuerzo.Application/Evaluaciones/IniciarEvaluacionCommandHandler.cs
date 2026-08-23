using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Evaluaciones;

public class IniciarEvaluacionCommandHandler(
    ITemaRepository temaRepository,
    IUsuarioRepository usuarioRepository,
    IEvaluacionRepository evaluacionRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<IniciarEvaluacionCommand, Guid>
{
    public async Task<Guid> Handle(IniciarEvaluacionCommand request, CancellationToken cancellationToken)
    {
        var tema = await temaRepository.ObtenerPorIdAsync(request.TemaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Tema), request.TemaId);

        var alumno = await usuarioRepository.ObtenerAlumnoPorUsuarioIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new NotFoundException(nameof(Alumno), request.UsuarioId);

        var evaluacion = new Evaluacion(tema.Id, alumno.Id);

        evaluacionRepository.Agregar(evaluacion);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);

        return evaluacion.Id;
    }
}