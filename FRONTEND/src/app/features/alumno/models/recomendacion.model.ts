export enum NivelDesempeno {
  Basico = 0,
  Intermedio = 1,
  Avanzado = 2,
}

export interface EjercicioSugerido {
  id: string;
  titulo: string;
}

export interface Recomendacion {
  id: string;
  nivel: NivelDesempeno;
  temasPorReforzar: string[];
  ejerciciosSugeridos: EjercicioSugerido[];
  retroalimentacion: string;
}