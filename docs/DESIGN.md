# DESIGN — testNet

> Sistema de diseño y directrices de experiencia de usuario (UX) del panel CRUD de testNet.
> **Alcance**: exclusivamente la capa de presentación frontend servida desde `src/testNet.API/wwwroot/` (`index.html`, `css/app.css`, `js/app.js`, `js/api.js`).
>
> **NO** aplica a la API REST, modelos de dominio, DTOs, validaciones, persistencia ni tests — esos están gobernados por `docs/SPEC.md` y `docs/ADR.md`.

---

## 1. Propósito

Definir la **fuente única de verdad** para decisiones visuales y de interacción del SPA: paleta de colores, tipografía, espaciados, componentes reutilizables, microinteracciones, patrones de feedback, accesibilidad y responsive. Cualquier cambio en el frontend debe mantener consistencia con este documento.

---

## 2. Stack del frontend

| Capa            | Tecnología                            | Motivo                                                          |
|-----------------|---------------------------------------|-----------------------------------------------------------------|
| Estilos utility | **Tailwind CSS v3 (CDN)**             | Velocidad de iteración sin build step                           |
| Estilos propios | **CSS plano** en `css/app.css`        | El CDN de Tailwind no procesa `@apply` → se usan clases planas |
| Tipografía      | **Inter** + **JetBrains Mono** (Google Fonts) | Sans-serif moderna + monoespaciada para importes/códigos |
| Lógica UI       | **JavaScript vanilla** (sin framework)| Mantiene el principio "cero dependencias innecesarias" del proyecto |
| Iconografía     | **Lucide-style inline SVG**           | Trazos consistentes, sin requests extra, fácil tematizar        |

> **Regla de oro**: no añadir frameworks UI (React, Vue, Alpine, etc.) ni librerías de componentes. Si se necesita un componente nuevo, se construye siguiendo los patrones aquí definidos.

---

## 3. Identidad visual

### 3.1 Paleta de marca (Brand)

Escala `brand` declarada en `tailwind.config` de `index.html` (líneas 15–19). Equivale a la familia **Indigo** de Tailwind:

| Token      | Hex       | Uso recomendado                                     |
|------------|-----------|-----------------------------------------------------|
| `brand-50` | `#eef2ff` | Fondos hover, halos de focus suaves                  |
| `brand-100`| `#e0e7ff` | Fondos sutiles, badges informativos                  |
| `brand-200`| `#c7d2fe` | Bordes en estados activos                            |
| `brand-300`| `#a5b4fc` | Acentos secundarios                                  |
| `brand-400`| `#818cf8` | Bordes en focus de inputs                            |
| `brand-500`| `#6366f1` | Anillos (focus ring), íconos destacados              |
| `brand-600`| `#4f46e5` | **Primario**: botones primarios, enlaces activos     |
| `brand-700`| `#4338ca` | Hover de primarios, textos sobre fondos claros       |
| `brand-800`| `#3730a3` | Texto de marca sobre fondos oscuros                  |
| `brand-900`| `#312e81` | Texto de alto contraste                              |

**Uso crítico**: el color de marca aparece **solo** en acciones primarias (Guardar, Nuevo, foco de inputs, íconos destacados). Nunca se usa como fondo de página ni como color de estado semántico (success / warning / error).

### 3.2 Escala neutra (slate)

| Token      | Hex       | Uso                                |
|------------|-----------|------------------------------------|
| `slate-50` | `#f8fafc` | Fondo de filas en hover            |
| `slate-100`| `#f1f5f9` | Fondo de cards, botones ghost, scrollbar thumb |
| `slate-200`| `#e2e8f0` | Bordes principales                 |
| `slate-300`| `#cbd5e1` | Bordes de inputs                   |
| `slate-400`| `#94a3b8` | Hints, texto deshabilitado         |
| `slate-500`| `#64748b` | Labels secundarios, texto de ayuda |
| `slate-600`| `#475569` | Texto cuerpo, íconos en reposo     |
| `slate-700`| `#334155` | Labels de formulario               |
| `slate-800`| `#1e293b` | Títulos                            |
| `slate-900`| `#0f172a` | Texto principal                    |

### 3.3 Semántica (estado)

| Color    | Hex       | Significado              | Uso                                          |
|----------|-----------|--------------------------|----------------------------------------------|
| Verde    | `#10b981` / `#059669` | Éxito / activo   | Badge "Activo", botón "Agregar" stock, toast success |
| Ámbar    | `#d97706` / `#b45309` | Advertencia      | Stock bajo, botón "Remover" stock            |
| Rojo     | `#e11d48` / `#be123c` | Error / peligro  | Botón eliminar, asterisco de requerido, toast error, modal de confirmación |
| Indigo   | `#6366f1`             | Informativo     | Toast info, focus ring                        |

**Reglas semánticas**:
- Verde = acción positiva (agregar, confirmar, activo)
- Ámbar = operación reversible o atención (remover stock, advertencia)
- Rojo = acción destructiva o error (eliminar, fallo de validación)
- Nunca usar rojo para destacar (solo para errores y eliminaciones)

### 3.4 Fondos

| Capa           | Color         | Uso                                      |
|----------------|---------------|------------------------------------------|
| `body`         | `slate-100`   | Lienzo de la app                          |
| `card`         | `white`       | Tarjetas, tablas, modales, header         |
| `header`       | `white/90` + `backdrop-blur` | Sticky translúcido sobre la tabla |
| `tabla-zebra`  | `slate-50`    | Header de tabla                          |
| `fila-hover`   | `slate-50`    | Hover de filas                           |
| `modal-backdrop` | `slate-900/55` + `backdrop-blur(2px)` | Overlay de modales              |

---

## 4. Tipografía

### 4.1 Familias

| Familia          | Uso                              | Pesos cargados   |
|------------------|----------------------------------|------------------|
| **Inter**        | Toda la UI                       | 400, 500, 600, 700, 800 |
| **JetBrains Mono**| Código de producto, importes numéricos | 400, 500    |

Declaradas en `tailwind.config` (`fontFamily.sans`, `fontFamily.mono`).

### 4.2 Escala tipográfica

| Estilo            | Tailwind      | Tamaño | Peso | Caso de uso                                   |
|-------------------|---------------|--------|------|-----------------------------------------------|
| H1 marca          | `text-base font-bold`     | 1rem     | 700  | Logo "testNet"                                |
| Subtítulo marca   | `text-xs`                 | 0.75rem  | 400  | "Gestión de Productos" debajo del logo        |
| H2 modal          | `text-lg font-bold`       | 1.125rem | 700  | Títulos de modales                            |
| H3 stat           | `text-2xl font-bold`      | 1.5rem   | 700  | Métricas del dashboard                        |
| Etiqueta stat     | `text-xs uppercase tracking-wide` | 0.75rem | 500 | Labels "Total", "Activos", etc.       |
| Label form        | `text-xs font-semibold`   | 0.8rem   | 600  | Etiquetas de campos (`form-label`)            |
| Input             | `text-sm`                 | 0.875rem | 400  | Texto en inputs/selects                       |
| Texto cuerpo      | `text-sm text-slate-600/700` | 0.875rem | 400 | Descripciones, hints, párrafos                |
| Texto pequeño     | `text-xs`                 | 0.75rem  | 400  | Hints, footer, badges                         |
| Código de producto | `font-mono`             | igual al contenedor | - | Columna "Código" de la tabla        |
| Moneda            | `font-mono` (vía `fmtMoney`) | igual al contenedor | - | Columna "Precio"                  |

### 4.3 Reglas de aplicación

- **Antialising**: `font-sans antialiased` en `<body>` para todos los textos.
- **No usar** más de **3 niveles de peso** por pantalla.
- **Mayúsculas + tracking**: solo para labels de stat (`uppercase tracking-wide`). No abusar.
- **Code/monospace**: aplicar a `code`, `productCode`, importes numéricos y `id` técnico.

---

## 5. Espaciado, radios y sombras

### 5.1 Radios (`border-radius`)

| Token     | Valor      | Uso                                                    |
|-----------|------------|--------------------------------------------------------|
| `rounded-lg`   | 0.5rem | Inputs, botones, badges pequeños, acciones inline      |
| `rounded-xl`   | 0.75rem | Contenedores de stat, logos, íconos de toast           |
| `rounded-2xl`  | 1rem    | Cards (sección stats, toolbar, tabla, modales)         |
| `rounded-full` | 9999px  | Badges, scrollbar thumb, avatares                      |

### 5.2 Sombras

| Token        | Valor                                    | Uso                              |
|--------------|------------------------------------------|----------------------------------|
| `shadow-sm`  | `0 1px 2px rgba(0,0,0,.06)`             | Botones primarios, cards de stat |
| `shadow`     | (Tailwind)                               | Dropdowns                        |
| `shadow-2xl` | `0 20px 50px -10px rgba(0,0,0,.3)`       | Modal panel                      |
| `shadow-toast` | `0 10px 25px -5px rgba(0,0,0,.15)`     | Toasts                           |

### 5.3 Espaciado

Se sigue la escala de Tailwind (`0.25rem` = unidad base). Reglas pragmáticas:

- **Padding interno de cards**: `p-4` (16 px) en mobile/desktop, `p-5` (20 px) en headers de modal.
- **Gap entre cards/stats**: `gap-3 sm:gap-4` (12–16 px).
- **Padding lateral del contenedor**: `max-w-7xl mx-auto px-4 sm:px-6 lg:px-8`.
- **Separación vertical de secciones**: `mb-4` a `mb-6`.

---

## 6. Iconografía

- **Set**: SVG inline estilo **Lucide** (stroke-width 2, `stroke-linecap="round"`, `stroke-linejoin="round"`, `fill="none"`).
- **Tamaños estándar**:
  - `w-4 h-4` (16 px) → dentro de botones y headers
  - `w-5 h-5` (20 px) → iconos destacados (logo, modal de confirmación)
  - `w-6 h-6` (24 px) → empty state, spinners
- **Color**: hereda de `currentColor`. No se colorea el SVG directamente.
- **Inventario de iconos usado**: cube (producto), check (activo), tag (inventario), dollar (valor), refresh, plus, search, info, file (redoc), refresh-cw (reset db), alert-triangle (peligro).

> **Regla**: para añadir un ícono nuevo, mantener el mismo set (Lucide-style) y la misma escala. No introducir otro set iconográfico.

---

## 7. Componentes

### 7.1 Botones (`.btn-base` + modificadores)

| Variante       | Color de fondo        | Texto  | Uso                                       |
|----------------|------------------------|--------|-------------------------------------------|
| `.btn-primary` | `brand-600` (hover `brand-700`) | blanco | Acción principal del contexto (Guardar, Nuevo) |
| `.btn-ghost`   | `slate-100` (hover `slate-200`) | `slate-600` | Cancelar, acciones secundarias |
| `.btn-danger`  | `#e11d48` (hover `#be123c`) | blanco | Eliminar                                  |
| `.btn-emerald` | `#059669` (hover `#047857`) | blanco | Agregar stock                             |
| `.btn-amber`   | `#d97706` (hover `#b45309`) | blanco | Remover stock                             |

**Estructura** (clase compartida `.btn-base`):
```css
display: inline-flex; align-items: center; justify-content: center;
gap: .375rem; padding: .5rem 1rem; font-size: .875rem; font-weight: 600;
border-radius: .5rem; border: 1px solid transparent; cursor: pointer;
transition: 150ms en bg/color/border/shadow/opacity;
```

**Reglas**:
- Toda acción destructiva usa `.btn-danger`.
- Toda confirmación positiva usa `.btn-primary` o `.btn-emerald`.
- `disabled` baja opacidad a `.6` y deshabilita cursor.
- El ícono precede al label, separados por `gap-1.5`.

### 7.2 Inputs y formularios

| Elemento       | Clases                                                                 |
|----------------|------------------------------------------------------------------------|
| Label          | `.form-label` (`text-xs font-semibold text-slate-700 mb-1.5`)          |
| Input / Select / Textarea | `.form-input` (fondo blanco, borde `slate-300`, radius `.5rem`) |
| Hint           | `.form-hint` (`text-xs text-slate-400 mt-1`)                           |
| Requerido      | Asterisco `<span class="text-rose-500">*</span>` adyacente al label    |
| Focus          | Borde `brand-400` + ring `0 0 0 3px rgba(99,102,241,.15)`              |
| Disabled       | Fondo `slate-100`, texto `slate-400`, cursor `not-allowed`             |

**Layout** del modal de producto: grid `grid-cols-1 sm:grid-cols-2 gap-4`. Campos de 2 columnas a `sm:` en adelante, full width en mobile.

### 7.3 Tabs de vista (`.view-tab`)

- Contenedor: `bg-slate-100 rounded-xl p-1` con `role="tablist"`.
- Tab inactivo: `text-slate-600`.
- Tab activo: `bg-white text-brand-600 shadow-sm`.
- **No usar** underlines ni borders animados; el cambio de fondo basta.

### 7.4 Tabla

| Zona       | Estilo                                                         |
|------------|----------------------------------------------------------------|
| Container  | `.bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden` |
| Header     | `bg-slate-50`, texto `text-xs uppercase tracking-wide text-slate-500 font-semibold` |
| Filas      | `divide-y divide-slate-100`, hover `bg-slate-50`               |
| Acciones   | Botones `.row-action` (cuadrados de 2 rem, hover cambia color por tipo) |

**Acciones inline**:
- Editar → hover `text-brand-600`
- Stock → hover `text-amber-600`
- Eliminar → hover `text-rose-600`

### 7.5 Badges (`.badge`)

| Variante         | Fondo     | Texto     | Uso                       |
|------------------|-----------|-----------|---------------------------|
| `.badge-active`  | `#d1fae5` | `#065f46` | Producto activo           |
| `.badge-inactive`| `#f1f5f9` | `#475569` | Producto inactivo         |
| `.badge-low`     | `#fef3c7` | `#92400e` | Stock bajo (≤ umbral)     |
| `.badge-out`     | `#fee2e2` | `#991b1b` | Sin stock (0)             |

Estructura: `inline-flex items-center gap-1 px-2 py-0.5 text-xs font-semibold rounded-full`.

### 7.6 Modales

- **Contenedor**: `position: fixed; inset: 0; z-index: 40; display: flex; align-items: center; justify-content: center; padding: 1rem;`
- **Backdrop**: `rgba(15, 23, 42, .55)` + `backdrop-filter: blur(2px)`. Animación `fade-in 150ms`.
- **Panel**: `bg-white rounded-2xl shadow-2xl`, animación `pop-in 180ms cubic-bezier(.16,1,.3,1)` (translateY 8px + scale .98 → 1).
- **Tamaños**: default `max-w-2xl`; `.sm:max-w-md` (28rem) para modales compactos (stock, confirmación).
- **Cierre**: backdrop con `data-close`, botón `.modal-close` (×), tecla `Escape` (gestionado en `app.js`).
- **Header**: `px-5 py-4 border-b border-slate-200`, título `text-lg font-bold text-slate-900`, botón cerrar a la derecha.
- **Footer**: `border-t border-slate-100`, alineado a la derecha con `gap-2`.

### 7.7 Toasts

- Posición: `fixed top-4 right-4 z-50`, ancho máx 320 px.
- Estructura: `.toast` con borde lateral 4 px según tipo.
- Animación entrada: `slide-in 250ms cubic-bezier(.16,1,.3,1)` (translateX 1rem → 0).
- Animación salida: `slide-out 200ms` (translateX 1rem + fade).
- **Duración por defecto**: 3.8 s. Click en el toast lo cierra antes.
- **Tipos**: `success` (verde), `error` (rojo), `info` (indigo).

### 7.8 Paginación

- Botón: `.page-btn` (`min-w-8 h-8 px-2 text-sm font-semibold text-slate-600 bg-white border border-slate-200 rounded-lg`).
- Hover: `text-brand-600 border-brand-200 bg-brand-50`.
- Disabled: `opacity .4 cursor-not-allowed`.
- Indicador de página: `text-sm font-semibold text-slate-700`.

---

## 8. Estados de UI obligatorios

Todo componente que cargue datos debe contemplar y mostrar **explícitamente** los 4 estados:

| Estado      | Tratamiento                                                  |
|-------------|--------------------------------------------------------------|
| **Loading** | `#loadingState` con spinner animado (`animate-spin w-6 h-6 text-brand-500`) y texto "Cargando productos…". Activado por `state.loading`. |
| **Empty**   | `#emptyState` con ícono en `slate-100`, título "No hay productos para mostrar", hint contextual. |
| **Error**   | Toast tipo `error` con el mensaje del `ApiError`. No se renderiza UI adicional; la tabla queda vacía. |
| **Success** | Toast tipo `success` con mensaje breve ("Producto creado", "Stock actualizado", etc.). |

**Regla**: nunca dejar la tabla en blanco "silenciosa" sin saber por qué. Loading/empty/error siempre visibles.

---

## 9. Patrones de UX

### 9.1 Búsqueda
- **Debounce** de 250 ms sobre `input` (constante en `app.js`).
- Filtra **en cliente** sobre `state.allCache` (modo "Todos" y "Activos"). En modo "Paginado" no aplica (se filtra en backend si se requiere).
- Búsqueda case-insensitive sobre `code` y `name` (contains).
- Placeholder: "Buscar por código o nombre…".

### 9.2 Confirmación de acciones destructivas
- Toda acción destructiva (eliminar producto) usa el **modal `#confirmModal`** con:
  - Ícono triangular ámbar
  - Nombre del producto a eliminar
  - Texto explícito: "Esta acción no se puede deshacer."
  - Cancelar (`.btn-ghost`) / Eliminar (`.btn-danger`)
- No se usa `window.confirm` (inconsistente con el resto del diseño).

### 9.3 Validación de formularios
- **Prevención del submit** sin completar campos requeridos: se hace desde `app.js` con `toast` de tipo `error`.
- Asterisco rojo en labels indica campos requeridos.
- `maxlength` HTML5 como primera barrera (`fCode`: 50, `fName`: 200, `fDescription`: 2000).
- `min`/`step` numéricos en `price` y `stockQuantity`.

### 9.4 Acciones de stock
- Modal compartido (`#stockModal`) con dos acciones explícitas:
  - **Agregar** → `.btn-emerald` (verde, positivo)
  - **Remover** → `.btn-amber` (ámbar, reversible pero requiere atención)
- Se muestra nombre del producto y stock actual antes de operar.
- `quantity` mínimo 1, `step` 1.

### 9.5 Seed / Reset DB
- Botón en header: "Reset DB" con ícono refresh-cw.
- Color `slate-800` (no destructive, pero sí operación fuerte).
- Llama a `POST /api/seed` y refresca la vista. Toast de éxito con el count.

### 9.6 Feedback inmediato
- **Optimista**: no se usa. Toda mutación espera la respuesta del servidor antes de mostrar feedback. Esto evita inconsistencias con la fuente de verdad.
- **Refrescar**: tras `POST`, `PUT`, `DELETE`, `PATCH` → se vuelve a llamar al endpoint correspondiente y se re-renderiza la tabla.

### 9.7 Foco y teclado
- El primer input del modal recibe foco al abrir.
- `Escape` cierra el modal activo.
- Tab order respetado (sin `tabindex` positivos).
- Botones de acción principales son `<button type="submit">` o `type="button"` según contexto.

---

## 10. Responsive

| Breakpoint  | Ancho mínimo | Cambios clave                                            |
|-------------|--------------|----------------------------------------------------------|
| Base        | 0            | Layout 1 columna, tabs full-width, modales full-width    |
| `sm` (640)  | 640 px       | Header muestra "Swagger/Redoc" labels, modal de producto en 2 columnas |
| `lg` (1024) | 1024 px      | Stats en 4 columnas, toolbar en fila                     |

- **Mobile-first**: todas las clases se aplican de base a `sm:` y `lg:`.
- **Tabla**: con `overflow-x-auto` en el contenedor; las columnas se mantienen en mobile con scroll horizontal (no se ocultan columnas).
- **Modales**: padding de 1 rem, panel full-width con `max-w-*` aplicado por variante.

---

## 11. Accesibilidad (a11y)

| Criterio            | Implementación                                                       |
|---------------------|----------------------------------------------------------------------|
| Contraste           | Texto principal `slate-900` sobre blanco (≥ 15:1). Slate-500/600 sobre blanco cumple AA. |
| Foco visible        | Ring de 3 px `rgba(99,102,241,.15)` + borde `brand-400` en inputs.   |
| Roles ARIA          | `role="tablist"` en tabs de vista, `aria-label` en navegación.       |
| Labels asociados    | Todo input tiene `<label for="...">`.                                |
| `prefers-reduced-motion` | Desactiva animaciones a 0.01 ms (media query en `app.css` líneas 172-174). |
| Cierre con teclado  | Botones accesibles; `Escape` cierra modales.                         |
| Texto alternativo   | Los SVG decorativos no tienen `aria-hidden` explícito, pero los íconos con texto asociado se duplican en label. |
| Idioma              | `<html lang="es">`.                                                  |
| Navegación          | Header sticky con skip implícito por orden DOM (header → main).       |

> **Pendiente conocido**: añadir `aria-hidden="true"` a SVGs decorativos y `role="dialog"` + `aria-modal="true"` a los modales.

---

## 12. Rendimiento percibido

- **CDN de Tailwind**: aceptable para una demo SPA; en producción se compilaría.
- **Sin reflows**: `state.loading` se libera **antes** de `renderTable()` para evitar el guard de re-entrada.
- **Debounce** de 250 ms en búsqueda.
- **Cache de stats**: `state.allCache` evita llamadas repetidas a `/api/products` cuando se cambia de vista.
- **`requestAnimationFrame` no usado**: las operaciones de render son síncronas y rápidas sobre <100 filas.

---

## 13. Anti-patrones (NO hacer)

| Anti-patrón                                     | Por qué evitarlo                                      |
|--------------------------------------------------|-------------------------------------------------------|
| Usar `window.confirm` / `window.alert`           | Inconsistente con el sistema de toasts/modales.       |
| Colores hex fuera de la paleta documentada       | Rompe la consistencia visual.                         |
| Botón primario verde o rojo                      | El verde es para "éxito/activo", el rojo para "peligro". El primario es siempre indigo/brand. |
| Tostada persistente sin auto-dismiss             | Los toasts siempre expiran (3.8 s) o se cierran al click. |
| Modal anidado dentro de otro modal               | Solo existe un nivel de modal a la vez.               |
| Animación que supere los 250 ms                  | Toda animación crítica es ≤ 250 ms (ver `app.css`).   |
| Tailwind en producción sin build                 | Aceptable solo porque el proyecto es demo/educativo.  |
| Insertar HTML sin escapar (`escapeHtml`)         | Riesgo XSS en campos de usuario (nombre, descripción). |
| Usar `innerHTML` para contenido estático confiable | Aceptable solo cuando el contenido es 100% controlado por el código (no por el usuario). |
| Sumar una dependencia JS solo para un componente | Construir el componente con vanilla siguiendo los patrones. |

---

## 14. Checklist para añadir un componente nuevo

Antes de mergear cualquier componente UI nuevo, verificar:

- [ ] ¿Usa solo colores de la paleta documentada? (sección 3)
- [ ] ¿Respeta la escala tipográfica? (sección 4)
- [ ] ¿Radio y sombra dentro de los tokens? (sección 5)
- [ ] ¿Íconos Lucide-style consistentes? (sección 6)
- [ ] ¿Implementa los 4 estados (loading / empty / error / success)? (sección 8)
- [ ] ¿Es responsive (mobile-first)? (sección 10)
- [ ] ¿Tiene foco visible y es navegable por teclado? (sección 11)
- [ ] ¿Escape de HTML en cualquier input de usuario? (sección 13)
- [ ] ¿No introduce dependencias nuevas? (sección 2)
- [ ] ¿Anima en ≤ 250 ms? (sección 13)

---

## 15. Referencia rápida de archivos

| Archivo                                        | Responsabilidad                                |
|------------------------------------------------|------------------------------------------------|
| `src/testNet.API/wwwroot/index.html`           | Estructura DOM, configuración de Tailwind, fuentes |
| `src/testNet.API/wwwroot/css/app.css`          | Componentes CSS planos, animaciones, a11y       |
| `src/testNet.API/wwwroot/js/api.js`            | Cliente HTTP (fetch wrapper + `ApiError`)       |
| `src/testNet.API/wwwroot/js/app.js`            | Estado, render, modales, toasts, debounce, escape de HTML, formato de moneda (`Intl`) |

---

**Mantenido por**: contributors del proyecto. Cualquier desviación de estas reglas debe justificarse y documentarse en este archivo.
