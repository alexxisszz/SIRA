import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Rol } from '../models/auth.model';

export function roleGuard(rolesPermitidos: Rol[]): CanActivateFn {
  return () => {
    const authService = inject(AuthService);
    const router = inject(Router);
    const rolActual = authService.rol();

    if (rolActual !== null && rolesPermitidos.includes(rolActual)) {
      return true;
    }

    return router.parseUrl('/login');
  };
}
