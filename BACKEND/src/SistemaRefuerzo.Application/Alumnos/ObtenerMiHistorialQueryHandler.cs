using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Application.Reportes.Docente;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Alumnos;

public class ObtenerMiHistorialQueryHandler(
    IUsuarioRepository usuarioRepository,
    IDocenteQueryRepository docenteQueryRepository) : IRequestHandler<ObtenerMiHistorialQuery, List<ResultadoHistoricoDto>>
{
    public async Task<List<ResultadoHistoricoDto>> Handle(ObtenerMiHistorialQuery request, CancellationToken cancellationToken)
    {
        var alumno = await usuarioRepository.ObtenerAlumnoPorUsuarioIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new NotFoundException(nameof(Alumno), request.UsuarioId);

        return await docenteQueryRepository.ObtenerResultadosPorAlumnoAsync(alumno.Id, cancellationToken);
    }
}
