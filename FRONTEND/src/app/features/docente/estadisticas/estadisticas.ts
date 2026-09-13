import { DecimalPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { DocenteService } from '../services/docente.service';
import { ResumenGrupo, EstadoAlumno } from '../models/docente.model';
import { NivelDesempeno } from '../../alumno/models/recomendacion.model';
import { CLASE_ESTADO, CLASE_NIVEL, ETIQUETA_NIVEL } from '../../../shared/utils/nivel.util';
import { convertirAVigesimal } from '../../../shared/utils/calificacion.util';

@Component({
  selector: 'app-estadisticas',
  imports: [DecimalPipe],
  templateUrl: './estadisticas.html',
})
export class Estadisticas {
  private readonly docenteService = inject(DocenteService);
  private readonly router = inject(Router);

  protected readonly resumen = signal<ResumenGrupo | null>(null);
  protected readonly cargando = signal(true);
  protected readonly error = signal(false);

  constructor() {
    this.docenteService.obtenerResumenGrupo().subscribe({
      next: (resumen) => {
        this.resumen.set(resumen);
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

  claseEstado(estado: EstadoAlumno): string {
    return CLASE_ESTADO[estado];
  }

  notaVigesimal(puntaje: number): number {
    return convertirAVigesimal(puntaje);
  }

  verDetalle(alumnoId: string): void {
    this.router.navigate(['/docente/alumnos', alumnoId]);
  }
}
