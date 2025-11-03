using System;

namespace TrabajoTarjeta
{
    public class MedioBoleto : Tarjeta
    {
        private DateTime? ultimoViaje;
        private DateTime? fechaUltimosViajes;
        private int viajesConDescuentoHoy;
        private const int MINUTOS_ENTRE_VIAJES = 5;
        private const int MAX_VIAJES_CON_DESCUENTO_POR_DIA = 2;

        public override decimal CalcularTarifa(decimal tarifaBase)
        {
            ActualizarContadorDiario();

            if (viajesConDescuentoHoy >= MAX_VIAJES_CON_DESCUENTO_POR_DIA)
            {
                return tarifaBase;
            }

            return tarifaBase / 2;
        }

        // ============================================================
        // MÉTODO MODIFICADO: Agregar validación horaria
        // ============================================================

        public override bool PuedeDescontar(decimal monto)
        {
            // ============================================================
            // NUEVA VALIDACIÓN: Horario permitido (L-V 6-22hs)
            // ============================================================
            if (!EsHorarioPermitido(DateTime.Now))
            {
                return false;
            }

            // Verificar tiempo mínimo entre viajes (5 minutos)
            if (ultimoViaje.HasValue)
            {
                TimeSpan tiempoTranscurrido = DateTime.Now - ultimoViaje.Value;

                if (tiempoTranscurrido.TotalMinutes < MINUTOS_ENTRE_VIAJES)
                {
                    return false;
                }
            }

            return base.PuedeDescontar(monto);
        }

        public override void Descontar(decimal monto)
        {
            base.Descontar(monto);
            ActualizarContadorDiario();

            if (monto < 1580)
            {
                viajesConDescuentoHoy++;
            }

            ultimoViaje = DateTime.Now;
            fechaUltimosViajes = DateTime.Now.Date;
        }

        private void ActualizarContadorDiario()
        {
            DateTime fechaActual = DateTime.Now;

            if (!fechaUltimosViajes.HasValue || fechaActual.Date > fechaUltimosViajes.Value)
            {
                viajesConDescuentoHoy = 0;
            }
        }

        // ============================================================
        // NUEVO MÉTODO: Validar horario y día
        // ============================================================

        /// <summary>
        /// Verifica si el horario y día son válidos para franquicias.
        /// Lunes a Viernes de 6:00 a 22:00.
        /// </summary>
        protected bool EsHorarioPermitido(DateTime fecha)
        {
            // Verificar día de la semana (L-V)
            if (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            // Verificar horario (6:00 a 22:00)
            int hora = fecha.Hour;
            if (hora < 6 || hora >= 22)
            {
                return false;
            }

            return true;
        }
    }
}