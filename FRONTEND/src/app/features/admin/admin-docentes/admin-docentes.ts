import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { AdminService } from '../services/admin.service';
import { AdminDocente } from '../models/admin.model';

@Component({
  selector: 'app-admin-docentes',
  imports: [ReactiveFormsModule],
  templateUrl: './admin-docentes.html',
})
export class AdminDocentes {
  private readonly adminService = inject(AdminService);
  private readonly formBuilder = inject(FormBuilder);

  protected readonly docentes = signal<AdminDocente[]>([]);
  protected readonly cargando = signal(true);
  protected readonly mostrarFormulario = signal(false);
  protected readonly editando = signal<AdminDocente | null>(null);
  protected readonly error = signal<string | null>(null);

  protected readonly formulario = this.formBuilder.nonNullable.group({
    correoElectronico: ['', [Validators.required, Validators.email]],
    contrasena: [''],
    nombres: ['', Validators.required],
    apellidos: ['', Validators.required],
  });

  constructor() {
    this.cargarDocentes();
  }

  private cargarDocentes(): void {
    this.cargando.set(true);
    this.adminService.obtenerDocentes().subscribe({
      next: (docentes) => {
        this.docentes.set(docentes);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false),
    });
  }

  abrirCrear(): void {
    this.editando.set(null);
    this.error.set(null);
    this.formulario.reset({ correoElectronico: '', contrasena: '', nombres: '', apellidos: '' });
    this.formulario.get('correoElectronico')!.enable();
    this.formulario.get('contrasena')!.setValidators(Validators.required);
    this.mostrarFormulario.set(true);
  }

  abrirEditar(docente: AdminDocente): void {
    this.editando.set(docente);
    this.error.set(null);
    this.formulario.reset({
      correoElectronico: docente.correoElectronico,
      contrasena: '',
      nombres: docente.nombres,
      apellidos: docente.apellidos,
    });
    this.formulario.get('correoElectronico')!.disable();
    this.formulario.get('contrasena')!.clearValidators();
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
      ? this.adminService.actualizarDocente(edicion.docenteId, { nombres: valores.nombres, apellidos: valores.apellidos })
      : this.adminService.crearDocente({
          correoElectronico: valores.correoElectronico,
          contrasena: valores.contrasena,
          nombres: valores.nombres,
          apellidos: valores.apellidos,
        });

    operacion.subscribe({
      next: () => {
        this.mostrarFormulario.set(false);
        this.cargarDocentes();
      },
      error: (err: HttpErrorResponse) => this.error.set(err.error?.mensaje ?? 'Ocurrió un error al guardar.'),
    });
  }

  cambiarEstado(docente: AdminDocente): void {
    this.adminService.cambiarEstadoDocente(docente.docenteId, !docente.activo).subscribe({
      next: () => this.cargarDocentes(),
    });
  }
}
