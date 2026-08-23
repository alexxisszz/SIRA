import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AdminService } from '../services/admin.service';
import { AdminRegla } from '../models/admin.model';

@Component({
  selector: 'app-admin-reglas',
  imports: [ReactiveFormsModule],
  templateUrl: './admin-reglas.html',
})
export class AdminReglas {
  private readonly adminService = inject(AdminService);
  private readonly formBuilder = inject(FormBuilder);

  protected readonly reglas = signal<AdminRegla[]>([]);
  protected readonly cargando = signal(true);
  protected readonly editandoId = signal<string | null>(null);

  protected readonly formulario = this.formBuilder.nonNullable.group({
    nombre: ['', Validators.required],
    descripcionCondicion: ['', Validators.required],
    descripcionConclusion: ['', Validators.required],
    prioridad: [10, Validators.required],
  });

  constructor() {
    this.cargarReglas();
  }

  private cargarReglas(): void {
    this.cargando.set(true);
    this.adminService.obtenerReglas().subscribe({
      next: (reglas) => {
        this.reglas.set(reglas);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false),
    });
  }

  abrirEditar(regla: AdminRegla): void {
    this.editandoId.set(regla.id);
    this.formulario.reset({
      nombre: regla.nombre,
      descripcionCondicion: regla.descripcionCondicion,
      descripcionConclusion: regla.descripcionConclusion,
      prioridad: regla.prioridad,
    });
  }

  cancelar(): void {
    this.editandoId.set(null);
  }

  guardar(): void {
    const id = this.editandoId();
    if (!id || this.formulario.invalid) {
      return;
    }

    this.adminService.actualizarRegla(id, this.formulario.getRawValue()).subscribe({
      next: () => {
        this.editandoId.set(null);
        this.cargarReglas();
      },
    });
  }

  cambiarEstado(regla: AdminRegla): void {
    this.adminService.cambiarEstadoRegla(regla.id, !regla.activa).subscribe({
      next: () => this.cargarReglas(),
    });
  }
}
