import { NivelDesempeno } from '../../features/alumno/models/recomendacion.model';

export const ETIQUETA_NIVEL: Record<NivelDesempeno, string> = {
  [NivelDesempeno.Basico]: 'Básico',
  [NivelDesempeno.Intermedio]: 'Intermedio',
  [NivelDesempeno.Avanzado]: 'Avanzado',
};

export const CLASE_NIVEL: Record<NivelDesempeno, string> = {
  [NivelDesempeno.Basico]: 'text-bg-danger',
  [NivelDesempeno.Intermedio]: 'text-bg-warning',
  [NivelDesempeno.Avanzado]: 'text-bg-success',
};
