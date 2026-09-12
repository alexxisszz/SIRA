import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { TemaService } from '../services/tema.service';
import { AlumnoService, PerfilAlumno } from '../services/alumno.service';
import { ContenidoTeorico, TipoContenidoTeorico } from '../models/contenido-teorico.model';
import { EstadoTema, TipoEvaluacion, VideoApoyo } from '../models/evaluacion.model';
import { ProgresoSubtema } from '../models/ejercicio.model';
import { NivelDesempeno } from '../models/recomendacion.model';
import { CLAVE_NIVEL, ETIQUETA_NIVEL } from '../../../shared/utils/nivel.util';
import { obtenerDescripcionTema } from '../../../shared/utils/tema-descripcion.util';
import { obtenerUrlEmbedYoutube } from '../../../shared/utils/youtube.util';

const TEORIA_VACIA = { titulo: '', parrafos: [] as string[] };

interface NivelCard {
  nivel: NivelDesempeno;
  etiqueta: string;
  aprobado: boolean;
  desbloqueado: boolean;
  teoria: { titulo: string; parrafos: string[] };
}

interface VideoEmbebido {
  titulo: string;
  url: SafeResourceUrl;
}

interface PasoContenido {
  numero: number;
  titulo: string;
  descripcion: string;
  icono: string;
}

const PASOS_CONTENIDO: PasoContenido[] = [
  { numero: 1, titulo: 'Prueba de entrada', descripcion: 'Evalúa tu nivel actual', icono: 'bi-clipboard-check' },
  { numero: 2, titulo: 'Teoría', descripcion: 'Conceptos y ejemplos', icono: 'bi-book' },
  { numero: 3, titulo: 'Práctica por niveles', descripcion: 'Ejercicios adaptados a tu nivel', icono: 'bi-bar-chart' },
  { numero: 4, titulo: 'Evaluación final', descripcion: 'Demuestra lo que aprendiste', icono: 'bi-award' },
  { numero: 5, titulo: 'Resultados y recomendaciones', descripcion: 'Conoce tu progreso', icono: 'bi-graph-up-arrow' },
];

@Component({
  selector: 'app-tema-detalle',
  imports: [RouterLink],
  templateUrl: './tema-detalle.html',
  styleUrl: './tema-detalle.scss',
})
export class TemaDetalle {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly temaService = inject(TemaService);
  private readonly alumnoService = inject(AlumnoService);
  private readonly sanitizer = inject(DomSanitizer);

  private readonly temaId = this.route.snapshot.paramMap.get('temaId')!;

  protected readonly estado = signal<EstadoTema | null>(null);
  protected readonly videos = signal<VideoEmbebido[]>([]);
  protected readonly temaNombre = signal('');
  protected readonly perfil = signal<PerfilAlumno | null>(null);
  protected readonly progresoPractica = signal<ProgresoSubtema[]>([]);
  protected readonly teoria = signal<ContenidoTeorico[]>([]);
  protected readonly cargando = signal(true);
  protected readonly error = signal(false);
  protected readonly teoriaAbierta = signal<NivelDesempeno | null>(null);
  protected readonly TipoEvaluacion = TipoEvaluacion;
  protected readonly pasos = PASOS_CONTENIDO;

  constructor() {
    forkJoin({
      estado: this.temaService.obtenerEstado(this.temaId),
      videos: this.temaService.obtenerVideos(this.temaId).pipe(catchError(() => of([] as VideoApoyo[]))),
      temas: this.temaService.obtenerTemas().pipe(catchError(() => of([]))),
      perfil: this.alumnoService.obtenerMiPerfil().pipe(catchError(() => of(null))),
      progresoPractica: this.temaService
        .obtenerProgresoPractica(this.temaId)
        .pipe(catchError(() => of([] as ProgresoSubtema[]))),
      teoria: this.temaService.obtenerTeoria(this.temaId).pipe(catchError(() => of([] as ContenidoTeorico[]))),
    }).subscribe({
      next: ({ estado, videos, temas, perfil, progresoPractica, teoria }) => {
        this.estado.set(estado);
        this.perfil.set(perfil);
        this.progresoPractica.set(progresoPractica);
        this.teoria.set(teoria);
        this.temaNombre.set(temas.find((t) => t.id === this.temaId)?.nombre ?? '');
        this.videos.set(
          videos
            .map((v) => ({ titulo: v.titulo, embedUrl: obtenerUrlEmbedYoutube(v.url) }))
            .filter((v): v is { titulo: string; embedUrl: string } => v.embedUrl !== null)
            .map((v) => ({ titulo: v.titulo, url: this.sanitizer.bypassSecurityTrustResourceUrl(v.embedUrl) })),
        );
        this.cargando.set(false);
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

  protected descripcionTema(): string {
    return obtenerDescripcionTema(this.temaNombre());
  }

  protected heroEtiqueta(): string {
    const estado = this.estado();
    if (!estado) return '';
    if (estado.finalAprobada) return 'Tema completado';
    if (!estado.diagnosticoCompletado) return 'Prueba de entrada';
    if (!estado.basicoAprobado) return 'Nivel Básico';
    if (!estado.intermedioAprobado) return 'Nivel Intermedio';
    if (!estado.avanzadoAprobado) return 'Nivel Avanzado';
    return 'Prueba final';
  }

  protected heroNota(): string {
    const estado = this.estado();
    if (!estado) return '';
    if (estado.finalAprobada) return '¡Felicidades! Completaste este tema.';
    if (!estado.diagnosticoCompletado) return 'Primer paso • Nivel diagnóstico';
    if (!estado.avanzadoAprobado) return 'En progreso • Aprueba cada nivel para avanzar';
    return 'Último paso • Rinde la prueba final';
  }

  private teoriaDeNivel(nivel: NivelDesempeno): { titulo: string; parrafos: string[] } {
    const contenido = this.teoria().find(
      (c) => c.tipo === TipoContenidoTeorico.NivelGeneral && c.clave === CLAVE_NIVEL[nivel],
    );
    return contenido ?? TEORIA_VACIA;
  }

  protected niveles(): NivelCard[] {
    const estado = this.estado();
    if (!estado) return [];

    return [
      {
        nivel: NivelDesempeno.Basico,
        etiqueta: ETIQUETA_NIVEL[NivelDesempeno.Basico],
        aprobado: estado.basicoAprobado,
        desbloqueado: true,
        teoria: this.teoriaDeNivel(NivelDesempeno.Basico),
      },
      {
        nivel: NivelDesempeno.Intermedio,
        etiqueta: ETIQUETA_NIVEL[NivelDesempeno.Intermedio],
        aprobado: estado.intermedioAprobado,
        desbloqueado: estado.basicoAprobado,
        teoria: this.teoriaDeNivel(NivelDesempeno.Intermedio),
      },
      {
        nivel: NivelDesempeno.Avanzado,
        etiqueta: ETIQUETA_NIVEL[NivelDesempeno.Avanzado],
        aprobado: estado.avanzadoAprobado,
        desbloqueado: estado.intermedioAprobado,
        teoria: this.teoriaDeNivel(NivelDesempeno.Avanzado),
      },
    ];
  }

  protected pasoActual(): number {
    const estado = this.estado();
    if (!estado) return 1;
    if (!estado.diagnosticoCompletado) return 1;
    if (!estado.avanzadoAprobado) return 3;
    if (!estado.finalAprobada) return 4;
    return 5;
  }

  protected estadoPaso(paso: PasoContenido): 'completado' | 'actual' | 'pendiente' {
    const actual = this.pasoActual();
    if (paso.numero < actual) return 'completado';
    if (paso.numero === actual) return 'actual';
    return 'pendiente';
  }

  alternarTeoria(nivel: NivelDesempeno): void {
    this.teoriaAbierta.set(this.teoriaAbierta() === nivel ? null : nivel);
  }

  iniciarDiagnostico(): void {
    this.router.navigate(['/temas', this.temaId, 'evaluacion'], {
      queryParams: { tipo: TipoEvaluacion.Diagnostica },
    });
  }

  iniciarNivel(nivel: NivelDesempeno): void {
    this.router.navigate(['/temas', this.temaId, 'evaluacion'], {
      queryParams: { tipo: TipoEvaluacion.PorNivel, nivel },
    });
  }

  private subtemasDominados(): string[] {
    return this.progresoPractica()
      .filter((p) => p.intentos >= 3 && p.aciertos / p.intentos >= 0.8)
      .map((p) => p.subtema);
  }

  practicarNivel(nivel: NivelDesempeno): void {
    this.temaService.obtenerPreguntas(this.temaId, nivel, { excluirSubtemas: this.subtemasDominados() }).subscribe({
      next: (preguntas) => {
        if (preguntas.length === 0) {
          // Si ya domina todos los subtemas de este nivel, se practica sin excluir nada.
          this.temaService.obtenerPreguntas(this.temaId, nivel).subscribe({
            next: (todas) => this.irAPracticar(todas, nivel),
          });
          return;
        }
        this.irAPracticar(preguntas, nivel);
      },
    });
  }

  private irAPracticar(preguntas: { id: string }[], nivel: NivelDesempeno): void {
    this.router.navigate(['/practicar'], {
      queryParams: {
        preguntas: preguntas.map((p) => p.id).join(','),
        temaId: this.temaId,
        nivel,
      },
    });
  }

  iniciarFinal(): void {
    this.router.navigate(['/temas', this.temaId, 'evaluacion'], {
      queryParams: { tipo: TipoEvaluacion.Final },
    });
  }
}
