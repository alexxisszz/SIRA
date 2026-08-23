import { DecimalPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { DocenteService } from '../services/docente.service';
import { Estadisticas as EstadisticasModel } from '../models/docente.model';

@Component({
  selector: 'app-estadisticas',
  imports: [DecimalPipe],
  templateUrl: './estadisticas.html',
})
export class Estadisticas {
  private readonly docenteService = inject(DocenteService);

  protected readonly estadisticas = signal<EstadisticasModel | null>(null);
  protected readonly cargando = signal(true);
  protected readonly error = signal(false);

  constructor() {
    this.docenteService.obtenerEstadisticas().subscribe({
      next: (estadisticas) => {
        this.estadisticas.set(estadisticas);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set(true);
        this.cargando.set(false);
      },
    });
  }
}
