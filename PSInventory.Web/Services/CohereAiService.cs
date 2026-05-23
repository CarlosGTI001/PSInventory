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
                    model = "command-r-plus",
                    messages = new[]
                    {
                        new { role = "user", content = $"Eres un experto en auditoría de infraestructura TI. Analiza los siguientes datos de inventario y genera un resumen profesional, estructurado y accionable de 3 a 5 párrafos en español con recomendaciones de actualización y posibles cuellos de botella:\n\n{resumenDatos}" }
                    },
                    stream = false
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                _httpClient.DefaultRequestHeaders.Add("accept", "application/json");

                // Endpoint V2 oficial según documentación
                var response = await _httpClient.PostAsync("https://api.cohere.com/v2/chat", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(responseBody);
                    // Estructura V2: message.content[0].text
                    return doc.RootElement.GetProperty("message").GetProperty("content")[0].GetProperty("text").GetString() ?? "No se pudo generar el análisis.";
                }
                
                var errorInfo = await response.Content.ReadAsStringAsync();
                return $"Nota: El servicio de IA (Cohere V2) respondió con un error {response.StatusCode}. Mostrando solo gráficos.";
            }
            catch (Exception ex)
            {
                return $"Error en análisis (Cohere): {ex.Message}";
            }
        }
    }
}
