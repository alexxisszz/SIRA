import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  AlumnoResumen,
  Estadisticas,
  FichaRegistroNota,
  GuardarNotasDocenteRequest,
  PerfilAlumno,
  ResultadoHistorico,
  ResumenGrupo,
  TipoEvaluacionFicha,
} from '../models/docente.model';

interface DimensionRespuesta {
  i1: number;
  i2: number;
  i3: number;
  promedio: number;
}

interface FichaRegistroNotasRespuesta {
  alumnos: {
    alumnoId: string;
    nombres: string;
    apellidos: string;
    d1: DimensionRespuesta | null;
    d2: DimensionRespuesta | null;
    d3: DimensionRespuesta | null;
    promedioFinal: number | null;
  }[];
}

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

  obtenerFichaNotas(temaId: string, tipo: TipoEvaluacionFicha): Observable<FichaRegistroNota[]> {
    return this.http
      .get<FichaRegistroNotasRespuesta>(
        `${environment.apiUrl}/docente/fichas-notas?temaId=${temaId}&tipo=${tipo}`,
      )
      .pipe(
        map((r) =>
          r.alumnos.map((a) => ({
            alumnoId: a.alumnoId,
            nombresApellidos: `${a.apellidos} ${a.nombres}`.trim(),
            d1I1: a.d1?.i1 ?? 0,
            d1I2: a.d1?.i2 ?? 0,
            d1I3: a.d1?.i3 ?? 0,
            d1Promedio: a.d1?.promedio ?? 0,
            d2I1: a.d2?.i1 ?? 0,
            d2I2: a.d2?.i2 ?? 0,
            d2I3: a.d2?.i3 ?? 0,
            d2Promedio: a.d2?.promedio ?? 0,
            d3I1: a.d3?.i1 ?? 0,
            d3I2: a.d3?.i2 ?? 0,
            d3I3: a.d3?.i3 ?? 0,
            d3Promedio: a.d3?.promedio ?? 0,
            promedioFinal: a.promedioFinal ?? 0,
          })),
        ),
      );
  }

  guardarNotasDocente(payload: GuardarNotasDocenteRequest): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/docente/fichas-notas/notas-manuales`, payload);
  }
}
