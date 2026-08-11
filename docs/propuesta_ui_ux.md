# Propuesta de Rediseño UI/UX y Sistema de Diseño (Vendemax Desktop)

## 1. Filosofía de Diseño
El nuevo diseño de **Vendemax Desktop** se basa en tres pilares fundamentales:
1. **Velocidad y Cero Fracción (Zero Friction):** El cajero o comerciante debe poder realizar cualquier operación crítica en menos de 3 segundos y preferiblemente sin despegar las manos del teclado numérico.
2. **Estética Profesional y Moderna:** Inspirado en las interfaces comerciales de nivel enterprise (Dark Mode, colores armónicos HSL, bordes redondeados suavemente, tipografía clara).
3. **Claridad Operativa:** Reducción de la carga cognitiva mediante jerarquía visual estricta, distinción clara de estados (éxito, advertencia, error) e indicadores financieros prominentes.

---

## 2. Sistema de Diseño (Design System)

### A. Paleta de Colores (Theme Dark & Light)
- **Fondo Principal (Background):** `#0F172A` (Slate 900) - Proporciona contraste cómodo y reduce el cansancio visual.
- **Superficie de Tarjetas (Surface Card):** `#1E293B` (Slate 800) - Eleva visualmente los paneles interactivos.
- **Bordes y Separadores:** `#334155` (Slate 700)
- **Color Primario (Action / Accent):** `#3B82F6` (Blue 500)
- **Éxito (Venta Cobrada / AFIP OK):** `#10B981` (Emerald 500)
- **Advertencia (Stock Bajo / Pendiente):** `#F59E0B` (Amber 500)
- **Peligro (Cancelado / Error CAE):** `#EF4444` (Red 500)
- **Texto Principal:** `#F8FAFC` (Slate 50)
- **Texto Secundario:** `#94A3B8` (Slate 400)

### B. Tipografía
- **Fuente Principal:** *Inter* / *Segoe UI Variable*
- **Jerarquía:**
  - Display Totales (Importe en Caja / Cobros): 32px Bold
  - Encabezados de Sección: 20px SemiBold
  - Etiquetas y Botones: 14px Medium
  - Tablas de Productos / DataGrid: 13px Regular (Altamente legible)

---

## 3. Rediseño de Pantallas Clave

### 1. Pantalla Principal de Ventas (POS Screen)
- **Panel Izquierdo (Ticket Activo):** 
  - Lista de ítems escaneados con modificación directa de cantidad (+/- o tipeo).
  - Resumen visual claro: Subtotal, Descuento, IVA/Impuestos y **TOTAL en letra grande**.
- **Panel Derecho (Modo Híbrido):**
  - Tab 1: *Buscador Rápido por Código/Nombre* con sugerencias instantáneas.
  - Tab 2: *Grid de Productos Frecuentes / Botonera Táctil* con tarjetas con foto y color distintivo por categoría.
- **Barra Inferior de Atajos (Dock Atajos):**
  - Muestra constante de las teclas activas (`F1 Nueva Venta`, `F2 Buscar`, `F3 Cobrar`, `F4 Fiado/Cliente`, `F5 Facturar AFIP`).

### 2. Dashboard de Inicio y Control Ejecutivo
- **Widgets Dinámicos:**
  - Venta total del día vs día anterior.
  - Métodos de pago desglosados (Efectivo, Mercado Pago QR, Tarjeta, Cuenta Corriente).
  - Alerta de productos más vendidos y stock a reponer.

---

## 4. Micro-Interacciones y Feedback Visual
- **Efecto de Confirmación de Pago:** Animación sutil de check verde al completar una venta con sonido opcional configurable.
- **Teclado Visual en Cobro:** Destacado automático de las teclas sugeridas durante el cálculo de vuelto en efectivo.
