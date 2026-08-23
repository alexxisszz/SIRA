import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AdminService } from '../services/admin.service';
import { AdminTema } from '../models/admin.model';

@Component({
  selector: 'app-admin-temas',
  imports: [ReactiveFormsModule],
  templateUrl: './admin-temas.html',
})
export class AdminTemas {
  private readonly adminService = inject(AdminService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly router = inject(Router);

  protected readonly temas = signal<AdminTema[]>([]);
  protected readonly cargando = signal(true);
  protected readonly mostrarFormulario = signal(false);
  protected readonly editando = signal<AdminTema | null>(null);

  protected readonly formulario = this.formBuilder.nonNullable.group({
    nombre: ['', Validators.required],
    orden: [1, [Validators.required, Validators.min(1)]],
  });

  constructor() {
    this.cargarTemas();
  }

  private cargarTemas(): void {
    this.cargando.set(true);
    this.adminService.obtenerTemas().subscribe({
      next: (temas) => {
        this.temas.set([...temas].sort((a, b) => a.orden - b.orden));
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false),
    });
  }

  abrirCrear(): void {
    this.editando.set(null);
    this.formulario.reset({ nombre: '', orden: this.temas().length + 1 });
    this.mostrarFormulario.set(true);
  }

  abrirEditar(tema: AdminTema): void {
    this.editando.set(tema);
    this.formulario.reset({ nombre: tema.nombre, orden: tema.orden });
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
      ? this.adminService.actualizarTema(edicion.id, valores)
      : this.adminService.crearTema(valores);

    operacion.subscribe({
      next: () => {
        this.mostrarFormulario.set(false);
        this.cargarTemas();
      },
    });
  }

  verPreguntas(tema: AdminTema): void {
    this.router.navigate(['/admin/temas', tema.id, 'preguntas']);
  }
}
