export enum TipoContenidoTeorico {
  NivelGeneral = 0,
  Subtema = 1,
}

export interface ContenidoTeorico {
  id: string;
  tipo: TipoContenidoTeorico;
  clave: string;
  titulo: string;
  parrafos: string[];
}
