import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { AdminService } from '../services/admin.service';
import { AdminVideoApoyo } from '../models/admin.model';

@Component({
  selector: 'app-admin-videos',
  imports: [ReactiveFormsModule],
  templateUrl: './admin-videos.html',
})
export class AdminVideos {
  private readonly adminService = inject(AdminService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);

  private readonly temaId = this.route.snapshot.paramMap.get('temaId')!;

  protected readonly videos = signal<AdminVideoApoyo[]>([]);
  protected readonly cargando = signal(true);
  protected readonly mostrarFormulario = signal(false);
  protected readonly editando = signal<AdminVideoApoyo | null>(null);
  protected readonly error = signal<string | null>(null);

  protected readonly formulario = this.formBuilder.nonNullable.group({
    titulo: ['', Validators.required],
    url: ['', Validators.required],
  });

  constructor() {
    this.cargarVideos();
  }

  private cargarVideos(): void {
    this.cargando.set(true);
    this.adminService.obtenerVideosPorTema(this.temaId).subscribe({
      next: (videos) => {
        this.videos.set(videos);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false),
    });
  }

  abrirCrear(): void {
    this.editando.set(null);
    this.error.set(null);
    this.formulario.reset({ titulo: '', url: '' });
    this.mostrarFormulario.set(true);
  }

  abrirEditar(video: AdminVideoApoyo): void {
    this.editando.set(video);
    this.error.set(null);
    this.formulario.reset({ titulo: video.titulo, url: video.url });
    this.mostrarFormulario.set(true);
  }

  cancelar(): void {
    this.mostrarFormulario.set(false);
  }

  guardar(): void {
    if (this.formulario.invalid) {
      return;
    }

    const valores = this.formulario.getRawValue();
    const edicion = this.editando();

    const operacion: Observable<unknown> = edicion
      ? this.adminService.actualizarVideo(edicion.id, valores)
      : this.adminService.crearVideo({ temaId: this.temaId, ...valores });

    operacion.subscribe({
      next: () => {
        this.mostrarFormulario.set(false);
        this.cargarVideos();
      },
      error: (err: HttpErrorResponse) => this.error.set(err.error?.mensaje ?? 'Ocurrió un error al guardar.'),
    });
  }

  eliminar(video: AdminVideoApoyo): void {
    this.adminService.eliminarVideo(video.id).subscribe({
      next: () => this.cargarVideos(),
    });
  }
}
