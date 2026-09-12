import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Pregunta } from '../models/evaluacion.model';
import { IntentoResultado } from '../models/ejercicio.model';

@Injectable({ providedIn: 'root' })
export class EjercicioService {
  private readonly http = inject(HttpClient);

  obtenerEjercicio(preguntaId: string): Observable<Pregunta> {
    return this.http.get<Pregunta>(`${environment.apiUrl}/ejercicios/${preguntaId}`);
  }

  registrarIntento(preguntaId: string, opcionSeleccionadaId: string): Observable<IntentoResultado> {
    return this.http.post<IntentoResultado>(`${environment.apiUrl}/ejercicios/${preguntaId}/intentos`, {
      opcionSeleccionadaId,
    });
  }
}
