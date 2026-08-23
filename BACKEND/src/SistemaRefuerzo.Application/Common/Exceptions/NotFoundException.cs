namespace SistemaRefuerzo.Application.Common.Exceptions;

public class NotFoundException(string entidad, object clave)
    : Exception($"'{entidad}' con clave '{clave}' no fue encontrado(a).");