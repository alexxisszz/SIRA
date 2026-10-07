export interface AdminAlumno {
  alumnoId: string;
  usuarioId: string;
  correoElectronico: string;
  activo: boolean;
  nombres: string;
  apellidos: string;
  grado: string;
}

export interface AdminDocente {
  docenteId: string;
  usuarioId: string;
  correoElectronico: string;
  activo: boolean;
  nombres: string;
  apellidos: string;
}

export interface AdminTema {
  id: string;
  nombre: string;
  orden: number;
}

export interface AdminOpcion {
  id: string;
  texto: string;
  esCorrecta: boolean;
}

export enum NivelDificultad {
  Basico = 0,
  Intermedio = 1,
  Avanzado = 2,
}

export enum IndicadorCognitivo {
  I1 = 0,
  I2 = 1,
  I3 = 2,
}

export enum TipoPregunta {
  OpcionMultiple = 0,
}

export interface AdminPregunta {
  id: string;
  temaId: string;
  enunciado: string;
  subtema: string;
  nivelDificultad: NivelDificultad;
  indicador: IndicadorCognitivo;
  tipo: TipoPregunta;
  explicacion: string | null;
  puntaje: number;
  opciones: AdminOpcion[];
}

export interface AdminVideoApoyo {
  id: string;
  temaId: string;
  titulo: string;
  url: string;
}

export enum TipoContenidoTeorico {
  NivelGeneral = 0,
  Subtema = 1,
}

export interface AdminContenidoTeorico {
  id: string;
  temaId: string;
  tipo: TipoContenidoTeorico;
  clave: string;
  titulo: string;
  parrafos: string[];
}

export interface AdminRegla {
  id: string;
  nombre: string;
  nombreClaseRegla: string;
  descripcionCondicion: string;
  descripcionConclusion: string;
  prioridad: number;
  activa: boolean;
}
