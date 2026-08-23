import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormArray, FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { AdminService } from '../services/admin.service';
import { AdminPregunta, NivelDificultad } from '../models/admin.model';

const ETIQUETA_NIVEL: Record<NivelDificultad, string> = {
  [NivelDificultad.Basico]: 'Básico',
  [NivelDificultad.Intermedio]: 'Intermedio',
  [NivelDificultad.Avanzado]: 'Avanzado',
};

@Component({
  selector: 'app-admin-preguntas',
  imports: [ReactiveFormsModule],
  templateUrl: './admin-preguntas.html',
})
export class AdminPreguntas {
  private readonly adminService = inject(AdminService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);

  private readonly temaId = this.route.snapshot.paramMap.get('temaId')!;

  protected readonly NivelDificultad = NivelDificultad;
  protected readonly etiquetaNivel = ETIQUETA_NIVEL;

  protected readonly preguntas = signal<AdminPregunta[]>([]);
  protected readonly cargando = signal(true);
  protected readonly mostrarFormulario = signal(false);
  protected readonly editando = signal<AdminPregunta | null>(null);
  protected readonly opcionCorrectaIndex = signal(0);
  protected readonly error = signal<string | null>(null);

  protected readonly formulario = this.formBuilder.nonNullable.group({
    enunciado: ['', Validators.required],
    nivelDificultad: [NivelDificultad.Basico, Validators.required],
    opciones: this.formBuilder.array<FormControl<string>>([]),
  });

  constructor() {
    this.cargarPreguntas();
  }

  private crearControlOpcion(texto = ''): FormControl<string> {
    return this.formBuilder.nonNullable.control(texto, Validators.required);
  }

  protected get opciones(): FormArray<FormControl<string>> {
    return this.formulario.controls.opciones;
  }

  private cargarPreguntas(): void {
    this.cargando.set(true);
    this.adminService.obtenerPreguntasPorTema(this.temaId).subscribe({
      next: (preguntas) => {
        this.preguntas.set(preguntas);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false),
    });
  }

  agregarOpcion(): void {
    this.opciones.push(this.crearControlOpcion());
  }

  quitarOpcion(index: number): void {
    this.opciones.removeAt(index);
    if (this.opcionCorrectaIndex() >= this.opciones.length) {
      this.opcionCorrectaIndex.set(0);
    }
  }

  marcarComoCorrecta(index: number): void {
    this.opcionCorrectaIndex.set(index);
  }

  abrirCrear(): void {
    this.editando.set(null);
    this.error.set(null);
    this.opciones.clear();
    this.agregarOpcion();
    this.agregarOpcion();
    this.opcionCorrectaIndex.set(0);
    this.formulario.patchValue({ enunciado: '', nivelDificultad: NivelDificultad.Basico });
    this.mostrarFormulario.set(true);
  }

  abrirEditar(pregunta: AdminPregunta): void {
    this.editando.set(pregunta);
    this.error.set(null);
    this.opciones.clear();
    pregunta.opciones.forEach((opcion) => this.opciones.push(this.crearControlOpcion(opcion.texto)));
    this.opcionCorrectaIndex.set(pregunta.opciones.findIndex((o) => o.esCorrecta));
    this.formulario.patchValue({ enunciado: pregunta.enunciado, nivelDificultad: pregunta.nivelDificultad });
    this.mostrarFormulario.set(true);
  }

  cancelar(): void {
    this.mostrarFormulario.set(false);
  }

  guardar(): void {
    if (this.formulario.invalid || this.opciones.length < 2) {
      return;
    }

    const valores = this.formulario.getRawValue();
    const opciones = valores.opciones.map((texto, index) => ({
      texto,
      esCorrecta: index === this.opcionCorrectaIndex(),
    }));

    const datos = { enunciado: valores.enunciado, nivelDificultad: valores.nivelDificultad, opciones };
    const edicion = this.editando();

    const operacion: Observable<unknown> = edicion
      ? this.adminService.actualizarPregunta(edicion.id, datos)
      : this.adminService.crearPregunta({ temaId: this.temaId, ...datos });

    operacion.subscribe({
      next: () => {
        this.mostrarFormulario.set(false);
        this.cargarPreguntas();
      },
      error: (err: HttpErrorResponse) => this.error.set(err.error?.mensaje ?? 'Ocurrió un error al guardar.'),
    });
  }

  eliminar(pregunta: AdminPregunta): void {
    this.adminService.eliminarPregunta(pregunta.id).subscribe({
      next: () => this.cargarPreguntas(),
    });
  }
}
