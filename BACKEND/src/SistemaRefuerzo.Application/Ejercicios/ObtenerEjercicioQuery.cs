using MediatR;
using SistemaRefuerzo.Application.Evaluaciones;

namespace SistemaRefuerzo.Application.Ejercicios;

public record ObtenerEjercicioQuery(Guid PreguntaId) : IRequest<PreguntaDto>;
