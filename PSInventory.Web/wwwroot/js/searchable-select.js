// Select Search Component
// Componente reutilizable para agregar búsqueda a los selects

class SearchableSelect {
    constructor(selectElement) {
        this.selectElement = selectElement;
        this.originalOptions = Array.from(selectElement.options);
        this.init();
    }

    init() {
        // Crear botón de búsqueda
        const searchButton = document.createElement('md-icon-button');
        searchButton.innerHTML = '<md-icon>search</md-icon>';
        searchButton.style.cssText = 'margin-left: 8px;';
        searchButton.setAttribute('title', 'Buscar');
        
        // Insertar botón después del select
        const selectField = this.selectElement.closest('.md3-select-field') || this.selectElement.closest('.form-field');
        if (selectField) {
            // Crear contenedor flex para alinear select y botón
            const wrapper = document.createElement('div');
            wrapper.style.cssText = 'display: flex; align-items: center; gap: 8px;';
            
            const selectParent = this.selectElement.parentElement;
            selectParent.parentNode.insertBefore(wrapper, selectParent);
            wrapper.appendChild(selectParent);
            wrapper.appendChild(searchButton);
        }

        // Agregar evento click
        searchButton.addEventListener('click', (e) => {
            e.preventDefault();
            this.openSearchDialog();
        });
    }

    openSearchDialog() {
        // Crear dialog
        const dialog = document.createElement('md-dialog');
        dialog.setAttribute('type', 'alert');
        
        dialog.innerHTML = `
            <div slot="headline">
                <div style="display: flex; align-items: center; gap: 12px;">
                    <md-icon>search</md-icon>
                    <span>Buscar</span>
                </div>
            </div>
            <div slot="content">
                <md-filled-text-field 
                    id="search-input"
                    label="Buscar..." 
                    style="width: 100%; margin-bottom: 16px;">
                    <md-icon slot="leading-icon">search</md-icon>
                </md-filled-text-field>
                <div id="search-results" style="max-height: 400px; overflow-y: auto;">
                    <!-- Results will be inserted here -->
                </div>
            </div>
            <div slot="actions">
                <md-text-button class="close-btn">Cerrar</md-text-button>
            </div>
        `;

        document.body.appendChild(dialog);

        // Poblar resultados iniciales
        const resultsContainer = dialog.querySelector('#search-results');
        this.displayResults(this.originalOptions, resultsContainer);

        // Agregar búsqueda en tiempo real
        const searchInput = dialog.querySelector('#search-input');
        searchInput.addEventListener('input', (e) => {
            const searchTerm = e.target.value.toLowerCase();
            const filteredOptions = this.originalOptions.filter(option => 
                option.text.toLowerCase().includes(searchTerm)
            );
            this.displayResults(filteredOptions, resultsContainer);
        });

        // Cerrar dialog
        dialog.querySelector('.close-btn').addEventListener('click', () => {
            dialog.close();
            setTimeout(() => dialog.remove(), 300);
        });

        // Abrir dialog
        dialog.show();
        
        // Focus en input
        setTimeout(() => searchInput.focus(), 100);
    }

    displayResults(options, container) {
        container.innerHTML = '';
        
        if (options.length === 0) {
            container.innerHTML = `
                <div style="text-align: center; padding: 32px; color: var(--md-sys-color-on-surface-variant);">
                    <md-icon style="font-size: 48px;">search_off</md-icon>
                    <p>No se encontraron resultados</p>
                </div>
            `;
            return;
        }

        options.forEach(option => {
            if (!option.value) return; // Skip empty option
            
            const resultItem = document.createElement('md-list-item');
            resultItem.setAttribute('type', 'button');
            resultItem.style.cssText = 'cursor: pointer; border-bottom: 1px solid var(--md-sys-color-outline-variant);';
            
            resultItem.innerHTML = `
                <div slot="headline">${option.text}</div>
            `;

            resultItem.addEventListener('click', () => {
                this.selectElement.value = option.value;
                
                // Disparar evento change
                this.selectElement.dispatchEvent(new Event('change', { bubbles: true }));
                
                // Cerrar dialog
                const dialog = container.closest('md-dialog');
                if (dialog) {
                    dialog.close();
                    setTimeout(() => dialog.remove(), 300);
                }

                // Mostrar feedback
                showSnackbar(`Seleccionado: ${option.text}`, 'success');
            });

            container.appendChild(resultItem);
        });
    }
}

// Auto-inicializar selects con muchas opciones
document.addEventListener('DOMContentLoaded', () => {
    const selects = document.querySelectorAll('select.md3-select');
    
    selects.forEach(select => {
        const optionCount = select.options.length;
        
        // Solo agregar búsqueda si hay más de 10 opciones
        if (optionCount > 10) {
            new SearchableSelect(select);
        }
    });
});

// Exportar para uso manual
window.SearchableSelect = SearchableSelect;
