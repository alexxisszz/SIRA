import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DocenteService } from '../services/docente.service';
import { ResultadoHistorico } from '../models/docente.model';
import { NivelDesempeno } from '../../alumno/models/recomendacion.model';
import { CLASE_NIVEL, ETIQUETA_NIVEL } from '../../../shared/utils/nivel.util';

@Component({
  selector: 'app-alumno-detalle',
  imports: [DatePipe],
  templateUrl: './alumno-detalle.html',
})
export class AlumnoDetalle {
  private readonly route = inject(ActivatedRoute);
  private readonly docenteService = inject(DocenteService);

  protected readonly resultados = signal<ResultadoHistorico[]>([]);
  protected readonly cargando = signal(true);
  protected readonly error = signal(false);

  constructor() {
    const alumnoId = this.route.snapshot.paramMap.get('alumnoId')!;

    this.docenteService.obtenerResultadosPorAlumno(alumnoId).subscribe({
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
}
