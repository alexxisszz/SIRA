import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ContenidoTeorico } from '../models/contenido-teorico.model';
import { EstadoTema, Pregunta, VideoApoyo } from '../models/evaluacion.model';
import { ProgresoSubtema } from '../models/ejercicio.model';
import { NivelDesempeno } from '../models/recomendacion.model';
import { Tema } from '../models/tema.model';

@Injectable({ providedIn: 'root' })
export class TemaService {
  private readonly http = inject(HttpClient);

  obtenerTemas(): Observable<Tema[]> {
    return this.http.get<Tema[]>(`${environment.apiUrl}/temas`);
  }

  obtenerPreguntas(
    temaId: string,
    nivel?: NivelDesempeno,
    opciones?: { subtema?: string; excluirSubtemas?: string[] },
  ): Observable<Pregunta[]> {
    const params = new URLSearchParams();
    if (nivel !== undefined) params.set('nivel', String(nivel));
    if (opciones?.subtema) params.set('subtema', opciones.subtema);
    if (opciones?.excluirSubtemas?.length) params.set('excluirSubtemas', opciones.excluirSubtemas.join(','));

    const query = params.toString();
    const url = `${environment.apiUrl}/temas/${temaId}/preguntas`;
    return this.http.get<Pregunta[]>(query ? `${url}?${query}` : url);
  }

  obtenerEstado(temaId: string): Observable<EstadoTema> {
    return this.http.get<EstadoTema>(`${environment.apiUrl}/temas/${temaId}/estado`);
  }

  obtenerVideos(temaId: string): Observable<VideoApoyo[]> {
    return this.http.get<VideoApoyo[]>(`${environment.apiUrl}/temas/${temaId}/videos`);
  }

  obtenerProgresoPractica(temaId: string): Observable<ProgresoSubtema[]> {
    return this.http.get<ProgresoSubtema[]>(`${environment.apiUrl}/temas/${temaId}/progreso-practica`);
  }

  obtenerTeoria(temaId: string): Observable<ContenidoTeorico[]> {
    return this.http.get<ContenidoTeorico[]>(`${environment.apiUrl}/temas/${temaId}/teoria`);
  }
}
