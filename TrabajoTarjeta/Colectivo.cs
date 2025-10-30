using System;

namespace TrabajoTarjeta
{
    public class Colectivo
    {
        private const decimal TARIFA_BASICA = 1580;
        private readonly string linea;

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

        // ============================================================
        // MÉTODO MODIFICADO: Ahora crea boleto con más información
        // ============================================================

        /// <summary>
        /// Método original que lanza excepción si no se puede pagar.
        /// Ahora crea boletos con información completa.
        /// </summary>
        public Boleto PagarCon(Tarjeta tarjeta)
        {
            if (tarjeta == null)
            {
                throw new ArgumentNullException(nameof(tarjeta));
            }

            // Guardar saldo antes del pago
            decimal saldoAntes = tarjeta.ObtenerSaldo();

            // Calcular tarifa según tipo de tarjeta
            decimal tarifa = tarjeta.CalcularTarifa(TARIFA_BASICA);

            // Verificar si puede pagar
            if (!tarjeta.PuedeDescontar(tarifa))
            {
                throw new InvalidOperationException("La tarjeta no tiene saldo suficiente para pagar el pasaje.");
            }

            // Realizar el descuento
            tarjeta.Descontar(tarifa);

            // Obtener saldo después del pago
            decimal saldoDespues = tarjeta.ObtenerSaldo();

            // ============================================================
            // NUEVO: Calcular total abonado (diferencia de saldos)
            // Si había saldo negativo, el total abonado puede ser mayor
            // ============================================================
            decimal totalAbonado = saldoAntes - saldoDespues;

            // ============================================================
            // MODIFICADO: Crear boleto con toda la información
            // ============================================================
            return new Boleto(
                fechaHora: DateTime.Now,
                tarifa: tarifa,
                saldoRestante: saldoDespues,
                linea: linea,
                tipoTarjeta: tarjeta.GetType().Name,  // NUEVO: Obtiene el nombre de la clase
                totalAbonado: totalAbonado,            // NUEVO: Total abonado
                idTarjeta: tarjeta.Id                  // NUEVO: ID de la tarjeta
            );
        }

        // ============================================================
        // MÉTODO MODIFICADO: Versión que retorna bool
        // ============================================================

        /// <summary>
        /// Intenta pagar con la tarjeta. Retorna false si no se puede en lugar de lanzar excepción.
        /// Ahora crea boletos con información completa.
        /// </summary>
        public bool TryPagarCon(Tarjeta tarjeta, out Boleto boleto)
        {
            boleto = null;

            if (tarjeta == null)
            {
                return false;
            }

            decimal saldoAntes = tarjeta.ObtenerSaldo();
            decimal tarifa = tarjeta.CalcularTarifa(TARIFA_BASICA);

            if (!tarjeta.PuedeDescontar(tarifa))
            {
                return false;
            }

            tarjeta.Descontar(tarifa);
            decimal saldoDespues = tarjeta.ObtenerSaldo();

            // Calcular total abonado
            decimal totalAbonado = saldoAntes - saldoDespues;

            // Crear boleto con información completa
            boleto = new Boleto(
                fechaHora: DateTime.Now,
                tarifa: tarifa,
                saldoRestante: saldoDespues,
                linea: linea,
                tipoTarjeta: tarjeta.GetType().Name,
                totalAbonado: totalAbonado,
                idTarjeta: tarjeta.Id
            );

            return true;
        }
    }
}