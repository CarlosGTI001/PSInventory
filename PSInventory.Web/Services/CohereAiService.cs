using System.Text;
using System.Text.Json;

namespace PSInventory.Web.Services
{
    public class CohereAiService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public CohereAiService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<string> GenerarAnalisisInfraestructura(string resumenDatos)
        {
            var apiKey = _configuration["Cohere:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return "Resumen automático: El inventario cuenta con activos distribuidos en diversas zonas. Se recomienda mantener actualizados los campos de almacenamiento y RAM para mejorar la precisión de los reportes. (Configure Cohere API Key para un análisis profundo gratuito).";
            }

            try
            {
                var requestBody = new
                {
                    model = "command-r-plus", // Modelo potente disponible en Cohere
                    message = $"Eres un experto en auditoría de infraestructura TI. Analiza los siguientes datos y genera un resumen profesional de 3 a 5 párrafos en español con recomendaciones:\n\n{resumenDatos}",
                    preamble = "Eres un asistente especializado en análisis de inventario tecnológico y auditoría de sistemas."
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                _httpClient.DefaultRequestHeaders.Add("accept", "application/json");

                var response = await _httpClient.PostAsync("https://api.cohere.ai/v1/chat", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(responseBody);
                    return doc.RootElement.GetProperty("text").GetString() ?? "No se pudo generar el análisis.";
                }
                
                var errorInfo = await response.Content.ReadAsStringAsync();
                return $"Nota: El servicio de IA (Cohere) respondió con un error. Mostrando solo gráficos. Detalle: {response.StatusCode}";
            }
            catch (Exception ex)
            {
                return $"Error en análisis (Cohere): {ex.Message}";
            }
        }
    }
}
