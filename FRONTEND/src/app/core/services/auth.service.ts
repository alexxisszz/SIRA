import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse, Rol } from '../models/auth.model';

const CLAVE_TOKEN = 'token';
const CLAVE_ROL = 'rol';
const CLAVE_USUARIO_ID = 'usuarioId';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);

  private readonly tokenSignal = signal<string | null>(localStorage.getItem(CLAVE_TOKEN));
  private readonly rolSignal = signal<Rol | null>(this.leerRolAlmacenado());

  readonly estaAutenticado = computed(() => this.tokenSignal() !== null);
  readonly rol = this.rolSignal.asReadonly();

  login(credenciales: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/auth/login`, credenciales).pipe(
      tap((respuesta) => {
        localStorage.setItem(CLAVE_TOKEN, respuesta.token);
        localStorage.setItem(CLAVE_ROL, respuesta.rol.toString());
        localStorage.setItem(CLAVE_USUARIO_ID, respuesta.usuarioId);
        this.tokenSignal.set(respuesta.token);
        this.rolSignal.set(respuesta.rol);
      }),
    );
  }

  logout(): void {
    localStorage.removeItem(CLAVE_TOKEN);
    localStorage.removeItem(CLAVE_ROL);
    localStorage.removeItem(CLAVE_USUARIO_ID);
    this.tokenSignal.set(null);
    this.rolSignal.set(null);
  }

  obtenerToken(): string | null {
    return this.tokenSignal();
  }

  private leerRolAlmacenado(): Rol | null {
    const valor = localStorage.getItem(CLAVE_ROL);
    return valor === null ? null : (Number(valor) as Rol);
  }
}