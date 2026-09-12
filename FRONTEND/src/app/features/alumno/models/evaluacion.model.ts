import { NivelDesempeno } from './recomendacion.model';

export enum TipoEvaluacion {
  Diagnostica = 0,
  PorNivel = 1,
  Final = 2,
}

export interface Opcion {
  id: string;
  texto: string;
}

export interface Pregunta {
  id: string;
  enunciado: string;
  subtema: string;
  opciones: Opcion[];
}

export interface IniciarEvaluacionResponse {
  evaluacionId: string;
}

export interface RegistrarRespuestaRequest {
  preguntaId: string;
  opcionSeleccionadaId: string;
}

export interface FinalizarEvaluacionResponse {
  recomendacionId: string;
}

export interface EstadoTema {
  temaDesbloqueado: boolean;
  diagnosticoCompletado: boolean;
  basicoAprobado: boolean;
  intermedioAprobado: boolean;
  avanzadoAprobado: boolean;
  finalAprobada: boolean;
}

export const NIVELES_ORDENADOS = [NivelDesempeno.Basico, NivelDesempeno.Intermedio, NivelDesempeno.Avanzado];

export interface VideoApoyo {
  id: string;
  titulo: string;
  url: string;
}
