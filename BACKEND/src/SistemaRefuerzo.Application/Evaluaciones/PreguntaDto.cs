namespace SistemaRefuerzo.Application.Evaluaciones;

public record OpcionDto(Guid Id, string Texto);

public record PreguntaDto(Guid Id, string Enunciado, List<OpcionDto> Opciones);