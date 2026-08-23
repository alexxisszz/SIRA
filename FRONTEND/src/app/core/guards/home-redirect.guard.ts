import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Rol } from '../models/auth.model';

export const homeRedirectGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const destino = (() => {
    switch (authService.rol()) {
      case Rol.Docente:
        return '/docente/alumnos';
      case Rol.Administrador:
        return '/admin/alumnos';
      default:
        return '/temas';
    }
  })();

  return router.parseUrl(destino);
};
