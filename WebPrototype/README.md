# Guía del Prototipo de Baja Fidelidad - STOCKPAP

Este directorio contiene el **Prototipo de Baja Fidelidad Interactivo (Wireframe)** desarrollado para la WebApp **STOCKPAP**, en cumplimiento de la actividad práctica de la Unidad 3: *Diseño para aplicaciones Web* del *Taller de Ingeniería de Software*.

---

## 🚀 Cómo Ejecutar e Interactuar con el Prototipo

El prototipo se ha estructurado como una **Single Page Application (SPA)** autocontenida con tecnologías puras: **HTML5, Vanilla CSS3 y JavaScript**. No requiere de servidores web complejos ni instalación de dependencias.

1. Navega a la carpeta [WebPrototype](file:///c:/Users/wendy/source/repos/wendyroxanariverahernandez-hash/STOCKPAP/WebPrototype).
2. Haz doble clic en el archivo `index.html` para abrirlo en cualquier navegador web moderno (Chrome, Edge, Firefox, Safari).
3. **Inicio de sesión (Login):** La pantalla inicial requiere autenticación. Puedes ingresar con las siguientes credenciales preestablecidas para probar la simulación de roles de negocio:
   - **Administrador (Acceso total):** Usuario: `administrador` | Contraseña: `wendy123`
   - **Cajero (Acceso restringido):** Usuario: `cajero` | Contraseña: `cajero123`
4. Interactúa con las pestañas del menú lateral izquierdo para alternar entre pantallas de forma instantánea.

---

## 🛠️ Entregable 1: Cómo publicar el enlace colaborativo en GitHub Pages (En 2 minutos)

Para cumplir con el entregable *"1. Enlace del prototipo colaborativo"*, pueden alojar este prototipo directamente en **GitHub Pages** de forma gratuita. Sigan estos pasos sencillos:

1. Suban la carpeta del proyecto `STOCKPAP` a su repositorio de GitHub.
2. En GitHub, ingresen a la configuración de su repositorio (**Settings**).
3. Busquen la sección **Pages** en el menú izquierdo (bajo el apartado de *Code and automation*).
4. En **Build and deployment**, seleccionen la rama `main` (u `master`) y elijan la carpeta donde se encuentra el prototipo (por ejemplo, si subieron todo, elijan la ruta de `/WebPrototype`).
5. Hagan clic en **Save**.
6. En unos segundos, GitHub generará una URL pública (ejemplo: `https://nombreusuario.github.io/STOCKPAP/WebPrototype/index.html`). **Ese es el enlace colaborativo que deben entregar**.

---

## 📢 Guión de Exposición en Clase (Presentación Final)

Para la presentación del equipo (**Wendy, Enoc y Guillermina**), aquí tienen estructurados los puntos clave que deben explicar en clase según los requerimientos de la actividad:

### 1. Objetivo de la Aplicación (STOCKPAP)
> *"STOCKPAP es una aplicación web diseñada para el control eficiente del inventario y la agilización de ventas en papelerías de mediana y pequeña escala. Su propósito principal es eliminar la pérdida de stock, alertar de forma preventiva cuando un artículo se está agotando, y proveer un punto de venta rápido y amigable para el cajero, facilitando el cálculo automático de cuentas, IVA y cambio en efectivo, mientras genera registros automáticos de cada entrada y salida."*

### 2. Flujo de Navegación (Representado en la pestaña "Flujo de Navegación")
* **Acceso y Autenticación:** El flujo comienza en el **Login (P1)**. El sistema autentica al usuario y detecta su rol (Admin o Cajero).
* **Consola Central (Dashboard - P2):** Al entrar, se despliega el Dashboard donde se consolidan los KPIs críticos (Total productos, alertas de stock bajo y ventas del día).
* **Operativa Diaria (Punto de Venta - P3-A):** El cajero busca productos por buscador o categorías, los añade al carrito con un clic, define las cantidades y hace clic en "Cobrar" para abrir el formulario de pago en efectivo **(F-A)**.
* **Control y Mantenimiento (Inventario y Proveedores - P3-B y P3-D):** Permite al administrador crear o modificar registros de productos **(F-B)** y registrar proveedores de papelería **(F-D)**.
* **Trazabilidad (Movimientos - P3-C):** Registra auditorías automáticas de salidas de inventario por ventas, y permite ingresos manuales de stock **(F-C)**.
* **Toma de Decisiones (Reportes - P3-E):** Consolida gráficos financieros de la rentabilidad del negocio.

### 3. Organización de Contenido
El contenido está estructurado de manera jerárquica para priorizar la rapidez del cajero:
* **Header Persistente:** Muestra el logotipo de la aplicación, el usuario que está operando en caja (importante para auditorías de arqueo), la fecha del día y un botón de cierre de sesión.
* **Sidebar Navegable:** Un menú lateral que da acceso directo a las herramientas principales. Puede colapsarse (`☰`) para ampliar el espacio de trabajo.
* **Área de Contenido en Bloques:** Utiliza tarjetas de información (cards) y tablas simplificadas de lectura rápida. Las alertas de stock bajo y las cantidades monetarias clave usan fuentes en negrita para llamar la atención del operario.
* **Bandeja Colaborativa de Feedback:** Incluye una sección en el borde inferior derecho para simular la discusión activa del equipo de diseño (Wendy, Enoc, Guillermina), donde comentaron las mejoras UI/UX realizadas durante el desarrollo del prototipo.

### 4. Decisiones de Diseño UI/UX (Baja Fidelidad)
* **Paleta en Escala de Grises:** De acuerdo con las pautas de baja fidelidad, se evitó el uso de colores finales o logotipos detallados. Toda la interfaz usa gris, negro y blanco, permitiendo enfocar la atención únicamente en la distribución espacial de los elementos y los textos descriptivos.
* **Placeholders Estándar de Imagen:** Las fotos de los cuadernos, lápices y plumas se simulan mediante cajas grises cruzadas en diagonal con una "X", representando la futura ubicación del recurso visual.
* **Notas Adhesivas (Sticky Notes):** Se implementaron pequeñas notas amarillas de "anotación" en cada pantalla. Es una práctica profesional en wireframes que le explica al profesor o al programador la lógica de negocio simulada (ejemplo: indicando qué campos son requeridos en la base de datos o cómo reacciona el ticket).
* **Validación de Datos Dinámica:** A pesar de ser un prototipo, el sistema simula lógica real: resta stock de los productos tras concretar una venta, arroja alertas si se ingresa menos efectivo del costo total, y registra movimientos históricos automáticos. Esto simula una navegación interactiva real para asombrar en la presentación.

---

## 🎨 Capturas de Pantallas para el Reporte (Entregable 2)
Para el documento o presentación final, tomen capturas de las siguientes pantallas clave directamente desde su navegador:
1. **Pantalla de Login:** Mostrando el formulario limpio y la nota adhesiva sobre los roles.
2. **Dashboard (Home):** Mostrando los KPI cards grises y el gráfico SVG lineal de ventas semanales.
3. **Punto de Venta (Ventas):** Llenando el ticket de venta con algunos cuadernos y plumas y haciendo clic en "Cobrar" para capturar el modal de pago activo.
4. **Inventario:** Mostrando las tarjetas de productos con sus placeholders de imágenes cruzadas y badges de stock bajo.
5. **Mapa de Navegación:** Capturando el diagrama de nodos y flechas interactivo que describe los flujos del sistema.
