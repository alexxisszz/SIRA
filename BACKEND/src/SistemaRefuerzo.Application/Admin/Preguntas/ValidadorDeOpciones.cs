using SistemaRefuerzo.Application.Common.Exceptions;

namespace SistemaRefuerzo.Application.Admin.Preguntas;

public static class ValidadorDeOpciones
{
    private const int MinimoOpciones = 2;

    public static void Validar(List<OpcionInput> opciones)
    {
        if (opciones.Count < MinimoOpciones)
            throw new ReglaDeNegocioException($"La pregunta debe tener al menos {MinimoOpciones} opciones.");

        if (opciones.Count(o => o.EsCorrecta) != 1)
            throw new ReglaDeNegocioException("La pregunta debe tener exactamente una opción correcta.");
    }
}
