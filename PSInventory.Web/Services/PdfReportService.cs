using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PSInventory.Web.Models.ViewModels;

namespace PSInventory.Web.Services
{
    public class PdfReportService
    {
        private const string EmpresaNombre = "PRESIDENTE SPORTS";
        private const string ColorPrimario = "#047394";
        private const string ColorSecundario = "#ff5c00";

        // Ruta al logo (se asigna al iniciar la aplicación desde IWebHostEnvironment)
        public static string? LogoPath { get; set; }

        public static class ReportStyles
        {
            public static TextStyle HeaderTitle => TextStyle.Default
                .FontSize(18)
                .Bold()
                .FontColor(ColorPrimario);

            public static TextStyle SectionTitle => TextStyle.Default
                .FontSize(12)
                .Bold()
                .FontColor(Colors.Grey.Darken3);

            public static TextStyle TableHeader => TextStyle.Default
                .FontSize(10)
                .Bold()
                .FontColor(Colors.White);

            public static TextStyle TableCell => TextStyle.Default
                .FontSize(9)
                .FontColor(Colors.Grey.Darken3);

            public static TextStyle FooterText => TextStyle.Default
                .FontSize(8)
                .FontColor(Colors.Grey.Medium);

            public static TextStyle FilterText => TextStyle.Default
                .FontSize(9)
                .FontColor(Colors.Grey.Darken2);
        }

        public static void GenerarHeader(IContainer container, string tituloReporte, string usuario)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    // Logo (izquierda)
                    row.ConstantItem(80).Column(column =>
                    {
                        if (!string.IsNullOrEmpty(LogoPath) && File.Exists(LogoPath))
                        {
                            column.Item().AlignCenter().AlignMiddle()
                                .Height(50).Width(70)
                                .Image(LogoPath).FitArea();
                        }
                        else
                        {
                            column.Item().AlignCenter().AlignMiddle().Height(50).Width(50)
                                .Border(2).BorderColor(ColorPrimario)
                                .Background(Colors.Grey.Lighten3)
                                .AlignCenter().AlignMiddle()
                                .Text("PS").FontSize(20).Bold().FontColor(ColorPrimario);
                        }
                    });

                    // Información empresa (centro)
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().AlignCenter().Text(EmpresaNombre)
                            .FontSize(16).Bold().FontColor(ColorPrimario);
                        
                        column.Item().AlignCenter().Text(tituloReporte)
                            .FontSize(14).Bold().FontColor(Colors.Grey.Darken3);
                        
                        column.Item().AlignCenter().PaddingTop(5).Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                            .FontSize(9).FontColor(Colors.Grey.Medium);
                    });

                    // Usuario (derecha)
                    row.ConstantItem(100).Column(column =>
                    {
                        column.Item().AlignRight().Text($"Usuario: {usuario}")
                            .FontSize(9).FontColor(Colors.Grey.Darken2);
                        
                        column.Item().AlignRight().Text($"Fecha: {DateTime.Now:dd/MM/yyyy}")
                            .FontSize(9).FontColor(Colors.Grey.Darken2);
                        
                        column.Item().AlignRight().Text($"Hora: {DateTime.Now:HH:mm:ss}")
                            .FontSize(9).FontColor(Colors.Grey.Darken2);
                    });
                });

                // Línea separadora
                col.Item().PaddingTop(10).BorderBottom(2).BorderColor(ColorPrimario).Text("");
            });
        }

        public static void GenerarFiltros(IContainer container, Dictionary<string, string> filtros)
        {
            if (filtros == null || !filtros.Any()) return;

            container.Background(Colors.Grey.Lighten4).Padding(10).Column(column =>
            {
                column.Item().Text("Filtros Aplicados:").Style(ReportStyles.SectionTitle);
                
                foreach (var filtro in filtros)
                {
                    if (!string.IsNullOrEmpty(filtro.Value))
                    {
                        column.Item().PaddingLeft(10).Row(row =>
                        {
                            row.ConstantItem(100).Text($"• {filtro.Key}:").Style(ReportStyles.FilterText).Bold();
                            row.RelativeItem().Text(filtro.Value).Style(ReportStyles.FilterText);
                        });
                    }
                }
            });
        }

        public static void GenerarFooter(IContainer container, string usuario)
        {
            container.BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(5)
                .Row(row =>
                {
                    row.RelativeItem().AlignLeft()
                        .Text(x =>
                        {
                            x.Span("Página ").Style(ReportStyles.FooterText);
                            x.CurrentPageNumber().Style(ReportStyles.FooterText).Bold();
                            x.Span(" de ").Style(ReportStyles.FooterText);
                            x.TotalPages().Style(ReportStyles.FooterText).Bold();
                        });

                    row.RelativeItem().AlignCenter()
                        .Text($"Generado por: {usuario}").Style(ReportStyles.FooterText);

                    row.RelativeItem().AlignRight()
                        .Text($"{DateTime.Now:dd/MM/yyyy HH:mm}").Style(ReportStyles.FooterText);
                });
        }

        public static void GenerarTablaSimple(
            IContainer container, 
            List<string> headers, 
            List<List<string>> filas,
            List<int>? columnWidths = null)
        {
            container.Table(table =>
            {
                // Definir columnas
                var numColumns = headers.Count;
                
                table.ColumnsDefinition(columns =>
                {
                    if (columnWidths != null && columnWidths.Count == numColumns)
                    {
                        foreach (var width in columnWidths)
                        {
                            columns.ConstantColumn(width);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < numColumns; i++)
                        {
                            columns.RelativeColumn();
                        }
                    }
                });

                // Header
                table.Header(header =>
                {
                    foreach (var headerText in headers)
                    {
                        header.Cell().Background(ColorPrimario).Padding(5)
                            .Text(headerText).Style(ReportStyles.TableHeader);
                    }
                });

                // Filas
                int rowIndex = 0;
                foreach (var fila in filas)
                {
                    var bgColor = rowIndex % 2 == 0 ? Colors.White : Colors.Grey.Lighten5;
                    
                    foreach (var celda in fila)
                    {
                        table.Cell().Background(bgColor).BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2).Padding(5)
                            .Text(celda ?? "").Style(ReportStyles.TableCell);
                    }
                    
                    rowIndex++;
                }
            });
        }

        public static void GenerarResumenTotales(IContainer container, Dictionary<string, string> totales)
        {
            if (totales == null || !totales.Any()) return;

            container.Background(Colors.Grey.Lighten4).Padding(10).Column(column =>
            {
                column.Item().Text("RESUMEN TOTALES").Style(ReportStyles.SectionTitle);
                
                foreach (var total in totales)
                {
                    column.Item().PaddingTop(3).Row(row =>
                    {
                        row.RelativeItem().AlignRight()
                            .Text($"{total.Key}:").FontSize(10).Bold().FontColor(Colors.Grey.Darken3);
                        
                        row.ConstantItem(150).AlignRight()
                            .Text(total.Value).FontSize(11).Bold().FontColor(ColorPrimario);
                    });
                }
            });
        }

        public static byte[] GenerarPdfDinamico(
            string titulo, 
            string usuario, 
            Dictionary<string, string> filtros, 
            List<string> headers, 
            List<List<string>> filas, 
            bool horizontal = false,
            List<int>? customWidths = null)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(horizontal ? PageSizes.Letter.Landscape() : PageSizes.Letter);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Element(c => GenerarHeader(c, titulo, usuario));

                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        if (filtros != null && filtros.Any())
                        {
                            column.Item().PaddingBottom(10).Element(c => GenerarFiltros(c, filtros));
                        }

                        column.Item().Element(c => GenerarTablaSimple(c, headers, filas, customWidths));
                    });

                    page.Footer().Element(c => GenerarFooter(c, usuario));
                });
            });

            return document.GeneratePdf();
        }

        public static byte[] GenerarPdfInfraestructuraGrafica(
            string usuario,
            InfraReporteGraficoExportViewModel data)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Element(c => GenerarHeader(c, "Reporte Gráfico de Infraestructura", usuario));

                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        // Sección de Análisis AI
                        if (!string.IsNullOrWhiteSpace(data.AnalisisAi))
                        {
                            column.Item().PaddingBottom(20).Background(Colors.Grey.Lighten4).Padding(15).Column(aiCol =>
                            {
                                aiCol.Item().Row(row =>
                                {
                                    row.RelativeItem().Text(x =>
                                    {
                                        x.Span("✦ ").FontColor(ColorSecundario).FontSize(14).Bold();
                                        x.Span("Análisis y Resumen Inteligente").Style(ReportStyles.SectionTitle).FontColor(ColorPrimario);
                                    });
                                });
                                aiCol.Item().PaddingTop(10).Text(data.AnalisisAi).Style(ReportStyles.TableCell).FontSize(10).LineHeight(1.4f);
                            });
                        }

                        // Cuadrícula de Gráficos (2 por página o similar)
                        foreach (var grafico in data.Graficos)
                        {
                            column.Item().ShowEntire().PaddingBottom(30).Column(gCol =>
                            {
                                gCol.Item().PaddingBottom(10).BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                    .Text(grafico.Titulo).Style(ReportStyles.SectionTitle);

                                try
                                {
                                    var base64Data = grafico.Base64Image;
                                    if (base64Data.Contains(",")) base64Data = base64Data.Split(',')[1];
                                    var imageBytes = Convert.FromBase64String(base64Data);
                                    
                                    gCol.Item().PaddingTop(10).AlignCenter().Height(250).Image(imageBytes).FitArea();
                                }
                                catch
                                {
                                    gCol.Item().Padding(20).AlignCenter().Text("[Error al procesar imagen del gráfico]").FontColor(Colors.Red.Medium);
                                }
                            });
                        }
                    });

                    page.Footer().Element(c => GenerarFooter(c, usuario));
                });
            });

            return document.GeneratePdf();
        }

        public static byte[] GenerarPdfVacio(string titulo, string mensaje)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Element(c => GenerarHeader(c, titulo, "Sistema"));

                    page.Content().PaddingVertical(20).AlignCenter().AlignMiddle().Column(column =>
                    {
                        column.Item().AlignCenter().Text("⚠").FontSize(48).FontColor(Colors.Grey.Medium);
                        column.Item().AlignCenter().PaddingTop(10).Text(mensaje)
                            .FontSize(14).FontColor(Colors.Grey.Darken2);
                    });

                    page.Footer().Element(c => GenerarFooter(c, "Sistema"));
                });
            });

            return document.GeneratePdf();
        }

        public static byte[] GenerarPdfSucursalInfraestructura(string usuario, InfraSucursalResumenViewModel data)
        {
            if (data.Sucursal == null)
            {
                return GenerarPdfVacio("Reporte por Sucursal", "No se encontró información de la sucursal.");
            }

            var isLandscape = data.ViewLayout == "horizontal";
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(isLandscape ? PageSizes.Letter.Landscape() : PageSizes.Letter);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                    page.Header().Element(c => GenerarHeader(c, $"Reporte de Sucursal: {data.Sucursal.Id} - {data.Sucursal.Nombre}", usuario));

                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        // Info Sucursal
                        column.Item().PaddingBottom(12).Background(Colors.Grey.Lighten4).Padding(10).Column(infoCol =>
                        {
                            infoCol.Item().Text(x =>
                            {
                                x.Span("Sucursal: ").Bold();
                                x.Span($"{data.Sucursal.Id} - {data.Sucursal.Nombre}").Bold().FontColor(ColorPrimario);
                                x.Span($" | Zona: {data.Sucursal.Region}");
                                if (!string.IsNullOrEmpty(data.Sucursal.Direccion)) x.Span($" | Dirección: {data.Sucursal.Direccion}");
                                if (!string.IsNullOrEmpty(data.Sucursal.Telefono)) x.Span($" | Tel: {data.Sucursal.Telefono}");
                            });
                            if (data.DepartamentosRelacionados.Any())
                            {
                                infoCol.Item().PaddingTop(4).Text($"Departamentos: {string.Join(", ", data.DepartamentosRelacionados)}").FontSize(8).FontColor(Colors.Grey.Darken2);
                            }
                        });

                        // Resumen Estadísticas
                        column.Item().PaddingBottom(12).Row(r =>
                        {
                            r.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6).AlignCenter().Text(t => { t.Span("Equipos: ").Bold(); t.Span(data.TotalEquipos.ToString()); });
                            r.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6).AlignCenter().Text(t => { t.Span("Activos: ").Bold(); t.Span(data.EquiposActivos.ToString()); });
                            r.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6).AlignCenter().Text(t => { t.Span("Servicios: ").Bold(); t.Span(data.TotalServicios.ToString()); });
                            r.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6).AlignCenter().Text(t => { t.Span("Accesorios: ").Bold(); t.Span(data.TotalAccesorios.ToString()); });
                            r.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6).AlignCenter().Text(t => { t.Span("Artículos Inv.: ").Bold(); t.Span(data.TotalArticulos.ToString()); });
                        });

                        // Sección Equipos
                        if (data.Equipos.Any())
                        {
                            column.Item().PaddingBottom(15).Column(eqCol =>
                            {
                                eqCol.Item().PaddingBottom(5).Text("EQUIPOS DE CÓMPUTO").Style(ReportStyles.SectionTitle);
                                eqCol.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(cols =>
                                    {
                                        cols.ConstantColumn(60);
                                        cols.RelativeColumn(2);
                                        cols.ConstantColumn(80);
                                        cols.RelativeColumn(2);
                                        cols.RelativeColumn(1.5f);
                                        cols.ConstantColumn(50);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Background(ColorPrimario).Padding(4).Text("Código").Style(ReportStyles.TableHeader);
                                        header.Cell().Background(ColorPrimario).Padding(4).Text("Equipo").Style(ReportStyles.TableHeader);
                                        header.Cell().Background(ColorPrimario).Padding(4).Text("Serial").Style(ReportStyles.TableHeader);
                                        header.Cell().Background(ColorPrimario).Padding(4).Text("Especificaciones").Style(ReportStyles.TableHeader);
                                        header.Cell().Background(ColorPrimario).Padding(4).Text("Departamentos").Style(ReportStyles.TableHeader);
                                        header.Cell().Background(ColorPrimario).Padding(4).Text("Estado").Style(ReportStyles.TableHeader);
                                    });

                                    foreach (var eq in data.Equipos)
                                    {
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(eq.CodigoActivo ?? "—");
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(eq.NombreEquipo).Bold();
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(eq.Serial);
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text($"{eq.Marca} {eq.Modelo} | {eq.Procesador} | {eq.Ram} | {eq.Almacenamiento}");
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(string.IsNullOrEmpty(eq.Departamentos) ? "—" : eq.Departamentos);
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(eq.Activo ? "Activo" : "Inactivo");
                                    }
                                });
                            });
                        }

                        // Sección Artículos de Inventario
                        if (data.Articulos.Any())
                        {
                            column.Item().PaddingBottom(15).Column(artCol =>
                            {
                                artCol.Item().PaddingBottom(5).Text("ARTÍCULOS DE INVENTARIO ASIGNADOS").Style(ReportStyles.SectionTitle);
                                artCol.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(cols =>
                                    {
                                        cols.RelativeColumn(2);
                                        cols.RelativeColumn(1.5f);
                                        cols.ConstantColumn(80);
                                        cols.ConstantColumn(45);
                                        cols.ConstantColumn(60);
                                        cols.RelativeColumn(1.5f);
                                        cols.ConstantColumn(65);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Background(ColorPrimario).Padding(4).Text("Artículo (Marca/Modelo)").Style(ReportStyles.TableHeader);
                                        header.Cell().Background(ColorPrimario).Padding(4).Text("Categoría").Style(ReportStyles.TableHeader);
                                        header.Cell().Background(ColorPrimario).Padding(4).Text("Serial / ID").Style(ReportStyles.TableHeader);
                                        header.Cell().Background(ColorPrimario).Padding(4).Text("Cant.").Style(ReportStyles.TableHeader);
                                        header.Cell().Background(ColorPrimario).Padding(4).Text("Estado").Style(ReportStyles.TableHeader);
                                        header.Cell().Background(ColorPrimario).Padding(4).Text("Responsable").Style(ReportStyles.TableHeader);
                                        header.Cell().Background(ColorPrimario).Padding(4).Text("Fecha").Style(ReportStyles.TableHeader);
                                    });

                                    foreach (var art in data.Articulos)
                                    {
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text($"{art.Marca} {art.Modelo}").Bold();
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(art.Categoria);
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(art.Serial ?? $"ID: {art.ItemId}");
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(art.Cantidad.ToString());
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(art.Estado);
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(art.Responsable ?? "—");
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(art.FechaAsignacion?.ToString("dd/MM/yyyy") ?? "—");
                                    }
                                });
                            });
                        }

                        // Sección Servicios
                        if (data.Servicios.Any())
                        {
                            column.Item().PaddingBottom(15).Column(srvCol =>
                            {
                                srvCol.Item().PaddingBottom(5).Text("SERVICIOS Y CONECTIVIDAD").Style(ReportStyles.SectionTitle);
                                srvCol.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(cols =>
                                    {
                                        cols.RelativeColumn(2);
                                        cols.RelativeColumn(2);
                                        cols.RelativeColumn(2);
                                        cols.RelativeColumn(2);
                                        cols.ConstantColumn(50);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Background(ColorPrimario).Padding(4).Text("Tipo Servicio").Style(ReportStyles.TableHeader);
                                        header.Cell().Background(ColorPrimario).Padding(4).Text("Operador").Style(ReportStyles.TableHeader);
                                        header.Cell().Background(ColorPrimario).Padding(4).Text("Nº Servicio").Style(ReportStyles.TableHeader);
                                        header.Cell().Background(ColorPrimario).Padding(4).Text("Velocidad").Style(ReportStyles.TableHeader);
                                        header.Cell().Background(ColorPrimario).Padding(4).Text("Estado").Style(ReportStyles.TableHeader);
                                    });

                                    foreach (var srv in data.Servicios)
                                    {
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(srv.TipoServicio).Bold();
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(srv.Operador);
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(srv.NumeroServicio ?? "—");
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text($"{srv.VelocidadBajadaMbps?.ToString("0.##") ?? "0"} ↓ / {srv.VelocidadSubidaMbps?.ToString("0.##") ?? "0"} ↑ Mbps");
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(srv.Activo ? "Activo" : "Inactivo");
                                    }
                                });
                            });
                        }

                        // Sección Accesorios
                        if (data.Accesorios.Any())
                        {
                            column.Item().PaddingBottom(15).Column(accCol =>
                            {
                                accCol.Item().PaddingBottom(5).Text("ACCESORIOS Y OTROS").Style(ReportStyles.SectionTitle);
                                accCol.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(cols =>
                                    {
                                        cols.RelativeColumn(2);
                                        cols.ConstantColumn(50);
                                        cols.RelativeColumn(4);
                                        cols.ConstantColumn(50);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Background(ColorPrimario).Padding(4).Text("Tipo Accesorio").Style(ReportStyles.TableHeader);
                                        header.Cell().Background(ColorPrimario).Padding(4).Text("Cant.").Style(ReportStyles.TableHeader);
                                        header.Cell().Background(ColorPrimario).Padding(4).Text("Especificaciones").Style(ReportStyles.TableHeader);
                                        header.Cell().Background(ColorPrimario).Padding(4).Text("Estado").Style(ReportStyles.TableHeader);
                                    });

                                    foreach (var acc in data.Accesorios)
                                    {
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(acc.TipoAccesorio).Bold();
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(acc.Cantidad.ToString());
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(acc.Especificaciones ?? "—");
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(acc.Activo ? "Activo" : "Inactivo");
                                    }
                                });
                            });
                        }
                    });

                    page.Footer().Element(c => GenerarFooter(c, usuario));
                });
            });

            return document.GeneratePdf();
        }
    }
}
