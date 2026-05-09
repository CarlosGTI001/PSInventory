using System;

namespace PSInventory.Web.Services
{
    public static class MovimientoLogicoService
    {
        public const string MarcaEliminadoLogico = "[ANULADO_LOGICO]";

        public static bool EsEliminadoLogico(string? observaciones)
        {
            return !string.IsNullOrWhiteSpace(observaciones)
                && observaciones.Contains(MarcaEliminadoLogico, StringComparison.OrdinalIgnoreCase);
        }

        public static string ConstruirObservacionEliminado(string? observacionesActuales, string usuario, string motivoAnulacion)
        {
            var motivo = string.IsNullOrWhiteSpace(motivoAnulacion)
                ? "Sin motivo especificado"
                : motivoAnulacion.Trim();

            var sello = $"{MarcaEliminadoLogico} {DateTime.Now:dd/MM/yyyy HH:mm} por {usuario}. Motivo: {motivo}.";
            if (string.IsNullOrWhiteSpace(observacionesActuales))
            {
                return sello;
            }

            return $"{sello} Observación original: {observacionesActuales.Trim()}";
        }
    }
}
