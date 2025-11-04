using System;

namespace TrabajoTarjeta
{
    public class BoletoGratuito : Tarjeta
    {
        private DateTime? fechaUltimosViajes;
        private int viajesGratuitosHoy;
        private const int MAX_VIAJES_GRATUITOS_POR_DIA = 2;
        private readonly ITiempoProvider tiempoProvider;

        // Constructor por defecto usa tiempo real
        public BoletoGratuito() : this(new TiempoReal())
        {
        }

        // Constructor que permite inyectar tiempo falso para tests
        public BoletoGratuito(ITiempoProvider tiempo)
        {
            this.tiempoProvider = tiempo;
        }

        public override decimal CalcularTarifa(decimal tarifaBase)
        {
            ActualizarContadorDiario();

            if (viajesGratuitosHoy >= MAX_VIAJES_GRATUITOS_POR_DIA)
            {
                return tarifaBase;
            }

            return 0;
        }

        public override bool PuedeDescontar(decimal monto)
        {
            if (!EsHorarioPermitido(tiempoProvider.Now()))
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

            fechaUltimosViajes = tiempoProvider.Now().Date;
        }

        private void ActualizarContadorDiario()
        {
            DateTime fechaActual = tiempoProvider.Now();

            if (!fechaUltimosViajes.HasValue || fechaActual.Date > fechaUltimosViajes.Value)
            {
                viajesGratuitosHoy = 0;
            }
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