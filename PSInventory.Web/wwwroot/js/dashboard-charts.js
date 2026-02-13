// Dashboard Charts con Chart.js
// PSInventory - Gráficas interactivas del dashboard

// Configuración global de Chart.js
Chart.defaults.font.family = "'Roboto', sans-serif";
Chart.defaults.color = '#5f6368';

// Paleta de colores MD3
const MD3_COLORS = {
    primary: '#047394',
    primaryLight: '#5bc9e5',
    primaryDark: '#035a72',
    secondary: '#ff5c00',
    secondaryLight: '#ff8a3d',
    secondaryDark: '#cc4a00',
    success: '#10b981',
    warning: '#f59e0b',
    error: '#ef4444',
    surface: '#1e1e1e',
    surfaceVariant: '#2d2d2d'
};

// 1. Gráfica de Items por Estado (Pie Chart)
async function cargarItemsPorEstado() {
    try {
        const response = await fetch('/Home/GetItemsPorEstado');
        const data = await response.json();
        
        const ctx = document.getElementById('chartItemsPorEstado');
        if (!ctx) return;
        
        new Chart(ctx, {
            type: 'doughnut',
            data: data,
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            padding: 15,
                            font: {
                                size: 12
                            }
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                const label = context.label || '';
                                const value = context.parsed || 0;
                                const total = context.dataset.data.reduce((a, b) => a + b, 0);
                                const percentage = ((value / total) * 100).toFixed(1);
                                return `${label}: ${value} items (${percentage}%)`;
                            }
                        }
                    }
                }
            }
        });
    } catch (error) {
        console.error('Error al cargar Items por Estado:', error);
    }
}

// 2. Gráfica de Items por Categoría (Bar Chart)
async function cargarItemsPorCategoria() {
    try {
        const response = await fetch('/Home/GetItemsPorCategoria');
        const data = await response.json();
        
        const ctx = document.getElementById('chartItemsPorCategoria');
        if (!ctx) return;
        
        new Chart(ctx, {
            type: 'bar',
            data: data,
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                return `Items: ${context.parsed.y}`;
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 1
                        },
                        grid: {
                            color: 'rgba(255, 255, 255, 0.1)'
                        }
                    },
                    x: {
                        grid: {
                            display: false
                        }
                    }
                }
            }
        });
    } catch (error) {
        console.error('Error al cargar Items por Categoría:', error);
    }
}

// 3. Gráfica de Items por Sucursal (Horizontal Bar)
async function cargarItemsPorSucursal() {
    try {
        const response = await fetch('/Home/GetItemsPorSucursal');
        const data = await response.json();
        
        const ctx = document.getElementById('chartItemsPorSucursal');
        if (!ctx) return;
        
        new Chart(ctx, {
            type: 'bar',
            data: data,
            options: {
                indexAxis: 'y',
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                return `Items: ${context.parsed.x}`;
                            }
                        }
                    }
                },
                scales: {
                    x: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 1
                        },
                        grid: {
                            color: 'rgba(255, 255, 255, 0.1)'
                        }
                    },
                    y: {
                        grid: {
                            display: false
                        }
                    }
                }
            }
        });
    } catch (error) {
        console.error('Error al cargar Items por Sucursal:', error);
    }
}

// 4. Gráfica de Compras por Mes (Line Chart - Dual Axis)
async function cargarComprasPorMes() {
    try {
        const response = await fetch('/Home/GetComprasPorMes');
        const data = await response.json();
        
        const ctx = document.getElementById('chartComprasPorMes');
        if (!ctx) return;
        
        new Chart(ctx, {
            type: 'line',
            data: data,
            options: {
                responsive: true,
                maintainAspectRatio: true,
                interaction: {
                    mode: 'index',
                    intersect: false,
                },
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            padding: 15,
                            font: {
                                size: 12
                            }
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                let label = context.dataset.label || '';
                                if (label) {
                                    label += ': ';
                                }
                                if (context.parsed.y !== null) {
                                    if (context.datasetIndex === 1) {
                                        label += '$' + context.parsed.y.toFixed(2);
                                    } else {
                                        label += context.parsed.y;
                                    }
                                }
                                return label;
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        type: 'linear',
                        display: true,
                        position: 'left',
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Cantidad'
                        },
                        grid: {
                            color: 'rgba(255, 255, 255, 0.1)'
                        }
                    },
                    y1: {
                        type: 'linear',
                        display: true,
                        position: 'right',
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Monto ($)'
                        },
                        grid: {
                            drawOnChartArea: false,
                        }
                    },
                    x: {
                        grid: {
                            display: false
                        }
                    }
                }
            }
        });
    } catch (error) {
        console.error('Error al cargar Compras por Mes:', error);
    }
}

// Inicializar todas las gráficas cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', function() {
    // Verificar que Chart.js esté cargado
    if (typeof Chart === 'undefined') {
        console.error('Chart.js no está cargado');
        return;
    }
    
    // Cargar todas las gráficas
    cargarItemsPorEstado();
    cargarItemsPorCategoria();
    cargarItemsPorSucursal();
    cargarComprasPorMes();
});
