import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  FinalizarEvaluacionResponse,
  IniciarEvaluacionResponse,
  Pregunta,
  RegistrarRespuestaRequest,
  TipoEvaluacion,
} from '../models/evaluacion.model';
import { NivelDesempeno } from '../models/recomendacion.model';

@Injectable({ providedIn: 'root' })
export class EvaluacionService {
  private readonly http = inject(HttpClient);

  iniciar(temaId: string, tipo: TipoEvaluacion, nivel?: NivelDesempeno): Observable<IniciarEvaluacionResponse> {
    return this.http.post<IniciarEvaluacionResponse>(`${environment.apiUrl}/evaluaciones`, {
      temaId,
      tipo,
      nivel: nivel ?? null,
    });
  }

  obtenerPreguntas(evaluacionId: string): Observable<Pregunta[]> {
    return this.http.get<Pregunta[]>(`${environment.apiUrl}/evaluaciones/${evaluacionId}/preguntas`);
  }

  registrarRespuesta(evaluacionId: string, respuesta: RegistrarRespuestaRequest): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/evaluaciones/${evaluacionId}/respuestas`, respuesta);
  }

  finalizar(evaluacionId: string): Observable<FinalizarEvaluacionResponse> {
    return this.http.post<FinalizarEvaluacionResponse>(
      `${environment.apiUrl}/evaluaciones/${evaluacionId}/finalizar`,
      {},
    );
  }
}
