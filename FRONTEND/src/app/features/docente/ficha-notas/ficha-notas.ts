import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { catchError, forkJoin, map, of } from 'rxjs';
import { DocenteService } from '../services/docente.service';
import { TemaService } from '../../alumno/services/tema.service';
import { FichaRegistroNota, TIPO_EVALUACION_FICHA_VALOR, TipoEvaluacionFicha } from '../models/docente.model';
import { Tema } from '../../alumno/models/tema.model';

type DimensionEditable = 'd2' | 'd3';
type NotasIndicadores = [number | null, number | null, number | null];

interface FilaFicha {
  base: FichaRegistroNota;
  d2: NotasIndicadores;
  d3: NotasIndicadores;
  modificada: boolean;
  errorGuardado: boolean;
}

/** Escala vigesimal usada en el sistema educativo peruano (ver calificacion.util.ts). */
const NOTA_MINIMA = 0;
const NOTA_MAXIMA = 20;

@Component({
  selector: 'app-ficha-notas',
  imports: [FormsModule],
  templateUrl: './ficha-notas.html',
  styleUrl: './ficha-notas.scss',
})
export class FichaNotas {
  private readonly docenteService = inject(DocenteService);
  private readonly temaService = inject(TemaService);

  protected readonly tipos: TipoEvaluacionFicha[] = ['Pretest', 'Postest'];
  protected readonly indicadores = [0, 1, 2] as const;
  protected readonly dimensiones = [
    {
      clave: 'D1',
      nombre: 'Cognitiva',
      indicadores: ['Comprensión de contenidos teóricos', 'Retención de información académica', 'Adquisición de habilidades'],
    },
    {
      clave: 'D2',
      nombre: 'Procedimental',
      indicadores: ['Aplicación de conocimientos', 'Desarrollo de habilidades', 'Resolución de problemas'],
    },
    {
      clave: 'D3',
      nombre: 'Actitudinal',
      indicadores: ['Motivación hacia el aprendizaje', 'Responsabilidad académica', 'Perseverancia en las actividades'],
    },
  ] as const;
  protected readonly notaMinima = NOTA_MINIMA;
  protected readonly notaMaxima = NOTA_MAXIMA;

  protected readonly temas = signal<Tema[]>([]);
  protected readonly temaSeleccionadoId = signal<string | null>(null);
  protected readonly tipoSeleccionado = signal<TipoEvaluacionFicha>('Pretest');

  protected readonly filas = signal<FilaFicha[]>([]);
  protected readonly cargando = signal(true);
  protected readonly error = signal(false);
  protected readonly guardando = signal(false);
  protected readonly mensajeGuardado = signal<string | null>(null);
  protected readonly errorGuardado = signal<string | null>(null);

  protected readonly temaSeleccionadoNombre = computed(
    () => this.temas().find((t) => t.id === this.temaSeleccionadoId())?.nombre ?? '',
  );

  protected readonly filasModificadas = computed(() => this.filas().filter((f) => f.modificada));

  protected readonly hayNotasInvalidas = computed(() =>
    this.filasModificadas().some((f) => !this.filaCompletaYValida(f)),
  );

  protected readonly puedeGuardar = computed(
    () => this.filasModificadas().length > 0 && !this.hayNotasInvalidas() && !this.guardando(),
  );

  constructor() {
    this.temaService.obtenerTemas().subscribe({
      next: (temas) => {
        this.temas.set(temas);
        if (temas.length > 0) {
          this.temaSeleccionadoId.set(temas[0].id);
          this.cargarFicha();
        } else {
          this.cargando.set(false);
        }
      },
      error: () => {
        this.error.set(true);
        this.cargando.set(false);
      },
    });
  }

  private cargarFicha(): void {
    const temaId = this.temaSeleccionadoId();
    if (!temaId) return;

    this.cargando.set(true);
    this.error.set(false);
    this.mensajeGuardado.set(null);
    this.errorGuardado.set(null);

    this.docenteService.obtenerFichaNotas(temaId, this.tipoSeleccionado()).subscribe({
      next: (ficha) => {
        this.filas.set(ficha.map((f) => this.crearFila(f)));
        this.cargando.set(false);
      },
      error: () => {
        this.error.set(true);
        this.cargando.set(false);
      },
    });
  }

  private crearFila(base: FichaRegistroNota): FilaFicha {
    return {
      base,
      d2: [base.d2I1, base.d2I2, base.d2I3],
      d3: [base.d3I1, base.d3I2, base.d3I3],
      modificada: false,
      errorGuardado: false,
    };
  }

  private confirmarDescarteCambios(): boolean {
    return this.filasModificadas().length === 0 || confirm('Hay notas sin guardar. ¿Deseas descartar los cambios?');
  }

  cambiarTema(temaId: string): void {
    if (!this.confirmarDescarteCambios()) return;
    this.temaSeleccionadoId.set(temaId);
    this.cargarFicha();
  }

  cambiarTipo(tipo: TipoEvaluacionFicha): void {
    if (tipo === this.tipoSeleccionado() || !this.confirmarDescarteCambios()) return;
    this.tipoSeleccionado.set(tipo);
    this.cargarFicha();
  }

  actualizarNota(alumnoId: string, dimension: DimensionEditable, indice: number, valor: number | null): void {
    this.mensajeGuardado.set(null);
    this.filas.update((filas) =>
      filas.map((f) => {
        if (f.base.alumnoId !== alumnoId) return f;
        const notas = [...f[dimension]] as NotasIndicadores;
        notas[indice] = valor === null || Number.isNaN(valor) ? null : valor;
        return { ...f, [dimension]: notas, modificada: true, errorGuardado: false };
      }),
    );
  }

  notaValida(nota: number | null): boolean {
    return nota !== null && nota >= NOTA_MINIMA && nota <= NOTA_MAXIMA;
  }

  private filaCompletaYValida(fila: FilaFicha): boolean {
    return [...fila.d2, ...fila.d3].every((n) => this.notaValida(n));
  }

  promedio(notas: readonly (number | null)[]): number | null {
    if (notas.some((n) => !this.notaValida(n))) return null;
    const valores = notas as number[];
    return this.redondear(valores.reduce((suma, n) => suma + n, 0) / valores.length);
  }

  promedioFinal(fila: FilaFicha): number | null {
    return this.promedio([fila.base.d1Promedio, this.promedio(fila.d2), this.promedio(fila.d3)]);
  }

  mostrarNota(nota: number | null): string {
    return nota === null ? '-' : nota.toFixed(1);
  }

  private redondear(valor: number): number {
    return Math.round(valor * 10) / 10;
  }

  guardarCambios(): void {
    const temaId = this.temaSeleccionadoId();
    if (!temaId || !this.puedeGuardar()) return;

    const tipo = this.tipoSeleccionado();
    const pendientes = this.filasModificadas();

    this.guardando.set(true);
    this.mensajeGuardado.set(null);
    this.errorGuardado.set(null);

    const peticiones = pendientes.map((fila) => {
      const [d2I1, d2I2, d2I3] = fila.d2 as number[];
      const [d3I1, d3I2, d3I3] = fila.d3 as number[];
      return this.docenteService
        .guardarNotasDocente({
          alumnoId: fila.base.alumnoId,
          temaId,
          tipoEvaluacion: TIPO_EVALUACION_FICHA_VALOR[tipo],
          d2I1,
          d2I2,
          d2I3,
          d3I1,
          d3I2,
          d3I3,
        })
        .pipe(
          map(() => ({ alumnoId: fila.base.alumnoId, ok: true })),
          catchError(() => of({ alumnoId: fila.base.alumnoId, ok: false })),
        );
    });

    forkJoin(peticiones).subscribe((resultados) => {
      const exitosos = new Set(resultados.filter((r) => r.ok).map((r) => r.alumnoId));
      const fallidos = new Set(resultados.filter((r) => !r.ok).map((r) => r.alumnoId));

      this.filas.update((filas) =>
        filas.map((f) => {
          if (exitosos.has(f.base.alumnoId)) return this.crearFila(this.baseActualizada(f));
          if (fallidos.has(f.base.alumnoId)) return { ...f, errorGuardado: true };
          return f;
        }),
      );

      if (fallidos.size > 0) {
        this.errorGuardado.set(`No se pudieron guardar las notas de ${fallidos.size} alumno(s). Intenta nuevamente.`);
      }
      if (exitosos.size > 0) {
        this.mensajeGuardado.set(`Se guardaron las notas de ${exitosos.size} alumno(s).`);
      }
      this.guardando.set(false);
    });
  }

  /** Refleja en la fila base las notas recien guardadas, para que "Descartar" no vuelva a valores antiguos. */
  private baseActualizada(fila: FilaFicha): FichaRegistroNota {
    const [d2I1, d2I2, d2I3] = fila.d2 as number[];
    const [d3I1, d3I2, d3I3] = fila.d3 as number[];
    return {
      ...fila.base,
      d2I1,
      d2I2,
      d2I3,
      d2Promedio: this.promedio(fila.d2) ?? 0,
      d3I1,
      d3I2,
      d3I3,
      d3Promedio: this.promedio(fila.d3) ?? 0,
      promedioFinal: this.promedioFinal(fila) ?? 0,
    };
  }

  descartarCambios(): void {
    this.filas.update((filas) => filas.map((f) => (f.modificada ? this.crearFila(f.base) : f)));
    this.errorGuardado.set(null);
  }
}
