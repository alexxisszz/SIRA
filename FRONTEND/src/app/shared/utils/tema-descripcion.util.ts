const DESCRIPCION_TEMA: Record<string, string> = {
  'Porcentajes I': 'Conceptos básicos, cálculo de porcentajes, aumentos y descuentos simples.',
  'Porcentajes II': 'Descuentos sucesivos, porcentaje de porcentaje y problemas aplicados.',
};

export function obtenerDescripcionTema(nombreTema: string): string {
  return DESCRIPCION_TEMA[nombreTema] ?? 'Practica y refuerza este tema a tu propio ritmo.';
}
