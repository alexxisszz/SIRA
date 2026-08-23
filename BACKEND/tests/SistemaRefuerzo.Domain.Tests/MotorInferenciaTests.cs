using SistemaRefuerzo.Domain.Enums;
using SistemaRefuerzo.Domain.InferenceEngine;
using SistemaRefuerzo.Domain.InferenceEngine.Reglas;

namespace SistemaRefuerzo.Domain.Tests;

public class MotorInferenciaTests
{
    private static IReadOnlyCollection<IRegla> TodasLasReglas() =>
    [
        new ReglaNivelBasico(),
        new ReglaNivelIntermedio(),
        new ReglaNivelAvanzado(),
        new ReglaRefuerzoTeorico(),
    ];

    [Theory]
    [InlineData(0, NivelDesempeno.Basico)]
    [InlineData(49, NivelDesempeno.Basico)]
    [InlineData(50, NivelDesempeno.Intermedio)]
    [InlineData(79, NivelDesempeno.Intermedio)]
    [InlineData(80, NivelDesempeno.Avanzado)]
    [InlineData(100, NivelDesempeno.Avanzado)]
    public void Debe_asignar_el_nivel_correspondiente_segun_el_puntaje(int puntaje, NivelDesempeno nivelEsperado)
    {
        var hechos = new BaseDeHechos();
        hechos.Establecer(ClavesHechos.Puntaje, puntaje);
        hechos.Establecer(ClavesHechos.FallosConsecutivos, 0);

        new MotorInferencia().Ejecutar(hechos, TodasLasReglas());

        Assert.Equal(nivelEsperado, hechos.Obtener<NivelDesempeno>(ClavesHechos.NivelAsignado));
    }

    [Fact]
    public void Debe_requerir_refuerzo_teorico_cuando_hay_tres_o_mas_fallos_consecutivos()
    {
        var hechos = new BaseDeHechos();
        hechos.Establecer(ClavesHechos.Puntaje, 40);
        hechos.Establecer(ClavesHechos.FallosConsecutivos, 3);

        new MotorInferencia().Ejecutar(hechos, TodasLasReglas());

        Assert.True(hechos.Obtener<bool>(ClavesHechos.RequiereRefuerzoTeorico));
    }

    [Fact]
    public void No_debe_requerir_refuerzo_teorico_cuando_hay_menos_de_tres_fallos_consecutivos()
    {
        var hechos = new BaseDeHechos();
        hechos.Establecer(ClavesHechos.Puntaje, 90);
        hechos.Establecer(ClavesHechos.FallosConsecutivos, 2);

        new MotorInferencia().Ejecutar(hechos, TodasLasReglas());

        Assert.False(hechos.Contiene(ClavesHechos.RequiereRefuerzoTeorico));
    }

    [Fact]
    public void Cada_regla_se_dispara_como_maximo_una_vez_por_ejecucion()
    {
        var contadorDisparos = 0;
        var reglaDeConteo = new ReglaDeConteo(() => contadorDisparos++);

        var hechos = new BaseDeHechos();
        hechos.Establecer("Contador", 0);

        new MotorInferencia().Ejecutar(hechos, [reglaDeConteo]);

        Assert.Equal(1, contadorDisparos);
    }

    private class ReglaDeConteo(Action alDispararse) : IRegla
    {
        public string Nombre => nameof(ReglaDeConteo);
        public int Prioridad => 0;
        public bool Evaluar(BaseDeHechos hechos) => true;
        public void Ejecutar(BaseDeHechos hechos) => alDispararse();
    }
}