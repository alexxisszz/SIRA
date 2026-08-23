using MediatR;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Admin.Preguntas;

public record ActualizarPreguntaCommand(
    Guid PreguntaId,
    string Enunciado,
    NivelDesempeno NivelDificultad,
    List<OpcionInput> Opciones) : IRequest;
