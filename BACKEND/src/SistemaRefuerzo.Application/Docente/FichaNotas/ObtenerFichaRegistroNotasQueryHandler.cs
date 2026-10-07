using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Application.Reportes.Docente;

/// <summary>
/// Arma la Ficha de Registro de Notas de un tema para Pretest o Postest: lista a todos los alumnos
/// (igual que el resumen de grupo) con sus notas D1/D2/D3, el promedio por dimensión
/// P = (I1 + I2 + I3) / 3 y el promedio final (promedio de las 3 dimensiones).
/// </summary>
public class ObtenerFichaRegistroNotasQueryHandler(
    IAlumnoRepository alumnoRepository,
    ITemaRepository temaRepository,
    IFichaRegistroNotaRepository fichaRegistroNotaRepository) : IRequestHandler<ObtenerFichaRegistroNotasQuery, FichaRegistroNotasDto>
{
    private const int Decimales = 2;

    public async Task<FichaRegistroNotasDto> Handle(ObtenerFichaRegistroNotasQuery request, CancellationToken cancellationToken)
    {
        if (!FichaRegistroNota.EsTipoValido(request.TipoEvaluacion))
            throw new ReglaDeNegocioException("La ficha de registro de notas solo admite evaluaciones Pretest o Postest.");

        var tema = await temaRepository.ObtenerPorIdAsync(request.TemaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Tema), request.TemaId);

        var alumnos = await alumnoRepository.ListarAsync(cancellationToken);
        var fichasPorAlumno = (await fichaRegistroNotaRepository.ListarPorTemaAsync(tema.Id, request.TipoEvaluacion, cancellationToken))
            .ToDictionary(f => f.AlumnoId);

        var filas = alumnos
            .OrderBy(a => a.Apellidos)
            .ThenBy(a => a.Nombres)
            .Select(alumno => fichasPorAlumno.TryGetValue(alumno.Id, out var ficha)
                ? CrearFila(alumno, ficha)
                : new FichaAlumnoDto(alumno.Id, alumno.Nombres, alumno.Apellidos, false, null, null, null, null, null))
            .ToList();

        return new FichaRegistroNotasDto(tema.Id, tema.Nombre, request.TipoEvaluacion, filas);
    }

    private static FichaAlumnoDto CrearFila(Alumno alumno, FichaRegistroNota ficha)
    {
        var d1 = CrearDimension(ficha.D1I1, ficha.D1I2, ficha.D1I3);
        var d2 = CrearDimension(ficha.D2I1, ficha.D2I2, ficha.D2I3);
        var d3 = CrearDimension(ficha.D3I1, ficha.D3I2, ficha.D3I3);
        var promedioFinal = Math.Round((d1.Promedio + d2.Promedio + d3.Promedio) / 3, Decimales);

        return new FichaAlumnoDto(
            alumno.Id, alumno.Nombres, alumno.Apellidos, true, d1, d2, d3, promedioFinal, ficha.FechaActualizacion);
    }

    private static DimensionFichaDto CrearDimension(decimal i1, decimal i2, decimal i3) =>
        new(i1, i2, i3, Math.Round((i1 + i2 + i3) / 3, Decimales));
}
