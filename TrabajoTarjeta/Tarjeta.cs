using System;
using System.Collections.Generic;
using System.Linq;

namespace TrabajoTarjeta
{
    /// <summary>
    /// Representa una tarjeta del sistema de transporte urbano de Rosario.
    /// Permite cargar saldo, descontar montos y gestionar saldo negativo (viajes plus).
    /// </summary>
    public class Tarjeta
    {
        // ============================================================
        // NUEVO: Contador e ID para identificar tarjetas
        // ============================================================

        /// <summary>
        /// Contador estático para generar IDs únicos.
        /// </summary>
        private static int contadorId = 1;

        /// <summary>
        /// Identificador único de la tarjeta.
        /// </summary>
        public int Id { get; }

        // ============================================================
        // CÓDIGO EXISTENTE
        // ============================================================

        /// <summary>
        /// Saldo actual de la tarjeta.
        /// Puede ser negativo hasta el límite establecido en SALDO_NEGATIVO_MAXIMO (-$1200).
        /// </summary>
        protected decimal saldo;

        /// <summary>
        /// Lista de montos válidos que pueden ser cargados a la tarjeta.
        /// Valores aceptados: $2000, $3000, $4000, $5000, $8000, $10000, $15000, $20000, $25000, $30000.
        /// </summary>
        private readonly List<decimal> cargasAceptadas = new List<decimal>
        { 2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000 };

        /// <summary>
        /// Límite máximo de saldo que puede tener una tarjeta.
        /// Valor: $40000.
        /// </summary>
        private const decimal LIMITE_SALDO = 40000;

        /// <summary>
        /// Límite de saldo negativo permitido (viajes plus).
        /// Una tarjeta puede quedar con hasta -$1200 de saldo.
        /// </summary>
        private const decimal SALDO_NEGATIVO_MAXIMO = -1200;

        /// <summary>
        /// Constructor de la tarjeta.
        /// Inicializa el saldo en $0 y asigna un ID único.
        /// </summary>
        public Tarjeta()
        {
            // ============================================================
            // MODIFICADO: Agregar asignación de ID
            // ============================================================
            Id = contadorId++;
            saldo = 0;
        }

        /// <summary>
        /// Obtiene el saldo actual de la tarjeta.
        /// </summary>
        /// <returns>El saldo actual, que puede ser positivo, cero o negativo (hasta -$1200).</returns>
        public virtual decimal ObtenerSaldo()
        {
            return saldo;
        }

        /// <summary>
        /// Carga saldo a la tarjeta.
        /// Solo acepta montos de la lista de cargas válidas.
        /// Si hay saldo negativo, la carga se aplica normalmente (no se descuenta automáticamente).
        /// </summary>
        /// <param name="monto">Monto a cargar. Debe estar en la lista de cargasAceptadas.</param>
        /// <exception cref="ArgumentException">Se lanza si el monto no es una carga válida.</exception>
        /// <exception cref="InvalidOperationException">Se lanza si la carga excede el límite de $40000.</exception>
        public virtual void Cargar(decimal monto)
        {
            if (!cargasAceptadas.Contains(monto))
            {
                throw new ArgumentException($"El monto {monto} no es una carga válida.");
            }
            if (saldo + monto > LIMITE_SALDO)
            {
                throw new InvalidOperationException($"La carga excede el límite de saldo de ${LIMITE_SALDO}.");
            }
            saldo += monto;
        }

        /// <summary>
        /// Verifica si es posible descontar un monto sin exceder el límite de saldo negativo.
        /// Evalúa si después del descuento el saldo quedaría por encima o igual a -$1200.
        /// </summary>
        /// <param name="monto">Monto que se desea descontar.</param>
        /// <returns>True si el descuento es posible, False si excedería el límite negativo.</returns>
        public virtual bool PuedeDescontar(decimal monto)
        {
            return (saldo - monto) >= SALDO_NEGATIVO_MAXIMO;
        }

        /// <summary>
        /// Descuenta un monto del saldo de la tarjeta.
        /// Permite que el saldo quede negativo hasta -$1200 (viajes plus).
        /// Este método se utiliza al pagar un boleto.
        /// </summary>
        /// <param name="monto">Monto a descontar del saldo.</param>
        /// <exception cref="InvalidOperationException">
        /// Se lanza si el descuento haría que el saldo quede por debajo de -$1200.
        /// </exception>
        public virtual void Descontar(decimal monto)
        {
            if (!PuedeDescontar(monto))
            {
                throw new InvalidOperationException($"No se puede descontar ${monto}. El saldo quedaría por debajo del límite permitido de ${SALDO_NEGATIVO_MAXIMO}.");
            }
            saldo -= monto;
        }

        /// <summary>
        /// Calcula la tarifa a pagar según el tipo de tarjeta.
        /// En la tarjeta base, devuelve la tarifa completa sin descuentos.
        /// Las clases heredadas pueden sobrescribir este método para aplicar descuentos (ej: medio boleto).
        /// </summary>
        /// <param name="tarifaBase">Tarifa base del boleto ($1580).</param>
        /// <returns>La tarifa que debe pagar esta tarjeta.</returns>
        public virtual decimal CalcularTarifa(decimal tarifaBase)
        {
            return tarifaBase;
        }

        /// <summary>
        /// Obtiene el límite de saldo negativo permitido.
        /// Útil para tests y validaciones.
        /// </summary>
        /// <returns>El límite de saldo negativo: -$1200.</returns>
        public decimal ObtenerLimiteSaldoNegativo()
        {
            return SALDO_NEGATIVO_MAXIMO;
        }
    }
}