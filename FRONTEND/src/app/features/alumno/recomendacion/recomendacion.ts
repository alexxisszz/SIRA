import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { RecomendacionService } from '../services/recomendacion.service';
import { NivelDesempeno, Recomendacion as RecomendacionModel } from '../models/recomendacion.model';
import { CLASE_NIVEL, ETIQUETA_NIVEL } from '../../../shared/utils/nivel.util';

@Component({
  selector: 'app-recomendacion',
  imports: [],
  templateUrl: './recomendacion.html',
})
export class Recomendacion {
  private readonly route = inject(ActivatedRoute);
  private readonly recomendacionService = inject(RecomendacionService);

  protected readonly recomendacion = signal<RecomendacionModel | null>(null);
  protected readonly cargando = signal(true);
  protected readonly error = signal(false);

  constructor() {
    const recomendacionId = this.route.snapshot.paramMap.get('recomendacionId')!;

    this.recomendacionService.obtenerPorId(recomendacionId).subscribe({
      next: (recomendacion) => {
        this.recomendacion.set(recomendacion);
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
}
