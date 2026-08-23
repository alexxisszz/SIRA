namespace SistemaRefuerzo.Domain.InferenceEngine;

public interface IRegla
{
    string Nombre { get; }
    int Prioridad { get; }
    bool Evaluar(BaseDeHechos hechos);
    void Ejecutar(BaseDeHechos hechos);
}