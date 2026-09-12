export enum NivelDesempeno {
  Basico = 0,
  Intermedio = 1,
  Avanzado = 2,
}

export interface EjercicioSugerido {
  id: string;
  titulo: string;
}

export interface RespuestaDetalle {
  preguntaId: string;
  enunciado: string;
  opcionSeleccionadaTexto: string;
  opcionCorrectaTexto: string;
  esCorrecta: boolean;
}

export interface ProgresoSubtemaComparado {
  subtema: string;
  porcentajeInicial: number;
  porcentajeFinal: number;
}

export interface ReporteProgresoTema {
  puntajeInicial: number;
  puntajeFinal: number;
  nivelInicial: NivelDesempeno | null;
  nivelFinal: NivelDesempeno;
  subtemas: ProgresoSubtemaComparado[];
}

export interface Recomendacion {
  id: string;
  temaId: string;
  puntaje: number;
  nivel: NivelDesempeno;
  temasPorReforzar: string[];
  subtemasDominados: string[];
  ejerciciosSugeridos: EjercicioSugerido[];
  retroalimentacion: string;
  respuestasDetalle: RespuestaDetalle[];
  reporteProgreso: ReporteProgresoTema | null;
}
