/**
 * El motor de inferencia calcula el puntaje en porcentaje (0-100), ya que las reglas
 * de nivel dependen de esos umbrales. Para mostrarlo al usuario se convierte a la
 * escala vigesimal (0-20) usada en el sistema educativo peruano.
 */
export function convertirAVigesimal(puntajePorcentaje: number): number {
  return Math.round((puntajePorcentaje / 5) * 10) / 10;
}
