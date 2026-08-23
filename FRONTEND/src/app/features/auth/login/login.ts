import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { Rol } from '../../../core/models/auth.model';

function rutaPorRol(rol: Rol): string {
  switch (rol) {
    case Rol.Docente:
      return '/docente/alumnos';
    case Rol.Administrador:
      return '/admin/alumnos';
    default:
      return '/temas';
  }
}

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly cargando = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly mostrarContrasena = signal(false);

  alternarMostrarContrasena(): void {
    this.mostrarContrasena.update((valor) => !valor);
  }

  protected readonly formulario = this.formBuilder.nonNullable.group({
    correoElectronico: ['', [Validators.required, Validators.email]],
    contrasena: ['', Validators.required],
  });

  onSubmit(): void {
    if (this.formulario.invalid) {
      return;
    }

    this.cargando.set(true);
    this.error.set(null);

    this.authService.login(this.formulario.getRawValue()).subscribe({
      next: (respuesta) => this.router.navigate([rutaPorRol(respuesta.rol)]),
      error: () => {
        this.error.set('Correo o contraseña incorrectos.');
        this.cargando.set(false);
      },
    });
  }
}
