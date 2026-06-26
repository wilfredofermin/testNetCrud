/* testNet — Lógica del SPA CRUD.
 * Estado central + render declarativo + modales. Sin frameworks.
 * Consume window.ProductApi (js/api.js).
 */
(function () {
    'use strict';

    const { ProductApi } = window;
    const $ = (sel, root = document) => root.querySelector(sel);
    const $$ = (sel, root = document) => Array.from(root.querySelectorAll(sel));

    /* ----------------------------- Estado ----------------------------- */
    const state = {
        view: 'all',        // 'all' | 'active' | 'paged'
        page: 1,
        pageSize: 10,
        search: '',
        products: [],       // lista en memoria (all/active) o página actual (paged)
        pagedMeta: null,    // metadatos de paginación { totalPages, totalCount, ... }
        loading: false,
        allCache: [],       // cache de getAll para stats y búsqueda local
    };

    /* ----------------------------- Utilidades ----------------------------- */
    const fmtMoney = (n, currency = 'USD') =>
        new Intl.NumberFormat('es-MX', { style: 'currency', currency, maximumFractionDigits: 2 })
            .format(n || 0);

    const fmtInt = (n) => new Intl.NumberFormat('es-MX').format(n || 0);

    const fmtDate = (iso) => {
        if (!iso) return '—';
        const d = new Date(iso);
        if (Number.isNaN(d.getTime())) return '—';
        return d.toLocaleDateString('es-MX', { day: '2-digit', month: 'short', year: 'numeric' });
    };

    const escapeHtml = (s) => String(s ?? '')
        .replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;').replace(/'/g, '&#39;');

    const debounce = (fn, ms = 250) => {
        let t; return (...args) => { clearTimeout(t); t = setTimeout(() => fn(...args), ms); };
    };

    /* ----------------------------- Toasts ----------------------------- */
    const TOAST_ICONS = {
        success: '<svg class="toast-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg>',
        error: '<svg class="toast-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/></svg>',
        info: '<svg class="toast-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><line x1="12" y1="16" x2="12" y2="12"/><line x1="12" y1="8" x2="12.01" y2="8"/></svg>',
    };

    function toast(message, type = 'info', timeout = 3800) {
        const el = document.createElement('div');
        el.className = `toast toast-${type}`;
        el.innerHTML = `${TOAST_ICONS[type] || TOAST_ICONS.info}<p class="text-sm text-slate-700 flex-1">${escapeHtml(message)}</p>`;
        $('#toastContainer').appendChild(el);
        const remove = () => {
            el.classList.add('leaving');
            el.addEventListener('animationend', () => el.remove(), { once: true });
        };
        const timer = setTimeout(remove, timeout);
        el.addEventListener('click', () => { clearTimeout(timer); remove(); });
    }

    /* ----------------------------- Carga de datos ----------------------------- */
    async function loadAll() {
        state.allCache = await ProductApi.getAll();
        return state.allCache;
    }

    async function fetchView() {
        state.loading = true;
        renderLoading();
        try {
            if (state.view === 'paged') {
                const data = await ProductApi.getPaged(state.page, state.pageSize);
                state.products = data.items || [];
                state.pagedMeta = data;
                // mantener stats frescos
                if (!state.allCache.length) await loadAll();
            } else if (state.view === 'active') {
                state.products = await ProductApi.getActive();
                if (!state.allCache.length) await loadAll();
            } else {
                state.products = await loadAll();
            }
        } catch (err) {
            toast(err.message || 'Error al cargar productos', 'error');
            state.products = [];
            state.pagedMeta = null;
        } finally {
            // Importante: liberar el estado de carga ANTES de renderizar,
            // porque renderTable() hace guard `if (state.loading) return`.
            state.loading = false;
            renderLoading();
            renderTable();
            renderPagination();
            renderStats();
        }
    }

    function refreshStatsCache() {
        // tras operaciones mutables, refresca el cache de stats sin re-render completo
        return loadAll().then(() => renderStats()).catch(() => {});
    }

    /* ----------------------------- Render: stats ----------------------------- */
    function renderStats() {
        const list = state.allCache || [];
        const total = list.length;
        const active = list.filter(p => p.isActive).length;
        const stock = list.reduce((s, p) => s + (p.stockQuantity || 0), 0);
        const value = list.reduce((s, p) => s + (p.price || 0) * (p.stockQuantity || 0), 0);
        $('#statTotal').textContent = fmtInt(total);
        $('#statActive').textContent = fmtInt(active);
        $('#statStock').textContent = fmtInt(stock);
        $('#statValue').textContent = fmtMoney(value, 'USD');
    }

    /* ----------------------------- Render: tabla ----------------------------- */
    function visibleProducts() {
        const q = state.search.trim().toLowerCase();
        if (!q) return state.products;
        return state.products.filter(p =>
            (p.code || '').toLowerCase().includes(q) ||
            (p.name || '').toLowerCase().includes(q));
    }

    function stockBadge(p) {
        if (p.stockQuantity <= 0)
            return `<span class="badge badge-out">Sin stock</span>`;
        if (p.stockQuantity <= 5)
            return `<span class="badge badge-low">${fmtInt(p.stockQuantity)} · bajo</span>`;
        return `<span class="font-mono text-slate-700">${fmtInt(p.stockQuantity)}</span>`;
    }

    function statusBadge(p) {
        return p.isActive
            ? `<span class="badge badge-active">● Activo</span>`
            : `<span class="badge badge-inactive">○ Inactivo</span>`;
    }

    function rowActions(p) {
        const id = p.id;
        return `
        <div class="flex items-center justify-end gap-1">
            <button class="row-action edit" data-action="edit" data-id="${id}" title="Editar">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/></svg>
            </button>
            <button class="row-action stock" data-action="stock" data-id="${id}" title="Ajustar stock">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M20.59 13.41l-7.17 7.17a2 2 0 0 1-2.83 0L2 12V2h10l8.59 8.59a2 2 0 0 1 0 2.82z"/><line x1="7" y1="7" x2="7.01" y2="7"/></svg>
            </button>
            <button class="row-action delete" data-action="delete" data-id="${id}" title="Eliminar">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/></svg>
            </button>
        </div>`;
    }

    function rowTemplate(p) {
        const nameCell = `<div class="min-w-0">
            <p class="font-medium text-slate-900 truncate" title="${escapeHtml(p.name)}">${escapeHtml(p.name)}</p>
            ${p.description ? `<p class="text-xs text-slate-400 truncate max-w-[28ch]" title="${escapeHtml(p.description)}">${escapeHtml(p.description)}</p>` : ''}
        </div>`;
        return `<tr data-id="${p.id}">
            <td class="px-4 py-3"><span class="font-mono text-xs font-semibold text-brand-700 bg-brand-50 px-2 py-0.5 rounded">${escapeHtml(p.code)}</span></td>
            <td class="px-4 py-3 max-w-xs">${nameCell}</td>
            <td class="px-4 py-3 text-right whitespace-nowrap">
                <span class="font-semibold text-slate-900">${fmtMoney(p.price, p.currency)}</span>
            </td>
            <td class="px-4 py-3 text-center">${stockBadge(p)}</td>
            <td class="px-4 py-3 text-center">${statusBadge(p)}</td>
            <td class="px-4 py-3">${rowActions(p)}</td>
        </tr>`;
    }

    function renderLoading() {
        const tbody = $('#productRows');
        if (state.loading) {
            tbody.innerHTML = '';
            $('#loadingState').classList.remove('hidden');
            $('#emptyState').classList.add('hidden');
        } else {
            $('#loadingState').classList.add('hidden');
        }
    }

    function renderTable() {
        if (state.loading) return;
        const rows = visibleProducts();
        const tbody = $('#productRows');
        tbody.innerHTML = rows.map(rowTemplate).join('');

        const emptyEl = $('#emptyState');
        if (rows.length === 0) {
            emptyEl.classList.remove('hidden');
            $('#emptyHint').textContent = state.search
                ? `Sin coincidencias para "${state.search}".`
                : 'Crea uno nuevo o reinicia la base de datos.';
        } else {
            emptyEl.classList.add('hidden');
        }
    }

    /* ----------------------------- Render: paginación ----------------------------- */
    function renderPagination() {
        const bar = $('#pagination');
        if (state.view !== 'paged' || !state.pagedMeta) {
            bar.classList.add('hidden');
            bar.classList.remove('flex');
            return;
        }
        bar.classList.remove('hidden');
        bar.classList.add('flex');

        const m = state.pagedMeta;
        $('#pageInfo').textContent =
            `${fmtInt(m.totalCount)} producto(s) · ${fmtInt((m.page - 1) * m.pageSize + (m.items?.length || 0))} mostrados`;
        $('#pageIndicator').textContent = `${m.page} / ${m.totalPages || 1}`;
        $('#btnFirst').disabled = !m.hasPreviousPage;
        $('#btnPrev').disabled = !m.hasPreviousPage;
        $('#btnNext').disabled = !m.hasNextPage;
        $('#btnLast').disabled = !m.hasNextPage;
    }

    /* ----------------------------- Render: tabs ----------------------------- */
    function renderTabs() {
        $$('.view-tab').forEach(btn => {
            btn.classList.toggle('active', btn.dataset.view === state.view);
        });
        const psWrap = $('#pageSizeWrap');
        if (state.view === 'paged') {
            psWrap.classList.remove('hidden');
            psWrap.classList.add('flex');
        } else {
            psWrap.classList.add('hidden');
            psWrap.classList.remove('flex');
        }
    }

    /* ----------------------------- Modales ----------------------------- */
    function openModal(id) {
        const m = document.getElementById(id);
        m.classList.remove('hidden');
        document.body.style.overflow = 'hidden';
    }
    function closeModal(id) {
        const m = document.getElementById(id);
        m.classList.add('hidden');
        document.body.style.overflow = '';
    }
    function closeAllModals() {
        $$('.modal').forEach(m => m.classList.add('hidden'));
        document.body.style.overflow = '';
    }

    /* --- Modal producto (crear / editar) --- */
    function openProductModal(product) {
        const isEdit = !!product;
        $('#productModalTitle').textContent = isEdit ? 'Editar producto' : 'Nuevo producto';
        $('#fId').value = isEdit ? product.id : '';
        $('#fCode').value = isEdit ? product.code : '';
        $('#fCode').disabled = isEdit; // el código no se actualiza en PUT
        $('#fName').value = isEdit ? product.name : '';
        $('#fDescription').value = isEdit ? (product.description || '') : '';
        $('#fPrice').value = isEdit ? product.price : '';
        $('#fStock').value = isEdit ? product.stockQuantity : 0;
        $('#fCurrency').value = isEdit ? (product.currency || 'USD') : 'USD';
        $('#fIsActive').checked = isEdit ? product.isActive : true;

        const activeWrap = $('#activeWrap');
        if (isEdit) { activeWrap.classList.remove('hidden'); activeWrap.classList.add('flex'); }
        else { activeWrap.classList.add('hidden'); activeWrap.classList.remove('flex'); }

        openModal('productModal');
        setTimeout(() => $('#fCode').focus(), 50);
    }

    async function submitProduct(e) {
        e.preventDefault();
        const id = $('#fId').value;
        const isEdit = !!id;

        const price = parseFloat($('#fPrice').value);
        const stock = parseInt($('#fStock').value, 10);

        if (isEdit) {
            const dto = {
                name: $('#fName').value.trim(),
                description: $('#fDescription').value.trim(),
                price: Number.isFinite(price) ? price : 0,
                stockQuantity: Number.isFinite(stock) ? stock : 0,
                currency: $('#fCurrency').value,
                isActive: $('#fIsActive').checked,
            };
            try {
                await ProductApi.update(id, dto);
                toast('Producto actualizado', 'success');
                closeModal('productModal');
                await fetchView();
                refreshStatsCache();
            } catch (err) {
                toast(err.message || 'No se pudo actualizar', 'error');
            }
        } else {
            const dto = {
                code: $('#fCode').value.trim(),
                name: $('#fName').value.trim(),
                description: $('#fDescription').value.trim(),
                price: Number.isFinite(price) ? price : 0,
                stockQuantity: Number.isFinite(stock) ? stock : 0,
                currency: $('#fCurrency').value,
            };
            try {
                await ProductApi.create(dto);
                toast('Producto creado', 'success');
                closeModal('productModal');
                await fetchView();
                refreshStatsCache();
            } catch (err) {
                toast(err.message || 'No se pudo crear', 'error');
            }
        }
    }

    /* --- Modal stock --- */
    let stockTarget = null;
    function openStockModal(product) {
        stockTarget = product;
        $('#stockProductName').textContent = `${product.code} · ${product.name}`;
        $('#stockCurrent').textContent = fmtInt(product.stockQuantity);
        $('#stockQuantity').value = 1;
        openModal('stockModal');
        setTimeout(() => $('#stockQuantity').focus(), 50);
    }

    async function adjustStock(delta) {
        if (!stockTarget) return;
        const qty = parseInt($('#stockQuantity').value, 10);
        if (!Number.isFinite(qty) || qty <= 0) {
            toast('La cantidad debe ser un entero positivo', 'error');
            return;
        }
        const btn = delta > 0 ? $('#btnStockAdd') : $('#btnStockRemove');
        btn.disabled = true;
        try {
            const updated = delta > 0
                ? await ProductApi.addStock(stockTarget.id, qty)
                : await ProductApi.removeStock(stockTarget.id, qty);
            toast(`Stock ${delta > 0 ? 'aumentado' : 'reducido'} en ${qty}`, 'success');
            stockTarget = null;
            closeModal('stockModal');
            await fetchView();
            refreshStatsCache();
            if (updated) { /* podrías abrir el detalle */ }
        } catch (err) {
            toast(err.message || 'No se pudo ajustar el stock', 'error');
        } finally {
            btn.disabled = false;
        }
    }

    /* --- Modal confirmar borrado --- */
    let deleteTarget = null;
    function openConfirmModal(product) {
        deleteTarget = product;
        $('#confirmName').textContent = `${product.code} · ${product.name}`;
        openModal('confirmModal');
    }

    async function confirmDelete() {
        if (!deleteTarget) return;
        const btn = $('#btnConfirmDelete');
        btn.disabled = true;
        try {
            await ProductApi.remove(deleteTarget.id);
            toast('Producto eliminado', 'success');
            deleteTarget = null;
            closeModal('confirmModal');
            await fetchView();
            refreshStatsCache();
        } catch (err) {
            toast(err.message || 'No se pudo eliminar', 'error');
        } finally {
            btn.disabled = false;
        }
    }

    /* ----------------------------- Acciones de fila ----------------------------- */
    async function onRowAction(e) {
        const btn = e.target.closest('[data-action]');
        if (!btn) return;
        const id = btn.dataset.id;
        let product = state.products.find(p => p.id === id) || state.allCache.find(p => p.id === id);
        if (!product) {
            try { product = await ProductApi.getById(id); } catch { toast('Producto no encontrado', 'error'); return; }
        }
        const action = btn.dataset.action;
        if (action === 'edit') openProductModal(product);
        else if (action === 'stock') openStockModal(product);
        else if (action === 'delete') openConfirmModal(product);
    }

    /* ----------------------------- Seed ----------------------------- */
    async function onSeed() {
        const btn = $('#btnSeed');
        btn.disabled = true;
        try {
            const res = await ProductApi.seed();
            toast(`Base de datos reseedeada con ${res?.count ?? 10} productos`, 'success');
            state.page = 1;
            await fetchView();
            refreshStatsCache();
        } catch (err) {
            toast(err.message || 'No se pudo reseedear', 'error');
        } finally {
            btn.disabled = false;
        }
    }

    /* ----------------------------- View switch ----------------------------- */
    function switchView(view) {
        if (state.view === view) return;
        state.view = view;
        state.page = 1;
        renderTabs();
        fetchView();
    }

    /* ----------------------------- Wiring ----------------------------- */
    function init() {
        // Tabs de vista
        $$('.view-tab').forEach(btn =>
            btn.addEventListener('click', () => switchView(btn.dataset.view)));

        // Búsqueda (debounce, filtrado local)
        $('#searchInput').addEventListener('input', debounce(() => {
            state.search = $('#searchInput').value;
            renderTable();
        }, 200));

        // Tamaño de página
        $('#pageSizeSelect').addEventListener('change', () => {
            state.pageSize = parseInt($('#pageSizeSelect').value, 10) || 10;
            state.page = 1;
            fetchView();
        });

        // Toolbar
        $('#btnRefresh').addEventListener('click', () => fetchView());
        $('#btnNew').addEventListener('click', () => openProductModal(null));
        $('#btnSeed').addEventListener('click', onSeed);

        // Tabla (delegación)
        $('#productRows').addEventListener('click', onRowAction);

        // Paginación
        $('#btnFirst').addEventListener('click', () => { state.page = 1; fetchView(); });
        $('#btnPrev').addEventListener('click', () => { state.page = Math.max(1, state.page - 1); fetchView(); });
        $('#btnNext').addEventListener('click', () => { state.page += 1; fetchView(); });
        $('#btnLast').addEventListener('click', () => { state.page = state.pagedMeta?.totalPages || 1; fetchView(); });

        // Formulario producto
        $('#productForm').addEventListener('submit', submitProduct);

        // Stock
        $('#btnStockAdd').addEventListener('click', () => adjustStock(1));
        $('#btnStockRemove').addEventListener('click', () => adjustStock(-1));

        // Confirmar borrado
        $('#btnConfirmDelete').addEventListener('click', confirmDelete);

        // Cerrar modales (backdrop, [data-close], Esc)
        document.addEventListener('click', (e) => {
            if (e.target.matches('[data-close]') || e.target.closest('[data-close]')) {
                const modal = e.target.closest('.modal');
                if (modal) closeModal(modal.id);
            }
        });
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') closeAllModals();
        });

        // Arranque
        renderTabs();
        fetchView();
    }

    document.addEventListener('DOMContentLoaded', init);
})();
