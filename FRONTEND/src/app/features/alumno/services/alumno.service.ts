import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ResultadoHistorico } from '../../docente/models/docente.model';

export interface PerfilAlumno {
  nombres: string;
  apellidos: string;
  grado: string;
}

export interface ResumenProgreso {
  ejerciciosIntentados: number;
  ejerciciosCorrectos: number;
  subtemasTrabajados: number;
  diasConActividad: number;
  ultimaActividad: string | null;
}

@Injectable({ providedIn: 'root' })
export class AlumnoService {
  private readonly http = inject(HttpClient);

  obtenerMiHistorial(): Observable<ResultadoHistorico[]> {
    return this.http.get<ResultadoHistorico[]>(`${environment.apiUrl}/alumno/historial`);
  }

  obtenerMiPerfil(): Observable<PerfilAlumno> {
    return this.http.get<PerfilAlumno>(`${environment.apiUrl}/alumno/perfil`);
  }

  obtenerResumenProgreso(): Observable<ResumenProgreso> {
    return this.http.get<ResumenProgreso>(`${environment.apiUrl}/alumno/resumen-progreso`);
  }
}
