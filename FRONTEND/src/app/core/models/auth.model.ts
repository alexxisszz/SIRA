export enum Rol {
  Administrador = 0,
  Docente = 1,
  Alumno = 2,
}

export interface LoginRequest {
  correoElectronico: string;
  contrasena: string;
}

export interface LoginResponse {
  token: string;
  usuarioId: string;
  rol: Rol;
}