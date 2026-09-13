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

/** Debe coincidir exactamente con los nombres de `NivelDesempeno` en el backend (usados como `Clave` de ContenidoTeorico). */
export const CLAVE_NIVEL: Record<NivelDesempeno, string> = {
  [NivelDesempeno.Basico]: 'Basico',
  [NivelDesempeno.Intermedio]: 'Intermedio',
  [NivelDesempeno.Avanzado]: 'Avanzado',
};

/** Estados de progreso del alumno calculados en backend (ver `AlumnoResumenDto.Estado`/`RendimientoAlumnoDto.Estado`). */
export const CLASE_ESTADO: Record<string, string> = {
  'Sin evaluar': 'text-bg-secondary',
  'En progreso': 'text-bg-info',
  'Requiere apoyo': 'text-bg-danger',
  'Buen progreso': 'text-bg-success',
};

/** Estados de dominio de un subtema (ver `SubtemaRendimientoDto.Estado`). */
export const CLASE_ESTADO_SUBTEMA: Record<string, string> = {
  Dominado: 'text-bg-success',
  'En progreso': 'text-bg-warning',
  Reforzar: 'text-bg-danger',
};
