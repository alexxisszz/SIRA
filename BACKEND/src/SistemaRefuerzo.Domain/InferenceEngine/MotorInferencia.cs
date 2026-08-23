namespace SistemaRefuerzo.Domain.InferenceEngine;

/// <summary>
/// Encadenamiento hacia adelante (forward chaining): en cada ciclo evalúa las reglas
/// pendientes por prioridad; cada regla se dispara como máximo una vez por ejecución,
/// lo que garantiza la terminación del algoritmo.
/// </summary>
public class MotorInferencia
{
    public void Ejecutar(BaseDeHechos hechos, IEnumerable<IRegla> reglas)
    {
        var reglasOrdenadas = reglas.OrderByDescending(r => r.Prioridad).ToList();
        var reglasDisparadas = new HashSet<string>();

        bool huboCambios;
        do
        {
            huboCambios = false;
            foreach (var regla in reglasOrdenadas)
            {
                if (reglasDisparadas.Contains(regla.Nombre))
                    continue;

                if (!regla.Evaluar(hechos))
                    continue;

                regla.Ejecutar(hechos);
                reglasDisparadas.Add(regla.Nombre);
                huboCambios = true;
            }
        } while (huboCambios);
    }
}