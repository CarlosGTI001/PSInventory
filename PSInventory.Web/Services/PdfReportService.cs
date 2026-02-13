using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PSInventory.Web.Services
{
    public class PdfReportService
    {
        private const string EmpresaNombre = "PRESIDENTE SPORTS S.A.";
        private const string ColorPrimario = "#047394";
        private const string ColorSecundario = "#ff5c00";

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
            container.Row(row =>
            {
                // Logo placeholder (izquierda)
                row.ConstantItem(80).Column(column =>
                {
                    column.Item().AlignCenter().AlignMiddle().Height(50).Width(50)
                        .Border(2).BorderColor(ColorPrimario)
                        .Background(Colors.Grey.Lighten3)
                        .AlignCenter().AlignMiddle()
                        .Text("PS").FontSize(20).Bold().FontColor(ColorPrimario);
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
            container.PaddingTop(10).BorderBottom(2).BorderColor(ColorPrimario);
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
    }
}
