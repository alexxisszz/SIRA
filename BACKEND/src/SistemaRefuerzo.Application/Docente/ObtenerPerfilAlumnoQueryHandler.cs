using MediatR;
using SistemaRefuerzo.Application.Common.Interfaces;

namespace SistemaRefuerzo.Application.Reportes.Docente;

public class ObtenerPerfilAlumnoQueryHandler(IDocenteQueryRepository docenteQueryRepository)
    : IRequestHandler<ObtenerPerfilAlumnoQuery, PerfilRendimientoAlumnoDto>
{
    public Task<PerfilRendimientoAlumnoDto> Handle(ObtenerPerfilAlumnoQuery request, CancellationToken cancellationToken) =>
        docenteQueryRepository.ObtenerPerfilAlumnoAsync(request.AlumnoId, cancellationToken);
}
