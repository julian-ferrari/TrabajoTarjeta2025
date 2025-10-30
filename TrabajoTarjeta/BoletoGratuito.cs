using System;

namespace TrabajoTarjeta
{
    /// <summary>
    /// Tarjeta de boleto gratuito estudiantil.
    /// Permite a estudiantes viajar sin costo.
    /// 
    /// RESTRICCIONES DE USO:
    /// - Máximo 2 viajes gratuitos por día
    /// - Del tercer viaje en adelante paga tarifa completa ($1580)
    /// </summary>
    public class BoletoGratuito : Tarjeta
    {
        // ============================================================
        // NUEVOS ATRIBUTOS: Control de viajes gratuitos
        // ============================================================

        /// <summary>
        /// Fecha (solo día) de los últimos viajes.
        /// Se usa para resetear el contador cuando cambia el día.
        /// </summary>
        private DateTime? fechaUltimosViajes;

        /// <summary>
        /// Contador de viajes gratuitos realizados hoy.
        /// Se resetea cada día.
        /// </summary>
        private int viajesGratuitosHoy;

        // ============================================================
        // CONSTANTE: Límite de viajes gratuitos
        // ============================================================

        /// <summary>
        /// Cantidad máxima de viajes gratuitos permitidos por día.
        /// </summary>
        private const int MAX_VIAJES_GRATUITOS_POR_DIA = 2;

        // ============================================================
        // MÉTODO MODIFICADO: CalcularTarifa
        // Ahora considera el límite diario de viajes gratuitos
        // ============================================================

        /// <summary>
        /// Calcula la tarifa para boleto gratuito estudiantil.
        /// 
        /// LÓGICA:
        /// - Primeros 2 viajes del día: $0 (gratis)
        /// - Del tercer viaje en adelante: $1580 (tarifa completa)
        /// </summary>
        /// <param name="tarifaBase">Tarifa completa del boleto ($1580).</param>
        /// <returns>$0 para los primeros 2 viajes, $1580 del tercero en adelante.</returns>
        public override decimal CalcularTarifa(decimal tarifaBase)
        {
            // Actualizar contador si cambió el día
            ActualizarContadorDiario();

            // Si ya usó sus 2 viajes gratuitos hoy, cobra tarifa completa
            if (viajesGratuitosHoy >= MAX_VIAJES_GRATUITOS_POR_DIA)
            {
                return tarifaBase; // $1580 - Tarifa completa
            }

            // Todavía tiene viajes gratuitos disponibles
            return 0; // $0 - Gratis
        }

        // ============================================================
        // MÉTODO MODIFICADO: PuedeDescontar
        // Ahora diferencia entre viajes gratuitos y pagos
        // ============================================================

        /// <summary>
        /// Verifica si puede descontar el monto.
        /// 
        /// LÓGICA:
        /// - Si el viaje es gratis (monto = 0): Siempre puede viajar
        /// - Si debe pagar (monto > 0): Verifica que tenga saldo suficiente
        /// </summary>
        /// <param name="monto">Monto que se desea descontar.</param>
        /// <returns>True si puede viajar, False si debe pagar y no tiene saldo.</returns>
        public override bool PuedeDescontar(decimal monto)
        {
            // ============================================================
            // NUEVA LÓGICA: Si el viaje es gratis, siempre puede
            // ============================================================
            if (monto == 0)
            {
                return true; // Viaje gratuito, no requiere saldo
            }

            // ============================================================
            // Si debe pagar (3er viaje en adelante), valida el saldo
            // ============================================================
            return base.PuedeDescontar(monto);
        }

        // ============================================================
        // MÉTODO MODIFICADO: Descontar
        // Ahora actualiza el contador de viajes gratuitos
        // ============================================================

        /// <summary>
        /// Descuenta el monto del saldo y actualiza el contador de viajes gratuitos.
        /// 
        /// ACTUALIZA:
        /// - Contador de viajes gratuitos (si fue gratis)
        /// - Fecha de los últimos viajes (para reset diario)
        /// </summary>
        /// <param name="monto">Monto a descontar.</param>
        public override void Descontar(decimal monto)
        {
            // Realizar el descuento (lógica heredada)
            base.Descontar(monto);

            // Actualizar contador si cambió el día
            ActualizarContadorDiario();

            // ============================================================
            // NUEVO: Incrementar contador si fue viaje gratuito
            // Si monto == 0 significa que viajó gratis
            // ============================================================
            if (monto == 0)
            {
                viajesGratuitosHoy++;
            }

            // ============================================================
            // NUEVO: Guardar fecha del viaje
            // ============================================================
            fechaUltimosViajes = DateTime.Now.Date;
        }

        // ============================================================
        // NUEVO MÉTODO PRIVADO: ActualizarContadorDiario
        // Resetea el contador cuando cambia el día
        // ============================================================

        /// <summary>
        /// Resetea el contador de viajes gratuitos si cambió el día.
        /// Se llama antes de cada operación para asegurar que el contador esté actualizado.
        /// </summary>
        private void ActualizarContadorDiario()
        {
            DateTime fechaActual = DateTime.Now;

            // Si no hay fecha guardada, o si la fecha actual es posterior
            if (!fechaUltimosViajes.HasValue || fechaActual.Date > fechaUltimosViajes.Value)
            {
                viajesGratuitosHoy = 0; // Resetear contador para el nuevo día
            }
        }
    }
}