using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Ejercicios;

public class ObtenerProgresoPracticaQueryHandler(
    IUsuarioRepository usuarioRepository,
    IPracticaQueryRepository practicaQueryRepository) : IRequestHandler<ObtenerProgresoPracticaQuery, List<ProgresoSubtemaDto>>
{
    public async Task<List<ProgresoSubtemaDto>> Handle(ObtenerProgresoPracticaQuery request, CancellationToken cancellationToken)
    {
        var alumno = await usuarioRepository.ObtenerAlumnoPorUsuarioIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new NotFoundException(nameof(Alumno), request.UsuarioId);

        return await practicaQueryRepository.ObtenerProgresoPorSubtemaAsync(alumno.Id, request.TemaId, cancellationToken);
    }
}
