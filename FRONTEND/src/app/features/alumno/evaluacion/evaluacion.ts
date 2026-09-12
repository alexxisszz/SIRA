import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { concatMap, from, toArray } from 'rxjs';
import { EvaluacionService } from '../services/evaluacion.service';
import { Pregunta, TipoEvaluacion } from '../models/evaluacion.model';
import { NivelDesempeno } from '../models/recomendacion.model';
import { ETIQUETA_NIVEL } from '../../../shared/utils/nivel.util';

@Component({
  selector: 'app-evaluacion',
  imports: [RouterLink],
  templateUrl: './evaluacion.html',
  styleUrl: './evaluacion.scss',
})
export class Evaluacion {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly evaluacionService = inject(EvaluacionService);

  private readonly temaId = this.route.snapshot.paramMap.get('temaId')!;
  private readonly tipo = this.leerTipoDeQueryParams();
  private readonly nivel = this.leerNivelDeQueryParams();
  private evaluacionId = '';

  protected readonly preguntas = signal<Pregunta[]>([]);
  protected readonly respuestas = signal<Record<string, string>>({});
  protected readonly cargando = signal(true);
  protected readonly enviando = signal(false);
  protected readonly error = signal(false);
  protected readonly mensajeError = signal('Ocurrió un error al procesar la evaluación.');

  protected readonly totalRespondidas = computed(() => Object.keys(this.respuestas()).length);
  protected readonly formularioCompleto = computed(
    () => this.preguntas().length > 0 && this.totalRespondidas() === this.preguntas().length,
  );
  protected readonly porcentajeProgreso = computed(() => {
    const total = this.preguntas().length;
    return total > 0 ? Math.round((this.totalRespondidas() / total) * 100) : 0;
  });

  protected readonly titulo = computed(() => {
    if (this.tipo === TipoEvaluacion.Diagnostica) return 'Prueba de entrada';
    if (this.tipo === TipoEvaluacion.Final) return 'Prueba final';
    return `Evaluación de nivel ${this.nivel !== undefined ? ETIQUETA_NIVEL[this.nivel] : ''}`;
  });

  constructor() {
    this.evaluacionService
      .iniciar(this.temaId, this.tipo, this.nivel)
      .pipe(concatMap((evaluacion) => {
        this.evaluacionId = evaluacion.evaluacionId;
        return this.evaluacionService.obtenerPreguntas(evaluacion.evaluacionId);
      }))
      .subscribe({
        next: (preguntas) => {
          this.preguntas.set(preguntas);
          this.cargando.set(false);
        },
        error: (err: HttpErrorResponse) => {
          if (err.status === 400 && err.error?.mensaje) {
            this.mensajeError.set(err.error.mensaje);
          }
          this.error.set(true);
          this.cargando.set(false);
        },
      });
  }

  private leerTipoDeQueryParams(): TipoEvaluacion {
    const valor = this.route.snapshot.queryParamMap.get('tipo');
    return valor !== null ? (Number(valor) as TipoEvaluacion) : TipoEvaluacion.Diagnostica;
  }

  private leerNivelDeQueryParams(): NivelDesempeno | undefined {
    const valor = this.route.snapshot.queryParamMap.get('nivel');
    return valor !== null ? (Number(valor) as NivelDesempeno) : undefined;
  }

  seleccionarOpcion(preguntaId: string, opcionId: string): void {
    this.respuestas.update((actual) => ({ ...actual, [preguntaId]: opcionId }));
  }

  letraOpcion(indice: number): string {
    return String.fromCharCode(65 + indice);
  }

  finalizarEvaluacion(): void {
    if (!this.formularioCompleto()) {
      return;
    }

    this.enviando.set(true);
    const respuestas = this.respuestas();

    from(this.preguntas())
      .pipe(
        concatMap((pregunta) =>
          this.evaluacionService.registrarRespuesta(this.evaluacionId, {
            preguntaId: pregunta.id,
            opcionSeleccionadaId: respuestas[pregunta.id],
          }),
        ),
        toArray(),
        concatMap(() => this.evaluacionService.finalizar(this.evaluacionId)),
      )
      .subscribe({
        next: ({ recomendacionId }) => this.router.navigate(['/recomendaciones', recomendacionId]),
        error: () => {
          this.error.set(true);
          this.enviando.set(false);
        },
      });
  }
}
