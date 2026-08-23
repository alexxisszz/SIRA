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

export interface AdminPregunta {
  id: string;
  temaId: string;
  enunciado: string;
  nivelDificultad: NivelDificultad;
  opciones: AdminOpcion[];
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
