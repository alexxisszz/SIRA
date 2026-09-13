using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Application.Recomendaciones;
using SistemaRefuerzo.Domain.Entities;
using SistemaRefuerzo.Domain.InferenceEngine;

namespace SistemaRefuerzo.Application.Evaluaciones;

public class FinalizarEvaluacionCommandHandler(
    IUsuarioRepository usuarioRepository,
    IEvaluacionRepository evaluacionRepository,
    ITemaRepository temaRepository,
    IPreguntaRepository preguntaRepository,
    IResultadoRepository resultadoRepository,
    IReglaRepository reglaRepository,
    IRecomendacionRepository recomendacionRepository,
    GeneradorDeRecomendacion generadorDeRecomendacion,
    IUnitOfWork unitOfWork) : IRequestHandler<FinalizarEvaluacionCommand, Guid>
{
    public async Task<Guid> Handle(FinalizarEvaluacionCommand request, CancellationToken cancellationToken)
    {
        var alumno = await usuarioRepository.ObtenerAlumnoPorUsuarioIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new NotFoundException(nameof(Alumno), request.UsuarioId);

        var evaluacion = await evaluacionRepository.ObtenerPorIdAsync(request.EvaluacionId, cancellationToken)
            ?? throw new NotFoundException(nameof(Evaluacion), request.EvaluacionId);

        if (evaluacion.AlumnoId != alumno.Id)
            throw new NotFoundException(nameof(Evaluacion), request.EvaluacionId);

        var tema = await temaRepository.ObtenerPorIdAsync(evaluacion.TemaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Tema), evaluacion.TemaId);

        // 1. Las respuestas del alumno se traducen a HECHOS: Puntaje, FallosConsecutivos y,
        //    por cada subtema del tema, el % de aciertos obtenido en esta evaluación.
        Resultado resultado;
        try
        {
            resultado = evaluacion.Finalizar();
        }
        catch (InvalidOperationException ex)
        {
            throw new ReglaDeNegocioException(ex.Message);
        }

        resultadoRepository.Agregar(resultado);

        var preguntasDelTema = (await preguntaRepository.ObtenerPorTemaAsync(tema.Id, cancellationToken))
            .ToDictionary(p => p.Id);

        var desempenoPorSubtema = CalculadoraDesempenoPorSubtema.Calcular(evaluacion.Respuestas, preguntasDelTema);

        var hechos = new BaseDeHechos();
        hechos.Establecer(ClavesHechos.Puntaje, resultado.Puntaje);
        hechos.Establecer(ClavesHechos.FallosConsecutivos, resultado.FallosConsecutivos);
        hechos.Establecer(ClavesHechos.DesempenoPorSubtema, desempenoPorSubtema);

        // 2. Se cargan las reglas activas de la BASE DE CONOCIMIENTO y se resuelven a su IRegla ejecutable.
        var reglasActivas = await reglaRepository.ObtenerActivasAsync(cancellationToken);
        var reglasEjecutables = reglasActivas.Select(r => RegistroReglas.Resolver(r.NombreClaseRegla));

        // 3. El MOTOR DE INFERENCIA evalúa los hechos contra las reglas (nivel, refuerzo teórico
        //    y análisis de subtemas dominados/con dificultad) y deja conclusiones en la base de hechos.
        var reglasDisparadas = new MotorInferencia().Ejecutar(hechos, reglasEjecutables);

        // 4. Las conclusiones se traducen en una recomendación concreta para el alumno,
        //    guardando también qué reglas dispararon (trazabilidad para el panel docente).
        var recomendacion = await generadorDeRecomendacion.GenerarAsync(
            resultado, tema, preguntasDelTema, hechos, reglasDisparadas, cancellationToken);
        recomendacionRepository.Agregar(recomendacion);

        await unitOfWork.GuardarCambiosAsync(cancellationToken);

        return recomendacion.Id;
    }
}