using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Alumnos;

public class ObtenerMiPerfilQueryHandler(IUsuarioRepository usuarioRepository)
    : IRequestHandler<ObtenerMiPerfilQuery, PerfilAlumnoDto>
{
    public async Task<PerfilAlumnoDto> Handle(ObtenerMiPerfilQuery request, CancellationToken cancellationToken)
    {
        var alumno = await usuarioRepository.ObtenerAlumnoPorUsuarioIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new NotFoundException(nameof(Alumno), request.UsuarioId);

        return new PerfilAlumnoDto(alumno.Nombres, alumno.Apellidos, alumno.Grado);
    }
}
