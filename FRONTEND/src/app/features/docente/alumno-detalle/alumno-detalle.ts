import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DocenteService } from '../services/docente.service';
import { EstadoSubtema, PerfilAlumno } from '../models/docente.model';
import { NivelDesempeno } from '../../alumno/models/recomendacion.model';
import { CLASE_ESTADO_SUBTEMA, CLASE_NIVEL, ETIQUETA_NIVEL } from '../../../shared/utils/nivel.util';
import { convertirAVigesimal } from '../../../shared/utils/calificacion.util';

@Component({
  selector: 'app-alumno-detalle',
  imports: [DatePipe, DecimalPipe],
  templateUrl: './alumno-detalle.html',
})
export class AlumnoDetalle {
  private readonly route = inject(ActivatedRoute);
  private readonly docenteService = inject(DocenteService);
  protected readonly router = inject(Router);

  protected readonly perfil = signal<PerfilAlumno | null>(null);
  protected readonly cargando = signal(true);
  protected readonly error = signal(false);

  constructor() {
    const alumnoId = this.route.snapshot.paramMap.get('alumnoId')!;

    this.docenteService.obtenerPerfilAlumno(alumnoId).subscribe({
      next: (perfil) => {
        this.perfil.set(perfil);
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

  claseEstadoSubtema(estado: EstadoSubtema): string {
    return CLASE_ESTADO_SUBTEMA[estado];
  }

  notaVigesimal(puntaje: number): number {
    return convertirAVigesimal(puntaje);
  }

  /** Escala 0-100 -> altura relativa (%) para las barras del gráfico de evolución. */
  alturaBarra(puntaje: number, maximo: number): number {
    return maximo > 0 ? Math.max((puntaje / maximo) * 100, 4) : 4;
  }
}
