using Microsoft.EntityFrameworkCore;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;
using SistemaRefuerzo.Domain.Enums;
using SistemaRefuerzo.Domain.InferenceEngine.Reglas;

namespace SistemaRefuerzo.Infrastructure.Persistence.Seed;

/// <summary>
/// Carga los datos mínimos para operar el sistema: los dos temas de la primera versión
/// (Porcentajes I y II), un banco de preguntas por nivel y subtema, la Base de Conocimiento
/// (las 4 reglas del documento de alcance) y un alumno de prueba.
/// </summary>
public static class DbInitializer
{
    private const int CantidadPreguntasPorNivel = 10;

    private static readonly string[] SubtemasPorcentajesI =
    [
        "Concepto y cálculo básico",
        "Porcentaje de una cantidad",
        "Determinar qué porcentaje representa una cantidad",
        "Porcentajes sucesivos",
        "Aumentos",
        "Descuentos",
        "Problemas aplicados",
    ];

    private static readonly string[] SubtemasPorcentajesII =
    [
        "Descuentos sucesivos",
        "Aumentos sucesivos",
        "Porcentajes compuestos",
        "Variaciones porcentuales",
        "Aplicaciones de porcentajes",
        "Problemas de mayor nivel",
    ];

    public static async Task SeedAsync(AppDbContext dbContext, IPasswordHasher passwordHasher)
    {
        await dbContext.Database.MigrateAsync();

        var reglasEsperadas = new[]
        {
            new Regla(
                "Nivel básico por puntaje bajo",
                nameof(ReglaNivelBasico),
                "Puntaje < 50",
                "Asignar nivel Básico",
                prioridad: 10),
            new Regla(
                "Nivel intermedio por puntaje medio",
                nameof(ReglaNivelIntermedio),
                "Puntaje >= 50 Y Puntaje < 80",
                "Asignar nivel Intermedio",
                prioridad: 10),
            new Regla(
                "Nivel avanzado por puntaje alto",
                nameof(ReglaNivelAvanzado),
                "Puntaje >= 80",
                "Asignar nivel Avanzado",
                prioridad: 10),
            new Regla(
                "Refuerzo teórico por fallos consecutivos",
                nameof(ReglaRefuerzoTeorico),
                "FallosConsecutivos >= 3",
                "Mostrar teoría y ejercicios de refuerzo",
                prioridad: 20),
            new Regla(
                "Análisis de subtemas del diagnóstico",
                nameof(ReglaAnalisisSubtemas),
                "Existe desempeño registrado por subtema",
                "Clasificar cada subtema como dominado o con dificultad",
                prioridad: 15),
            new Regla(
                "Subir dificultad por aciertos consecutivos en un subtema",
                nameof(ReglaSubirDificultadPorSubtema),
                "AciertosConsecutivosSubtema >= 3 (práctica libre)",
                "Sugerir subir la dificultad del subtema",
                prioridad: 10),
            new Regla(
                "Bajar dificultad por fallos consecutivos en un subtema",
                nameof(ReglaBajarDificultadPorSubtema),
                "FallosConsecutivosSubtema >= 3 (práctica libre)",
                "Sugerir bajar la dificultad del subtema",
                prioridad: 10),
        };

        var nombresDeClaseExistentes = (await dbContext.Reglas.Select(r => r.NombreClaseRegla).ToListAsync())
            .ToHashSet();
        var reglasNuevas = reglasEsperadas.Where(r => !nombresDeClaseExistentes.Contains(r.NombreClaseRegla));
        dbContext.Reglas.AddRange(reglasNuevas);

        var temaCreado = false;
        if (!await dbContext.Temas.AnyAsync())
        {
            dbContext.Temas.AddRange(new Tema("Porcentajes I", orden: 1), new Tema("Porcentajes II", orden: 2));
            await dbContext.SaveChangesAsync();
            temaCreado = true;
        }

        var porcentajesI = await dbContext.Temas.SingleAsync(t => t.Nombre == "Porcentajes I");
        var porcentajesII = await dbContext.Temas.SingleAsync(t => t.Nombre == "Porcentajes II");

        await ReemplazarBancoSiEsLegadoAsync(dbContext, porcentajesI.Id, "Porcentajes I", SubtemasPorcentajesI);
        await ReemplazarBancoSiEsLegadoAsync(dbContext, porcentajesII.Id, "Porcentajes II", SubtemasPorcentajesII);

        await SembrarContenidoTeoricoAsync(dbContext, porcentajesI.Id, porcentajesII.Id);
        await SembrarVideosApoyoAsync(dbContext, porcentajesI.Id, porcentajesII.Id);

        if (temaCreado && !await dbContext.Usuarios.AnyAsync())
        {
            var usuarioAlumno = new Usuario("alumno@colegio.edu.pe", passwordHasher.Hashear("Alumno123!"), Rol.Alumno);
            var alumno = new Alumno(usuarioAlumno.Id, "Ana", "Torres", "3ro de Secundaria");

            dbContext.Usuarios.Add(usuarioAlumno);
            dbContext.Alumnos.Add(alumno);
        }

        if (!await dbContext.Docentes.AnyAsync())
        {
            var usuarioDocente = new Usuario("docente@colegio.edu.pe", passwordHasher.Hashear("Docente123!"), Rol.Docente);
            var docente = new Docente(usuarioDocente.Id, "Carlos", "Ramírez");

            dbContext.Usuarios.Add(usuarioDocente);
            dbContext.Docentes.Add(docente);
        }

        if (!await dbContext.Usuarios.AnyAsync(u => u.Rol == Rol.Administrador))
        {
            var usuarioAdmin = new Usuario("admin@colegio.edu.pe", passwordHasher.Hashear("Admin123!"), Rol.Administrador);
            dbContext.Usuarios.Add(usuarioAdmin);
        }

        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Siembra el contenido teórico administrable (introducción por nivel + refuerzo por
    /// subtema) si aún no existe esa combinación Tema+Tipo+Clave — no duplica en reinicios
    /// posteriores ni pisa contenido ya editado desde el panel de admin.
    /// </summary>
    private static async Task SembrarContenidoTeoricoAsync(AppDbContext dbContext, Guid porcentajesIId, Guid porcentajesIIId)
    {
        var existentes = await dbContext.ContenidosTeoricos
            .Select(c => new { c.TemaId, c.Tipo, c.Clave })
            .ToListAsync();
        var existe = existentes.ToHashSet();

        var contenidoNivel = new (TipoContenidoTeorico Tipo, string Clave, string Titulo, string[] Parrafos)[]
        {
            (TipoContenidoTeorico.NivelGeneral, nameof(NivelDesempeno.Basico), "Tanto por ciento de un número",
            [
                "El a% de un número N se calcula como (a ÷ 100) × N.",
                "Ejemplo: el 15% de 80 = (15 ÷ 100) × 80 = 12.",
                "El \"tanto por cuanto\" funciona igual pero con otra base: el 3 por 20 de 1000 = (3 ÷ 20) × 1000 = 150.",
                "El tanto por mil (‰) se calcula dividiendo entre 1000 en vez de 100: el 3‰ de 7000 = (3 ÷ 1000) × 7000 = 21.",
            ]),
            (TipoContenidoTeorico.NivelGeneral, nameof(NivelDesempeno.Intermedio), "Operaciones con porcentajes",
            [
                "Aumentar N en su x%: N + x%N = (100 + x)% N.",
                "Disminuir N en su x%: N − x%N = (100 − x)% N.",
                "¿Qué porcentaje es A de B? Se usa una regla de tres: x = (A × 100%) ÷ B.",
                "Porcentaje de porcentaje: calcular el 20% del 8% de 20 000 es multiplicar (20/100) × (8/100) × 20 000 = 320.",
            ]),
            (TipoContenidoTeorico.NivelGeneral, nameof(NivelDesempeno.Avanzado), "Aumentos y descuentos sucesivos",
            [
                "Dos o más cambios porcentuales sucesivos se combinan multiplicando sus factores, no sumando los porcentajes.",
                "Ejemplo: dos descuentos sucesivos del 20% y 30% equivalen a aplicar 80% × 70% = 56% del valor original, es decir, un descuento único del 44%.",
                "Ejemplo: dos aumentos sucesivos del 10% y 20% equivalen a 110% × 120% = 132%, es decir, un aumento único del 32%.",
                "La misma idea se aplica a problemas de áreas: si la base de un rectángulo aumenta 30% y la altura 20%, el área varía como 130% × 120% = 156%, un aumento del 56%.",
            ]),
        };

        var contenidoSubtema = new (string Clave, string Titulo, string[] Parrafos)[]
        {
            ("Concepto y cálculo básico", "Concepto y cálculo básico de porcentaje",
            [
                "Un porcentaje es una forma de expresar una cantidad como parte de 100. El símbolo % significa \"de cada 100\".",
                "Para calcular el a% de un número N, se multiplica: (a ÷ 100) × N.",
                "Ejemplo: el 20% de 150 = (20 ÷ 100) × 150 = 30.",
            ]),
            ("Porcentaje de una cantidad", "Porcentaje de una cantidad",
            [
                "Calcular el porcentaje de una cantidad es lo mismo que calcular una fracción de ella: el a% equivale a la fracción a/100.",
                "Ejemplo: el 35% de 200 = (35/100) × 200 = 70.",
                "Truco útil: el 10% de un número se obtiene corriendo la coma un lugar (dividiendo entre 10); el 50% es la mitad; el 25% es la cuarta parte.",
            ]),
            ("Determinar qué porcentaje representa una cantidad", "Qué porcentaje representa una cantidad de otra",
            [
                "Para saber qué porcentaje es A de B, se usa: (A ÷ B) × 100.",
                "Ejemplo: ¿qué porcentaje es 45 de 180? (45 ÷ 180) × 100 = 25%.",
                "Es el proceso inverso a calcular \"el a% de B\": aquí ya conoces la parte y el total, y buscas el porcentaje.",
            ]),
            ("Porcentajes sucesivos", "Porcentajes sucesivos",
            [
                "Cuando un valor sufre dos cambios porcentuales seguidos (por ejemplo, sube y luego baja), NO se pueden sumar ni restar los porcentajes directamente.",
                "Hay que aplicar cada cambio uno después del otro sobre el resultado anterior.",
                "Ejemplo: un precio de S/200 sube 20% (pasa a 240) y luego baja 10% (240 − 24 = 216). El resultado NO es lo mismo que aplicar \"sube 10% neto\".",
            ]),
            ("Aumentos", "Aumentos porcentuales",
            [
                "Aumentar un valor N en un a% significa sumarle el a% de N: N + (a/100)×N, que equivale a multiplicar N × (1 + a/100).",
                "Ejemplo: aumentar S/300 en 15% = 300 × 1.15 = 345.",
                "Cuidado: \"aumentar en 15%\" no es lo mismo que \"aumentar a 15%\" del valor original.",
            ]),
            ("Descuentos", "Descuentos porcentuales",
            [
                "Aplicar un descuento del a% a un valor N significa restarle el a% de N: N − (a/100)×N, que equivale a multiplicar N × (1 − a/100).",
                "Ejemplo: un producto de S/300 con 20% de descuento cuesta 300 × 0.80 = 240.",
                "El precio final siempre se puede calcular multiplicando directamente por (1 − descuento), sin calcular primero el descuento por separado.",
            ]),
            ("Problemas aplicados", "Problemas aplicados de porcentajes",
            [
                "En problemas de la vida real, identifica primero cuál es el TOTAL (el 100%) y cuál es la PARTE de la que se habla.",
                "Palabras clave: \"gastó\", \"ahorró\", \"le quedó\" suelen indicar que debes calcular una parte del total, o el total a partir de una parte conocida.",
                "Ejemplo: si Juan gastó el 30% de sus ahorros y le quedaron S/140, esos S/140 representan el 70% del total. El total = 140 ÷ 0.70 = 200.",
            ]),
            ("Descuentos sucesivos", "Descuentos sucesivos",
            [
                "Dos descuentos sucesivos no se suman: se aplican uno tras otro, cada uno sobre el precio ya rebajado por el anterior.",
                "Ejemplo: dos descuentos del 20% y 10% sobre S/500: primero 500 × 0.80 = 400, luego 400 × 0.90 = 360 (no es lo mismo que un descuento único del 30%, que daría 350).",
                "El descuento único equivalente se obtiene multiplicando los factores: 0.80 × 0.90 = 0.72, es decir, un descuento total del 28%.",
            ]),
            ("Aumentos sucesivos", "Aumentos sucesivos",
            [
                "Igual que los descuentos, dos aumentos sucesivos se combinan multiplicando sus factores, no sumando los porcentajes.",
                "Ejemplo: dos aumentos del 10% y 20% equivalen a multiplicar por 1.10 × 1.20 = 1.32, es decir, un aumento único del 32% (no 30%).",
            ]),
            ("Porcentajes compuestos", "Porcentajes compuestos (crecimiento en varios periodos)",
            [
                "Cuando un valor crece el mismo porcentaje durante varios periodos (por ejemplo, años), cada periodo se calcula sobre el resultado del periodo anterior, no sobre el valor original.",
                "Ejemplo: un capital de S/1000 crece 10% cada año durante 2 años: año 1 → 1000 × 1.10 = 1100; año 2 → 1100 × 1.10 = 1210 (no 1200).",
                "Esto es la misma idea que los aumentos sucesivos, aplicada varias veces con el mismo porcentaje.",
            ]),
            ("Variaciones porcentuales", "Variaciones porcentuales",
            [
                "La variación porcentual entre un valor inicial y uno final se calcula como: ((Valor final − Valor inicial) ÷ Valor inicial) × 100.",
                "Si el resultado es positivo, hubo un aumento; si es negativo, hubo una disminución.",
                "Ejemplo: un valor pasa de 200 a 250. Variación = ((250 − 200) ÷ 200) × 100 = 25%.",
            ]),
            ("Aplicaciones de porcentajes", "Aplicaciones de porcentajes (impuestos, recargos)",
            [
                "Los impuestos y recargos funcionan como un aumento porcentual sobre un precio base: precio final = precio base × (1 + tasa).",
                "Ejemplo: un producto de S/100 con 18% de impuesto cuesta 100 × 1.18 = 118.",
                "Si el precio final ya incluye el impuesto y se pide el precio base, hay que dividir en vez de multiplicar: precio base = precio final ÷ (1 + tasa).",
            ]),
            ("Problemas de mayor nivel", "Problemas de mayor nivel (combinando varias operaciones)",
            [
                "Estos problemas combinan dos o más operaciones porcentuales en un solo enunciado: por ejemplo, un descuento seguido de un recargo.",
                "La clave es resolver en el orden en que ocurren los hechos, aplicando cada operación sobre el resultado de la anterior — igual que en descuentos o aumentos sucesivos.",
                "Ejemplo: un producto de S/400 tiene 25% de descuento y luego 10% de recargo por pagar con tarjeta: 400 × 0.75 = 300, luego 300 × 1.10 = 330.",
            ]),
        };

        void AgregarSiNoExiste(Guid temaId, TipoContenidoTeorico tipo, string clave, string titulo, string[] parrafos)
        {
            if (existe.Contains(new { TemaId = temaId, Tipo = tipo, Clave = clave }))
                return;

            dbContext.ContenidosTeoricos.Add(new ContenidoTeorico(temaId, tipo, clave, titulo, parrafos));
        }

        foreach (var temaId in new[] { porcentajesIId, porcentajesIIId })
            foreach (var (tipo, clave, titulo, parrafos) in contenidoNivel)
                AgregarSiNoExiste(temaId, tipo, clave, titulo, parrafos);

        foreach (var (clave, titulo, parrafos) in contenidoSubtema.Take(SubtemasPorcentajesI.Length))
            AgregarSiNoExiste(porcentajesIId, TipoContenidoTeorico.Subtema, clave, titulo, parrafos);

        foreach (var (clave, titulo, parrafos) in contenidoSubtema.Skip(SubtemasPorcentajesI.Length))
            AgregarSiNoExiste(porcentajesIIId, TipoContenidoTeorico.Subtema, clave, titulo, parrafos);

        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Siembra un video de apoyo introductorio por tema si aún no tiene ninguno — solo para
    /// que una instalación nueva no arranque con el módulo de videos vacío. No reemplaza
    /// videos ya cargados ni por seed ni por el admin.
    /// </summary>
    private static async Task SembrarVideosApoyoAsync(AppDbContext dbContext, Guid porcentajesIId, Guid porcentajesIIId)
    {
        async Task AgregarSiNoTieneVideos(Guid temaId, string titulo, string url)
        {
            if (await dbContext.VideosApoyo.AnyAsync(v => v.TemaId == temaId))
                return;

            dbContext.VideosApoyo.Add(new VideoApoyo(temaId, titulo, url));
        }

        await AgregarSiNoTieneVideos(porcentajesIId, "Qué es el porcentaje", "https://www.youtube.com/watch?v=2UmRmPVq8-M");
        await AgregarSiNoTieneVideos(porcentajesIIId, "Aumentos y descuentos sucesivos", "https://www.youtube.com/watch?v=9zY_qPJb-KM");

        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Si el tema no tiene preguntas, o tiene preguntas de una versión anterior del banco
    /// (sin subtema asignado), reemplaza todo su banco de preguntas por el generado actualmente.
    /// Preguntas ya migradas (con subtema) no se tocan para no perder ediciones hechas desde el panel de admin.
    /// </summary>
    private static async Task ReemplazarBancoSiEsLegadoAsync(AppDbContext dbContext, Guid temaId, string nombreTema, string[] subtemas)
    {
        var preguntasExistentes = await dbContext.Preguntas.Where(p => p.TemaId == temaId).ToListAsync();
        var esLegado = preguntasExistentes.Count == 0 || preguntasExistentes.Any(p => string.IsNullOrEmpty(p.Subtema));
        if (!esLegado)
            return;

        dbContext.Preguntas.RemoveRange(preguntasExistentes);
        dbContext.Preguntas.AddRange(CrearBancoDePreguntas(temaId, nombreTema, subtemas));
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Genera 10 preguntas por cada nivel de dificultad (30 por tema), repartidas de forma
    /// rotativa entre los subtemas del tema, para que tanto la prueba diagnóstica (que toma
    /// preguntas de todos los niveles) como las evaluaciones por nivel dispongan de banco
    /// suficiente y el motor de reglas pueda identificar fallas por subtema.
    /// </summary>
    private static List<Pregunta> CrearBancoDePreguntas(Guid temaId, string nombreTema, string[] subtemas)
    {
        var preguntas = new List<Pregunta>();

        foreach (var nivel in new[] { NivelDesempeno.Basico, NivelDesempeno.Intermedio, NivelDesempeno.Avanzado })
        {
            for (var i = 0; i < CantidadPreguntasPorNivel; i++)
            {
                var subtema = subtemas[i % subtemas.Length];
                preguntas.Add(GenerarPregunta(temaId, nombreTema, subtema, nivel, i));
            }
        }

        return preguntas;
    }

    private static Pregunta GenerarPregunta(Guid temaId, string nombreTema, string subtema, NivelDesempeno nivel, int indice)
    {
        // N (base) y P (porcentaje) crecen según el nivel para variar la dificultad;
        // ambos se eligen múltiplos de 100 y 5 respectivamente para que las operaciones
        // den siempre números enteros exactos.
        var (n, p1, p2) = nivel switch
        {
            NivelDesempeno.Basico => (100 + indice * 100, 10 + (indice % 5) * 10, 20 + (indice % 4) * 10),
            NivelDesempeno.Intermedio => (200 + indice * 100, 15 + (indice % 6) * 5, 10 + (indice % 5) * 10),
            _ => (500 + indice * 100, 20 + (indice % 4) * 10, 15 + (indice % 5) * 5),
        };

        string enunciado;
        int correcta;
        string explicacion;

        switch (subtema)
        {
            case "Concepto y cálculo básico":
            case "Porcentaje de una cantidad":
                correcta = n * p1 / 100;
                enunciado = $"[{nombreTema}] ¿A cuánto equivale el {p1}% de {n}?";
                explicacion = $"El {p1}% de {n} se calcula como {n} × {p1} ÷ 100 = {correcta}.";
                break;

            case "Determinar qué porcentaje representa una cantidad":
                correcta = p1;
                var parte = n * p1 / 100;
                enunciado = $"[{nombreTema}] ¿Qué porcentaje representa {parte} de {n}?";
                explicacion = $"El porcentaje se obtiene con ({parte} ÷ {n}) × 100 = {p1}%.";
                break;

            case "Aumentos":
                correcta = n + (n * p1 / 100);
                enunciado = $"[{nombreTema}] Un producto cuesta S/{n} y aumenta {p1}%. ¿Cuál es el nuevo precio?";
                explicacion = $"Precio final = {n} + ({n} × {p1} ÷ 100) = {correcta}.";
                break;

            case "Descuentos":
                correcta = n - (n * p1 / 100);
                enunciado = $"[{nombreTema}] Un producto cuesta S/{n} y tiene un descuento de {p1}%. ¿Cuál es el precio final?";
                explicacion = $"Precio final = {n} − ({n} × {p1} ÷ 100) = {correcta}.";
                break;

            case "Porcentajes sucesivos":
            case "Aumentos sucesivos":
            {
                var intermedio = n + (n * p1 / 100);
                correcta = intermedio - (intermedio * p2 / 100);
                enunciado = $"[{nombreTema}] Un precio de S/{n} sube {p1}% y luego baja {p2}%. ¿Cuál es el precio final?";
                explicacion = $"Primero sube a {intermedio}, luego baja {p2}%: {intermedio} − ({intermedio} × {p2} ÷ 100) = {correcta}.";
                break;
            }

            case "Descuentos sucesivos":
            {
                var intermedio = n - (n * p1 / 100);
                correcta = intermedio - (intermedio * p2 / 100);
                enunciado = $"[{nombreTema}] Un producto de S/{n} recibe dos descuentos sucesivos de {p1}% y {p2}%. ¿Cuál es el precio final?";
                explicacion = $"Primer descuento: {n} − ({n} × {p1} ÷ 100) = {intermedio}. Segundo descuento: {intermedio} − ({intermedio} × {p2} ÷ 100) = {correcta}.";
                break;
            }

            case "Porcentajes compuestos":
            {
                var añoUno = n + (n * p1 / 100);
                correcta = añoUno + (añoUno * p1 / 100);
                enunciado = $"[{nombreTema}] Un capital de S/{n} crece {p1}% cada año durante 2 años. ¿Cuál es el monto final?";
                explicacion = $"Año 1: {n} + ({n} × {p1} ÷ 100) = {añoUno}. Año 2: {añoUno} + ({añoUno} × {p1} ÷ 100) = {correcta}.";
                break;
            }

            case "Variaciones porcentuales":
            {
                var valorFinal = n + (n * p1 / 100);
                correcta = p1;
                enunciado = $"[{nombreTema}] Un valor pasa de {n} a {valorFinal}. ¿Cuál es la variación porcentual?";
                explicacion = $"Variación = (({valorFinal} − {n}) ÷ {n}) × 100 = {p1}%.";
                break;
            }

            case "Aplicaciones de porcentajes":
                correcta = n + (n * p1 / 100);
                enunciado = $"[{nombreTema}] El precio de un producto sin impuestos es S/{n} y se le agrega {p1}% de impuesto. ¿Cuál es el precio final?";
                explicacion = $"Precio final = {n} + ({n} × {p1} ÷ 100) = {correcta}.";
                break;

            case "Problemas de mayor nivel":
            {
                var conDescuento = n - (n * p1 / 100);
                correcta = conDescuento + (conDescuento * p2 / 100);
                enunciado = $"[{nombreTema}] Juan compra un producto de S/{n} con {p1}% de descuento y luego paga {p2}% de recargo por tarjeta sobre el precio con descuento. ¿Cuánto paga en total?";
                explicacion = $"Con descuento: {n} − ({n} × {p1} ÷ 100) = {conDescuento}. Con recargo: {conDescuento} + ({conDescuento} × {p2} ÷ 100) = {correcta}.";
                break;
            }

            default: // "Problemas aplicados"
                correcta = n * p1 / 100;
                enunciado = $"[{nombreTema}] Ana tiene S/{n} de ahorros y gasta el {p1}% en útiles escolares. ¿Cuánto gastó?";
                explicacion = $"Gasto = {n} × {p1} ÷ 100 = {correcta}.";
                break;
        }

        var pregunta = new Pregunta(temaId, enunciado, nivel, subtema, TipoPregunta.OpcionMultiple, explicacion, puntaje: 1);

        var incorrectas = new[] { correcta + 10, Math.Max(correcta - 10, 1), correcta + 20 }
            .Where(valor => valor != correcta)
            .Distinct()
            .Take(3)
            .ToList();

        pregunta.AgregarOpcion(correcta.ToString(), esCorrecta: true);
        foreach (var incorrecta in incorrectas)
            pregunta.AgregarOpcion(incorrecta.ToString(), esCorrecta: false);

        return pregunta;
    }
}
