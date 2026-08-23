import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { DocenteService } from '../services/docente.service';
import { AlumnoResumen } from '../models/docente.model';
import { NivelDesempeno } from '../../alumno/models/recomendacion.model';
import { CLASE_NIVEL, ETIQUETA_NIVEL } from '../../../shared/utils/nivel.util';

@Component({
  selector: 'app-alumnos-list',
  imports: [],
  templateUrl: './alumnos-list.html',
})
export class AlumnosList {
  private readonly docenteService = inject(DocenteService);
  private readonly router = inject(Router);

  protected readonly alumnos = signal<AlumnoResumen[]>([]);
  protected readonly cargando = signal(true);
  protected readonly error = signal(false);

  constructor() {
    this.docenteService.obtenerAlumnos().subscribe({
      next: (alumnos) => {
        this.alumnos.set(alumnos);
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

  verDetalle(alumnoId: string): void {
    this.router.navigate(['/docente/alumnos', alumnoId]);
  }
}
