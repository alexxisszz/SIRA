export enum AccionDificultad {
  Ninguna = 0,
  SubirDificultad = 1,
  BajarDificultad = 2,
}

export interface IntentoResultado {
  esCorrecta: boolean;
  opcionCorrectaId: string;
  explicacion: string | null;
  subtema: string;
  accionSugerida: AccionDificultad;
}

export interface ProgresoSubtema {
  subtema: string;
  intentos: number;
  aciertos: number;
}
