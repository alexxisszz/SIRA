export interface Opcion {
  id: string;
  texto: string;
}

export interface Pregunta {
  id: string;
  enunciado: string;
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