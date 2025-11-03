using System;

namespace TrabajoTarjeta
{
    public class BoletoGratuito : Tarjeta
    {
        private DateTime? fechaUltimosViajes;
        private int viajesGratuitosHoy;
        private const int MAX_VIAJES_GRATUITOS_POR_DIA = 2;

        public override decimal CalcularTarifa(decimal tarifaBase)
        {
            ActualizarContadorDiario();

            if (viajesGratuitosHoy >= MAX_VIAJES_GRATUITOS_POR_DIA)
            {
                return tarifaBase;
            }

            return 0;
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

            if (monto == 0)
            {
                return true;
            }

            return base.PuedeDescontar(monto);
        }

        public override void Descontar(decimal monto)
        {
            base.Descontar(monto);
            ActualizarContadorDiario();

            if (monto == 0)
            {
                viajesGratuitosHoy++;
            }

            fechaUltimosViajes = DateTime.Now.Date;
        }

        private void ActualizarContadorDiario()
        {
            DateTime fechaActual = DateTime.Now;

            if (!fechaUltimosViajes.HasValue || fechaActual.Date > fechaUltimosViajes.Value)
            {
                viajesGratuitosHoy = 0;
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