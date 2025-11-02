using System;

namespace TrabajoTarjeta
{
    public class FranquiciaCompleta : Tarjeta
    {
        public override decimal CalcularTarifa(decimal tarifaBase)
        {
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

            return true;
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