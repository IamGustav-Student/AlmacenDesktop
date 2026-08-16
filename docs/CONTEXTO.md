# Contexto del proyecto — Vendemax Desktop

> Documento de referencia rápida para retomar trabajo en este repo sin tener que releer todo el código. Se actualiza a mano cuando cambia algo estructural (no en cada commit).

## Qué es

**Vendemax Desktop** es un punto de venta (POS) de escritorio para comercios locales de Argentina: ventas, stock, compras a proveedores, caja, cuenta corriente/fiados, facturación electrónica AFIP, cobro con QR de Mercado Pago, código de barras y etiquetas, impresión térmica de tickets.

Es **single-tenant**: cada cliente instala su propio `.exe` con su propia base de datos local (SQLite). No hay backend propio para el POS en sí — corre 100% en la PC del comercio. Lo único que sale a internet es: validación de licencia, catálogo compartido de productos, chequeo de actualizaciones y AFIP.

El repo en GitHub todavía se llama `AlmacenDesktop` (`IamGustav-Student/AlmacenDesktop`) y el namespace C#/csproj/sln también — es el nombre original, antes del rebrand a "Vendemax Desktop". No se renombró (toca casi todos los archivos); si se pide un rebrand completo, avisar antes de encararlo.

## Por qué existe (contexto de negocio)

Este proyecto pasó a ser el foco principal por sobre **VendemaxWeb** (SaaS multi-tenant en la nube, repo separado) a partir de 2026-08-10: vendido por licencia, sin costo de infraestructura corriendo 24/7 — genera ingresos con menor gasto operativo que un SaaS. VendemaxWeb quedó relegado, no abandonado.

## Stack

- **.NET 8, WinForms** (`net8.0-windows`)
- **Entity Framework Core + SQLite** — base de datos local, un archivo `almacen.db` junto al `.exe`, 41 migraciones aplicadas automáticamente al arrancar (`context.Database.Migrate()`)
- Dependencias relevantes: `ClosedXML` (import/export Excel), `ZXing.Net` (códigos de barra/QR), `ESCPOS_NET` (impresión térmica), `System.Security.Cryptography.Pkcs`/`ProtectedData` (certificados AFIP, DPAPI), `Obfuscar` (ofuscación del build Release)
- Publicación: self-contained single-file (`dotnet publish -r win-x64 --self-contained true -p:PublishSingleFile=true`) — no requiere .NET instalado en la PC del cliente

## Estructura

```
Forms/       47 formularios WinForms (pantallas) — la mayor parte de la lógica de UI vive acá
Services/    Lógica de negocio y clientes HTTP: AFIP, licencias, ticket/impresión, catálogo compartido, backup, ventas, Excel, código de barras
Modelos/     Entidades de EF Core (Producto, Venta, Cliente, Caja, Compra, Pago, Usuario, ConfiguracionAfip, etc.)
Data/        AlmacenDbContext (único DbContext)
Helpers/     Utilidades estáticas — ver convenciones abajo
Migrations/  Migraciones EF Core (41 al momento de escribir esto)
docs/        Este archivo + investigaciones de UX/UI (históricas, no todas ejecutadas)
server/      Servidor Node standalone de licencias — DEPRECADO, ya no se deploya (ver abajo)
```

`Program.cs` es el entry point: configura DI (`ConfigurarServicios`), corre migraciones + seed mínimo (`InicializarBaseDeDatos` — ya no siembra productos falsos, solo un usuario admin, un cliente "Consumidor Final" y un proveedor genérico), dispara en background la sincronización del catálogo compartido, valida la licencia (activación si no hay, revalidación online si la hay), y finalmente abre `MenuPrincipal`.

## Sistemas que hablan con el ecosistema ProgramadorGS

Todo lo que sale a internet apunta a **`ops-dashboard`** (`https://www.programadorgs.com.ar/ops`), no a un backend propio de este repo:

- **Licencias**: `Services/LicenseService.cs` valida contra `ops-dashboard/licencias/validar` (payload + firma HMAC-SHA256 verificada localmente antes de confiar). El servidor Node viejo (`server/`) queda en el repo como referencia histórica pero no se deploya más — la lógica se migró completa a ops-dashboard, con `DesktopLicense` como modelo y alta automática vía webhook desde `subscription-hub` (checkout real con Mercado Pago).
- **Catálogo compartido de productos** (`Services/CatalogoCompartidoService.cs`): al cargar un producto nuevo por código de barras, si otra instalación ya cargó ese mismo código, el nombre se sugiere solo. Solo viaja nombre + código de barras — nunca costo/precio/stock/proveedor. Sync incremental en cada arranque de la app (`GET /catalogo/todos?desde=`), guardando el checkpoint en `catalogo_ultimo_sync.txt` junto al `.exe` (no en la DB).
- **Actualizador automático** (`Services/UpdateService.cs`): consulta `Constantes.GITHUB_RELEASES_API` (releases del repo `IamGustav-Student/AlmacenDesktop`), descarga el nuevo `.exe` y se reemplaza solo vía un script `.bat` que espera a que el proceso cierre y relanza la app — sin tocar `almacen.db`.
- **Venta del producto**: `subscription-hub` tiene el producto real "Vendemax Desktop" (slug `vendemax-desktop`, tipo `licencia`, checkout simplificado sin usuario/contraseña).

Estos tres sistemas viven en repos hermanos dentro del mismo workspace (`ops-dashboard`, `subscription-hub`) — no hace falta ir a buscarlos a otro lado si hay que tocar el lado servidor de alguno de estos flujos.

## Convenciones establecidas (importante respetar al tocar código nuevo)

- **Manejo de excepciones**: nunca mostrar `ex.Message` directo en un `MessageBox`. `DbUpdateException` y varias otras excepciones de .NET traen el mensaje útil en `InnerException` (a veces anidado), no en el mensaje de primer nivel. Usar siempre `Helpers/ExceptionHelper.ObtenerMensaje(ex)`, que camina hasta el `InnerException` más profundo.
- **AutoScaleMode**: todos los formularios deben fijar `this.AutoScaleMode = AutoScaleMode.None` explícitamente en `InitializeComponent()`. Sin esto, el escalado de texto/DPI de Windows corre las posiciones absolutas en pixels y corta o desalinea controles — bug real encontrado y corregido en 10 formularios distintos (v1.0.5–v1.0.7). Layout fijo, no responsive: las pantallas están pensadas para una resolución de diseño fija, no para adaptarse fluidamente a cualquier tamaño de ventana.
- **Versión + release van juntos**: cualquier cambio de comportamiento que afecte instalaciones nuevas amerita bump de `<Version>` en `AlmacenDesktop.csproj` + release nueva en GitHub en el mismo momento — no se deja el código adelantado al último release publicado, porque el auto-updater y `Product.downloadUrl` del hub dependen de que "la última release" sea realmente lo último.
- **Nombre del asset de release**: siempre `VendemaxDesktop-v{version}-win-x64.exe` (no el nombre por default de `dotnet publish`, que sería `AlmacenDesktop.exe`).
- **Notas de release**: cortas, en criollo, sin jerga técnica (nada de nombres de archivos internos, "errorlevel", etc.) — se muestran directo al usuario final en el diálogo de actualización.
- **Búsquedas en SQLite vía EF Core**: `string.Contains()` traduce a `instr()`, que es sensible a mayúsculas (a diferencia de `LIKE`). Para búsquedas case-insensitive hay que normalizar con `.ToLower()` en ambos lados de la comparación.
- **`Product.downloadUrl` NO se toca más** (desde 2026-08-16). Quedó fijo en `https://www.programadorgs.com.ar/descargar/vendemax-desktop`, una ruta del gateway que resuelve la última release contra la API de GitHub y redirige. Si volvés a poner ahí una URL de github.com, el link se vuelve a clavar en una versión: antes de este cambio estuvo ofreciendo la v1.0.7 cuando ya iba la v1.2.0.

## Cómo compilar y publicar una release

```bash
dotnet build -c Release                     # build normal, revisa 0 errores
dotnet publish -c Release -r win-x64 --self-contained true \
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true \
  -o publish/vX.X.X                          # genera el .exe (~180-190MB)
# renombrar a VendemaxDesktop-vX.X.X-win-x64.exe antes de subir
gh release create vX.X.X "publish/vX.X.X/VendemaxDesktop-vX.X.X-win-x64.exe" \
  --title "Vendemax Desktop vX.X.X" --notes "..."
```

El token de GitHub usado para releases está en `.github-token.env` (gitignoreado, no versionado). Después de publicar **no hay que actualizar ninguna base**: alcanza con confirmar que `https://www.programadorgs.com.ar/descargar/vendemax-desktop` ya redirige a la versión nueva (cachea 5 minutos).

## Historial de versiones (resumen)

| Versión | Cambio principal |
|---|---|
| v1.0.0 | Primera release pública |
| v1.0.1 | Búsqueda en vivo + actualizador automático + ícono |
| v1.0.2 | Catálogo compartido de productos (sugerencia al escanear) |
| v1.0.3 | Se sacó la siembra de ~110-136 productos falsos de ejemplo |
| v1.0.4 | Notas de actualización ilegibles (Markdown sin convertir) + script de reemplazo que podía colgarse |
| v1.0.5 | Botones del diálogo de actualización cortados — primer caso de bug de `AutoScaleMode` |
| v1.0.6 | Configuración se rompía si el Spooler de Windows estaba caído + búsqueda de productos sensible a mayúsculas |
| v1.0.7 | Barrida completa: mensajes de error genéricos → `ExceptionHelper` en toda la app; `AutoScaleMode` en los 9 formularios que faltaban; solapamiento real de título/botón en Clientes |
| v1.1.0 | Productos sueltos por peso / unidad fraccionable (`UnidadMedida`, cantidades decimales) + backup automático verifica integridad y loguea fallos |
| v1.1.1 | Menú que expone las 7 pantallas ocultas + pantalla de inicio con evolución de 12 meses + fix de gráficos rotos y de ORDER BY decimal |
| v1.1.2 | Descuento de stock incorrecto en productos fraccionables: import de Excel leía el stock con `int.TryParse` (fallaba silencioso con decimales) + `VentasForm` validaba contra una lista de stock que no se refrescaba entre ventas |
| v1.1.3 | `Pago.Observaciones` sin default rompía el cobro de cuenta corriente (columna NOT NULL) + rename "Fiado" → "Cuenta Corriente" en toda la UI |
| v1.1.4 | Escape/Limpiar en Clientes y Proveedores no dejaba la pantalla en blanco (rebind del grid autoseleccionaba la primera fila) |
| v1.1.5 | Restaurar un backup desde Configuración, sin tocar archivos a mano — reusa el patrón de reemplazo-y-relanzado del auto-actualizador |

## Roadmap en curso — evolución como software de almacén (desde 2026-08-12)

Plan completo (con justificación técnica y análisis de riesgos) en el archivo de plan de la sesión: `C:\Users\iamgu\.claude\plans\melodic-purring-kettle.md`, sección "Evolución de Vendemax Desktop como software para almacenes". Este resumen es para poder retomar desde otra PC.

### Hallazgo que originó el plan

**7 módulos completos y funcionales no se podían abrir desde ningún lado.** Verificado con grep (`new XForm(` → 0 referencias): `ComprasForm` (circuito de reposición a proveedores), `DashboardForm` (KPIs + gráficos), `HistorialVentasForm`, `HistorialCajasForm`, `ProveedoresForm`, `ReporteFiadosForm`, `EtiquetasForm`. El menú tenía 8 botones y ninguno los enlazaba. Exponerlos es lo más barato y lo que más funcionalidad agrega.

### Estado de las fases

| Fase | Versión | Qué hace | Estado |
|---|---|---|---|
| 1 | v1.1.1 | Menú data-driven que expone los 7 módulos ocultos | ✅ hecho |
| 3 | v1.1.1 | Pantalla de inicio con evolución mes a mes (12 meses) | ✅ hecho |
| — | v1.1.1 | Fix: gráficos rotos (SqlClient) + ORDER BY decimal en SQLite | ✅ hecho |
| 2 | — | `Helpers/Theme.cs` — paleta y tipografía centralizadas | ⏳ pendiente |
| 4 | — | `Forms/BaseForm.cs` + atajos de teclado unificados | ⏳ pendiente |
| 5 | — | Roll-out del tema por tráfico (Ventas → Productos → Clientes → resto) | ⏳ pendiente |
| 6 | — | Balanza: lectura de código de barras con peso embebido | ⏳ pendiente |
| 7 | — | Precio mayorista / por cantidad | ⏳ pendiente |

> Las versiones v1.1.2 a v1.1.5 **no** corresponden a las fases de esta tabla — se usaron para una tanda de bugs reales encontrados al usar la app después de v1.1.0/v1.1.1 (ver sección siguiente). Las fases 2/4/5 de este roadmap siguen sin numerar hasta que se retomen.

### Fase 1 — qué se hizo exactamente (v1.1.1)

- `Forms/MenuPrincipal.Designer.cs` pasó a ser **solo estructura**: un `panelMenu` (Dock.Left, 250px, navy) con un único hijo `flowMenu` (`FlowLayoutPanel`, Dock.Fill, AutoScroll, TopDown, `WrapContents=false`). Ya no declara un `Button` por pantalla.
- `Forms/MenuPrincipal.cs` arma el menú en runtime desde una lista de `ItemMenu { Grupo, Texto, SoloAdmin, Crear, Destacado }`. **Agregar una pantalla nueva = una línea en `ObtenerItems()`**, sin tocar el Designer ni escribir un handler.
- 15 ítems en 5 grupos: Operación diaria (4), Clientes (2), Inventario (5), Reportes (2), Administración (2) + Salir.
- Los ítems de admin se **ocultan** en vez de mostrarse en gris (para un usuario no técnico, botones muertos confunden). Si un grupo se queda sin ítems visibles, su encabezado tampoco se dibuja. `ValidarAccesoAdmin()` sigue como segunda barrera al hacer click.
- `DashboardForm.InicializarGraficos()`: antes fijaba `Size` = `MinimumSize` = `MaximumSize` = 1000x780, lo que impedía que entrara en notebooks de 1366x768. Ahora `MinimumSize = 900x600` + `AutoScroll = true`.

**Gotcha del FlowLayoutPanel con AutoScroll:** el ancho de cada ítem se calcula como `flowMenu.ClientSize.Width - 22 - SystemInformation.VerticalScrollBarWidth`. Ese último descuento va **siempre**, aunque la barra vertical todavía no se vea: si no se reserva, al aparecer la barra en pantallas bajas reduce el área útil y dispara además una barra horizontal. Verificado con harness: sin reservar → `ScrollH visible: True`; reservando → ambas en False.

**Cómo se verificó (útil para repetirlo):** no se puede correr la app entera sin pasar por el gate de licencia, así que se armó un proyecto WinForms descartable en el scratchpad que referencia `AlmacenDesktop.csproj`, instancia `MenuPrincipal` con un `Usuario` falso (Admin y Vendedor), lo muestra, y vuelca (a) un PNG con `DrawToBitmap` y (b) un reporte de texto con la posición/alto de cada ítem y el estado de los scrollbars. Confirmó los 15 ítems para Admin, 11 para Vendedor, y que todo entra sin scroll. Ojo: `AlmacenDesktop.Program` es `internal`, así que desde afuera no se puede setear `ConnectionStringGlobal` — el `AlmacenDbContext` cae a su fallback de `almacen.db` junto al exe.

### Fase 3 — pantalla de inicio con evolución mes a mes (v1.1.1)

El panel derecho del menú (antes vacío en gris) ahora muestra un gráfico de **ventas y ganancia estimada de los últimos 12 meses**. Decisión del usuario: va en el panel del menú (no dentro del Resumen del Negocio) y la métrica elegida fue específicamente la evolución mes a mes.

- `MenuPrincipal.Designer.cs` suma `panelContenido` (Dock.Fill) con `lblTituloHome` + `lblBienvenida` (Dock.Top) y `panelGrafico` (Dock.Fill, tarjeta blanca).
- **Orden de docking (importante):** el control `Fill` se agrega **primero** a `Controls` y los bordes después. Es el criterio que ya usaba `ProductosForm.Designer.cs:284-285` (`panelMain` Fill primero, `panelEditor` Left después) — se siguió ese ejemplo en vez de adivinar.
- `ObtenerVentasPorMes()` arma los 12 buckets **incluyendo los meses sin ventas**; si se omitieran, el gráfico saltearía huecos y mentiría sobre la evolución.
- La ganancia es **estimada**: usa el costo *actual* del producto, no el costo al momento de la venta (el esquema no guarda snapshot de costo en `DetalleVenta`). Mismo criterio que ya usaba el Resumen del Negocio, así que ambas pantallas dan el mismo número.
- La guía de inicio y el gráfico son **excluyentes**: si hay ≤1 usuario (instalación nueva, sin historial que graficar) se muestra la guía dentro de la misma tarjeta; si no, se carga el gráfico. Evita además que la carga async le pise el panel.

### Dos bugs reales encontrados al exponer las pantallas ocultas (corregidos en v1.0.8)

Ambos estaban **latentes justamente porque `DashboardForm` era inalcanzable** — nunca se habían ejecutado.

**1. Cualquier `Chart` reventaba la app.** `System.Windows.Forms.DataVisualization` (el port de los Chart de .NET Framework) referencia `System.Data.SqlClient` para el data-binding de sus series, pero ese assembly **no se copiaba a la salida del build**. Resultado: al pintarse cualquier gráfico saltaba `Could not load file or assembly 'System.Data.SqlClient'` como **excepción no controlada** (fuera de todo try/catch, porque ocurre durante el pintado, no en la construcción). Fix: `<PackageReference Include="System.Data.SqlClient" Version="4.8.6" />` en `AlmacenDesktop.csproj`. **No se usa SQL Server en ningún lado** — está solo para satisfacer esa dependencia. Si algún día se saca esa referencia, todos los gráficos vuelven a romperse.

**2. SQLite no puede ordenar por `decimal` — regresión de v1.1.0.** Cuando `Stock` y `Cantidad` pasaron de `int` a `decimal` (soporte de productos por peso), dos consultas de `DashboardForm` quedaron rotas: `.OrderBy(p => p.Stock)` y `.OrderByDescending(x => x.Unidades)` (suma de `Cantidad`). Error: *"SQLite does not support expressions of type 'decimal' in ORDER BY clauses"*. Como el form atrapa la excepción y la muestra en un `MessageBox` modal, desde afuera solo se veía un cuelgue. Fix: `.AsEnumerable()` antes del `OrderBy` para ordenar en memoria (son pocas filas).

> **Regla para el futuro:** en este proyecto, cualquier `OrderBy`/`OrderByDescending` sobre `Stock`, `StockMinimo`, `Cantidad`, `Precio`, `Costo` o `Subtotal` **debe ir después de un `.AsEnumerable()`**. `ReporteGananciasForm` ya lo hacía bien (trae a memoria antes de agrupar) y por eso nunca falló.

### Decisiones tomadas (no re-litigar)

- **Exponer los módulos ocultos primero**, antes de construir features nuevas.
- **Balanza vía código de barras con peso embebido** (EAN-13 prefijo 2x, el estándar de Kretz/Systel), NO conexión serie/USB directa — esta última necesita el protocolo puntual de cada modelo y hardware físico para probar.
- **Vencimientos/lotes queda FUERA** — el usuario tiene un proyecto propio en desarrollo, se integra después.
- **NO se cambia el modelo de ventanas modales** (`Hide()/ShowDialog()/Show()` en `MenuPrincipal.AbrirFormulario`). Se ofreció y se descartó por riesgoso. Es la limitación de UX más grande que queda (no se puede consultar un precio mientras se cobra).
- **Light mode** para el tema. Razón: mostrador con luz fuerte, uso todo el día, dueños no técnicos. El navy queda como *chrome* (sidebar), la menta solo en pantallas pre-login.
- **Promociones complejas (2x1, combos multi-producto) siguen fuera** — detrás del gate de validación con clientes reales de `roadmap_realista_uiux_v1.md`.

### Corrección importante sobre AutoScaleMode (leer antes de tocar layout)

El repo tiene **dos criterios de escalado conviviendo, y eso está bien**:

- **16 formularios** (incluidos `VentasForm` y `MenuPrincipal`) usan `AutoScaleDimensions = (7F, 15F)` + `AutoScaleMode.Font`. Es la combinación **canónica y correcta** de WinForms. **NO tocarlos** — migrarlos a `None` sería churn con riesgo de regresión.
- **10 formularios** usan `AutoScaleMode.None` (los 9 corregidos en v1.0.7 + `ActualizacionForm`). Esos no tenían **ninguna** de las dos propiedades configuradas, que sí es la situación riesgosa.

**Regla:** nunca asignar `AutoScaleMode` en `OnLoad`/`OnShown` — asignarlo después del layout dispara `PerformAutoScale()` y reintroduce el bug de botones cortados de v1.0.5. Va siempre en `InitializeComponent()` o en el constructor.

### Otro dato útil de la exploración

- No existe ninguna infraestructura de UI compartida: cero clase base, cero clase de tema, cero UserControl. Conviven 3 paletas, 52 `Color.FromArgb` hardcodeados, 130+ colores con nombre y 14 tamaños de fuente.
- "Space Grotesk" y "JetBrains Mono" (usadas en `ActivationForm`/`LockForm`) **no vienen instaladas en Windows** — esas pantallas ya renderizan con el fallback de GDI+, el branding tipográfico nunca funcionó.
- `ProductosForm` tiene `ClientSize = 1084x611` y se corta en notebooks de 1366x768. Su `panelEditor` solo tiene ~40px libres entre el último campo y los botones, así que sumar campos ahí requiere `AutoScroll = true`.

## Tanda de bugs post-v1.1.0/v1.1.1 (v1.1.2–v1.1.5)

Todos surgieron de usar de verdad las funcionalidades que trajeron v1.1.0 (productos fraccionables) y v1.1.1 (pantallas antes inalcanzables) — mismo patrón que los dos bugs de Dashboard: código que nunca se había ejercitado.

**v1.1.2 — descuento de stock roto en productos fraccionables:**
- `Services/ExcelService.cs` leía la columna Stock con `int.TryParse`, que devuelve `false` en silencio ante un valor decimal (`"12,5"`) y deja el stock en 0 sin avisar. Fix: usar el mismo `LeerDecimal` que ya usaban Precio/Costo.
- `Forms/VentasForm.cs` cargaba la lista de productos (con su `Stock`) una sola vez en `VentasForm_Load` y nunca la refrescaba. Vender el mismo producto fraccionable dos veces en la misma sesión validaba "stock suficiente" contra un número desactualizado.

**v1.1.3 — cobro de cuenta corriente rompía + rename Fiado → Cuenta Corriente:**
`Modelos/Pago.cs` tiene `Observaciones` sin valor por defecto, y ni `CuentaCorrienteForm` ni `ReporteFiadosForm` lo completaban al registrar un pago — pero la columna en SQLite es `NOT NULL`. Cualquier intento real de cobrar tiraba `SQLite Error 19: NOT NULL constraint failed: Pagos.Observaciones`, un bug de "nunca se probó con datos reales" (los reportes se veían bien, pero nadie había cobrado nada). Fix: `Observaciones = ""` por defecto en el modelo. De paso se renombró "Fiado" a "Cuenta Corriente" en toda la UI (menú, VentasForm, mensajes) con una migración de datos para no perder el `MetodoPago` ya guardado en ventas viejas.

**v1.1.4 — Escape/Limpiar dejaba Clientes y Proveedores con el registro anterior pegado:**
`Limpiar()` limpiaba los campos y **después** llamaba a `CargarDatos()`, que rebindea el `DataSource` del grid. WinForms autoselecciona la primera fila al rebindear un `DataGridView`, lo que dispara `SelectionChanged` de nuevo y repuebla los campos con ese registro — el usuario apretaba Escape para cargar uno nuevo y la pantalla nunca quedaba realmente en blanco. Fix: desconectar el handler de `SelectionChanged` mientras se recarga/limpia la selección, limpiar los campos al final. **Gotcha general de WinForms para tener en cuenta en cualquier grid con selección + un botón "Nuevo/Limpiar":** rebind de `DataSource` ⇒ autoselección de fila 0 ⇒ reentrada del handler de selección, si no se desconecta a tiempo.

**v1.1.5 — restaurar un backup (feature nueva, no bug):**
Hasta acá la app solo podía *crear* backups; cargar uno de vuelta era 100% manual (cerrar la app, buscar el `.db`, renombrarlo a mano) — inviable para el usuario final sin conocimientos técnicos que pidió esto. Se agregó un botón "Restaurar un Backup" en Configuración: elige el archivo con un diálogo normal, se guarda una copia de la base actual por las dudas, y la app se cierra y reabre sola ya restaurada. El reemplazo real corre en un script auxiliar que **reutiliza el mismo patrón ya probado del auto-actualizador** (`Services/UpdateService.cs`): espera a que el proceso cierre y reintenta el reemplazo con un límite acotado de reintentos (no infinito), garantizando que la app siempre vuelva a abrirse al final aunque el archivo tarde en soltarse. Verificado con un harness aislado que simula un PID real y un lock de archivo real, sin correr la GUI completa — mismo método ya usado para verificar el fix del actualizador en v1.0.4.

## Licencia demo multi-dispositivo (ops-dashboard)

Para mostrar el sistema a prospectos sin pisar la licencia de un cliente real: `DesktopLicense` en `ops-dashboard` tiene un campo `esDemo` (default `false`). Si `esDemo = true`, `licencias/validar/route.ts` **no** rechaza por `machineFingerprint` distinto — se puede activar en varias PCs a la vez. Las licencias normales de clientes pagos siguen atadas a un solo equipo, sin cambios. Se marca/desmarca desde el panel admin (`/licencias-desktop/:id`, botón "Marcar como demo"). Pusheado a `master` de `ops-dashboard` — **falta correr la migración de Prisma** (`prisma migrate dev` o `db push`) contra la base real si todavía no se hizo, si no el panel y `licencias/validar` van a tirar error por la columna faltante.

## Ecosistema — repos relacionados

- `ops-dashboard` — licencias, catálogo compartido, health checks, panel admin del ecosistema
- `subscription-hub` — venta/checkout/facturación de todos los productos SaaS/licencia de ProgramadorGS
- `VendemaxWeb` — el SaaS multi-tenant hermano, relegado en prioridad pero no abandonado
