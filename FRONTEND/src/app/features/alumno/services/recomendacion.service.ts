import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Recomendacion } from '../models/recomendacion.model';

@Injectable({ providedIn: 'root' })
export class RecomendacionService {
  private readonly http = inject(HttpClient);

  obtenerPorId(recomendacionId: string): Observable<Recomendacion> {
    return this.http.get<Recomendacion>(`${environment.apiUrl}/recomendaciones/${recomendacionId}`);
  }
}