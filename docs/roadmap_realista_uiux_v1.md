# Roadmap Realista de UI/UX — Vendemax Desktop (v1, sin migración de framework)

## Qué es este documento y qué reemplaza

Los otros cuatro documentos de esta carpeta (`benchmarking_competidores.md`, `propuesta_ui_ux.md`,
`roadmap_y_funcionalidades_v2.md`, `INVESTIGACION_MEJORAS_UXUI_VENDEMAX_DESKTOP.md`) son investigación
de competidores sin validar con clientes reales, y su plan de implementación asume una migración de
WinForms a WinUI3/WPF — una reescritura de framework, no un rediseño visual, con un costo que ninguno
de los cuatro documentos dimensiona correctamente.

Este documento toma lo que sí tiene mérito de esos cuatro (jerarquía tipográfica, consistencia de
color, buscador rápido) y lo aterriza a algo que se puede construir **sobre el WinForms actual**, en
días u horas por ítem, no en semanas por fase.

## Principios

1. **Sin migración de framework.** WinForms se queda. Todo lo de acá se logra con `Font`, `BackColor`,
   `ForeColor`, `FlatStyle` y layout de los controles que ya existen.
2. **Light mode por defecto.** Los cuatro documentos asumen dark mode porque "reduce fatiga visual" —
   cierto en oficina, dudoso en un mostrador con luz de local. Sin datos propios que lo contradigan,
   se prioriza contraste claro y legible.
3. **Cambios chicos y medibles**, no un big-bang de 12 semanas.
4. **Nada de features de negocio nuevas** (fidelización, variantes, promociones) sin hablar antes con
   clientes reales — ver "Explícitamente descartado" más abajo.

## Fase 0 — Paleta y tipografía consistentes (días, no semanas)

Hoy los colores están puestos a mano y no coinciden entre pantallas (`Color.Firebrick` en un botón
eliminar, `Color.ForestGreen` en otro, `Color.FromArgb(0,122,204)` en un tercero, `Color.SteelBlue` en
otro más). No hay una sola fuente de verdad.

- Agregar una clase estática `Theme` (o ampliar `Constantes.cs`) con los colores semánticos:
  fondo, superficie de tarjeta, texto primario/secundario, éxito, advertencia, peligro, acción primaria
  — mismos nombres que ya proponían los docs viejos, pero en tonos claros por default.
- Reemplazar los `Color.XXX` sueltos de cada formulario por esas constantes.
- Unificar la fuente: `Segoe UI` para texto general, tamaño más grande y bold para importes/totales
  (ya existe esto parcialmente en `VentasForm.lblTotal`, 24pt — llevarlo al resto de las pantallas
  donde se muestran montos).

## Fase 1 — Feedback visual en las pantallas de alto tráfico

- Reforzar de forma consistente los tres estados (éxito/advertencia/error) en Ventas, Compras y Caja
  usando los colores de la Fase 0 — hoy cada pantalla lo resuelve un poco distinto.
- Convertir la alerta de stock bajo (`Constantes.ALERTA_STOCK_MINIMO`, hoy solo visible si se entra al
  reporte) en un aviso discreto visible desde el Dashboard/Menú Principal, sin bloquear con un
  `MessageBox`.

## Fase 2 — Consistencia de los buscadores (seguir lo ya empezado)

- Ya se agregó búsqueda en vivo en Ventas, Clientes, Compras, Cuenta Corriente y Reporte de Fiados —
  unificar el estilo visual (misma altura, mismo placeholder con 🔍, mismo comportamiento) entre las
  cinco pantallas, que hoy están ligeramente distintas entre sí.
- Opcional más adelante: un atajo global (`Ctrl+K`) que abra un selector rápido para saltar entre
  pantallas (Ventas, Clientes, Productos, Caja...) — no un buscador universal de datos como pedían los
  docs viejos, que sería mucho más caro de construir bien.

## Fase 3 — Gate humano antes de construir features grandes

Ninguno de estos entra al roadmap técnico hasta validar con 2-3 clientes reales que lo necesitan:

- Programa de fidelización / puntos.
- Variantes de producto (talle, color).
- Promociones complejas (2x1, descuentos por categoría).
- Modo táctil / botonera con fotos.
- Asistente de reabastecimiento inteligente.

## Explícitamente descartado (por ahora)

- **Migración a WinUI3 / WPF / glassmorphism** — costo de reescritura no justificado sin ingresos
  recurrentes que lo sostengan.
- **Sincronización con VendemaxWeb** — reintroduce la infraestructura en la nube corriendo 24/7 que se
  decidió evitar a propósito al priorizar Desktop sobre Web.
- **Dark mode por defecto** — sin datos que lo respalden para el público objetivo (dueños de
  almacén/kiosco, locales con luz brillante). Puede ofrecerse como opción una vez resuelto lo demás,
  nunca como default.

## Cómo se mide el éxito

No es "se ve como una app de Silicon Valley" — es "se ve prolijo y profesional, es más rápido de usar
que antes, y no le costó semanas de desarrollo a un producto que recién está empezando a facturar".

## Fase 4 — Catálogo compartido de productos (2026-08-11, en curso)

No es UI/UX estrictamente, pero ataca el mismo problema de fondo: la primera carga de productos de un
cliente nuevo es lenta y tediosa. En vez de tipear cada nombre a mano, cuando se escanea/carga un
código de barras que ya cargó otro cliente en cualquier otra instalación de Vendemax Desktop, el nombre
se sugiere solo.

- **Qué se comparte:** únicamente nombre + código de barras. Nunca costo, precio, stock ni proveedor —
  eso es información comercial de cada comercio, no del producto en sí.
- **Sin pantalla de consentimiento** — funciona automático para todos los clientes (decisión explícita
  del usuario, con el trade-off entendido: no hay opt-out).
- **No es un volcado masivo en la instalación nueva** — se consulta por código de barras bajo demanda,
  en el momento en que hace falta (mismo patrón que usan apps de escaneo de código de barras tipo Open
  Food Facts), para no ensuciar el catálogo de un negocio con productos de rubros ajenos.
- **Costo de infraestructura: cero adicional** — la tabla nueva (`SharedProduct`) vive en el mismo
  Postgres que ops-dashboard ya usa para las licencias, no es un servicio nuevo. Se descartó guardarlo
  en Excel: igual necesitaría un servidor que lo sirva (no elimina la dependencia de nube) y con muchas
  instalaciones escribiendo al mismo tiempo se puede corromper — una tabla con upsert resuelve la
  escritura concurrente gratis.
- Detalle técnico completo en el plan `melodic-purring-kettle.md`, sección "Catálogo compartido de
  productos".
