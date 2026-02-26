// Enhanced Confirmation Dialogs
// Diálogos de confirmación mejorados para Edit y Delete

/**
 * Muestra un diálogo comparando los datos originales vs los modificados
 * @param {Object} originalData - Datos originales del formulario
 * @param {Object} newData - Datos nuevos del formulario
 * @param {string} entityName - Nombre de la entidad (ej: "Categoría", "Usuario")
 * @returns {Promise<boolean>} - true si el usuario confirma, false si cancela
 */
async function showEditConfirmationDialog(originalData, newData, entityName = "Registro") {
    return new Promise((resolve) => {
        const dialog = document.createElement('md-dialog');
        
        // Encontrar campos que cambiaron
        const changes = [];
        const allKeys = new Set([...Object.keys(originalData), ...Object.keys(newData)]);
        
        allKeys.forEach(key => {
            const oldValue = originalData[key] ?? '';
            const newValue = newData[key] ?? '';
            
            // Ignorar campos técnicos
            if (key === '__RequestVerificationToken' || key.startsWith('_')) return;
            
            if (oldValue != newValue) {
                changes.push({
                    field: formatFieldName(key),
                    oldValue: formatValue(oldValue),
                    newValue: formatValue(newValue)
                });
            }
        });

        if (changes.length === 0) {
            // No hay cambios
            const noChangesDialog = document.createElement('md-dialog');
            noChangesDialog.innerHTML = `
                <div slot="headline" style="font-size: 24px; font-weight: 500; padding: 24px 24px 0 24px;">
                    <div style="display: flex; align-items: center; gap: 12px;">
                        <md-icon style="color: var(--md-sys-color-primary); font-size: 28px;">info</md-icon>
                        <span>Sin Cambios</span>
                    </div>
                </div>
                <div slot="content" style="padding: 24px; min-width: 320px;">
                    <p style="margin: 0; font-size: 16px; line-height: 24px; color: var(--md-sys-color-on-surface);">
                        No se detectaron cambios en los datos del formulario.
                    </p>
                </div>
                <div slot="actions" style="padding: 0 24px 24px 24px; display: flex; gap: 8px; justify-content: flex-end;">
                    <md-filled-button class="close-btn">
                        Entendido
                    </md-filled-button>
                </div>
            `;
            document.body.appendChild(noChangesDialog);
            noChangesDialog.querySelector('.close-btn').addEventListener('click', () => {
                noChangesDialog.close();
                setTimeout(() => noChangesDialog.remove(), 300);
            });
            noChangesDialog.show();
            resolve(false);
            return;
        }

        dialog.innerHTML = `
            <div slot="headline" style="font-size: 24px; font-weight: 500; padding: 24px 24px 0 24px;">
                <div style="display: flex; align-items: center; gap: 12px;">
                    <md-icon style="color: var(--md-sys-color-tertiary); font-size: 28px;">edit_note</md-icon>
                    <span>Confirmar Modificación</span>
                </div>
            </div>
            <div slot="content" style="padding: 24px; min-width: 500px; max-width: 700px;">
                <p style="margin: 0 0 20px 0; font-size: 16px; line-height: 24px; color: var(--md-sys-color-on-surface);">
                    Se detectaron <strong>${changes.length} cambio${changes.length > 1 ? 's' : ''}</strong> en ${entityName}:
                </p>
                
                <div style="max-height: 450px; overflow-y: auto; padding-right: 4px;">
                    ${changes.map(change => `
                        <div style="
                            background: var(--md-sys-color-surface-variant);
                            border-radius: 12px;
                            padding: 16px;
                            margin-bottom: 16px;
                            border-left: 4px solid var(--md-sys-color-primary);
                        ">
                            <div style="
                                color: var(--md-sys-color-primary);
                                margin-bottom: 12px;
                                display: flex;
                                align-items: center;
                                gap: 8px;
                                font-size: 14px;
                                font-weight: 600;
                                text-transform: uppercase;
                                letter-spacing: 0.5px;
                            ">
                                <md-icon style="font-size: 18px;">arrow_forward</md-icon>
                                ${change.field}
                            </div>
                            
                            <div style="display: grid; grid-template-columns: 1fr auto 1fr; gap: 12px; align-items: center;">
                                <div style="
                                    background: rgba(244, 67, 54, 0.1);
                                    border: 1px solid rgba(244, 67, 54, 0.3);
                                    border-radius: 8px;
                                    padding: 12px;
                                ">
                                    <div style="
                                        color: var(--md-sys-color-error);
                                        margin-bottom: 6px;
                                        display: flex;
                                        align-items: center;
                                        gap: 4px;
                                        font-size: 12px;
                                        font-weight: 600;
                                        text-transform: uppercase;
                                        letter-spacing: 0.5px;
                                    ">
                                        <md-icon style="font-size: 16px;">remove</md-icon>
                                        Anterior
                                    </div>
                                    <div style="color: var(--md-sys-color-on-surface); font-size: 14px; line-height: 20px;">
                                        ${change.oldValue || '<em style="opacity: 0.5;">Sin valor</em>'}
                                    </div>
                                </div>
                                
                                <md-icon style="color: var(--md-sys-color-primary); font-size: 24px;">arrow_forward</md-icon>
                                
                                <div style="
                                    background: rgba(76, 175, 80, 0.1);
                                    border: 1px solid rgba(76, 175, 80, 0.3);
                                    border-radius: 8px;
                                    padding: 12px;
                                ">
                                    <div style="
                                        color: #2e7d32;
                                        margin-bottom: 6px;
                                        display: flex;
                                        align-items: center;
                                        gap: 4px;
                                        font-size: 12px;
                                        font-weight: 600;
                                        text-transform: uppercase;
                                        letter-spacing: 0.5px;
                                    ">
                                        <md-icon style="font-size: 16px;">add</md-icon>
                                        Nuevo
                                    </div>
                                    <div style="color: var(--md-sys-color-on-surface); font-size: 14px; line-height: 20px;">
                                        ${change.newValue || '<em style="opacity: 0.5;">Sin valor</em>'}
                                    </div>
                                </div>
                            </div>
                        </div>
                    `).join('')}
                </div>
                
                <div style="
                    background: var(--md-sys-color-tertiary-container);
                    border-radius: 8px;
                    padding: 12px 16px;
                    margin-top: 20px;
                    display: flex;
                    align-items: center;
                    gap: 12px;
                ">
                    <md-icon style="color: var(--md-sys-color-on-tertiary-container);">help</md-icon>
                    <span style="color: var(--md-sys-color-on-tertiary-container); font-size: 13px; line-height: 18px;">
                        ¿Deseas continuar con esta modificación?
                    </span>
                </div>
            </div>
            <div slot="actions" style="padding: 0 24px 24px 24px; display: flex; gap: 8px; justify-content: flex-end;">
                <md-text-button class="cancel-btn">
                    <md-icon slot="icon">cancel</md-icon>
                    Cancelar
                </md-text-button>
                <md-filled-button class="confirm-btn">
                    <md-icon slot="icon">check</md-icon>
                    Guardar Cambios
                </md-filled-button>
            </div>
        `;

        document.body.appendChild(dialog);

        dialog.querySelector('.cancel-btn').addEventListener('click', () => {
            dialog.close();
            setTimeout(() => dialog.remove(), 300);
            resolve(false);
        });

        dialog.querySelector('.confirm-btn').addEventListener('click', () => {
            dialog.close();
            setTimeout(() => dialog.remove(), 300);
            resolve(true);
        });

        dialog.show();
    });
}

/**
 * Muestra un diálogo de confirmación para eliminar con información detallada
 * @param {string} entityName - Nombre de la entidad (ej: "Categoría")
 * @param {Object} entityData - Datos del elemento a eliminar
 * @returns {Promise<boolean>} - true si el usuario confirma, false si cancela
 */
async function showDeleteConfirmationDialog(entityName, entityData) {
    return new Promise((resolve) => {
        const dialog = document.createElement('md-dialog');
        
        // Preparar campos a mostrar (excluir campos técnicos)
        const displayFields = Object.entries(entityData)
            .filter(([key]) => !key.startsWith('_') && key !== 'Id')
            .map(([key, value]) => ({
                field: formatFieldName(key),
                value: formatValue(value)
            }));

        dialog.innerHTML = `
            <div slot="headline" style="font-size: 24px; font-weight: 500; padding: 24px 24px 0 24px;">
                <div style="display: flex; align-items: center; gap: 12px;">
                    <md-icon style="color: var(--md-sys-color-error); font-size: 28px;">delete_forever</md-icon>
                    <span>Confirmar Eliminación</span>
                </div>
            </div>
            <div slot="content" style="padding: 24px; min-width: 400px; max-width: 600px;">
                <div style="
                    background: var(--md-sys-color-error-container);
                    border-radius: 12px;
                    padding: 16px;
                    margin-bottom: 20px;
                    border-left: 4px solid var(--md-sys-color-error);
                ">
                    <div style="display: flex; align-items: center; gap: 12px; margin-bottom: 8px;">
                        <md-icon style="color: var(--md-sys-color-error); font-size: 24px;">warning</md-icon>
                        <span style="color: var(--md-sys-color-on-error-container); font-size: 16px; font-weight: 500;">
                            ¡Esta acción no se puede deshacer!
                        </span>
                    </div>
                    <p style="color: var(--md-sys-color-on-error-container); margin: 0; font-size: 14px; line-height: 20px;">
                        Estás a punto de eliminar este registro de ${entityName}.
                    </p>
                </div>

                <div style="
                    background: var(--md-sys-color-surface-variant);
                    border-radius: 12px;
                    padding: 16px;
                    margin-bottom: 20px;
                ">
                    <div style="margin-bottom: 16px; color: var(--md-sys-color-primary); font-size: 14px; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px;">
                        Información del Registro
                    </div>
                    ${displayFields.map(field => `
                        <div style="
                            display: flex;
                            justify-content: space-between;
                            padding: 12px 0;
                            border-bottom: 1px solid var(--md-sys-color-outline-variant);
                            gap: 16px;
                        ">
                            <span style="color: var(--md-sys-color-on-surface-variant); font-size: 14px; font-weight: 500; min-width: 120px;">
                                ${field.field}:
                            </span>
                            <span style="font-size: 14px; font-weight: 400; text-align: right; flex: 1;">
                                ${field.value || '<em style="opacity: 0.5;">Sin valor</em>'}
                            </span>
                        </div>
                    `).join('')}
                </div>

                <div style="
                    background: var(--md-sys-color-tertiary-container);
                    border-radius: 8px;
                    padding: 12px 16px;
                    display: flex;
                    align-items: center;
                    gap: 12px;
                ">
                    <md-icon style="color: var(--md-sys-color-on-tertiary-container);">info</md-icon>
                    <span style="color: var(--md-sys-color-on-tertiary-container); font-size: 13px; line-height: 18px;">
                        Verifica que sea el registro correcto antes de continuar.
                    </span>
                </div>
            </div>
            <div slot="actions" style="padding: 0 24px 24px 24px; display: flex; gap: 8px; justify-content: flex-end;">
                <md-text-button class="cancel-btn">
                    <md-icon slot="icon">cancel</md-icon>
                    Cancelar
                </md-text-button>
                <md-filled-button class="delete-btn" style="--md-filled-button-container-color: var(--md-sys-color-error);">
                    <md-icon slot="icon">delete</md-icon>
                    Eliminar
                </md-filled-button>
            </div>
        `;

        document.body.appendChild(dialog);

        dialog.querySelector('.cancel-btn').addEventListener('click', () => {
            dialog.close();
            setTimeout(() => dialog.remove(), 300);
            resolve(false);
        });

        dialog.querySelector('.delete-btn').addEventListener('click', () => {
            dialog.close();
            setTimeout(() => dialog.remove(), 300);
            resolve(true);
        });

        dialog.show();
    });
}

// Helpers
function formatFieldName(fieldName) {
    // Convertir CamelCase a palabras separadas
    return fieldName
        .replace(/([A-Z])/g, ' $1')
        .replace(/^./, str => str.toUpperCase())
        .replace(/Id$/, '')
        .trim();
}

function formatValue(value) {
    if (value === null || value === undefined || value === '') {
        return '';
    }
    
    if (typeof value === 'boolean') {
        return value ? 'Sí' : 'No';
    }
    
    if (value === 'true') return 'Sí';
    if (value === 'false') return 'No';
    
    // Truncar valores muy largos
    if (typeof value === 'string' && value.length > 100) {
        return value.substring(0, 97) + '...';
    }
    
    return String(value);
}

/**
 * Intercepta el submit del formulario de edición y muestra confirmación
 * @param {string} formSelector - Selector del formulario
 * @param {string} entityName - Nombre de la entidad
 */
function setupEditConfirmation(formSelector, entityName) {
    const form = document.querySelector(formSelector);
    if (!form) return;

    // Guardar datos originales al cargar
    const originalData = {};
    new FormData(form).forEach((value, key) => {
        originalData[key] = value;
    });

    form.addEventListener('submit', async function(e) {
        e.preventDefault();
        
        // Obtener datos actuales
        const currentData = {};
        new FormData(form).forEach((value, key) => {
            currentData[key] = value;
        });

        // Mostrar diálogo de confirmación
        const confirmed = await showEditConfirmationDialog(originalData, currentData, entityName);
        
        if (confirmed) {
            // Remover listener para evitar loop
            form.removeEventListener('submit', arguments.callee);
            // Sincronizar md-filled-text-field antes de enviar
            if (window.syncMdTextFields) syncMdTextFields(form);
            form.submit();
        }
    });
}

// Exportar funciones
window.showEditConfirmationDialog = showEditConfirmationDialog;
window.showDeleteConfirmationDialog = showDeleteConfirmationDialog;
window.setupEditConfirmation = setupEditConfirmation;

/**
 * Función simple de confirmación para compatibilidad con vistas existentes
 * @param {string} message - Mensaje a mostrar
 * @param {string} title - Título del diálogo (opcional)
 * @returns {Promise<boolean>} - true si confirma, false si cancela
 */
async function showConfirmDialog(message, title = "Confirmar") {
    return new Promise((resolve) => {
        const dialog = document.createElement('md-dialog');
        
        dialog.innerHTML = `
            <div slot="headline" style="font-size: 24px; font-weight: 500; padding: 24px 24px 16px 24px;">
                ${title}
            </div>
            <div slot="content" style="padding: 0 24px 24px 24px; min-width: 280px; max-width: 560px;">
                <p style="margin: 0; color: var(--md-sys-color-on-surface); font-size: 16px; line-height: 24px;">
                    ${message}
                </p>
            </div>
            <div slot="actions" style="padding: 0 24px 24px 24px; display: flex; gap: 8px; justify-content: flex-end;">
                <md-text-button class="cancel-btn">
                    Cancelar
                </md-text-button>
                <md-filled-button class="confirm-btn" style="--md-filled-button-container-color: var(--md-sys-color-error);">
                    Confirmar
                </md-filled-button>
            </div>
        `;
        
        document.body.appendChild(dialog);
        dialog.show();
        
        const confirmBtn = dialog.querySelector('.confirm-btn');
        const cancelBtn = dialog.querySelector('.cancel-btn');
        
        confirmBtn.addEventListener('click', () => {
            dialog.close();
            document.body.removeChild(dialog);
            resolve(true);
        });
        
        cancelBtn.addEventListener('click', () => {
            dialog.close();
            document.body.removeChild(dialog);
            resolve(false);
        });
        
        dialog.addEventListener('close', () => {
            if (dialog.parentElement) {
                document.body.removeChild(dialog);
            }
            resolve(false);
        });
    });
}

// Exportar también la función simple
window.showConfirmDialog = showConfirmDialog;
