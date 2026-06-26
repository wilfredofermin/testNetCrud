/* testNet — Cliente API.
 * Capa fina sobre fetch para los endpoints de ProductsController y /api/seed.
 * Devuelve siempre JSON útil; lanza ApiError con el mensaje del middleware para que la UI lo muestre.
 */
(function (global) {
    'use strict';

    const BASE = '/api';

    class ApiError extends Error {
        constructor(message, status, payload) {
            super(message || `HTTP ${status}`);
            this.name = 'ApiError';
            this.status = status;
            this.payload = payload;
        }
    }

    async function request(method, path, { query, body } = {}) {
        let url = BASE + path;
        if (query) {
            const params = new URLSearchParams();
            for (const [k, v] of Object.entries(query)) {
                if (v === undefined || v === null || v === '') continue;
                params.append(k, String(v));
            }
            const qs = params.toString();
            if (qs) url += `?${qs}`;
        }

        const init = { method, headers: {} };
        if (body !== undefined) {
            init.headers['Content-Type'] = 'application/json';
            init.body = JSON.stringify(body);
        }

        let res;
        try {
            res = await fetch(url, init);
        } catch (err) {
            throw new ApiError('No se pudo conectar con el servidor.', 0, err);
        }

        const isJson = (res.headers.get('content-type') || '').includes('application/json');
        const data = isJson ? await res.json().catch(() => null) : null;

        if (!res.ok) {
            const message = (data && (data.error || data.title || data.message)) ||
                            `Error ${res.status} ${res.statusText}`.trim();
            throw new ApiError(message, res.status, data);
        }

        // 204 No Content o vacío
        if (res.status === 204 || !isJson) return null;
        return data;
    }

    const ProductApi = {
        // GET /api/products
        getAll: () => request('GET', '/products'),

        // GET /api/products/active
        getActive: () => request('GET', '/products/active'),

        // GET /api/products/paged?page=&pageSize=
        getPaged: (page, pageSize) =>
            request('GET', '/products/paged', { query: { page, pageSize } }),

        // GET /api/products/{id}
        getById: (id) => request('GET', `/products/${id}`),

        // GET /api/products/code/{code}
        getByCode: (code) => request('GET', `/products/code/${encodeURIComponent(code)}`),

        // POST /api/products
        create: (dto) => request('POST', '/products', { body: dto }),

        // PUT /api/products/{id}
        update: (id, dto) => request('PUT', `/products/${id}`, { body: dto }),

        // DELETE /api/products/{id}
        remove: (id) => request('DELETE', `/products/${id}`),

        // PATCH /api/products/{id}/stock/add
        addStock: (id, quantity) =>
            request('PATCH', `/products/${id}/stock/add`, { body: { quantity } }),

        // PATCH /api/products/{id}/stock/remove
        removeStock: (id, quantity) =>
            request('PATCH', `/products/${id}/stock/remove`, { body: { quantity } }),

        // POST /api/seed  (fuera de /api/products, pero mismo controlador raíz)
        seed: () => request('POST', '/seed'),

        ApiError,
    };

    global.ProductApi = ProductApi;
})(window);
