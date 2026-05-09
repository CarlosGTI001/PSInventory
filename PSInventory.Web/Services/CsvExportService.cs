using System.Text;

namespace PSInventory.Web.Services
{
    public static class CsvExportService
    {
        public static byte[] BuildCsv(IEnumerable<string> headers, IEnumerable<IEnumerable<string?>> rows)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(",", headers.Select(Escape)));

            foreach (var row in rows)
            {
                sb.AppendLine(string.Join(",", row.Select(value => Escape(value ?? string.Empty))));
            }

            var preamble = Encoding.UTF8.GetPreamble();
            var content = Encoding.UTF8.GetBytes(sb.ToString());
            var output = new byte[preamble.Length + content.Length];
            Buffer.BlockCopy(preamble, 0, output, 0, preamble.Length);
            Buffer.BlockCopy(content, 0, output, preamble.Length, content.Length);
            return output;
        }

        private static string Escape(string value)
        {
            if (value.Contains('"'))
            {
                value = value.Replace("\"", "\"\"");
            }

            if (value.Contains(',') || value.Contains('\n') || value.Contains('\r') || value.Contains(';'))
            {
                return $"\"{value}\"";
            }

            return value;
        }
    }
}
