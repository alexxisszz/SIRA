import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AlumnoResumen, Estadisticas, ResultadoHistorico } from '../models/docente.model';

@Injectable({ providedIn: 'root' })
export class DocenteService {
  private readonly http = inject(HttpClient);

  obtenerAlumnos(): Observable<AlumnoResumen[]> {
    return this.http.get<AlumnoResumen[]>(`${environment.apiUrl}/docente/alumnos`);
  }

  obtenerResultadosPorAlumno(alumnoId: string): Observable<ResultadoHistorico[]> {
    return this.http.get<ResultadoHistorico[]>(`${environment.apiUrl}/docente/alumnos/${alumnoId}/resultados`);
  }

  obtenerEstadisticas(): Observable<Estadisticas> {
    return this.http.get<Estadisticas>(`${environment.apiUrl}/docente/estadisticas`);
  }
}
