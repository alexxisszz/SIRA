import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Pregunta } from '../models/evaluacion.model';
import { Tema } from '../models/tema.model';

@Injectable({ providedIn: 'root' })
export class TemaService {
  private readonly http = inject(HttpClient);

  obtenerTemas(): Observable<Tema[]> {
    return this.http.get<Tema[]>(`${environment.apiUrl}/temas`);
  }

  obtenerPreguntas(temaId: string): Observable<Pregunta[]> {
    return this.http.get<Pregunta[]>(`${environment.apiUrl}/temas/${temaId}/preguntas`);
  }
}