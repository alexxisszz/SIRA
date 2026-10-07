import type { NivelDesempeno } from '../../alumno/models/recomendacion.model';

export type EstadoAlumno = 'Sin evaluar' | 'En progreso' | 'Requiere apoyo' | 'Buen progreso';

export interface AlumnoResumen {
  alumnoId: string;
  nombres: string;
  apellidos: string;
  grado: string;
  evaluacionesRealizadas: number;
  nivelActual: NivelDesempeno | null;
  ultimaEvaluacion: string | null;
  porcentajeAvance: number;
  ultimoPuntaje: number | null;
  estado: EstadoAlumno;
}

export interface ResultadoHistorico {
  evaluacionId: string;
  temaNombre: string;
  puntaje: number;
  fallosConsecutivos: number;
  fechaCalculo: string;
  nivel: NivelDesempeno;
  retroalimentacion: string;
}

export interface EstadisticaPorNivel {
  nivel: string;
  cantidad: number;
}

export interface EstadisticaPorTema {
  temaNombre: string;
  evaluacionesRealizadas: number;
  puntajePromedio: number;
  distribucionNiveles: EstadisticaPorNivel[];
}

export interface Estadisticas {
  totalEvaluaciones: number;
  puntajePromedioGeneral: number;
  porTema: EstadisticaPorTema[];
}

export interface NivelDistribucion {
  nivel: NivelDesempeno;
  cantidad: number;
}

export interface DificultadGrupo {
  subtema: string;
  porcentajeError: number;
  totalIntentos: number;
}

export interface AlertaAlumno {
  alumnoId: string;
  nombres: string;
  apellidos: string;
  nivelActual: NivelDesempeno | null;
  motivo: string;
}

export interface RendimientoAlumno {
  alumnoId: string;
  nombres: string;
  apellidos: string;
  puntajeEntrada: number | null;
  nivelInicial: NivelDesempeno | null;
  nivelActual: NivelDesempeno | null;
  porcentajeAvance: number;
  ultimoPuntaje: number | null;
  ultimaActividad: string | null;
  estado: EstadoAlumno;
}

export interface ResumenGrupo {
  totalEstudiantes: number;
  promedioGeneral: number;
  avancePromedio: number;
  estudiantesRequierenApoyo: number;
  distribucionNiveles: NivelDistribucion[];
  dificultadesDelGrupo: DificultadGrupo[];
  alertas: AlertaAlumno[];
  rendimiento: RendimientoAlumno[];
}

export type EstadoSubtema = 'Dominado' | 'En progreso' | 'Reforzar';

export interface SubtemaRendimiento {
  subtema: string;
  porcentajeAciertos: number;
  estado: EstadoSubtema;
}

export interface DecisionSistema {
  reglaAplicada: string;
  motivo: string;
  accion: string;
}

export interface PuntoEvolucion {
  fecha: string;
  etiqueta: string;
  puntaje: number;
}

export interface PerfilAlumno {
  alumnoId: string;
  nombres: string;
  apellidos: string;
  grado: string;
  temaActualNombre: string | null;
  ultimaActividad: string | null;
  nivelInicial: NivelDesempeno | null;
  nivelActual: NivelDesempeno | null;
  evolucion: string;
  ultimaNota: number | null;
  porcentajeAciertosGlobal: number | null;
  cantidadEvaluaciones: number;
  rendimientoPorSubtema: SubtemaRendimiento[];
  fortalezas: string[];
  dificultades: string[];
  recomendacionActual: string | null;
  decisionesSistema: DecisionSistema[];
  evolucionPuntajes: PuntoEvolucion[];
  historial: ResultadoHistorico[];
}

export type TipoEvaluacionFicha = 'Pretest' | 'Postest';

/**
 * El backend serializa enums como número (sin JsonStringEnumConverter), así que el
 * body del POST debe enviar el valor numérico de TipoEvaluacion (Pretest=3, Postest=4).
 * El GET sí acepta el nombre como query string (ASP.NET model binding lo soporta).
 */
export const TIPO_EVALUACION_FICHA_VALOR: Record<TipoEvaluacionFicha, number> = {
  Pretest: 3,
  Postest: 4,
};

/**
 * Fila de la ficha de registro de notas (escala vigesimal 0-20).
 * D1 (Cognitiva) la calcula el sistema; D2 (Procedimental) y D3 (Actitudinal) las registra el docente.
 */
export interface FichaRegistroNota {
  alumnoId: string;
  nombresApellidos: string;
  d1I1: number;
  d1I2: number;
  d1I3: number;
  d1Promedio: number;
  d2I1: number;
  d2I2: number;
  d2I3: number;
  d2Promedio: number;
  d3I1: number;
  d3I2: number;
  d3I3: number;
  d3Promedio: number;
  promedioFinal: number;
}

export interface GuardarNotasDocenteRequest {
  alumnoId: string;
  temaId: string;
  tipoEvaluacion: number;
  d2I1: number;
  d2I2: number;
  d2I3: number;
  d3I1: number;
  d3I2: number;
  d3I3: number;
}
