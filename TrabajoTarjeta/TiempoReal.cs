using System;

namespace TrabajoTarjeta
{
    /// <summary>
    /// Implementación que usa el tiempo real del sistema.
    /// </summary>
    public class TiempoReal : ITiempoProvider
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
