namespace SistemaRefuerzo.Application.Admin;

public record AdminReglaDto(
    Guid Id,
    string Nombre,
    string NombreClaseRegla,
    string DescripcionCondicion,
    string DescripcionConclusion,
    int Prioridad,
    bool Activa);
