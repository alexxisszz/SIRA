using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Reportes.Docente;

/// <summary>
/// Registra las notas que el docente ingresa manualmente en la Ficha de Registro de Notas:
/// D2 (Procedimental) y D3 (Actitudinal). Crea la fila de la ficha si aún no existe; D1 no se toca
/// porque la calcula el sistema al finalizar la evaluación Pretest/Postest.
/// </summary>
public class RegistrarNotasDocenteCommandHandler(
    IAlumnoRepository alumnoRepository,
    ITemaRepository temaRepository,
    IFichaRegistroNotaRepository fichaRegistroNotaRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<RegistrarNotasDocenteCommand>
{
    public async Task Handle(RegistrarNotasDocenteCommand request, CancellationToken cancellationToken)
    {
        if (!FichaRegistroNota.EsTipoValido(request.TipoEvaluacion))
            throw new ReglaDeNegocioException("La ficha de registro de notas solo admite evaluaciones Pretest o Postest.");

        decimal[] notas = [request.D2I1, request.D2I2, request.D2I3, request.D3I1, request.D3I2, request.D3I3];
        if (notas.Any(n => n < FichaRegistroNota.NotaMinima || n > FichaRegistroNota.NotaMaxima))
            throw new ReglaDeNegocioException(
                $"Las notas deben estar entre {FichaRegistroNota.NotaMinima} y {FichaRegistroNota.NotaMaxima}.");

        _ = await alumnoRepository.ObtenerPorIdAsync(request.AlumnoId, cancellationToken)
            ?? throw new NotFoundException(nameof(Alumno), request.AlumnoId);

        _ = await temaRepository.ObtenerPorIdAsync(request.TemaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Tema), request.TemaId);

        var ficha = await fichaRegistroNotaRepository.ObtenerAsync(
                request.AlumnoId, request.TemaId, request.TipoEvaluacion, cancellationToken)
            ?? FichaRegistroNota.Crear(request.AlumnoId, request.TemaId, request.TipoEvaluacion);

        ficha.ActualizarD2D3(request.D2I1, request.D2I2, request.D2I3, request.D3I1, request.D3I2, request.D3I3);

        await fichaRegistroNotaRepository.GuardarAsync(ficha, cancellationToken);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
