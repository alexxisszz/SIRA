import { NivelDesempeno } from '../../alumno/models/recomendacion.model';

export interface AlumnoResumen {
  alumnoId: string;
  nombres: string;
  apellidos: string;
  grado: string;
  evaluacionesRealizadas: number;
  ultimoNivel: NivelDesempeno | null;
  ultimaEvaluacion: string | null;
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
