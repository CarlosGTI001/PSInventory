/**
 * Search Dialog - Modal de búsqueda para select fields
 * Permite buscar y seleccionar elementos de manera más eficiente
 */

/**
 * Abre un diálogo de búsqueda para un select
 * @param {string} selectId - ID del elemento select
 * @param {string} title - Título del diálogo
 * @param {string} searchPlaceholder - Placeholder del campo de búsqueda
 */
window.openSearchDialog = function(selectId, title, searchPlaceholder = 'Buscar...') {
    const selectElement = document.getElementById(selectId);
    if (!selectElement) {
        console.error('Select element not found:', selectId);
        return;
    }

    // Obtener todas las opciones del select (excepto la opción vacía)
    const options = Array.from(selectElement.options)
        .filter(opt => opt.value !== '')
        .map(opt => ({
            value: opt.value,
            text: opt.text,
            selected: opt.selected
        }));

    // Crear el diálogo
    const dialog = document.createElement('md-dialog');
    dialog.id = 'search-dialog-' + selectId;
    
    dialog.innerHTML = `
        <div slot="headline" style="padding: 24px 24px 0 24px;">
            <div style="display: flex; align-items: center; gap: 12px;">
                <md-icon>search</md-icon>
                <span style="font-size: 24px; font-weight: 500;">${title}</span>
            </div>
        </div>
        <div slot="content" style="padding: 24px; min-width: 400px; max-width: 600px;">
            <md-filled-text-field 
                id="search-input-${selectId}"
                label="${searchPlaceholder}"
                type="search"
                style="width: 100%; margin-bottom: 16px;">
                <md-icon slot="leading-icon">search</md-icon>
            </md-filled-text-field>
            
            <div id="search-results-${selectId}" 
                 style="max-height: 400px; overflow-y: auto; border: 1px solid var(--md-sys-color-outline); border-radius: 12px;">
                ${options.map(opt => `
                    <div class="search-result-item ${opt.selected ? 'selected' : ''}" 
                         data-value="${opt.value}"
                         style="padding: 16px; cursor: pointer; display: flex; align-items: center; gap: 12px; border-bottom: 1px solid var(--md-sys-color-surface-variant);">
                        ${opt.selected ? '<md-icon style="color: var(--md-sys-color-primary);">check_circle</md-icon>' : '<md-icon style="color: var(--md-sys-color-outline);">radio_button_unchecked</md-icon>'}
                        <span>${opt.text}</span>
                    </div>
                `).join('')}
            </div>
            
            <div id="no-results-${selectId}" 
                 style="display: none; padding: 32px; text-align: center; color: var(--md-sys-color-on-surface-variant);">
                <md-icon style="font-size: 48px; opacity: 0.5;">search_off</md-icon>
                <p style="margin-top: 8px;">No se encontraron resultados</p>
            </div>
        </div>
        <div slot="actions" style="padding: 0 24px 24px 24px; gap: 8px;">
            <md-text-button id="cancel-search-${selectId}">
                Cancelar
            </md-text-button>
        </div>
    `;

    document.body.appendChild(dialog);

    // Agregar estilos para hover
    const style = document.createElement('style');
    style.textContent = `
        .search-result-item:hover {
            background-color: var(--md-sys-color-surface-variant);
        }
        .search-result-item.selected {
            background-color: var(--md-sys-color-primary-container);
        }
    `;
    document.head.appendChild(style);

    // Funcionalidad de búsqueda
    const searchInput = dialog.querySelector(`#search-input-${selectId}`);
    const resultsContainer = dialog.querySelector(`#search-results-${selectId}`);
    const noResults = dialog.querySelector(`#no-results-${selectId}`);

    searchInput.addEventListener('input', (e) => {
        const searchTerm = e.target.value.toLowerCase();
        const items = resultsContainer.querySelectorAll('.search-result-item');
        let visibleCount = 0;

        items.forEach(item => {
            const text = item.textContent.toLowerCase();
            if (text.includes(searchTerm)) {
                item.style.display = 'flex';
                visibleCount++;
            } else {
                item.style.display = 'none';
            }
        });

        resultsContainer.style.display = visibleCount > 0 ? 'block' : 'none';
        noResults.style.display = visibleCount > 0 ? 'none' : 'block';
    });

    // Click en un resultado
    resultsContainer.addEventListener('click', (e) => {
        const item = e.target.closest('.search-result-item');
        if (item) {
            const value = item.dataset.value;
            selectElement.value = value;
            
            // Disparar evento change para que funcionen las validaciones
            selectElement.dispatchEvent(new Event('change', { bubbles: true }));
            
            // Actualizar visual del select (si es Material Design)
            if (selectElement.classList.contains('md3-select')) {
                selectElement.parentElement.classList.add('md3-select-filled');
            }
            
            dialog.close();
        }
    });

    // Botón cancelar
    dialog.querySelector(`#cancel-search-${selectId}`).addEventListener('click', () => {
        dialog.close();
    });

    // Limpiar el diálogo al cerrarse
    dialog.addEventListener('close', () => {
        setTimeout(() => dialog.remove(), 300);
    });

    // Abrir el diálogo
    dialog.show();
    
    // Focus en el campo de búsqueda
    setTimeout(() => searchInput.focus(), 100);
};

/**
 * Inicializa los botones de búsqueda para todos los selects con clase 'searchable-select'
 */
window.initSearchableSelects = function() {
    document.querySelectorAll('.searchable-select-wrapper').forEach(wrapper => {
        const button = wrapper.querySelector('.search-button');
        const select = wrapper.querySelector('select');
        
        if (button && select && !button.dataset.initialized) {
            button.dataset.initialized = 'true';
            button.addEventListener('click', () => {
                const title = button.dataset.title || 'Buscar';
                const placeholder = button.dataset.placeholder || 'Escriba para buscar...';
                openSearchDialog(select.id, title, placeholder);
            });
        }
    });
};

// Auto-inicializar cuando el DOM esté listo
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', window.initSearchableSelects);
} else {
    window.initSearchableSelects();
}
