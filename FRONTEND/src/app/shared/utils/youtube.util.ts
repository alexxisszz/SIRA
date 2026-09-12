export function obtenerIdYoutube(url: string): string | null {
  const patrones = [
    /(?:youtube\.com\/watch\?v=|youtube\.com\/embed\/|youtu\.be\/)([a-zA-Z0-9_-]{6,})/,
  ];

  for (const patron of patrones) {
    const coincidencia = url.match(patron);
    if (coincidencia) return coincidencia[1];
  }

  return null;
}

export function obtenerUrlEmbedYoutube(url: string): string | null {
  const id = obtenerIdYoutube(url);
  return id ? `https://www.youtube-nocookie.com/embed/${id}` : null;
}
