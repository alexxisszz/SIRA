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
