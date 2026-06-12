/* ==========================================================================
   LÓGICA DINÁMICA DE WIREFRAME - STOCKPAP
   Simulación interactiva de base de datos local y navegación.
   ========================================================================== */

// ── BASE DE DATOS MOCK DE INICIO (Datos tomados de database_setup.sql) ──
let db = {
    usuarios: [
        { id: 1, username: 'administrador', password: 'wendy123', rol: 'Admin' },
        { id: 2, username: 'cajero', password: 'cajero123', rol: 'Cajero' }
    ],
    productos: [],
    proveedores: [
        { id: 1, empresa: 'Distribuidora Escolar del Centro', contacto: 'Juan Pérez', telefono: '555-1234-5678', email: 'ventas@descolar.com', direccion: 'Av. Juárez 123, CDMX' },
        { id: 2, empresa: 'Papelería y Más SA de CV', contacto: 'María García', telefono: '555-8765-4321', email: 'contacto@papeleriaymas.com', direccion: 'Calle Insurgentes 456, CDMX' }
    ],
    movimientos: [
        { id: 1, fecha: '2026-05-27 08:10:00', tipo: 'Entrada', producto: 'Cuaderno Profesional 100 hojas', cantidad: 50, stockAnterior: 100, stockNuevo: 150, motivo: 'Compra a proveedor' },
        { id: 2, fecha: '2026-05-27 08:15:00', tipo: 'Entrada', producto: 'Pluma BIC Cristal Azul', cantidad: 30, stockAnterior: 50, stockNuevo: 80, motivo: 'Reabastecimiento' },
        { id: 3, fecha: '2026-05-27 08:20:00', tipo: 'Salida', producto: 'Marcadores de Colores', cantidad: -7, stockAnterior: 15, stockNuevo: 8, motivo: 'Venta a cliente (Ticket #1023)' },
        { id: 4, fecha: '2026-05-27 08:30:00', tipo: 'Salida', producto: 'Resma Papel Bond A4', cantidad: -5, stockAnterior: 50, stockNuevo: 45, motivo: 'Pedido mayoreo' }
    ],
    ventasHistory: [
        { id: 1020, fecha: '2026-05-26 14:22:00', subtotal: 100.00, iva: 16.00, total: 116.00 },
        { id: 1021, fecha: '2026-05-26 16:45:00', subtotal: 215.00, iva: 34.40, total: 249.40 },
        { id: 1022, fecha: '2026-05-27 08:05:00', subtotal: 80.00,  iva: 12.80, total: 92.80 },
        { id: 1023, fecha: '2026-05-27 08:20:00', subtotal: 455.00, iva: 72.80, total: 527.80 } // Detalle: 7 Marcadores ($455)
    ]
};

// ESTADOS GLOBALES DE LA SESIÓN DE LA APP
let currentUser = null;
let currentCart = []; // { productoId, cantidad, precioVenta }
let categoryActiveInventory = "Todas";

// ── LOGICA DE RUTAS Y NAVEGACIÓN SPA ──
document.addEventListener("DOMContentLoaded", () => {
    initApp();
});

function initApp() {
    // Vincular Eventos de Login
    document.getElementById("btn-login-submit").addEventListener("click", handleLogin);
    document.getElementById("btn-logout").addEventListener("click", handleLogout);
    
    // Vincular Barra Lateral (Menú)
    const navItems = document.querySelectorAll(".sidebar-nav .nav-item");
    navItems.forEach(item => {
        item.addEventListener("click", (e) => {
            e.preventDefault();
            const targetSec = item.getAttribute("data-target");
            switchSection(targetSec);
        });
    });

    // Toggle Sidebar
    document.getElementById("btn-sidebar-toggle").addEventListener("click", toggleSidebar);

    // Búsqueda y Filtros en Punto de Venta
    document.getElementById("sales-search-input").addEventListener("input", renderSalesCatalog);
    document.getElementById("sales-category-select").addEventListener("change", renderSalesCatalog);
    document.getElementById("btn-clear-cart").addEventListener("click", clearCart);
    document.getElementById("btn-checkout-trigger").addEventListener("click", openCheckoutModal);

    // Formulario de Cobro (Checkout)
    document.getElementById("chk-amount-paid").addEventListener("input", calculateChange);
    document.getElementById("btn-confirm-checkout").addEventListener("click", confirmCheckout);

    // Búsqueda y Filtros en Inventario
    document.getElementById("inventory-search").addEventListener("input", renderInventoryList);
    document.getElementById("btn-add-product-modal").addEventListener("click", () => openProductModal(null));
    document.getElementById("btn-save-product").addEventListener("click", saveProduct);

    // Registrar Proveedor
    document.getElementById("btn-add-provider-modal").addEventListener("click", openProviderModal);
    document.getElementById("btn-save-provider").addEventListener("click", saveProvider);

    // Registrar Movimiento Manual
    document.getElementById("btn-add-movement-modal").addEventListener("click", openMovementModal);
    document.getElementById("btn-save-movement").addEventListener("click", saveMovement);
    document.getElementById("movement-filter-type").addEventListener("change", renderMovementsList);

    // Comentarios del Equipo
    document.getElementById("btn-collab-toggle").addEventListener("click", toggleCollabTray);
    document.getElementById("btn-add-comment").addEventListener("click", addNewComment);

    // Cargar Relación en dropdown de movimientos
    populateMovementProductDropdown();

    // Actualizar fecha del sistema en UI
    updateDateDisplay();
}

function updateDateDisplay() {
    const d = new Date();
    const formatted = `${String(d.getDate()).padStart(2, '0')}/${String(d.getMonth() + 1).padStart(2, '0')}/${d.getFullYear()}`;
    document.getElementById("current-date-info").textContent = `Fecha: [${formatted}]`;
}

// Alternar colapsado de barra lateral
function toggleSidebar() {
    const sidebar = document.getElementById("app-sidebar");
    sidebar.classList.toggle("collapsed");
}

// Cambio de pantalla principal a secciones SPA
function switchSection(sectionId) {
    if (sectionId === 'login-screen-forced') {
        handleLogout();
        return;
    }

    // Remover active de menú
    const navItems = document.querySelectorAll(".sidebar-nav .nav-item");
    navItems.forEach(item => {
        item.classList.remove("active");
        if (item.getAttribute("data-target") === sectionId) {
            item.classList.add("active");
        }
    });

    // Ocultar todas las secciones
    const sections = document.querySelectorAll(".content-section");
    sections.forEach(s => s.classList.add("hidden"));

    // Mostrar sección solicitada
    const target = document.getElementById(sectionId);
    if (target) {
        target.classList.remove("hidden");
    }

    // Inicializar cargas de datos de la sección
    if (sectionId === "view-dashboard") {
        renderDashboard();
    } else if (sectionId === "view-inventario") {
        renderInventoryPills();
        renderInventoryList();
    } else if (sectionId === "view-ventas") {
        renderSalesCatalog();
        renderCartItems();
    } else if (sectionId === "view-movimientos") {
        renderMovementsList();
    } else if (sectionId === "view-proveedores") {
        renderProvidersList();
    } else if (sectionId === "view-reportes") {
        renderReportesView();
    }
}

// ── CONTROL DE ACCESO (LOGIN) ──
function handleLogin() {
    const userVal = document.getElementById("login-username").value.trim();
    const passVal = document.getElementById("login-password").value.trim();
    const errorMsg = document.getElementById("login-error-msg");

    // Verificar en la db mock
    const found = db.usuarios.find(u => u.username === userVal && u.password === passVal);

    if (found) {
        currentUser = found;
        errorMsg.textContent = "";
        
        // Configurar UI
        document.getElementById("current-user-info").textContent = `Rol: [${currentUser.Rol}] | Usuario: [${currentUser.username}]`;
        document.getElementById("login-screen").classList.add("hidden");
        document.getElementById("main-screen").classList.remove("hidden");

        // Ocultar botones de CRUD si es Cajero (Seguridad de Negocio)
        toggleRolesUI();

        // Ir a dashboard
        switchSection("view-dashboard");
    } else {
        errorMsg.textContent = "Usuario o contraseña incorrectos.";
    }
}

function handleLogout() {
    currentUser = null;
    currentCart = [];
    document.getElementById("login-screen").classList.remove("hidden");
    document.getElementById("main-screen").classList.add("hidden");
    document.getElementById("login-password").value = "";
}

function toggleRolesUI() {
    const isAdmin = currentUser.Rol === 'Admin';
    
    // Ocultar botón agregar producto en inventario
    const btnAddProd = document.getElementById("btn-add-product-modal");
    if (btnAddProd) {
        btnAddProd.style.display = isAdmin ? "inline-flex" : "none";
    }

    // Ocultar botones agregar proveedor
    const btnAddProv = document.getElementById("btn-add-provider-modal");
    if (btnAddProv) {
        btnAddProv.style.display = isAdmin ? "inline-flex" : "none";
    }

    // Ocultar registrar movimiento manual
    const btnAddMov = document.getElementById("btn-add-movement-modal");
    if (btnAddMov) {
        btnAddMov.style.display = isAdmin ? "inline-flex" : "none";
    }
}


// ── SECCIÓN 1: DASHBOARD / HOME ──
function renderDashboard() {
    // KPIs
    document.getElementById("dashboard-total-products").textContent = db.productos.length;
    
    let lowStockCount = 0;
    db.productos.forEach(p => {
        if (p.stock <= p.stockMinimo) lowStockCount++;
    });
    document.getElementById("dashboard-low-stock").textContent = lowStockCount;

    // Calcular ventas acumuladas hoy
    let totalSalesVal = 0;
    db.ventasHistory.forEach(v => {
        // En una simulación consideramos las ventas
        totalSalesVal += parseFloat(v.total);
    });
    document.getElementById("dashboard-today-sales").textContent = `$${totalSalesVal.toFixed(2)}`;
    document.getElementById("dashboard-total-providers").textContent = db.proveedores.length;

    // Actividad reciente (últimos 4 movimientos)
    const container = document.getElementById("dashboard-activity-list");
    container.innerHTML = "";

    const lastMovs = db.movimientos.slice().reverse().slice(0, 4);
    
    lastMovs.forEach(mov => {
        const li = document.createElement("li");
        li.className = "activity-item";
        
        const badgeClass = mov.tipo.toLowerCase() === 'entrada' ? 'entrada' : 'salida';
        const qtySign = mov.cantidad > 0 ? `+${mov.cantidad}` : `${mov.cantidad}`;
        
        li.innerHTML = `
            <div>
                <span class="act-badge ${badgeClass}">${mov.tipo}</span>
                <strong>${mov.producto}</strong> (${qtySign} u)
                <div style="font-size: 10px; color: #718096; margin-top: 2px;">${mov.motivo}</div>
            </div>
            <span class="act-time">${mov.fecha.split(" ")[1]}</span>
        `;
        container.appendChild(li);
    });
}

function filterInventarioBajoStock() {
    categoryActiveInventory = "Bajo Stock";
    switchSection("view-inventario");
}


// ── SECCIÓN 2: PUNTO DE VENTA (VENTAS) ──
function renderSalesCatalog() {
    const grid = document.getElementById("sales-catalog-grid");
    grid.innerHTML = "";

    const searchVal = document.getElementById("sales-search-input").value.toLowerCase();
    const catVal = document.getElementById("sales-category-select").value;

    let filtered = db.productos;

    // Filtros
    if (catVal !== "Todas") {
        filtered = filtered.filter(p => p.categoria === catVal);
    }
    if (searchVal) {
        filtered = filtered.filter(p => p.nombre.toLowerCase().includes(searchVal) || String(p.id) === searchVal);
    }

    if (filtered.length === 0) {
        grid.innerHTML = `<div style="grid-column: 1/-1; text-align: center; color: #888; padding: 20px;">Sin coincidencias</div>`;
        return;
    }

    filtered.forEach(p => {
        const isLow = p.stock <= p.stockMinimo;
        const card = document.createElement("div");
        card.className = `sale-product-card ${isLow ? 'low-stock-card' : ''}`;
        
        const firstLetter = p.nombre.charAt(0).toUpperCase();

        card.innerHTML = `
            <div>
                <div class="sale-card-img-placeholder">
                    <div class="placeholder-cross"></div>
                    <span style="font-size: 24px; font-weight: 700; z-index: 1;">${firstLetter}</span>
                </div>
                <h4 class="sale-card-name" title="${p.nombre}">${p.nombre}</h4>
                <div class="sale-card-price">$${p.precioVenta.toFixed(2)}</div>
                <div class="sale-card-stock ${isLow ? 'danger' : ''}">
                    ${isLow ? `⚠ Stock: ${p.stock}` : `Stock: ${p.stock}`}
                </div>
            </div>
            <button class="wireframe-btn btn-primary btn-sm btn-full-width" onclick="addToCart(${p.id})">Agregar</button>
        `;
        grid.appendChild(card);
    });
}

function addToCart(productId) {
    const prod = db.productos.find(p => p.id === productId);
    if (!prod) return;

    if (prod.stock <= 0) {
        alert(`No hay inventario disponible para: ${prod.nombre}`);
        return;
    }

    const cartItem = currentCart.find(item => item.productoId === productId);
    if (cartItem) {
        if (cartItem.cantidad + 1 > prod.stock) {
            alert(`No puedes vender más de las existencias actuales (${prod.stock} u.)`);
            return;
        }
        cartItem.cantidad += 1;
    } else {
        currentCart.push({
            productoId: productId,
            cantidad: 1,
            precioVenta: prod.precioVenta
        });
    }

    renderCartItems();
}

function renderCartItems() {
    const tbody = document.getElementById("sales-ticket-items");
    tbody.innerHTML = "";

    if (currentCart.length === 0) {
        tbody.innerHTML = `
            <tr class="empty-row-msg">
                <td colspan="5" style="text-align: center; color: #888; padding: 30px 10px;">
                    El ticket está vacío. Agrega productos desde el catálogo rápido.
                </td>
            </tr>
        `;
        updateCartTotals();
        return;
    }

    currentCart.forEach((item, index) => {
        const prod = db.productos.find(p => p.id === item.productoId);
        if (!prod) return;

        const tr = document.createElement("tr");
        const subtotal = item.cantidad * item.precioVenta;

        tr.innerHTML = `
            <td><strong>${prod.nombre}</strong></td>
            <td>
                <input type="number" class="ticket-qty-input" min="1" max="${prod.stock}" value="${item.cantidad}" onchange="updateCartItemQty(${item.productoId}, this.value)">
            </td>
            <td>$${item.precioVenta.toFixed(2)}</td>
            <td><strong>$${subtotal.toFixed(2)}</strong></td>
            <td>
                <button class="btn-remove-item" onclick="removeFromCart(${index})">&times;</button>
            </td>
        `;
        tbody.appendChild(tr);
    });

    updateCartTotals();
}

function updateCartItemQty(productId, qtyVal) {
    const qty = parseInt(qtyVal);
    const prod = db.productos.find(p => p.id === productId);
    const item = currentCart.find(c => c.productoId === productId);
    
    if (!item || !prod) return;

    if (isNaN(qty) || qty <= 0) {
        item.cantidad = 1;
    } else if (qty > prod.stock) {
        alert(`Solo existen ${prod.stock} unidades de este producto.`);
        item.cantidad = prod.stock;
    } else {
        item.cantidad = qty;
    }
    renderCartItems();
}

function removeFromCart(index) {
    currentCart.splice(index, 1);
    renderCartItems();
}

function clearCart() {
    currentCart = [];
    renderCartItems();
}

function updateCartTotals() {
    let subtotal = 0;
    currentCart.forEach(item => {
        subtotal += item.cantidad * item.precioVenta;
    });

    const iva = subtotal * 0.16;
    const total = subtotal + iva;

    document.getElementById("ticket-subtotal").textContent = `$${subtotal.toFixed(2)}`;
    document.getElementById("ticket-iva").textContent = `$${iva.toFixed(2)}`;
    document.getElementById("ticket-total").textContent = `$${total.toFixed(2)}`;
}

// Checkout Form Modal
function openCheckoutModal() {
    if (currentCart.length === 0) {
        alert("El ticket de venta está vacío.");
        return;
    }

    let subtotal = 0;
    currentCart.forEach(item => {
        subtotal += item.cantidad * item.precioVenta;
    });
    const total = subtotal * 1.16;

    document.getElementById("checkout-total-lbl").textContent = `$${total.toFixed(2)}`;
    document.getElementById("chk-amount-paid").value = Math.ceil(total);
    document.getElementById("checkout-error-msg").textContent = "";
    
    calculateChange();
    showModal('modal-checkout');
}

function calculateChange() {
    const totalText = document.getElementById("checkout-total-lbl").textContent.replace("$", "");
    const total = parseFloat(totalText);
    const amountPaid = parseFloat(document.getElementById("chk-amount-paid").value);
    const changeLbl = document.getElementById("checkout-change-lbl");

    if (isNaN(amountPaid) || amountPaid < total) {
        changeLbl.textContent = "$0.00";
    } else {
        const change = amountPaid - total;
        changeLbl.textContent = `$${change.toFixed(2)}`;
    }
}

function confirmCheckout() {
    const totalText = document.getElementById("checkout-total-lbl").textContent.replace("$", "");
    const total = parseFloat(totalText);
    const amountPaid = parseFloat(document.getElementById("chk-amount-paid").value);
    const errorMsg = document.getElementById("checkout-error-msg");

    if (isNaN(amountPaid) || amountPaid < total) {
        errorMsg.textContent = "El efectivo recibido es menor al total a pagar.";
        return;
    }

    // 1. Descontar Inventario en la DB mock
    // 2. Registrar movimientos automáticos de salida
    const d = new Date();
    const dateStr = d.getFullYear() + "-" + 
                    String(d.getMonth() + 1).padStart(2, '0') + "-" + 
                    String(d.getDate()).padStart(2, '0') + " " + 
                    String(d.getHours()).padStart(2, '0') + ":" + 
                    String(d.getMinutes()).padStart(2, '0') + ":" + 
                    String(d.getSeconds()).padStart(2, '0');

    const nextVentaId = db.ventasHistory.length > 0 ? db.ventasHistory[db.ventasHistory.length - 1].id + 1 : 1000;

    currentCart.forEach(item => {
        const prod = db.productos.find(p => p.id === item.productoId);
        if (prod) {
            const antStock = prod.stock;
            prod.stock -= item.cantidad; // Restar stock
            
            // Loguear movimiento
            db.movimientos.push({
                id: db.movimientos.length + 1,
                fecha: dateStr,
                tipo: 'Salida',
                producto: prod.nombre,
                cantidad: -item.cantidad,
                stockAnterior: antStock,
                stockNuevo: prod.stock,
                motivo: `Venta a cliente (Ticket #${nextVentaId})`
            });
        }
    });

    // 3. Registrar Venta Histórica
    const subtotalVal = total / 1.16;
    const ivaVal = total - subtotalVal;
    
    db.ventasHistory.push({
        id: nextVentaId,
        fecha: dateStr,
        subtotal: subtotalVal,
        iva: ivaVal,
        total: total
    });

    // Mensaje de Éxito
    alert(`¡Venta procesada con éxito!\n\nTicket: #${nextVentaId}\nTotal: $${total.toFixed(2)}\nEfectivo: $${amountPaid.toFixed(2)}\nCambio: $${(amountPaid - total).toFixed(2)}`);

    // Reset y Cargas
    closeModal('modal-checkout');
    clearCart();
    switchSection('view-dashboard');
}


// ── SECCIÓN 3: INVENTARIO / CATALOGO (CRUD) ──
function renderInventoryPills() {
    const container = document.getElementById("inventory-category-pills");
    container.innerHTML = "";

    const cats = ["Todas", "Bajo Stock", "Cuadernos", "Escritura", "Papel", "Marcadores", "Organización", "Adhesivos", "Corte"];
    
    cats.forEach(c => {
        const btn = document.createElement("button");
        btn.className = `pill-btn ${c === categoryActiveInventory ? 'active' : ''}`;
        btn.textContent = c;
        btn.addEventListener("click", () => {
            categoryActiveInventory = c;
            // Refrescar pills
            renderInventoryPills();
            renderInventoryList();
        });
        container.appendChild(btn);
    });
}

function renderInventoryList() {
    const grid = document.getElementById("inventory-products-grid");
    grid.innerHTML = "";

    const searchVal = document.getElementById("inventory-search").value.toLowerCase();
    
    let filtered = db.productos;

    // Filtros de categoría y stock bajo
    if (categoryActiveInventory === "Bajo Stock") {
        filtered = filtered.filter(p => p.stock <= p.stockMinimo);
    } else if (categoryActiveInventory !== "Todas") {
        filtered = filtered.filter(p => p.categoria === categoryActiveInventory);
    }

    // Filtro de texto
    if (searchVal) {
        filtered = filtered.filter(p => p.nombre.toLowerCase().includes(searchVal) || String(p.id) === searchVal);
    }

    // Actualizar etiqueta resumen
    let lowStockCount = 0;
    filtered.forEach(p => { if (p.stock <= p.stockMinimo) lowStockCount++; });
    document.getElementById("inventory-status-summary").textContent = `${filtered.length} productos • ${lowStockCount} con stock bajo`;

    if (filtered.length === 0) {
        grid.innerHTML = `<div style="grid-column: 1/-1; text-align: center; color: #888; padding: 40px;">No se encontraron productos en el inventario.</div>`;
        return;
    }

    filtered.forEach(p => {
        const isLow = p.stock <= p.stockMinimo;
        const card = document.createElement("div");
        card.className = "product-card";

        const firstLetter = p.nombre.charAt(0).toUpperCase();
        const badgeHtml = isLow 
            ? `<span class="product-card-stock-badge badge-low">⚠ Stock Bajo (${p.stock})</span>`
            : `<span class="product-card-stock-badge badge-ok">✓ Stock: ${p.stock} u</span>`;

        // Si el usuario es Cajero no puede modificar existencias (oculta acciones de CRUD)
        const showActions = currentUser && currentUser.Rol === 'Admin';
        const actionsHtml = showActions 
            ? `
            <div class="product-card-actions">
                <button class="wireframe-btn btn-outline btn-sm" onclick="openProductModal(${p.id})">✏ Editar</button>
                <button class="wireframe-btn btn-outline btn-sm" style="color: #c53030; border-color: #feb2b2;" onclick="deleteProduct(${p.id})">🗑 Eliminar</button>
            </div>
            `
            : ``;

        card.innerHTML = `
            <div>
                <div class="product-card-img">
                    <div class="placeholder-cross"></div>
                    <span style="font-size: 36px; font-weight: 700; z-index: 1;">${firstLetter}</span>
                </div>
                <h3 class="product-card-title">${p.nombre}</h3>
                <div class="product-card-meta">
                    <span>Categoría: <strong>${p.categoria}</strong></span>
                    <span>P. Compra: <strong>$${p.precioCompra.toFixed(2)}</strong></span>
                    <span class="product-card-price">$${p.precioVenta.toFixed(2)}</span>
                </div>
            </div>
            <div>
                ${badgeHtml}
                ${actionsHtml}
            </div>
        `;
        grid.appendChild(card);
    });
}

function openProductModal(productId = null) {
    const title = document.getElementById("product-modal-title");
    const form = document.getElementById("product-form");
    form.reset();

    if (productId) {
        // Modo Edición
        title.textContent = "✏ Editar Producto";
        const prod = db.productos.find(p => p.id === productId);
        if (prod) {
            document.getElementById("prod-id").value = prod.id;
            document.getElementById("prod-name").value = prod.nombre;
            document.getElementById("prod-category").value = prod.categoria;
            document.getElementById("prod-price-buy").value = prod.precioCompra;
            document.getElementById("prod-price-sell").value = prod.precioVenta;
            document.getElementById("prod-stock").value = prod.stock;
            // Deshabilitar campo stock inicial en edición (se ajusta por movimientos)
            document.getElementById("prod-stock").disabled = true;
            document.getElementById("prod-stock-min").value = prod.stockMinimo;
            document.getElementById("prod-image-path").value = prod.imagePath || "";
        }
    } else {
        // Modo Crear
        title.textContent = "➕ Registrar Producto";
        document.getElementById("prod-id").value = "";
        document.getElementById("prod-stock").disabled = false;
    }

    showModal('modal-product');
}

function saveProduct() {
    const idVal = document.getElementById("prod-id").value;
    const nameVal = document.getElementById("prod-name").value.trim();
    const catVal = document.getElementById("prod-category").value;
    const priceBuyVal = parseFloat(document.getElementById("prod-price-buy").value);
    const priceSellVal = parseFloat(document.getElementById("prod-price-sell").value);
    const stockVal = parseInt(document.getElementById("prod-stock").value);
    const stockMinVal = parseInt(document.getElementById("prod-stock-min").value);
    const imgVal = document.getElementById("prod-image-path").value.trim();

    if (!nameVal || isNaN(priceBuyVal) || isNaN(priceSellVal) || isNaN(stockMinVal) || (!idVal && isNaN(stockVal))) {
        alert("Por favor completa los campos marcados con asterisco (*).");
        return;
    }

    if (idVal) {
        // Editar
        const prod = db.productos.find(p => p.id === parseInt(idVal));
        if (prod) {
            prod.nombre = nameVal;
            prod.categoria = catVal;
            prod.precioCompra = priceBuyVal;
            prod.precioVenta = priceSellVal;
            prod.stockMinimo = stockMinVal;
            prod.imagePath = imgVal;
        }
    } else {
        // Crear
        const nextId = db.productos.length > 0 ? db.productos[db.productos.length - 1].id + 1 : 1;
        db.productos.push({
            id: nextId,
            nombre: nameVal,
            categoria: catVal,
            precioCompra: priceBuyVal,
            precioVenta: priceSellVal,
            stock: stockVal,
            stockMinimo: stockMinVal,
            imagePath: imgVal
        });

        // Registrar movimiento inicial de Entrada
        const d = new Date();
        const dateStr = d.getFullYear() + "-" + String(d.getMonth() + 1).padStart(2, '0') + "-" + String(d.getDate()).padStart(2, '0') + " " + String(d.getHours()).padStart(2, '0') + ":" + String(d.getMinutes()).padStart(2, '0') + ":" + String(d.getSeconds()).padStart(2, '0');
        db.movimientos.push({
            id: db.movimientos.length + 1,
            fecha: dateStr,
            tipo: 'Entrada',
            producto: nameVal,
            cantidad: stockVal,
            stockAnterior: 0,
            stockNuevo: stockVal,
            motivo: 'Registro inicial de producto nuevo'
        });
    }

    closeModal('modal-product');
    populateMovementProductDropdown();
    renderInventoryList();
}

function deleteProduct(productId) {
    const prod = db.productos.find(p => p.id === productId);
    if (!prod) return;

    const res = confirm(`¿Estás seguro de que deseas eliminar el producto:\n\n"${prod.nombre}"?\n\nEsta acción no se puede deshacer.`);
    if (res) {
        db.productos = db.productos.filter(p => p.id !== productId);
        renderInventoryList();
        populateMovementProductDropdown();
    }
}


// ── SECCIÓN 4: MOVIMIENTOS ──
function renderMovementsList() {
    const tbody = document.getElementById("movements-table-body");
    tbody.innerHTML = "";

    const typeFilter = document.getElementById("movement-filter-type").value;
    
    let filtered = db.movimientos;
    if (typeFilter !== "Todos") {
        filtered = filtered.filter(m => m.tipo === typeFilter);
    }

    // Ordenar de más reciente a más antiguo
    const sorted = filtered.slice().reverse();

    document.getElementById("movements-count-lbl").textContent = `Registros: ${sorted.length}`;

    if (sorted.length === 0) {
        tbody.innerHTML = `<tr><td colspan="8" style="text-align: center; color: #888;">No hay registros de movimientos en esta bitácora</td></tr>`;
        return;
    }

    sorted.forEach(m => {
        const tr = document.createElement("tr");
        const badgeClass = m.tipo.toLowerCase() === 'entrada' ? 'badge-entrada' : (m.tipo.toLowerCase() === 'salida' ? 'badge-salida' : 'badge-ajuste');
        const qtySign = m.cantidad > 0 ? `+${m.cantidad}` : `${m.cantidad}`;

        tr.innerHTML = `
            <td><code>#${m.id}</code></td>
            <td>${m.fecha}</td>
            <td><span class="badge-type ${badgeClass}">${m.tipo}</span></td>
            <td><strong>${m.producto}</strong></td>
            <td style="text-align: right; font-weight: bold;">${qtySign}</td>
            <td style="text-align: right; color: #718096;">${m.stockAnterior}</td>
            <td style="text-align: right; font-weight: bold;">${m.stockNuevo}</td>
            <td>${m.motivo}</td>
        `;
        tbody.appendChild(tr);
    });
}

function populateMovementProductDropdown() {
    const select = document.getElementById("mov-product-id");
    select.innerHTML = "";
    
    db.productos.forEach(p => {
        const opt = document.createElement("option");
        opt.value = p.id;
        opt.textContent = `${p.nombre} (Stock: ${p.stock})`;
        select.appendChild(opt);
    });
}

function openMovementModal() {
    const form = document.getElementById("movement-form");
    form.reset();
    populateMovementProductDropdown();
    showModal('modal-movement');
}

function saveMovement() {
    const prodIdVal = parseInt(document.getElementById("mov-product-id").value);
    const typeVal = document.getElementById("mov-type").value;
    const qtyVal = parseInt(document.getElementById("mov-quantity").value);
    const reasonVal = document.getElementById("mov-reason").value.trim();

    if (isNaN(prodIdVal) || !typeVal || isNaN(qtyVal) || qtyVal <= 0 || !reasonVal) {
        alert("Completa todos los campos obligatorios (*) y asegúrate de ingresar una cantidad positiva.");
        return;
    }

    const prod = db.productos.find(p => p.id === prodIdVal);
    if (!prod) return;

    const antStock = prod.stock;
    let newStock = antStock;
    let netQuantity = qtyVal;

    if (typeVal === "Entrada") {
        newStock += qtyVal;
    } else {
        // Es ajuste (puede ser salida/merma)
        // Pedir en la interfaz si suma o resta
        // Para simplificar, en ajustes restamos inventario por merma/daño
        newStock -= qtyVal;
        netQuantity = -qtyVal;
        if (newStock < 0) {
            alert("No puedes ajustar el inventario a un valor menor a cero.");
            return;
        }
    }

    // Actualizar stock de producto
    prod.stock = newStock;

    // Registrar en bitácora
    const d = new Date();
    const dateStr = d.getFullYear() + "-" + String(d.getMonth() + 1).padStart(2, '0') + "-" + String(d.getDate()).padStart(2, '0') + " " + String(d.getHours()).padStart(2, '0') + ":" + String(d.getMinutes()).padStart(2, '0') + ":" + String(d.getSeconds()).padStart(2, '0');
    
    db.movimientos.push({
        id: db.movimientos.length + 1,
        fecha: dateStr,
        tipo: typeVal,
        producto: prod.nombre,
        cantidad: netQuantity,
        stockAnterior: antStock,
        stockNuevo: newStock,
        motivo: reasonVal
    });

    closeModal('modal-movement');
    renderMovementsList();
}


// ── SECCIÓN 5: PROVEEDORES ──
function renderProvidersList() {
    const tbody = document.getElementById("providers-table-body");
    tbody.innerHTML = "";

    const showActions = currentUser && currentUser.Rol === 'Admin';

    db.proveedores.forEach(p => {
        const tr = document.createElement("tr");

        const actionBtnHtml = showActions 
            ? `<button class="wireframe-btn btn-outline btn-sm" style="color: #c53030; border-color: #feb2b2;" onclick="deleteProvider(${p.id})">🗑 Eliminar</button>`
            : `<span style="font-size: 11px; color: #a0aec0;">[Sin permisos]</span>`;

        tr.innerHTML = `
            <td><code>#${p.id}</code></td>
            <td><strong>${p.empresa}</strong></td>
            <td>${p.contacto || 'N/A'}</td>
            <td>${p.telefono || 'N/A'}</td>
            <td>${p.email || 'N/A'}</td>
            <td><small>${p.direccion || 'N/A'}</small></td>
            <td>${actionBtnHtml}</td>
        `;
        tbody.appendChild(tr);
    });
}

function openProviderModal() {
    const form = document.getElementById("provider-form");
    form.reset();
    showModal('modal-provider');
}

function saveProvider() {
    const compVal = document.getElementById("prov-company").value.trim();
    const contVal = document.getElementById("prov-contact").value.trim();
    const telVal = document.getElementById("prov-phone").value.trim();
    const emailVal = document.getElementById("prov-email").value.trim();
    const addrVal = document.getElementById("prov-address").value.trim();

    if (!compVal) {
        alert("El nombre de la empresa proveedora es obligatorio.");
        return;
    }

    const nextId = db.proveedores.length > 0 ? db.proveedores[db.proveedores.length - 1].id + 1 : 1;
    db.proveedores.push({
        id: nextId,
        empresa: compVal,
        contacto: contVal,
        telefono: telVal,
        email: emailVal,
        direccion: addrVal
    });

    closeModal('modal-provider');
    renderProvidersList();
}

function deleteProvider(id) {
    const prov = db.proveedores.find(p => p.id === id);
    if (!prov) return;

    if (confirm(`¿Eliminar al proveedor "${prov.empresa}"?\nEsta acción no se puede deshacer.`)) {
        db.proveedores = db.proveedores.filter(p => p.id !== id);
        renderProvidersList();
    }
}


// ── SECCIÓN 6: REPORTES ──
function renderReportesView() {
    const tbody = document.getElementById("sales-history-tbody");
    tbody.innerHTML = "";

    // Ordenar historial de ventas de la más reciente a la más antigua
    const sortedSales = db.ventasHistory.slice().reverse();

    if (sortedSales.length === 0) {
        tbody.innerHTML = `<tr><td colspan="4" style="text-align: center; color: #888;">No hay ventas registradas en el historial</td></tr>`;
        return;
    }

    sortedSales.forEach(v => {
        const tr = document.createElement("tr");
        tr.innerHTML = `
            <td>
                <strong>Ticket #${v.id}</strong>
                <div style="font-size: 10px; color: #718096;">${v.fecha}</div>
            </td>
            <td>$${parseFloat(v.subtotal).toFixed(2)}</td>
            <td>$${parseFloat(v.iva).toFixed(2)}</td>
            <td style="font-weight: 700;">$${parseFloat(v.total).toFixed(2)}</td>
        `;
        tbody.appendChild(tr);
    });
}


// ── COMUNICACIÓN Y BANDEJA COLABORATIVA (Comentarios del Equipo) ──
function toggleCollabTray() {
    const tray = document.getElementById("collaboration-tray");
    tray.classList.toggle("collapsed");
}

function addNewComment() {
    const userVal = document.getElementById("collab-user-select").value;
    const textVal = document.getElementById("collab-comment-input").value.trim();

    if (!textVal) return;

    const container = document.getElementById("collab-comments-container");

    const authorClass = `author-${userVal.toLowerCase()}`;
    const authorName = userVal === 'Docente' ? 'Profesor/a (Revisor/a)' : (userVal === 'Wendy' ? 'Wendy Rivera' : (userVal === 'Enoc' ? 'Enoc H.' : 'Guillermina S.'));

    const d = new Date();
    const timeStr = `Hoy, ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}`;

    const div = document.createElement("div");
    div.className = "comment-item";
    div.innerHTML = `
        <div class="comment-header">
            <span class="comment-author ${authorClass}">${authorName}</span>
            <span class="comment-time">${timeStr}</span>
        </div>
        <p class="comment-text">${textVal}</p>
    `;

    container.appendChild(div);
    container.scrollTop = container.scrollHeight; // Autoscroll

    // Limpiar input
    document.getElementById("collab-comment-input").value = "";

    // Incrementar número de comentarios del badge
    const badge = document.querySelector(".collab-badge");
    const count = parseInt(badge.textContent) + 1;
    badge.textContent = count;
}


// ── FUNCIONES DE AYUDA DE MODALES ──
function showModal(modalId) {
    document.getElementById(modalId).classList.remove("hidden");
}

function closeModal(modalId) {
    document.getElementById(modalId).classList.add("hidden");
    
    // Si se cierra el modal de checkout, nos aseguramos de resetear inputs de cantidades si fuera necesario
    if (modalId === 'modal-checkout') {
        document.getElementById("checkout-error-msg").textContent = "";
    }
}

// Accesos rápidos de prueba desde el diagrama interactivo
function openCheckoutModalMock() {
    switchSection('view-ventas');
    // Forzar un item en el carrito si está vacío para permitir abrir el modal
    if (currentCart.length === 0) {
        addToCart(1); // Añadir cuaderno por defecto
    }
    openCheckoutModal();
}
