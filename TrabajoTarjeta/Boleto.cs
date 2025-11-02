using System;

namespace TrabajoTarjeta
{
    /// <summary>
    /// Representa un boleto emitido al pagar un viaje en colectivo.
    /// Contiene información detallada sobre el viaje realizado.
    /// </summary>
    public class Boleto
    {
        // ============================================================
        // PROPIEDADES EXISTENTES
        // ============================================================

        private readonly DateTime fechaHora;
        private readonly decimal tarifa;
        private readonly decimal saldoRestante;
        private readonly string linea;

        // ============================================================
        // NUEVAS PROPIEDADES: Tipo de tarjeta, Total abonado, ID
        // ============================================================

        /// <summary>
        /// Tipo de tarjeta utilizada para el pago.
        /// Ejemplos: "Tarjeta", "MedioBoleto", "FranquiciaCompleta", "BoletoGratuito"
        /// </summary>
        private readonly string tipoTarjeta;

        /// <summary>
        /// Total abonado por el viaje.
        /// Puede ser mayor a la tarifa si había saldo negativo previo.
        /// </summary>
        private readonly decimal totalAbonado;

        /// <summary>
        /// ID único de la tarjeta utilizada para el pago.
        /// </summary>
        private readonly int idTarjeta;

        /// <summary>
        /// Indica si el boleto corresponde a un trasbordo.
        /// </summary>
        private readonly bool esTrasbordo;

        // ============================================================
        // CONSTRUCTOR MODIFICADO: Ahora incluye esTrasbordo
        // ============================================================

        /// <summary>
        /// Constructor completo del boleto con toda la información del viaje.
        /// </summary>
        /// <param name="fechaHora">Fecha y hora del viaje</param>
        /// <param name="tarifa">Tarifa del boleto según tipo de tarjeta</param>
        /// <param name="saldoRestante">Saldo que quedó en la tarjeta después del pago</param>
        /// <param name="linea">Línea de colectivo</param>
        /// <param name="tipoTarjeta">Tipo de tarjeta utilizada</param>
        /// <param name="totalAbonado">Monto total pagado (puede incluir deuda)</param>
        /// <param name="idTarjeta">ID de la tarjeta</param>
        /// <param name="esTrasbordo">Si es un trasbordo o no</param>
        public Boleto(DateTime fechaHora, decimal tarifa, decimal saldoRestante, string linea,
                      string tipoTarjeta, decimal totalAbonado, int idTarjeta, bool esTrasbordo = false)
        {
            this.fechaHora = fechaHora;
            this.tarifa = tarifa;
            this.saldoRestante = saldoRestante;
            this.linea = linea ?? throw new ArgumentNullException(nameof(linea));
            this.tipoTarjeta = tipoTarjeta ?? "Tarjeta";
            this.totalAbonado = totalAbonado;
            this.idTarjeta = idTarjeta;
            this.esTrasbordo = esTrasbordo;
        }

        // ============================================================
        // MÉTODOS EXISTENTES (sin cambios)
        // ============================================================

        public DateTime ObtenerFechaHora()
        {
            return fechaHora;
        }

        public decimal ObtenerTarifa()
        {
            return tarifa;
        }

        public decimal ObtenerSaldoRestante()
        {
            return saldoRestante;
        }

        public string ObtenerLinea()
        {
            return linea;
        }

        // ============================================================
        // NUEVOS MÉTODOS: Getters para las nuevas propiedades
        // ============================================================

        /// <summary>
        /// Obtiene el tipo de tarjeta utilizada.
        /// </summary>
        /// <returns>Nombre del tipo de tarjeta (ej: "MedioBoleto")</returns>
        public string ObtenerTipoTarjeta()
        {
            return tipoTarjeta;
        }

        /// <summary>
        /// Obtiene el total abonado.
        /// Puede ser mayor a la tarifa si había saldo negativo.
        /// </summary>
        /// <returns>Monto total pagado</returns>
        public decimal ObtenerTotalAbonado()
        {
            return totalAbonado;
        }

        /// <summary>
        /// Obtiene el ID de la tarjeta utilizada.
        /// </summary>
        /// <returns>ID único de la tarjeta</returns>
        public int ObtenerIdTarjeta()
        {
            return idTarjeta;
        }

        /// <summary>
        /// Obtiene si el boleto es un trasbordo.
        /// </summary>
        /// <returns>True si es trasbordo, False si es viaje normal</returns>
        public bool EsTrasbordo()
        {
            return esTrasbordo;
        }

        // ============================================================
        // ToString MODIFICADO: Incluye información de trasbordo
        // ============================================================

        public override string ToString()
        {
            string infoTrasbordo = esTrasbordo ? " [TRASBORDO]" : "";
            return $"Boleto{infoTrasbordo} - Línea: {linea}, Fecha: {fechaHora:dd/MM/yyyy HH:mm:ss}, " +
                   $"Tipo: {tipoTarjeta}, Tarifa: ${tarifa}, Total abonado: ${totalAbonado}, " +
                   $"Saldo restante: ${saldoRestante}, ID Tarjeta: {idTarjeta}";
        }
    }
}