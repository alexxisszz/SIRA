using MediatR;

namespace SistemaRefuerzo.Application.Admin.Reglas;

public record ActualizarReglaCommand(
    Guid ReglaId,
    string Nombre,
    string DescripcionCondicion,
    string DescripcionConclusion,
    int Prioridad) : IRequest;
