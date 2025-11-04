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
        private readonly ITiempoProvider tiempoProvider;

        public MedioBoleto() : this(new TiempoReal())
        {
        }

        public MedioBoleto(ITiempoProvider tiempo)
        {
            this.tiempoProvider = tiempo;
        }

        public override decimal CalcularTarifa(decimal tarifaBase)
        {
            ActualizarContadorDiario();

            if (viajesConDescuentoHoy >= MAX_VIAJES_CON_DESCUENTO_POR_DIA)
            {
                return tarifaBase;
            }

            return tarifaBase / 2;
        }

        public override bool PuedeDescontar(decimal monto)
        {
            if (!EsHorarioPermitido(tiempoProvider.Now()))
            {
                return false;
            }

            if (ultimoViaje.HasValue)
            {
                TimeSpan tiempoTranscurrido = tiempoProvider.Now() - ultimoViaje.Value;

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

            ultimoViaje = tiempoProvider.Now();
            fechaUltimosViajes = tiempoProvider.Now().Date;
        }

        private void ActualizarContadorDiario()
        {
            DateTime fechaActual = tiempoProvider.Now();

            if (!fechaUltimosViajes.HasValue || fechaActual.Date > fechaUltimosViajes.Value)
            {
                viajesConDescuentoHoy = 0;
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
