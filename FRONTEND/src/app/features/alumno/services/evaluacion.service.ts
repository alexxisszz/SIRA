import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  FinalizarEvaluacionResponse,
  IniciarEvaluacionResponse,
  RegistrarRespuestaRequest,
} from '../models/evaluacion.model';

@Injectable({ providedIn: 'root' })
export class EvaluacionService {
  private readonly http = inject(HttpClient);

  iniciar(temaId: string): Observable<IniciarEvaluacionResponse> {
    return this.http.post<IniciarEvaluacionResponse>(`${environment.apiUrl}/evaluaciones`, { temaId });
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