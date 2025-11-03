using System;

namespace TrabajoTarjeta
{
    /// <summary>
    /// Representa un colectivo interurbano con tarifa diferencial.
    /// Tarifa base: $3000.
    /// Admite todas las franquicias con descuentos proporcionales.
    /// Soporta trasbordos gratuitos.
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
        /// Soporta trasbordos gratuitos.
        /// </summary>
        public new Boleto PagarCon(Tarjeta tarjeta)
        {
            if (tarjeta == null)
            {
                throw new ArgumentNullException(nameof(tarjeta));
            }

            decimal saldoAntes = tarjeta.ObtenerSaldo();
            
            // Verificar si es trasbordo (heredado de Colectivo)
            bool esTrasbordo = EsTrasbordo(tarjeta);

            // Si es trasbordo, la tarifa es 0
            decimal tarifa = esTrasbordo ? 0 : tarjeta.CalcularTarifa(TARIFA_INTERURBANA);

            if (!tarjeta.PuedeDescontar(tarifa))
            {
                throw new InvalidOperationException("La tarjeta no tiene saldo suficiente para pagar el pasaje.");
            }

            tarjeta.Descontar(tarifa);
            decimal saldoDespues = tarjeta.ObtenerSaldo();
            decimal totalAbonado = saldoAntes - saldoDespues;

            // Registrar el viaje para futuros trasbordos
            tarjeta.RegistrarViaje(linea);

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

        /// <summary>
        /// Intenta pagar con la tarjeta. Retorna false si no se puede.
        /// Soporta trasbordos gratuitos.
        /// </summary>
        public new bool TryPagarCon(Tarjeta tarjeta, out Boleto boleto)
        {
            boleto = null;

            if (tarjeta == null)
            {
                return false;
            }

            decimal saldoAntes = tarjeta.ObtenerSaldo();
            
            // Verificar si es trasbordo (heredado de Colectivo)
            bool esTrasbordo = EsTrasbordo(tarjeta);

            decimal tarifa = esTrasbordo ? 0 : tarjeta.CalcularTarifa(TARIFA_INTERURBANA);

            if (!tarjeta.PuedeDescontar(tarifa))
            {
                return false;
            }

            tarjeta.Descontar(tarifa);
            decimal saldoDespues = tarjeta.ObtenerSaldo();
            decimal totalAbonado = saldoAntes - saldoDespues;

            // Registrar el viaje para futuros trasbordos
            tarjeta.RegistrarViaje(linea);

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