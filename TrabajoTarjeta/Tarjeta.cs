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
        // SALDO PENDIENTE
        // ============================================================

        protected decimal saldoPendiente;

        // ============================================================
        // CARGAS Y LÍMITES
        // ============================================================

        private readonly List<decimal> cargasAceptadas = new List<decimal>
        { 2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000 };

        private const decimal LIMITE_SALDO = 56000;
        private const decimal SALDO_NEGATIVO_MAXIMO = -1200;

        // ============================================================
        // USO FRECUENTE
        // ============================================================

        protected int viajesMesActual;
        protected DateTime? fechaUltimoViajeMensual;

        // ============================================================
        // TRASBORDOS
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
            saldoPendiente = 0;
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
        // CARGAR
        // ============================================================

        public virtual void Cargar(decimal monto)
        {
            if (!cargasAceptadas.Contains(monto))
            {
                throw new ArgumentException($"El monto {monto} no es una carga válida.");
            }

            if (saldo + monto > LIMITE_SALDO)
            {
                decimal espacioDisponible = LIMITE_SALDO - saldo;
                saldo = LIMITE_SALDO;
                decimal excedente = monto - espacioDisponible;
                saldoPendiente += excedente;
            }
            else
            {
                saldo += monto;
            }
        }

        // ============================================================
        // ACREDITAR CARGA PENDIENTE
        // ============================================================

        public void AcreditarCarga()
        {
            if (saldoPendiente <= 0)
            {
                return;
            }

            decimal espacioDisponible = LIMITE_SALDO - saldo;

            if (espacioDisponible <= 0)
            {
                return;
            }

            if (saldoPendiente <= espacioDisponible)
            {
                saldo += saldoPendiente;
                saldoPendiente = 0;
            }
            else
            {
                saldo += espacioDisponible;
                saldoPendiente -= espacioDisponible;
            }
        }

        // ============================================================
        // VALIDACIÓN Y DESCUENTO
        // ============================================================

        public virtual bool PuedeDescontar(decimal monto)
        {
            return (saldo - monto) >= SALDO_NEGATIVO_MAXIMO;
        }

        public virtual void Descontar(decimal monto)
        {
            if (!PuedeDescontar(monto))
            {
                throw new InvalidOperationException($"No se puede descontar ${monto}. El saldo quedaría por debajo del límite permitido de ${SALDO_NEGATIVO_MAXIMO}.");
            }

            saldo -= monto;

            // Actualizar contador mensual de viajes
            ActualizarContadorMensual();
            viajesMesActual++;
            fechaUltimoViajeMensual = DateTime.Now;

            // Acreditar saldo pendiente
            AcreditarCarga();
        }

        // ============================================================
        // CALCULAR TARIFA CON USO FRECUENTE
        // ============================================================

        public virtual decimal CalcularTarifa(decimal tarifaBase)
        {
            ActualizarContadorMensual();

            // CLAVE: Consideramos el viaje que ESTÁ POR REALIZARSE
            // Por eso usamos viajesMesActual + 1
            int viajeActual = viajesMesActual + 1;

            // Aplicar descuento por uso frecuente
            if (viajeActual >= 30 && viajeActual < 60)
            {
                return tarifaBase * 0.80m; // 20% descuento (viajes 30-59)
            }
            else if (viajeActual >= 60 && viajeActual <= 80)
            {
                return tarifaBase * 0.75m; // 25% descuento (viajes 60-80)
            }

            return tarifaBase; // Tarifa normal (viajes 1-29 y 81+)
        }

        // ============================================================
        // TRASBORDOS
        // ============================================================

        public void RegistrarViaje(string linea)
        {
            fechaUltimoViaje = DateTime.Now;
            lineaUltimoViaje = linea;
        }

        // ============================================================
        // ACTUALIZAR CONTADOR MENSUAL
        // ============================================================

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