import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { TemaService } from '../services/tema.service';
import { AlumnoService, PerfilAlumno, ResumenProgreso } from '../services/alumno.service';
import { Tema } from '../models/tema.model';
import { EstadoTema } from '../models/evaluacion.model';
import { ResultadoHistorico } from '../../docente/models/docente.model';
import { obtenerDescripcionTema } from '../../../shared/utils/tema-descripcion.util';

interface TemaConEstado extends Tema {
  estado: EstadoTema | null;
}

const FRASES_MOTIVACIONALES = [
  'Los errores son una oportunidad para aprender.',
  'Cada ejercicio resuelto es un paso más hacia tu meta.',
  'La constancia vale más que la perfección.',
  'Equivocarte es parte de entender mejor un tema.',
  'Pequeños avances diarios generan grandes resultados.',
  'Confía en el proceso: la práctica hace al maestro.',
];

@Component({
  selector: 'app-tema-list',
  imports: [RouterLink, DatePipe],
  templateUrl: './tema-list.html',
  styleUrl: './tema-list.scss',
})
export class TemaList {
  private readonly temaService = inject(TemaService);
  private readonly alumnoService = inject(AlumnoService);
  private readonly router = inject(Router);

  protected readonly temas = signal<TemaConEstado[]>([]);
  protected readonly perfil = signal<PerfilAlumno | null>(null);
  protected readonly historial = signal<ResultadoHistorico[]>([]);
  protected readonly resumenProgreso = signal<ResumenProgreso | null>(null);
  protected readonly cargando = signal(true);
  protected readonly error = signal(false);

  constructor() {
    forkJoin({
      temas: this.temaService.obtenerTemas(),
      perfil: this.alumnoService.obtenerMiPerfil().pipe(catchError(() => of(null))),
      historial: this.alumnoService.obtenerMiHistorial().pipe(catchError(() => of([] as ResultadoHistorico[]))),
      resumenProgreso: this.alumnoService.obtenerResumenProgreso().pipe(catchError(() => of(null))),
    }).subscribe({
      next: ({ temas, perfil, historial, resumenProgreso }) => {
        this.perfil.set(perfil);
        this.historial.set(historial);
        this.resumenProgreso.set(resumenProgreso);

        if (temas.length === 0) {
          this.temas.set([]);
          this.cargando.set(false);
          return;
        }

        forkJoin(
          temas.map((tema) => this.temaService.obtenerEstado(tema.id).pipe(catchError(() => of(null)))),
        ).subscribe((estados) => {
          this.temas.set(temas.map((tema, i) => ({ ...tema, estado: estados[i] })));
          this.cargando.set(false);
        });
      },
      error: () => {
        this.error.set(true);
        this.cargando.set(false);
      },
    });
  }

  protected iniciales(): string {
    const perfil = this.perfil();
    if (!perfil) return '';
    return `${perfil.nombres.charAt(0)}${perfil.apellidos.charAt(0)}`.toUpperCase();
  }

  protected descripcion(tema: Tema): string {
    return obtenerDescripcionTema(tema.nombre);
  }

  protected estadoTexto(tema: TemaConEstado): string {
    const estado = tema.estado;
    if (!estado) return '';
    if (!estado.temaDesbloqueado) return 'Bloqueado';
    if (estado.finalAprobada) return 'Completado';
    if (!estado.diagnosticoCompletado) return 'Prueba de entrada pendiente';
    if (!estado.basicoAprobado) return 'En progreso · Nivel Básico';
    if (!estado.intermedioAprobado) return 'En progreso · Nivel Intermedio';
    if (!estado.avanzadoAprobado) return 'En progreso · Nivel Avanzado';
    return 'Prueba final disponible';
  }

  protected estadoClase(tema: TemaConEstado): string {
    const estado = tema.estado;
    if (!estado) return '';
    if (!estado.temaDesbloqueado) return 'tema-card__estado--bloqueado';
    if (estado.finalAprobada) return 'tema-card__estado--completado';
    return 'tema-card__estado--progreso';
  }

  protected progresoPorcentaje(tema: TemaConEstado): number {
    const estado = tema.estado;
    if (!estado || !estado.temaDesbloqueado) return 0;
    if (estado.finalAprobada) return 100;
    if (estado.avanzadoAprobado) return 80;
    if (estado.intermedioAprobado) return 60;
    if (estado.basicoAprobado) return 40;
    if (estado.diagnosticoCompletado) return 20;
    return 0;
  }

  protected temasCompletados(): number {
    return this.temas().filter((t) => t.estado?.finalAprobada).length;
  }

  protected progresoGeneralPorcentaje(): number {
    const total = this.temas().length;
    if (total === 0) return 0;
    return Math.round((this.temasCompletados() / total) * 100);
  }

  protected ejerciciosIntentados(): number {
    return this.resumenProgreso()?.ejerciciosIntentados ?? 0;
  }

  protected ejerciciosCorrectos(): number {
    return this.resumenProgreso()?.ejerciciosCorrectos ?? 0;
  }

  protected diasDePractica(): number {
    return this.resumenProgreso()?.diasConActividad ?? 0;
  }

  protected actividadReciente(): ResultadoHistorico[] {
    return this.historial().slice(0, 3);
  }

  protected consejo(): string {
    const diaDelAnio = Math.floor(
      (Date.now() - new Date(new Date().getFullYear(), 0, 0).getTime()) / (1000 * 60 * 60 * 24),
    );
    return FRASES_MOTIVACIONALES[diaDelAnio % FRASES_MOTIVACIONALES.length];
  }


  seleccionarTema(tema: TemaConEstado): void {
    if (!tema.estado || !tema.estado.temaDesbloqueado) return;
    this.router.navigate(['/temas', tema.id]);
  }
}
