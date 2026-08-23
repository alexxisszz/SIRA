using Microsoft.EntityFrameworkCore;
using SistemaRefuerzo.Domain.Entities;

namespace SistemaRefuerzo.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Alumno> Alumnos => Set<Alumno>();
    public DbSet<Docente> Docentes => Set<Docente>();
    public DbSet<Tema> Temas => Set<Tema>();
    public DbSet<Pregunta> Preguntas => Set<Pregunta>();
    public DbSet<OpcionPregunta> OpcionesPregunta => Set<OpcionPregunta>();
    public DbSet<Evaluacion> Evaluaciones => Set<Evaluacion>();
    public DbSet<RespuestaAlumno> RespuestasAlumno => Set<RespuestaAlumno>();
    public DbSet<Resultado> Resultados => Set<Resultado>();
    public DbSet<Regla> Reglas => Set<Regla>();
    public DbSet<Recomendacion> Recomendaciones => Set<Recomendacion>();
    public DbSet<EjercicioRecomendado> EjerciciosRecomendados => Set<EjercicioRecomendado>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}