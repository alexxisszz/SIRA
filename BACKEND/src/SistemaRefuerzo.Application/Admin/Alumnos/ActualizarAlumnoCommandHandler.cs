using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Admin.Alumnos;

public class ActualizarAlumnoCommandHandler(
    IAlumnoRepository alumnoRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ActualizarAlumnoCommand>
{
    public async Task Handle(ActualizarAlumnoCommand request, CancellationToken cancellationToken)
    {
        var alumno = await alumnoRepository.ObtenerPorIdAsync(request.AlumnoId, cancellationToken)
            ?? throw new NotFoundException(nameof(Alumno), request.AlumnoId);

        alumno.ActualizarDatos(request.Nombres, request.Apellidos, request.Grado);

        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
