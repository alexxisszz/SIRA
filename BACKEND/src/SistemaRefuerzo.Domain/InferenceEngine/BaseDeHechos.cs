namespace SistemaRefuerzo.Domain.InferenceEngine;

public class BaseDeHechos
{
    private readonly Dictionary<string, object> _hechos = new();

    public void Establecer(string clave, object valor) => _hechos[clave] = valor;

    public T Obtener<T>(string clave)
    {
        if (!_hechos.TryGetValue(clave, out var valor))
            throw new KeyNotFoundException($"No existe el hecho '{clave}' en la base de hechos.");

        return (T)valor;
    }

    public bool Contiene(string clave) => _hechos.ContainsKey(clave);
}