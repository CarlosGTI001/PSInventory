// Quick Add + Search for MD3 selects across forms
(function () {
    function getRequestToken() {
        return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
    }

    function routeByContext(entity) {
        const path = (window.location.pathname || '').toLowerCase();
        if (path.startsWith('/despacho')) return `/Despacho/Crear${entity}Rapida`;
        if (path.startsWith('/salidasinregistro')) return `/SalidaSinRegistro/Crear${entity}Rapida`;
        if (entity === 'Categoria') return '/Categorias/CrearRapido';
        if (entity === 'Departamento') return '/Departamentos/CrearRapido';
        if (entity === 'Region') return '/Regiones/CrearRapido';
        if (entity === 'Sucursal') return '/Sucursales/CrearRapido';
        return '';
    }

    function ensureSelectId(select) {
        if (select.id) return select.id;
        const autoId = `auto-select-${Math.random().toString(36).slice(2, 9)}`;
        select.id = autoId;
        return autoId;
    }

    function resolveConfig(select) {
        const idn = `${select.id || ''} ${select.name || ''}`.toLowerCase();
        if (idn.includes('categoria')) {
            return { entity: 'Categoria', title: 'Nueva Categoría', label: 'Nombre de la categoría', icon: 'category' };
        }
        if (idn.includes('departamento')) {
            return { entity: 'Departamento', title: 'Nuevo Departamento', label: 'Nombre del departamento', icon: 'business_center' };
        }
        if (idn.includes('region')) {
            return { entity: 'Region', title: 'Nueva Región', label: 'Nombre de la región', icon: 'public' };
        }
        if (idn.includes('sucursal')) {
            return { entity: 'Sucursal', title: 'Nueva Sucursal', label: 'Nombre de la sucursal', icon: 'store' };
        }
        return null;
    }

    function findRegionValueForSucursal(select) {
        const declared = select.getAttribute('data-region-source');
        if (declared) {
            return document.getElementById(declared)?.value || '';
        }

        const form = select.closest('form') || document;
        const candidates = form.querySelectorAll('select');
        for (const c of candidates) {
            if (c === select) continue;
            const key = `${c.id || ''} ${c.name || ''}`.toLowerCase();
            if (key.includes('region')) {
                return c.value || '';
            }
        }
        return '';
    }

    function upsertOption(select, value, text) {
        let opt = Array.from(select.options).find(o => o.value === String(value));
        if (!opt) {
            opt = document.createElement('option');
            opt.value = String(value);
            opt.textContent = text;
            select.appendChild(opt);
        } else {
            opt.textContent = text;
        }
        select.value = String(value);
        select.disabled = false;
        select.dispatchEvent(new Event('change', { bubbles: true }));
    }

    async function createOption(select, config) {
        const nombre = await window.showTextInputDialog?.(config.title, config.label, 'Crear');
        if (!nombre) return;

        const endpoint = routeByContext(config.entity);
        if (!endpoint) return;

        const body = { nombre };
        if (config.entity === 'Sucursal') {
            const regionId = findRegionValueForSucursal(select);
            if (!regionId) {
                window.showSnackbar?.('Primero selecciona una región.', 'warning');
                return;
            }
            body.regionId = Number(regionId);
        }

        const resp = await fetch(endpoint, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getRequestToken()
            },
            body: JSON.stringify(body)
        });

        let data = {};
        try { data = await resp.json(); } catch { }
        if (!resp.ok || !data.success) {
            window.showSnackbar?.(data.message || 'No se pudo crear el registro.', 'error');
            return;
        }

        upsertOption(select, data.option.value, data.option.text);
        window.showSnackbar?.(`${config.title.replace('Nuevo ', '').replace('Nueva ', '')} agregada correctamente.`, 'success');
    }

    function buildButton(icon, title) {
        const btn = document.createElement('md-icon-button');
        btn.type = 'button';
        btn.title = title;
        btn.innerHTML = `<md-icon>${icon}</md-icon>`;
        return btn;
    }

    function ensureActionContainer(select) {
        const field = select.closest('.md3-select-field') || select.parentElement;
        if (!field) return null;
        const wrapper = field.closest('.select-with-search');
        if (wrapper) return wrapper;

        const newWrap = document.createElement('div');
        newWrap.className = 'select-with-search';
        field.parentNode.insertBefore(newWrap, field);
        newWrap.appendChild(field);
        return newWrap;
    }

    function hasSearchButton(wrapper, selectId) {
        return !!wrapper.querySelector(`[data-select-action="search"][data-target="${selectId}"]`) ||
               !!wrapper.querySelector(`md-icon-button[onclick*="openSearchDialog('${selectId}'"]`);
    }

    function hasAddButton(wrapper, selectId) {
        return !!wrapper.querySelector(`[data-select-action="add"][data-target="${selectId}"]`) ||
               !!wrapper.querySelector(`md-icon-button[onclick*="crear"][title*="Agregar"]`);
    }

    function enhanceSelect(select) {
        if (select.dataset.quickAddEnhanced === '1') return;
        const selectId = ensureSelectId(select);
        const wrapper = ensureActionContainer(select);
        if (!wrapper) return;

        if (!hasSearchButton(wrapper, selectId)) {
            const searchBtn = buildButton('search', 'Buscar');
            searchBtn.dataset.selectAction = 'search';
            searchBtn.dataset.target = selectId;
            searchBtn.addEventListener('click', () => {
                window.openSearchDialog?.(selectId, 'Buscar', 'Escribe para buscar...');
            });
            wrapper.appendChild(searchBtn);
        }

        const config = resolveConfig(select);
        if (config && !hasAddButton(wrapper, selectId)) {
            const addBtn = buildButton('add', `Agregar ${config.entity.toLowerCase()}`);
            addBtn.dataset.selectAction = 'add';
            addBtn.dataset.target = selectId;
            addBtn.addEventListener('click', () => createOption(select, config));
            wrapper.appendChild(addBtn);
        }

        select.dataset.quickAddEnhanced = '1';
    }

    function enhanceAll() {
        document.querySelectorAll('select.md3-select').forEach(enhanceSelect);
    }

    window.enhanceQuickAddSelects = enhanceAll;

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', enhanceAll);
    } else {
        enhanceAll();
    }

    const observer = new MutationObserver(() => enhanceAll());
    observer.observe(document.documentElement, { childList: true, subtree: true });
})();
