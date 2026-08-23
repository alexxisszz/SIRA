using MediatR;
using SistemaRefuerzo.Application.Common.Exceptions;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Application.Recomendaciones;
using SistemaRefuerzo.Domain.Entities;
using SistemaRefuerzo.Domain.InferenceEngine;

namespace SistemaRefuerzo.Application.Evaluaciones;

public class FinalizarEvaluacionCommandHandler(
    IEvaluacionRepository evaluacionRepository,
    ITemaRepository temaRepository,
    IResultadoRepository resultadoRepository,
    IReglaRepository reglaRepository,
    IRecomendacionRepository recomendacionRepository,
    GeneradorDeRecomendacion generadorDeRecomendacion,
    IUnitOfWork unitOfWork) : IRequestHandler<FinalizarEvaluacionCommand, Guid>
{
    public async Task<Guid> Handle(FinalizarEvaluacionCommand request, CancellationToken cancellationToken)
    {
        var evaluacion = await evaluacionRepository.ObtenerPorIdAsync(request.EvaluacionId, cancellationToken)
            ?? throw new NotFoundException(nameof(Evaluacion), request.EvaluacionId);

        var tema = await temaRepository.ObtenerPorIdAsync(evaluacion.TemaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Tema), evaluacion.TemaId);

        // 1. Las respuestas del alumno se traducen a HECHOS (Puntaje, FallosConsecutivos).
        var resultado = evaluacion.Finalizar();
        resultadoRepository.Agregar(resultado);

        var hechos = new BaseDeHechos();
        hechos.Establecer(ClavesHechos.Puntaje, resultado.Puntaje);
        hechos.Establecer(ClavesHechos.FallosConsecutivos, resultado.FallosConsecutivos);

        // 2. Se cargan las reglas activas de la BASE DE CONOCIMIENTO y se resuelven a su IRegla ejecutable.
        var reglasActivas = await reglaRepository.ObtenerActivasAsync(cancellationToken);
        var reglasEjecutables = reglasActivas.Select(r => RegistroReglas.Resolver(r.NombreClaseRegla));

        // 3. El MOTOR DE INFERENCIA evalúa los hechos contra las reglas y deja conclusiones en la base de hechos.
        new MotorInferencia().Ejecutar(hechos, reglasEjecutables);

        // 4. Las conclusiones se traducen en una recomendación concreta para el alumno.
        var recomendacion = await generadorDeRecomendacion.GenerarAsync(resultado, tema, hechos, cancellationToken);
        recomendacionRepository.Agregar(recomendacion);

        await unitOfWork.GuardarCambiosAsync(cancellationToken);

        return recomendacion.Id;
    }
}