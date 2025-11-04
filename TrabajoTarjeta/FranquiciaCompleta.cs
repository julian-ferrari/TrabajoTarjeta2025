// FranquiciaCompleta.cs - MODIFICADO
using System;

namespace TrabajoTarjeta
{
    public class FranquiciaCompleta : Tarjeta
    {
        private readonly ITiempoProvider tiempoProvider;

        public FranquiciaCompleta() : this(new TiempoReal())
        {
        }

        public FranquiciaCompleta(ITiempoProvider tiempo)
        {
            this.tiempoProvider = tiempo;
        }

        public override decimal CalcularTarifa(decimal tarifaBase)
        {
            return 0;
        }

        public override bool PuedeDescontar(decimal monto)
        {
            if (!EsHorarioPermitido(tiempoProvider.Now()))
            {
                return false;
            }

            return true;
        }

        protected bool EsHorarioPermitido(DateTime fecha)
        {
            if (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            int hora = fecha.Hour;
            if (hora < 6 || hora >= 22)
            {
                return false;
            }

            return true;
        }
    }
}