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
        // ATRIBUTOS IDENTIFICACIÓN Y SALDO
        // ============================================================

        private static int contadorId = 1;
        public int Id { get; }
        protected decimal saldo;

        // ============================================================
        // NUEVO: Saldo pendiente de acreditación
        // ============================================================

        /// <summary>
        /// Saldo pendiente de acreditación cuando la carga excede el límite.
        /// </summary>
        protected decimal saldoPendiente;

        // ============================================================
        // CARGAS Y LÍMITES
        // ============================================================

        private readonly List<decimal> cargasAceptadas = new List<decimal>
        { 2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000 };

        /// <summary>
        /// MODIFICADO: Límite máximo de saldo (antes 40000, ahora 56000).
        /// </summary>
        private const decimal LIMITE_SALDO = 56000;

        private const decimal SALDO_NEGATIVO_MAXIMO = -1200;

        // ============================================================
        // ATRIBUTOS USO FRECUENTE
        // ============================================================

        protected int viajesMesActual;
        protected DateTime? fechaUltimoViajeMensual;

        // ============================================================
        // ATRIBUTOS TRASBORDOS
        // ============================================================

        protected DateTime? fechaUltimoViaje;
        protected string lineaUltimoViaje;

        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public Tarjeta()
        {
            Id = contadorId++;
            saldo = 0;
            saldoPendiente = 0;  // NUEVO
            viajesMesActual = 0;
            fechaUltimoViajeMensual = null;
            fechaUltimoViaje = null;
            lineaUltimoViaje = null;
        }

        // ============================================================
        // MÉTODOS DE CONSULTA
        // ============================================================

        public virtual decimal ObtenerSaldo()
        {
            return saldo;
        }

        /// <summary>
        /// NUEVO: Obtiene el saldo pendiente de acreditación.
        /// </summary>
        public decimal ObtenerSaldoPendiente()
        {
            return saldoPendiente;
        }

        public decimal ObtenerLimiteSaldoNegativo()
        {
            return SALDO_NEGATIVO_MAXIMO;
        }

        public int ObtenerViajesMesActual()
        {
            ActualizarContadorMensual();
            return viajesMesActual;
        }

        public DateTime? ObtenerFechaUltimoViaje()
        {
            return fechaUltimoViaje;
        }

        public string ObtenerLineaUltimoViaje()
        {
            return lineaUltimoViaje;
        }

        // ============================================================
        // MÉTODO MODIFICADO: Cargar con saldo pendiente
        // ============================================================

        /// <summary>
        /// Carga saldo a la tarjeta.
        /// Si la carga excede el límite de $56000, el excedente queda pendiente.
        /// </summary>
        public virtual void Cargar(decimal monto)
        {
            if (!cargasAceptadas.Contains(monto))
            {
                throw new ArgumentException($"El monto {monto} no es una carga válida.");
            }

            // Si el saldo actual + monto supera el límite
            if (saldo + monto > LIMITE_SALDO)
            {
                // Calcular cuánto se puede acreditar
                decimal espacioDisponible = LIMITE_SALDO - saldo;

                // Acreditar hasta el límite
                saldo = LIMITE_SALDO;

                // El resto queda pendiente
                decimal excedente = monto - espacioDisponible;
                saldoPendiente += excedente;
            }
            else
            {
                // Carga normal sin exceder límite
                saldo += monto;
            }
        }

        // ============================================================
        // NUEVO MÉTODO: Acreditar carga pendiente
        // ============================================================

        /// <summary>
        /// Acredita saldo pendiente hasta alcanzar el límite de $56000.
        /// Se llama automáticamente después de cada viaje.
        /// </summary>
        public void AcreditarCarga()
        {
            if (saldoPendiente <= 0)
            {
                return; // No hay nada pendiente
            }

            // Calcular cuánto espacio hay disponible
            decimal espacioDisponible = LIMITE_SALDO - saldo;

            if (espacioDisponible <= 0)
            {
                return; // Tarjeta llena, no se puede acreditar
            }

            // Acreditar lo que se pueda
            if (saldoPendiente <= espacioDisponible)
            {
                // Se puede acreditar todo lo pendiente
                saldo += saldoPendiente;
                saldoPendiente = 0;
            }
            else
            {
                // Solo se puede acreditar una parte
                saldo += espacioDisponible;
                saldoPendiente -= espacioDisponible;
            }
        }

        // ============================================================
        // MÉTODOS DE VALIDACIÓN Y DESCUENTO
        // ============================================================

        public virtual bool PuedeDescontar(decimal monto)
        {
            return (saldo - monto) >= SALDO_NEGATIVO_MAXIMO;
        }

        /// <summary>
        /// MODIFICADO: Ahora acredita saldo pendiente después de descontar.
        /// </summary>
        public virtual void Descontar(decimal monto)
        {
            if (!PuedeDescontar(monto))
            {
                throw new InvalidOperationException($"No se puede descontar ${monto}. El saldo quedaría por debajo del límite permitido de ${SALDO_NEGATIVO_MAXIMO}.");
            }

            saldo -= monto;

            // NUEVO: Acreditar saldo pendiente después del viaje
            AcreditarCarga();

            // Actualizar contador mensual de viajes
            ActualizarContadorMensual();
            viajesMesActual++;
            fechaUltimoViajeMensual = DateTime.Now;
        }

        // ============================================================
        // CÁLCULO DE TARIFA CON USO FRECUENTE
        // ============================================================

        public virtual decimal CalcularTarifa(decimal tarifaBase)
        {
            ActualizarContadorMensual();

            // Aplicar descuento por uso frecuente (solo para tarjetas normales)
            if (viajesMesActual >= 30 && viajesMesActual < 60)
            {
                return tarifaBase * 0.80m; // 20% descuento
            }
            else if (viajesMesActual >= 60 && viajesMesActual <= 80)
            {
                return tarifaBase * 0.75m; // 25% descuento
            }

            return tarifaBase; // Tarifa normal
        }

        // ============================================================
        // MÉTODOS AUXILIARES
        // ============================================================

        public void RegistrarViaje(string linea)
        {
            fechaUltimoViaje = DateTime.Now;
            lineaUltimoViaje = linea;
        }

        protected void ActualizarContadorMensual()
        {
            DateTime ahora = DateTime.Now;

            if (!fechaUltimoViajeMensual.HasValue ||
                ahora.Month != fechaUltimoViajeMensual.Value.Month ||
                ahora.Year != fechaUltimoViajeMensual.Value.Year)
            {
                viajesMesActual = 0;
            }
        }
    }
}