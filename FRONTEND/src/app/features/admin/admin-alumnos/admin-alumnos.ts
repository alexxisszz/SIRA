import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { AdminService } from '../services/admin.service';
import { AdminAlumno } from '../models/admin.model';

@Component({
  selector: 'app-admin-alumnos',
  imports: [ReactiveFormsModule],
  templateUrl: './admin-alumnos.html',
})
export class AdminAlumnos {
  private readonly adminService = inject(AdminService);
  private readonly formBuilder = inject(FormBuilder);

  protected readonly alumnos = signal<AdminAlumno[]>([]);
  protected readonly cargando = signal(true);
  protected readonly mostrarFormulario = signal(false);
  protected readonly editando = signal<AdminAlumno | null>(null);
  protected readonly error = signal<string | null>(null);

  protected readonly formulario = this.formBuilder.nonNullable.group({
    correoElectronico: ['', [Validators.required, Validators.email]],
    contrasena: [''],
    nombres: ['', Validators.required],
    apellidos: ['', Validators.required],
    grado: ['', Validators.required],
  });

  constructor() {
    this.cargarAlumnos();
  }

  private cargarAlumnos(): void {
    this.cargando.set(true);
    this.adminService.obtenerAlumnos().subscribe({
      next: (alumnos) => {
        this.alumnos.set(alumnos);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false),
    });
  }

  abrirCrear(): void {
    this.editando.set(null);
    this.error.set(null);
    this.formulario.reset({ correoElectronico: '', contrasena: '', nombres: '', apellidos: '', grado: '' });
    this.formulario.get('correoElectronico')!.enable();
    this.formulario.get('contrasena')!.setValidators(Validators.required);
    this.mostrarFormulario.set(true);
  }

  abrirEditar(alumno: AdminAlumno): void {
    this.editando.set(alumno);
    this.error.set(null);
    this.formulario.reset({
      correoElectronico: alumno.correoElectronico,
      contrasena: '',
      nombres: alumno.nombres,
      apellidos: alumno.apellidos,
      grado: alumno.grado,
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
      ? this.adminService.actualizarAlumno(edicion.alumnoId, {
          nombres: valores.nombres,
          apellidos: valores.apellidos,
          grado: valores.grado,
        })
      : this.adminService.crearAlumno({
          correoElectronico: valores.correoElectronico,
          contrasena: valores.contrasena,
          nombres: valores.nombres,
          apellidos: valores.apellidos,
          grado: valores.grado,
        });

    operacion.subscribe({
      next: () => {
        this.mostrarFormulario.set(false);
        this.cargarAlumnos();
      },
      error: (err: HttpErrorResponse) => this.error.set(err.error?.mensaje ?? 'Ocurrió un error al guardar.'),
    });
  }

  cambiarEstado(alumno: AdminAlumno): void {
    this.adminService.cambiarEstadoAlumno(alumno.alumnoId, !alumno.activo).subscribe({
      next: () => this.cargarAlumnos(),
    });
  }
}
