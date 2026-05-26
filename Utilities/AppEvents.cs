using System;

namespace STOCKPAP.Utilities
{
    /// <summary>
    /// Canal de eventos estático para comunicación entre vistas sin referencias directas.
    /// </summary>
    public static class AppEvents
    {
        /// <summary>Se dispara cuando se registra una venta exitosamente.</summary>
        public static event EventHandler VentaRealizada;

        /// <summary>Se dispara cuando se registra un movimiento de inventario manualmente.</summary>
        public static event EventHandler MovimientoRegistrado;

        public static void OnVentaRealizada()
            => VentaRealizada?.Invoke(null, EventArgs.Empty);

        public static void OnMovimientoRegistrado()
            => MovimientoRegistrado?.Invoke(null, EventArgs.Empty);
    }
}
