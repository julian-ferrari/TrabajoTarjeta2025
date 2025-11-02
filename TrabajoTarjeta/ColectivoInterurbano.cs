using System;

namespace TrabajoTarjeta
{
    /// <summary>
    /// Representa un colectivo interurbano con tarifa diferencial.
    /// Tarifa base: $3000.
    /// Admite todas las franquicias con descuentos proporcionales.
    /// </summary>
    public class ColectivoInterurbano : Colectivo
    {
        private const decimal TARIFA_INTERURBANA = 3000;

        /// <summary>
        /// Constructor del colectivo interurbano.
        /// </summary>
        /// <param name="linea">Línea del colectivo interurbano</param>
        public ColectivoInterurbano(string linea) : base(linea)
        {
        }

        /// <summary>
        /// Obtiene la tarifa base interurbana.
        /// </summary>
        /// <returns>$3000</returns>
        public new decimal ObtenerTarifaBasica()
        {
            return TARIFA_INTERURBANA;
        }

        /// <summary>
        /// Procesa el pago de un boleto interurbano con una tarjeta.
        /// Usa la tarifa interurbana de $3000.
        /// AGREGADO: Palabra clave 'new' para ocultar intencionalmente el método de la clase base.
        /// </summary>
        public new Boleto PagarCon(Tarjeta tarjeta)
        {
            if (tarjeta == null)
            {
                throw new ArgumentNullException(nameof(tarjeta));
            }

            decimal saldoAntes = tarjeta.ObtenerSaldo();
            decimal tarifa = tarjeta.CalcularTarifa(TARIFA_INTERURBANA);

            if (!tarjeta.PuedeDescontar(tarifa))
            {
                throw new InvalidOperationException("La tarjeta no tiene saldo suficiente para pagar el pasaje interurbano.");
            }

            tarjeta.Descontar(tarifa);
            decimal saldoDespues = tarjeta.ObtenerSaldo();
            decimal totalAbonado = saldoAntes - saldoDespues;

            return new Boleto(
                fechaHora: DateTime.Now,
                tarifa: tarifa,
                saldoRestante: saldoDespues,
                linea: ObtenerLinea(),
                tipoTarjeta: tarjeta.GetType().Name,
                totalAbonado: totalAbonado,
                idTarjeta: tarjeta.Id
            );
        }

        /// <summary>
        /// Intenta pagar con la tarjeta. Retorna false si no se puede.
        /// AGREGADO: Palabra clave 'new' para ocultar intencionalmente el método de la clase base.
        /// </summary>
        public new bool TryPagarCon(Tarjeta tarjeta, out Boleto boleto)
        {
            boleto = null;

            if (tarjeta == null)
            {
                return false;
            }

            decimal saldoAntes = tarjeta.ObtenerSaldo();
            decimal tarifa = tarjeta.CalcularTarifa(TARIFA_INTERURBANA);

            if (!tarjeta.PuedeDescontar(tarifa))
            {
                return false;
            }

            tarjeta.Descontar(tarifa);
            decimal saldoDespues = tarjeta.ObtenerSaldo();
            decimal totalAbonado = saldoAntes - saldoDespues;

            boleto = new Boleto(
                fechaHora: DateTime.Now,
                tarifa: tarifa,
                saldoRestante: saldoDespues,
                linea: ObtenerLinea(),
                tipoTarjeta: tarjeta.GetType().Name,
                totalAbonado: totalAbonado,
                idTarjeta: tarjeta.Id
            );

            return true;
        }
    }
}