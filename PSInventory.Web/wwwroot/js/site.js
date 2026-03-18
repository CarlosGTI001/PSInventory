// PSInventory - Site JavaScript

/**
 * Sincroniza los valores de md-filled-text-field y md-outlined-text-field
 * a inputs ocultos para garantizar que se incluyan en el form submission.
 * Necesario porque los web components de Material Web usan Shadow DOM
 * y no todos los navegadores soportan Form-Associated Custom Elements.
 */
function syncMdTextFields(form) {
    form.querySelectorAll('md-filled-text-field[name], md-outlined-text-field[name]').forEach(function(field) {
        var name = field.getAttribute('name');
        var value = field.value;
        var hidden = form.querySelector('input[type="hidden"][name="' + name + '"][data-md-sync]');
        if (!hidden) {
            hidden = document.createElement('input');
            hidden.type = 'hidden';
            hidden.name = name;
            hidden.setAttribute('data-md-sync', 'true');
            form.appendChild(hidden);
        }
        hidden.value = value;
    });
}
window.syncMdTextFields = syncMdTextFields;

// Sincronización automática en todos los forms al hacer submit
document.addEventListener('submit', function(e) {
    if (e.target && e.target.tagName === 'FORM') {
        syncMdTextFields(e.target);
    }
}, true); // capture=true para ejecutar antes de otros listeners

// Toggle drawer on mobile
document.addEventListener('DOMContentLoaded', function() {
    const menuButton = document.getElementById('menu-button');
    const appContainer = document.querySelector('.app-container');
    
    if (menuButton) {
        menuButton.addEventListener('click', function() {
            appContainer.classList.toggle('drawer-open');
        });
    }
    
    // Close drawer when clicking outside on mobile
    document.querySelector('.main-content')?.addEventListener('click', function() {
        if (window.innerWidth <= 1024 && appContainer.classList.contains('drawer-open')) {
            appContainer.classList.remove('drawer-open');
        }
    });
});

// Show loading overlay
function showLoading(message = 'Cargando...') {
    const overlay = document.createElement('div');
    overlay.className = 'loading-overlay';
    overlay.id = 'loading-overlay';
    overlay.innerHTML = `
        <div style="text-align: center;">
            <md-circular-progress indeterminate></md-circular-progress>
            <p class="md-typescale-body-medium" style="color: white; margin-top: 16px;">${message}</p>
        </div>
    `;
    document.body.appendChild(overlay);
}

// Hide loading overlay
function hideLoading() {
    const overlay = document.getElementById('loading-overlay');
    if (overlay) {
        overlay.remove();
    }
}

// Show snackbar notification
function showSnackbar(message, type = 'info') {
    const snackbar = document.createElement('md-snackbar');
    snackbar.textContent = message;
    
    // Color based on type
    if (type === 'success') {
        snackbar.style.setProperty('--md-snackbar-container-color', 'var(--md-sys-color-success)');
    } else if (type === 'error') {
        snackbar.style.setProperty('--md-snackbar-container-color', 'var(--md-sys-color-error)');
    } else if (type === 'warning') {
        snackbar.style.setProperty('--md-snackbar-container-color', 'var(--md-sys-color-warning)');
    }
    
    document.body.appendChild(snackbar);
    if (typeof snackbar.show === 'function') {
        snackbar.show();
    } else {
        snackbar.open = true;
    }
    
    setTimeout(() => {
        if (typeof snackbar.close === 'function') {
            snackbar.close();
        }
        snackbar.remove();
    }, 4000);
}

// Confirm dialog
async function confirmDialog(title, message) {
    return new Promise((resolve) => {
        const dialog = document.createElement('md-dialog');
        dialog.innerHTML = `
            <div slot="headline">${title}</div>
            <div slot="content">${message}</div>
            <div slot="actions">
                <md-text-button class="cancel-btn">Cancelar</md-text-button>
                <md-filled-button class="confirm-btn">Confirmar</md-filled-button>
            </div>
        `;
        
        document.body.appendChild(dialog);
        dialog.show();
        
        dialog.querySelector('.cancel-btn').addEventListener('click', () => {
            dialog.close();
            dialog.remove();
            resolve(false);
        });
        
        dialog.querySelector('.confirm-btn').addEventListener('click', () => {
            dialog.close();
            dialog.remove();
            resolve(true);
        });
    });
}

// Format currency
function formatCurrency(value) {
    return new Intl.NumberFormat('es-GT', {
        style: 'currency',
        currency: 'GTQ'
    }).format(value);
}

// Format date
function formatDate(dateString) {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('es-GT', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit'
    }).format(date);
}

// Delete item with confirmation
async function deleteItem(url, itemName) {
    const confirmed = await confirmDialog(
        'Confirmar eliminación',
        `¿Está seguro de que desea eliminar "${itemName}"? Esta acción no se puede deshacer.`
    );
    
    if (confirmed) {
        showLoading('Eliminando...');
        
        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            
            if (response.ok) {
                showSnackbar('Eliminado exitosamente', 'success');
                setTimeout(() => {
                    window.location.reload();
                }, 1000);
            } else {
                const error = await response.text();
                showSnackbar(`Error: ${error}`, 'error');
            }
        } catch (error) {
            showSnackbar(`Error: ${error.message}`, 'error');
        } finally {
            hideLoading();
        }
    }
}

// Export global functions
window.showLoading = showLoading;
window.hideLoading = hideLoading;
window.showSnackbar = showSnackbar;
window.confirmDialog = confirmDialog;
window.formatCurrency = formatCurrency;
window.formatDate = formatDate;
window.deleteItem = deleteItem;
