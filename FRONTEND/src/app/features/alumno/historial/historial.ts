import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { AlumnoService } from '../services/alumno.service';
import { ResultadoHistorico } from '../../docente/models/docente.model';
import { NivelDesempeno } from '../models/recomendacion.model';
import { CLASE_NIVEL, ETIQUETA_NIVEL } from '../../../shared/utils/nivel.util';
import { convertirAVigesimal } from '../../../shared/utils/calificacion.util';

@Component({
  selector: 'app-historial',
  imports: [DatePipe],
  templateUrl: './historial.html',
})
export class Historial {
  private readonly alumnoService = inject(AlumnoService);

  protected readonly resultados = signal<ResultadoHistorico[]>([]);
  protected readonly cargando = signal(true);
  protected readonly error = signal(false);

  constructor() {
    this.alumnoService.obtenerMiHistorial().subscribe({
      next: (resultados) => {
        this.resultados.set(resultados);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set(true);
        this.cargando.set(false);
      },
    });
  }

  etiquetaNivel(nivel: NivelDesempeno): string {
    return ETIQUETA_NIVEL[nivel];
  }

  claseNivel(nivel: NivelDesempeno): string {
    return CLASE_NIVEL[nivel];
  }

  notaVigesimal(puntaje: number): number {
    return convertirAVigesimal(puntaje);
  }
}
