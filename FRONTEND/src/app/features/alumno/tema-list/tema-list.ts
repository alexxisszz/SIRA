import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TemaService } from '../services/tema.service';
import { Tema } from '../models/tema.model';

@Component({
  selector: 'app-tema-list',
  imports: [],
  templateUrl: './tema-list.html',
})
export class TemaList {
  private readonly temaService = inject(TemaService);
  private readonly router = inject(Router);

  protected readonly temas = signal<Tema[]>([]);
  protected readonly cargando = signal(true);
  protected readonly error = signal(false);

  constructor() {
    this.temaService.obtenerTemas().subscribe({
      next: (temas) => {
        this.temas.set(temas);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set(true);
        this.cargando.set(false);
      },
    });
  }

  seleccionarTema(temaId: string): void {
    this.router.navigate(['/temas', temaId, 'evaluacion']);
  }
}