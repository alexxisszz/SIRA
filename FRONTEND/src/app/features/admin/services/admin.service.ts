import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  AdminAlumno,
  AdminDocente,
  AdminPregunta,
  AdminRegla,
  AdminTema,
  NivelDificultad,
} from '../models/admin.model';

export interface OpcionInput {
  texto: string;
  esCorrecta: boolean;
}

@Injectable({ providedIn: 'root' })
export class AdminService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/admin`;

  // Alumnos
  obtenerAlumnos(): Observable<AdminAlumno[]> {
    return this.http.get<AdminAlumno[]>(`${this.baseUrl}/alumnos`);
  }

  crearAlumno(datos: { correoElectronico: string; contrasena: string; nombres: string; apellidos: string; grado: string }) {
    return this.http.post<{ alumnoId: string }>(`${this.baseUrl}/alumnos`, datos);
  }

  actualizarAlumno(alumnoId: string, datos: { nombres: string; apellidos: string; grado: string }): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/alumnos/${alumnoId}`, datos);
  }

  cambiarEstadoAlumno(alumnoId: string, activo: boolean): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/alumnos/${alumnoId}/estado`, { activo });
  }

  // Docentes
  obtenerDocentes(): Observable<AdminDocente[]> {
    return this.http.get<AdminDocente[]>(`${this.baseUrl}/docentes`);
  }

  crearDocente(datos: { correoElectronico: string; contrasena: string; nombres: string; apellidos: string }) {
    return this.http.post<{ docenteId: string }>(`${this.baseUrl}/docentes`, datos);
  }

  actualizarDocente(docenteId: string, datos: { nombres: string; apellidos: string }): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/docentes/${docenteId}`, datos);
  }

  cambiarEstadoDocente(docenteId: string, activo: boolean): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/docentes/${docenteId}/estado`, { activo });
  }

  // Temas
  obtenerTemas(): Observable<AdminTema[]> {
    return this.http.get<AdminTema[]>(`${this.baseUrl}/temas`);
  }

  crearTema(datos: { nombre: string; orden: number }) {
    return this.http.post<{ temaId: string }>(`${this.baseUrl}/temas`, datos);
  }

  actualizarTema(temaId: string, datos: { nombre: string; orden: number }): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/temas/${temaId}`, datos);
  }

  // Preguntas
  obtenerPreguntasPorTema(temaId: string): Observable<AdminPregunta[]> {
    return this.http.get<AdminPregunta[]>(`${this.baseUrl}/temas/${temaId}/preguntas`);
  }

  crearPregunta(datos: { temaId: string; enunciado: string; nivelDificultad: NivelDificultad; opciones: OpcionInput[] }) {
    return this.http.post<{ preguntaId: string }>(`${this.baseUrl}/preguntas`, datos);
  }

  actualizarPregunta(
    preguntaId: string,
    datos: { enunciado: string; nivelDificultad: NivelDificultad; opciones: OpcionInput[] },
  ): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/preguntas/${preguntaId}`, datos);
  }

  eliminarPregunta(preguntaId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/preguntas/${preguntaId}`);
  }

  // Reglas
  obtenerReglas(): Observable<AdminRegla[]> {
    return this.http.get<AdminRegla[]>(`${this.baseUrl}/reglas`);
  }

  actualizarRegla(
    reglaId: string,
    datos: { nombre: string; descripcionCondicion: string; descripcionConclusion: string; prioridad: number },
  ): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/reglas/${reglaId}`, datos);
  }

  cambiarEstadoRegla(reglaId: string, activa: boolean): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/reglas/${reglaId}/estado`, { activa });
  }
}
