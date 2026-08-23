using MediatR;
using SistemaRefuerzo.Domain.Enums;

namespace SistemaRefuerzo.Application.Admin.Preguntas;

public record CrearPreguntaCommand(
    Guid TemaId,
    string Enunciado,
    NivelDesempeno NivelDificultad,
    List<OpcionInput> Opciones) : IRequest<Guid>;
