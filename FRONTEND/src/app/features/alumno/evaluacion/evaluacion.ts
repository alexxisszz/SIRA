import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { concatMap, forkJoin, from, toArray } from 'rxjs';
import { EvaluacionService } from '../services/evaluacion.service';
import { TemaService } from '../services/tema.service';
import { Pregunta } from '../models/evaluacion.model';

@Component({
  selector: 'app-evaluacion',
  imports: [],
  templateUrl: './evaluacion.html',
})
export class Evaluacion {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly temaService = inject(TemaService);
  private readonly evaluacionService = inject(EvaluacionService);

  private readonly temaId = this.route.snapshot.paramMap.get('temaId')!;
  private evaluacionId = '';

  protected readonly preguntas = signal<Pregunta[]>([]);
  protected readonly respuestas = signal<Record<string, string>>({});
  protected readonly cargando = signal(true);
  protected readonly enviando = signal(false);
  protected readonly error = signal(false);

  protected readonly totalRespondidas = computed(() => Object.keys(this.respuestas()).length);
  protected readonly formularioCompleto = computed(
    () => this.preguntas().length > 0 && this.totalRespondidas() === this.preguntas().length,
  );

  constructor() {
    forkJoin({
      preguntas: this.temaService.obtenerPreguntas(this.temaId),
      evaluacion: this.evaluacionService.iniciar(this.temaId),
    }).subscribe({
      next: ({ preguntas, evaluacion }) => {
        this.preguntas.set(preguntas);
        this.evaluacionId = evaluacion.evaluacionId;
        this.cargando.set(false);
      },
      error: () => {
        this.error.set(true);
        this.cargando.set(false);
      },
    });
  }

  seleccionarOpcion(preguntaId: string, opcionId: string): void {
    this.respuestas.update((actual) => ({ ...actual, [preguntaId]: opcionId }));
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