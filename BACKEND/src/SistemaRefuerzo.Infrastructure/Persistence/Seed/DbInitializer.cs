using Microsoft.EntityFrameworkCore;
using SistemaRefuerzo.Application.Common.Interfaces;
using SistemaRefuerzo.Domain.Entities;
using SistemaRefuerzo.Domain.Enums;
using SistemaRefuerzo.Domain.InferenceEngine.Reglas;

namespace SistemaRefuerzo.Infrastructure.Persistence.Seed;

/// <summary>
/// Carga los datos mínimos para operar el sistema: los dos temas de la primera versión
/// (Porcentajes I y II), un banco de preguntas por nivel, la Base de Conocimiento
/// (las 4 reglas del documento de alcance) y un alumno de prueba.
/// </summary>
public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext dbContext, IPasswordHasher passwordHasher)
    {
        await dbContext.Database.MigrateAsync();

        if (!await dbContext.Reglas.AnyAsync())
        {
            dbContext.Reglas.AddRange(
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
                    prioridad: 20));
        }

        if (!await dbContext.Temas.AnyAsync())
        {
            var porcentajesI = new Tema("Porcentajes I", orden: 1);
            var porcentajesII = new Tema("Porcentajes II", orden: 2);

            dbContext.Temas.AddRange(porcentajesI, porcentajesII);
            dbContext.Preguntas.AddRange(
                CrearBancoDePreguntas(porcentajesI.Id, "Porcentajes I")
                    .Concat(CrearBancoDePreguntas(porcentajesII.Id, "Porcentajes II")));

            if (!await dbContext.Usuarios.AnyAsync())
            {
                var usuarioAlumno = new Usuario("alumno@colegio.edu.pe", passwordHasher.Hashear("Alumno123!"), Rol.Alumno);
                var alumno = new Alumno(usuarioAlumno.Id, "Ana", "Torres", "3ro de Secundaria");

                dbContext.Usuarios.Add(usuarioAlumno);
                dbContext.Alumnos.Add(alumno);
            }
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

    private static List<Pregunta> CrearBancoDePreguntas(Guid temaId, string nombreTema)
    {
        var preguntas = new List<Pregunta>();

        void AgregarPregunta(NivelDesempeno nivel, string enunciado, string correcta, params string[] incorrectas)
        {
            var pregunta = new Pregunta(temaId, enunciado, nivel);
            pregunta.AgregarOpcion(correcta, esCorrecta: true);
            foreach (var incorrecta in incorrectas)
                pregunta.AgregarOpcion(incorrecta, esCorrecta: false);

            preguntas.Add(pregunta);
        }

        AgregarPregunta(NivelDesempeno.Basico, $"[{nombreTema}] ¿A cuánto equivale el 50% de 200?", "100", "50", "150", "200");
        AgregarPregunta(NivelDesempeno.Basico, $"[{nombreTema}] ¿A cuánto equivale el 10% de 90?", "9", "10", "18", "90");
        AgregarPregunta(NivelDesempeno.Intermedio, $"[{nombreTema}] Si el 25% de un número es 40, ¿cuál es el número?", "160", "100", "10", "40");
        AgregarPregunta(NivelDesempeno.Intermedio, $"[{nombreTema}] Un producto de S/80 tiene 15% de descuento. ¿Cuál es el precio final?", "68", "65", "12", "80");
        AgregarPregunta(NivelDesempeno.Avanzado, $"[{nombreTema}] Un precio aumenta 20% y luego baja 20%. ¿Cuál es el cambio neto respecto al precio original?", "Disminuye 4%", "No cambia", "Aumenta 4%", "Disminuye 20%");
        AgregarPregunta(NivelDesempeno.Avanzado, $"[{nombreTema}] Si María gastó el 60% de sus ahorros y le quedaron S/120, ¿cuánto tenía inicialmente?", "300", "200", "180", "72");

        return preguntas;
    }
}