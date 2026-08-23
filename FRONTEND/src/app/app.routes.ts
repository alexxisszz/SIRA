import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { homeRedirectGuard } from './core/guards/home-redirect.guard';
import { roleGuard } from './core/guards/role.guard';
import { Rol } from './core/models/auth.model';
import { DashboardLayout } from './shared/dashboard-layout/dashboard-layout';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then((m) => m.Login),
  },
  {
    path: '',
    component: DashboardLayout,
    canActivate: [authGuard],
    children: [
      { path: '', pathMatch: 'full', canActivate: [homeRedirectGuard], children: [] },
      {
        path: 'temas',
        canActivate: [roleGuard([Rol.Alumno])],
        loadComponent: () => import('./features/alumno/tema-list/tema-list').then((m) => m.TemaList),
      },
      {
        path: 'temas/:temaId/evaluacion',
        canActivate: [roleGuard([Rol.Alumno])],
        loadComponent: () => import('./features/alumno/evaluacion/evaluacion').then((m) => m.Evaluacion),
      },
      {
        path: 'recomendaciones/:recomendacionId',
        canActivate: [roleGuard([Rol.Alumno])],
        loadComponent: () =>
          import('./features/alumno/recomendacion/recomendacion').then((m) => m.Recomendacion),
      },
      {
        path: 'docente/alumnos',
        canActivate: [roleGuard([Rol.Docente])],
        loadComponent: () => import('./features/docente/alumnos-list/alumnos-list').then((m) => m.AlumnosList),
      },
      {
        path: 'docente/alumnos/:alumnoId',
        canActivate: [roleGuard([Rol.Docente])],
        loadComponent: () =>
          import('./features/docente/alumno-detalle/alumno-detalle').then((m) => m.AlumnoDetalle),
      },
      {
        path: 'docente/estadisticas',
        canActivate: [roleGuard([Rol.Docente])],
        loadComponent: () => import('./features/docente/estadisticas/estadisticas').then((m) => m.Estadisticas),
      },
      {
        path: 'admin/alumnos',
        canActivate: [roleGuard([Rol.Administrador])],
        loadComponent: () => import('./features/admin/admin-alumnos/admin-alumnos').then((m) => m.AdminAlumnos),
      },
      {
        path: 'admin/docentes',
        canActivate: [roleGuard([Rol.Administrador])],
        loadComponent: () => import('./features/admin/admin-docentes/admin-docentes').then((m) => m.AdminDocentes),
      },
      {
        path: 'admin/temas',
        canActivate: [roleGuard([Rol.Administrador])],
        loadComponent: () => import('./features/admin/admin-temas/admin-temas').then((m) => m.AdminTemas),
      },
      {
        path: 'admin/temas/:temaId/preguntas',
        canActivate: [roleGuard([Rol.Administrador])],
        loadComponent: () =>
          import('./features/admin/admin-preguntas/admin-preguntas').then((m) => m.AdminPreguntas),
      },
      {
        path: 'admin/reglas',
        canActivate: [roleGuard([Rol.Administrador])],
        loadComponent: () => import('./features/admin/admin-reglas/admin-reglas').then((m) => m.AdminReglas),
      },
    ],
  },
];
