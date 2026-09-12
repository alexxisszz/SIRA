using MediatR;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Ejercicios;

public record IntentoResultadoDto(
    bool EsCorrecta,
    Guid OpcionCorrectaId,
    string? Explicacion,
    string Subtema,
    AccionDificultad AccionSugerida);

public record RegistrarIntentoCommand(Guid UsuarioId, Guid PreguntaId, Guid OpcionSeleccionadaId) : IRequest<IntentoResultadoDto>;
