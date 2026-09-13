import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { DocenteService } from '../services/docente.service';
import { TemaService } from '../../alumno/services/tema.service';
import { AlumnoResumen, EstadoAlumno, ResumenGrupo } from '../models/docente.model';
import { Tema } from '../../alumno/models/tema.model';
import { NivelDesempeno } from '../../alumno/models/recomendacion.model';
import { CLASE_ESTADO, CLASE_NIVEL, ETIQUETA_NIVEL } from '../../../shared/utils/nivel.util';
import { convertirAVigesimal } from '../../../shared/utils/calificacion.util';

type ColumnaOrden = 'alumno' | 'grado' | 'progreso' | 'nivel' | 'ultimoResultado' | 'estado' | 'ultimaActividad';

const TAMANO_PAGINA = 8;

@Component({
  selector: 'app-alumnos-list',
  imports: [DatePipe, DecimalPipe, FormsModule],
  templateUrl: './alumnos-list.html',
  styleUrl: './alumnos-list.scss',
})
export class AlumnosList {
  private readonly docenteService = inject(DocenteService);
  private readonly temaService = inject(TemaService);
  private readonly router = inject(Router);

  protected readonly temas = signal<Tema[]>([]);
  protected readonly temaSeleccionadoId = signal<string | null>(null);

  protected readonly alumnos = signal<AlumnoResumen[]>([]);
  protected readonly resumen = signal<ResumenGrupo | null>(null);
  protected readonly cargando = signal(true);
  protected readonly error = signal(false);

  protected readonly busqueda = signal('');
  protected readonly filtroGrado = signal('');
  protected readonly filtroNivel = signal('');
  protected readonly filtroEstado = signal('');
  protected readonly columnaOrden = signal<ColumnaOrden>('alumno');
  protected readonly ordenAscendente = signal(true);
  protected readonly pagina = signal(1);

  protected readonly temaSeleccionadoNombre = computed(
    () => this.temas().find((t) => t.id === this.temaSeleccionadoId())?.nombre ?? '',
  );

  protected readonly grados = computed(() =>
    [...new Set(this.alumnos().map((a) => a.grado))].sort((a, b) => a.localeCompare(b)),
  );

  protected readonly alumnosFiltrados = computed(() => {
    const busqueda = this.busqueda().trim().toLowerCase();
    const grado = this.filtroGrado();
    const nivel = this.filtroNivel();
    const estado = this.filtroEstado();

    let resultado = this.alumnos().filter((a) => {
      const nombreCompleto = `${a.apellidos}, ${a.nombres}`.toLowerCase();
      const coincideBusqueda = !busqueda || nombreCompleto.includes(busqueda);
      const coincideGrado = !grado || a.grado === grado;
      const coincideNivel = !nivel || (a.nivelActual !== null && String(a.nivelActual) === nivel);
      const coincideEstado = !estado || a.estado === estado;
      return coincideBusqueda && coincideGrado && coincideNivel && coincideEstado;
    });

    const columna = this.columnaOrden();
    const signo = this.ordenAscendente() ? 1 : -1;
    resultado = [...resultado].sort((a, b) => {
      switch (columna) {
        case 'grado':
          return signo * a.grado.localeCompare(b.grado);
        case 'progreso':
          return signo * (a.porcentajeAvance - b.porcentajeAvance);
        case 'nivel':
          return signo * ((a.nivelActual ?? -1) - (b.nivelActual ?? -1));
        case 'ultimoResultado':
          return signo * ((a.ultimoPuntaje ?? -1) - (b.ultimoPuntaje ?? -1));
        case 'estado':
          return signo * a.estado.localeCompare(b.estado);
        case 'ultimaActividad':
          return signo * ((a.ultimaEvaluacion ?? '').localeCompare(b.ultimaEvaluacion ?? ''));
        default:
          return signo * `${a.apellidos}, ${a.nombres}`.localeCompare(`${b.apellidos}, ${b.nombres}`);
      }
    });

    return resultado;
  });

  protected readonly totalPaginas = computed(() => Math.max(1, Math.ceil(this.alumnosFiltrados().length / TAMANO_PAGINA)));

  protected readonly alumnosPagina = computed(() => {
    const inicio = (this.pagina() - 1) * TAMANO_PAGINA;
    return this.alumnosFiltrados().slice(inicio, inicio + TAMANO_PAGINA);
  });

  constructor() {
    this.temaService.obtenerTemas().subscribe({
      next: (temas) => {
        this.temas.set(temas);
        if (temas.length > 0) {
          this.temaSeleccionadoId.set(temas[0].id);
          this.cargarDatos(temas[0].id);
        } else {
          this.cargarDatos(null);
        }
      },
      error: () => this.cargarDatos(null),
    });
  }

  private cargarDatos(temaId: string | null): void {
    this.cargando.set(true);
    this.error.set(false);

    this.docenteService.obtenerAlumnos(temaId ?? undefined).subscribe({
      next: (alumnos) => {
        this.alumnos.set(alumnos);
        this.pagina.set(1);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set(true);
        this.cargando.set(false);
      },
    });

    this.docenteService.obtenerResumenGrupo(temaId ?? undefined).subscribe({
      next: (resumen) => this.resumen.set(resumen),
      error: () => {},
    });
  }

  cambiarTema(temaId: string): void {
    this.temaSeleccionadoId.set(temaId);
    this.cargarDatos(temaId);
  }

  ordenarPor(columna: ColumnaOrden): void {
    if (this.columnaOrden() === columna) {
      this.ordenAscendente.set(!this.ordenAscendente());
    } else {
      this.columnaOrden.set(columna);
      this.ordenAscendente.set(true);
    }
  }

  irAPagina(pagina: number): void {
    this.pagina.set(Math.min(Math.max(1, pagina), this.totalPaginas()));
  }

  iniciales(alumno: AlumnoResumen): string {
    return `${alumno.nombres.charAt(0)}${alumno.apellidos.charAt(0)}`.toUpperCase();
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

  porcentajeRequierenApoyo(): number {
    const resumen = this.resumen();
    if (!resumen || resumen.totalEstudiantes === 0) return 0;
    return (resumen.estudiantesRequierenApoyo / resumen.totalEstudiantes) * 100;
  }

  verDetalle(alumnoId: string): void {
    this.router.navigate(['/docente/alumnos', alumnoId]);
  }
}
