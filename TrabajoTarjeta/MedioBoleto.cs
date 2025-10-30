using System;

namespace TrabajoTarjeta
{
    /// <summary>
    /// Tarjeta con beneficio de medio boleto estudiantil.
    /// Paga el 50% del valor del pasaje ($790).
    /// 
    /// RESTRICCIONES DE USO:
    /// - Mínimo 5 minutos entre viajes
    /// - Máximo 2 viajes con descuento por día
    /// - Del tercer viaje en adelante paga tarifa completa ($1580)
    /// </summary>
    public class MedioBoleto : Tarjeta
    {
        // ============================================================
        // NUEVOS ATRIBUTOS: Control de tiempo y viajes
        // ============================================================

        /// <summary>
        /// Fecha y hora del último viaje realizado.
        /// Se usa para verificar los 5 minutos mínimos entre viajes.
        /// </summary>
        private DateTime? ultimoViaje;

        /// <summary>
        /// Fecha (solo día) de los últimos viajes.
        /// Se usa para resetear el contador cuando cambia el día.
        /// </summary>
        private DateTime? fechaUltimosViajes;

        /// <summary>
        /// Contador de viajes con descuento realizados hoy.
        /// Se resetea cada día.
        /// </summary>
        private int viajesConDescuentoHoy;

        // ============================================================
        // CONSTANTES: Límites de uso
        // ============================================================

        /// <summary>
        /// Minutos mínimos que deben pasar entre un viaje y otro.
        /// </summary>
        private const int MINUTOS_ENTRE_VIAJES = 5;

        /// <summary>
        /// Cantidad máxima de viajes con descuento permitidos por día.
        /// </summary>
        private const int MAX_VIAJES_CON_DESCUENTO_POR_DIA = 2;

        // ============================================================
        // MÉTODO MODIFICADO: CalcularTarifa
        // Ahora considera el límite diario de viajes
        // ============================================================

        /// <summary>
        /// Calcula la tarifa aplicando el descuento de medio boleto.
        /// 
        /// LÓGICA:
        /// - Primeros 2 viajes del día: $790 (mitad de $1580)
        /// - Del tercer viaje en adelante: $1580 (tarifa completa)
        /// </summary>
        /// <param name="tarifaBase">Tarifa completa del boleto ($1580).</param>
        /// <returns>$790 o $1580 según cantidad de viajes realizados hoy.</returns>
        public override decimal CalcularTarifa(decimal tarifaBase)
        {
            // Actualizar contador si cambió el día
            ActualizarContadorDiario();

            // Si ya usó sus 2 viajes con descuento hoy, cobra tarifa completa
            if (viajesConDescuentoHoy >= MAX_VIAJES_CON_DESCUENTO_POR_DIA)
            {
                return tarifaBase; // $1580 - Tarifa completa
            }

            // Todavía tiene viajes con descuento disponibles
            return tarifaBase / 2; // $790 - Medio boleto
        }

        // ============================================================
        // MÉTODO MODIFICADO: PuedeDescontar
        // Ahora verifica la restricción de 5 minutos
        // ============================================================

        /// <summary>
        /// Verifica si puede descontar considerando:
        /// 1. Restricción de tiempo: Mínimo 5 minutos desde el último viaje
        /// 2. Restricción de saldo: Saldo suficiente (heredado de Tarjeta)
        /// </summary>
        /// <param name="monto">Monto que se desea descontar.</param>
        /// <returns>True si puede viajar, False si no pasaron 5 minutos o no hay saldo.</returns>
        public override bool PuedeDescontar(decimal monto)
        {
            // ============================================================
            // NUEVA VALIDACIÓN: Verificar tiempo mínimo entre viajes
            // ============================================================
            if (ultimoViaje.HasValue)
            {
                TimeSpan tiempoTranscurrido = DateTime.Now - ultimoViaje.Value;

                if (tiempoTranscurrido.TotalMinutes < MINUTOS_ENTRE_VIAJES)
                {
                    return false; // No pasaron 5 minutos todavía
                }
            }

            // Verificar saldo (lógica heredada de Tarjeta)
            return base.PuedeDescontar(monto);
        }

        // ============================================================
        // MÉTODO MODIFICADO: Descontar
        // Ahora actualiza los contadores de uso
        // ============================================================

        /// <summary>
        /// Descuenta el monto del saldo y actualiza los contadores de uso.
        /// 
        /// ACTUALIZA:
        /// - Contador de viajes con descuento (si aplicó descuento)
        /// - Fecha/hora del último viaje
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
            // NUEVO: Incrementar contador si pagó con descuento
            // Si monto < 1580 significa que pagó medio boleto ($790)
            // ============================================================
            if (monto < 1580)
            {
                viajesConDescuentoHoy++;
            }

            // ============================================================
            // NUEVO: Guardar fecha/hora del viaje
            // ============================================================
            ultimoViaje = DateTime.Now;
            fechaUltimosViajes = DateTime.Now.Date;
        }

        // ============================================================
        // NUEVO MÉTODO PRIVADO: ActualizarContadorDiario
        // Resetea el contador cuando cambia el día
        // ============================================================

        /// <summary>
        /// Resetea el contador de viajes con descuento si cambió el día.
        /// Se llama antes de cada operación para asegurar que el contador esté actualizado.
        /// </summary>
        private void ActualizarContadorDiario()
        {
            DateTime fechaActual = DateTime.Now;

            // Si no hay fecha guardada, o si la fecha actual es posterior
            if (!fechaUltimosViajes.HasValue || fechaActual.Date > fechaUltimosViajes.Value)
            {
                viajesConDescuentoHoy = 0; // Resetear contador para el nuevo día
            }
        }
    }
}