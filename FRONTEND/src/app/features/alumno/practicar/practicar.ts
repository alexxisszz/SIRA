import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { EjercicioService } from '../services/ejercicio.service';
import { TemaService } from '../services/tema.service';
import { ContenidoTeorico, TipoContenidoTeorico } from '../models/contenido-teorico.model';
import { Pregunta } from '../models/evaluacion.model';
import { AccionDificultad, IntentoResultado } from '../models/ejercicio.model';
import { NivelDesempeno } from '../models/recomendacion.model';
import { ETIQUETA_NIVEL } from '../../../shared/utils/nivel.util';

@Component({
  selector: 'app-practicar',
  imports: [RouterLink],
  templateUrl: './practicar.html',
  styleUrl: './practicar.scss',
})
export class Practicar {
  private readonly route = inject(ActivatedRoute);
  private readonly ejercicioService = inject(EjercicioService);
  private readonly temaService = inject(TemaService);

  private readonly temaId = this.route.snapshot.queryParamMap.get('temaId');

  protected readonly colaIds = signal<string[]>(this.leerIdsDeQueryParams());
  protected readonly nivelActual = signal<NivelDesempeno | null>(this.leerNivelDeQueryParams());
  protected readonly indice = signal(0);
  protected readonly ejercicio = signal<Pregunta | null>(null);
  protected readonly opcionSeleccionada = signal<string | null>(null);
  protected readonly resultado = signal<IntentoResultado | null>(null);
  protected readonly aciertos = signal(0);
  protected readonly cargando = signal(true);
  protected readonly enviando = signal(false);
  protected readonly error = signal(false);
  protected readonly mensajeCambioNivel = signal<string | null>(null);
  protected readonly teoriaDelTema = signal<ContenidoTeorico[]>([]);

  protected readonly total = computed(() => this.colaIds().length);
  protected readonly terminado = computed(() => this.indice() >= this.total());
  protected readonly sugerirSubirNivel = computed(() => {
    const total = this.total();
    return total > 0 && this.aciertos() / total >= 0.8 && this.nivelActual() !== NivelDesempeno.Avanzado;
  });
  protected readonly teoriaSubtema = computed(() => {
    const res = this.resultado();
    if (!res || res.accionSugerida !== AccionDificultad.BajarDificultad) return null;
    return (
      this.teoriaDelTema().find((c) => c.tipo === TipoContenidoTeorico.Subtema && c.clave === res.subtema) ?? null
    );
  });

  constructor() {
    if (this.total() === 0) {
      this.cargando.set(false);
      this.error.set(true);
      return;
    }

    if (this.temaId) {
      this.temaService.obtenerTeoria(this.temaId).subscribe({
        next: (teoria) => this.teoriaDelTema.set(teoria),
      });
    }

    this.cargarEjercicioActual();
  }

  private leerIdsDeQueryParams(): string[] {
    const valor = this.route.snapshot.queryParamMap.get('preguntas');
    return valor ? valor.split(',').filter((id) => id.length > 0) : [];
  }

  private leerNivelDeQueryParams(): NivelDesempeno | null {
    const valor = this.route.snapshot.queryParamMap.get('nivel');
    return valor !== null ? (Number(valor) as NivelDesempeno) : null;
  }

  private cargarEjercicioActual(): void {
    this.cargando.set(true);
    this.opcionSeleccionada.set(null);
    this.resultado.set(null);

    this.ejercicioService.obtenerEjercicio(this.colaIds()[this.indice()]).subscribe({
      next: (ejercicio) => {
        this.ejercicio.set(ejercicio);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set(true);
        this.cargando.set(false);
      },
    });
  }

  letraOpcion(indice: number): string {
    return String.fromCharCode(65 + indice);
  }

  etiquetaNivel(nivel: NivelDesempeno): string {
    return ETIQUETA_NIVEL[nivel];
  }

  seleccionarOpcion(opcionId: string): void {
    if (this.resultado() !== null) {
      return;
    }
    this.opcionSeleccionada.set(opcionId);
  }

  enviarRespuesta(): void {
    const opcionId = this.opcionSeleccionada();
    const ejercicio = this.ejercicio();
    if (!opcionId || !ejercicio || this.resultado() !== null) {
      return;
    }

    this.enviando.set(true);
    this.ejercicioService.registrarIntento(ejercicio.id, opcionId).subscribe({
      next: (resultado) => {
        this.resultado.set(resultado);
        this.mensajeCambioNivel.set(null);

        if (resultado.esCorrecta) {
          this.aciertos.update((valor) => valor + 1);
        }

        this.enviando.set(false);
        this.reaccionarAAccionDelMotor(resultado);
      },
      error: () => {
        this.error.set(true);
        this.enviando.set(false);
      },
    });
  }

  private reaccionarAAccionDelMotor(resultado: IntentoResultado): void {
    const nivel = this.nivelActual();
    if (nivel === null || !this.temaId) {
      return;
    }

    if (resultado.accionSugerida === AccionDificultad.SubirDificultad && nivel !== NivelDesempeno.Avanzado) {
      this.cambiarNivel(
        nivel + 1,
        resultado.subtema,
        'Vas muy bien en este subtema: subimos su dificultad en los siguientes ejercicios.',
      );
    } else if (resultado.accionSugerida === AccionDificultad.BajarDificultad && nivel !== NivelDesempeno.Basico) {
      this.cambiarNivel(
        nivel - 1,
        resultado.subtema,
        'Bajamos la dificultad de este subtema para reforzar mejor los conceptos.',
      );
    }
  }

  private cambiarNivel(nuevoNivel: NivelDesempeno, subtema: string, mensaje: string): void {
    this.temaService.obtenerPreguntas(this.temaId!, nuevoNivel, { subtema }).subscribe({
      next: (preguntas) => {
        if (preguntas.length === 0) {
          return;
        }

        const colaActual = this.colaIds();
        const yaResueltos = colaActual.slice(0, this.indice() + 1);
        this.colaIds.set([...yaResueltos, ...preguntas.map((p) => p.id)]);
        this.nivelActual.set(nuevoNivel);
        this.mensajeCambioNivel.set(mensaje);
      },
    });
  }

  siguienteEjercicio(): void {
    this.indice.update((valor) => valor + 1);
    if (!this.terminado()) {
      this.cargarEjercicioActual();
    }
  }
}
