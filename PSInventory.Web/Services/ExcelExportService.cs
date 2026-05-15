using ClosedXML.Excel;

namespace PSInventory.Web.Services
{
    public static class ExcelExportService
    {
        public static byte[] BuildExcel(string sheetName, string[] headers, IEnumerable<string?[]> rows)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(sheetName);

            // Estilo del Header
            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#047394");
            headerRow.Style.Font.FontColor = XLColor.White;

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            // Datos
            int rowIndex = 2;
            foreach (var row in rows)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    worksheet.Cell(rowIndex, i + 1).Value = row[i] ?? string.Empty;
                }
                rowIndex++;
            }

            // Ajustar columnas
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
