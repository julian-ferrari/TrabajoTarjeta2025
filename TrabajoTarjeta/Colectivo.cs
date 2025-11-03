using System;

namespace TrabajoTarjeta
{
    public class Colectivo
    {
        private const decimal TARIFA_BASICA = 1580;

        // MODIFICADO: Cambiar de private a protected para que las clases hijas puedan acceder
        protected readonly string linea;

        public Colectivo(string linea)
        {
            this.linea = linea ?? throw new ArgumentNullException(nameof(linea));
        }

        public string ObtenerLinea()
        {
            return linea;
        }

        public decimal ObtenerTarifaBasica()
        {
            return TARIFA_BASICA;
        }

        // AGREGADO: Método auxiliar para verificar si es trasbordo
        protected bool EsTrasbordo(Tarjeta tarjeta)
        {
            // No hay último viaje
            if (!tarjeta.ObtenerFechaUltimoViaje().HasValue)
            {
                return false;
            }

            DateTime ahora = DateTime.Now;
            DateTime ultimoViaje = tarjeta.ObtenerFechaUltimoViaje().Value;

            // Verificar que sea línea diferente
            if (tarjeta.ObtenerLineaUltimoViaje() == linea)
            {
                return false;
            }

            // Verificar que hayan pasado menos de 1 hora
            TimeSpan diferencia = ahora - ultimoViaje;
            if (diferencia.TotalHours >= 1)
            {
                return false;
            }

            // Verificar día (L-S)
            if (ahora.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            // Verificar horario (7:00 a 22:00)
            if (ahora.Hour < 7 || ahora.Hour >= 22)
            {
                return false;
            }

            return true;
        }

        // ============================================================
        // MÉTODO MODIFICADO: Ahora soporta trasbordos
        // ============================================================

        /// <summary>
        /// Método original que lanza excepción si no se puede pagar.
        /// Ahora crea boletos con información completa y soporta trasbordos.
        /// </summary>
        public Boleto PagarCon(Tarjeta tarjeta)
        {
            if (tarjeta == null)
            {
                throw new ArgumentNullException(nameof(tarjeta));
            }

            // Guardar saldo antes del pago
            decimal saldoAntes = tarjeta.ObtenerSaldo();

            // Verificar si es trasbordo
            bool esTrasbordo = EsTrasbordo(tarjeta);

            // Si es trasbordo, la tarifa es 0
            decimal tarifa = esTrasbordo ? 0 : tarjeta.CalcularTarifa(TARIFA_BASICA);

            // Verificar si puede pagar
            if (!tarjeta.PuedeDescontar(tarifa))
            {
                throw new InvalidOperationException("La tarjeta no tiene saldo suficiente para pagar el pasaje.");
            }

            // Realizar el descuento
            tarjeta.Descontar(tarifa);

            // Obtener saldo después del pago
            decimal saldoDespues = tarjeta.ObtenerSaldo();

            // Calcular total abonado (diferencia de saldos)
            decimal totalAbonado = saldoAntes - saldoDespues;

            // Registrar el viaje para futuros trasbordos
            tarjeta.RegistrarViaje(linea);

            // Crear boleto con toda la información
            return new Boleto(
                fechaHora: DateTime.Now,
                tarifa: tarifa,
                saldoRestante: saldoDespues,
                linea: linea,
                tipoTarjeta: tarjeta.GetType().Name,
                totalAbonado: totalAbonado,
                idTarjeta: tarjeta.Id,
                esTrasbordo: esTrasbordo
            );
        }

        // ============================================================
        // MÉTODO MODIFICADO: Versión que retorna bool con trasbordos
        // ============================================================

        /// <summary>
        /// Intenta pagar con la tarjeta. Retorna false si no se puede en lugar de lanzar excepción.
        /// Ahora crea boletos con información completa y soporta trasbordos.
        /// </summary>
        public bool TryPagarCon(Tarjeta tarjeta, out Boleto boleto)
        {
            boleto = null;

            if (tarjeta == null)
            {
                return false;
            }

            decimal saldoAntes = tarjeta.ObtenerSaldo();
            bool esTrasbordo = EsTrasbordo(tarjeta);
            decimal tarifa = esTrasbordo ? 0 : tarjeta.CalcularTarifa(TARIFA_BASICA);

            if (!tarjeta.PuedeDescontar(tarifa))
            {
                return false;
            }

            tarjeta.Descontar(tarifa);
            decimal saldoDespues = tarjeta.ObtenerSaldo();

            // Calcular total abonado
            decimal totalAbonado = saldoAntes - saldoDespues;

            // Registrar el viaje
            tarjeta.RegistrarViaje(linea);

            // Crear boleto con información completa
            boleto = new Boleto(
                fechaHora: DateTime.Now,
                tarifa: tarifa,
                saldoRestante: saldoDespues,
                linea: linea,
                tipoTarjeta: tarjeta.GetType().Name,
                totalAbonado: totalAbonado,
                idTarjeta: tarjeta.Id,
                esTrasbordo: esTrasbordo
            );

            return true;
        }
    }
}