import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  AlumnoResumen,
  Estadisticas,
  PerfilAlumno,
  ResultadoHistorico,
  ResumenGrupo,
} from '../models/docente.model';

@Injectable({ providedIn: 'root' })
export class DocenteService {
  private readonly http = inject(HttpClient);

  obtenerAlumnos(temaId?: string): Observable<AlumnoResumen[]> {
    const url = `${environment.apiUrl}/docente/alumnos`;
    return this.http.get<AlumnoResumen[]>(temaId ? `${url}?temaId=${temaId}` : url);
  }

  obtenerResultadosPorAlumno(alumnoId: string): Observable<ResultadoHistorico[]> {
    return this.http.get<ResultadoHistorico[]>(`${environment.apiUrl}/docente/alumnos/${alumnoId}/resultados`);
  }

  obtenerEstadisticas(): Observable<Estadisticas> {
    return this.http.get<Estadisticas>(`${environment.apiUrl}/docente/estadisticas`);
  }

  obtenerResumenGrupo(temaId?: string): Observable<ResumenGrupo> {
    const url = `${environment.apiUrl}/docente/resumen-grupo`;
    return this.http.get<ResumenGrupo>(temaId ? `${url}?temaId=${temaId}` : url);
  }

  obtenerPerfilAlumno(alumnoId: string): Observable<PerfilAlumno> {
    return this.http.get<PerfilAlumno>(`${environment.apiUrl}/docente/alumnos/${alumnoId}/perfil`);
  }
}
