using System;

namespace TrabajoTarjeta
{
    /// <summary>
    /// Interfaz para abstracción del tiempo. Permite usar tiempo real o falso en tests.
    /// </summary>
    public interface ITiempoProvider
    {
        DateTime Now();
    }
}
