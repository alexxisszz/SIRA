import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { AdminService } from '../services/admin.service';
import { AdminContenidoTeorico, TipoContenidoTeorico } from '../models/admin.model';

const ETIQUETA_TIPO: Record<TipoContenidoTeorico, string> = {
  [TipoContenidoTeorico.NivelGeneral]: 'Nivel general',
  [TipoContenidoTeorico.Subtema]: 'Subtema',
};

@Component({
  selector: 'app-admin-contenido-teorico',
  imports: [ReactiveFormsModule],
  templateUrl: './admin-contenido-teorico.html',
})
export class AdminContenidoTeoricoComponent {
  private readonly adminService = inject(AdminService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);

  private readonly temaId = this.route.snapshot.paramMap.get('temaId')!;

  protected readonly TipoContenidoTeorico = TipoContenidoTeorico;
  protected readonly etiquetaTipo = ETIQUETA_TIPO;

  protected readonly contenidos = signal<AdminContenidoTeorico[]>([]);
  protected readonly cargando = signal(true);
  protected readonly mostrarFormulario = signal(false);
  protected readonly editando = signal<AdminContenidoTeorico | null>(null);
  protected readonly error = signal<string | null>(null);

  protected readonly formulario = this.formBuilder.nonNullable.group({
    tipo: [TipoContenidoTeorico.Subtema, Validators.required],
    clave: ['', Validators.required],
    titulo: ['', Validators.required],
    parrafos: ['', Validators.required],
  });

  constructor() {
    this.cargarContenidos();
  }

  private cargarContenidos(): void {
    this.cargando.set(true);
    this.adminService.obtenerTeoriaPorTema(this.temaId).subscribe({
      next: (contenidos) => {
        this.contenidos.set(contenidos);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false),
    });
  }

  abrirCrear(): void {
    this.editando.set(null);
    this.error.set(null);
    this.formulario.reset({ tipo: TipoContenidoTeorico.Subtema, clave: '', titulo: '', parrafos: '' });
    this.mostrarFormulario.set(true);
  }

  abrirEditar(contenido: AdminContenidoTeorico): void {
    this.editando.set(contenido);
    this.error.set(null);
    this.formulario.reset({
      tipo: contenido.tipo,
      clave: contenido.clave,
      titulo: contenido.titulo,
      parrafos: contenido.parrafos.join('\n'),
    });
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
    const parrafos = valores.parrafos
      .split('\n')
      .map((p) => p.trim())
      .filter((p) => p.length > 0);

    if (parrafos.length === 0) {
      this.error.set('Debes escribir al menos un párrafo.');
      return;
    }

    const edicion = this.editando();
    const datos = { clave: valores.clave, titulo: valores.titulo, parrafos };

    const operacion: Observable<unknown> = edicion
      ? this.adminService.actualizarContenidoTeorico(edicion.id, datos)
      : this.adminService.crearContenidoTeorico({ temaId: this.temaId, tipo: valores.tipo, ...datos });

    operacion.subscribe({
      next: () => {
        this.mostrarFormulario.set(false);
        this.cargarContenidos();
      },
      error: (err: HttpErrorResponse) => this.error.set(err.error?.mensaje ?? 'Ocurrió un error al guardar.'),
    });
  }

  eliminar(contenido: AdminContenidoTeorico): void {
    this.adminService.eliminarContenidoTeorico(contenido.id).subscribe({
      next: () => this.cargarContenidos(),
    });
  }
}
