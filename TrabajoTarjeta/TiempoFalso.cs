using System;

namespace TrabajoTarjeta
{
    /// <summary>
    /// Implementación de tiempo falso para testing.
    /// Permite controlar completamente la fecha y hora.
    /// </summary>
    public class TiempoFalso : ITiempoProvider
    {
        private DateTime tiempo;

        public TiempoFalso()
        {
            // Inicializar en un Lunes a las 10:00 (horario válido para franquicias)
            tiempo = new DateTime(2024, 10, 14, 10, 0, 0);
        }

        public TiempoFalso(DateTime fechaInicial)
        {
            tiempo = fechaInicial;
        }

        public DateTime Now()
        {
            return tiempo;
        }

        public void AgregarDias(int cantidad)
        {
            tiempo = tiempo.AddDays(cantidad);
        }

        public void AgregarHoras(int cantidad)
        {
            tiempo = tiempo.AddHours(cantidad);
        }

        public void AgregarMinutos(int cantidad)
        {
            tiempo = tiempo.AddMinutes(cantidad);
        }

        public void AgregarSegundos(int cantidad)
        {
            tiempo = tiempo.AddSeconds(cantidad);
        }

        public void EstablecerFecha(DateTime nuevaFecha)
        {
            tiempo = nuevaFecha;
        }

        public void EstablecerFecha(int año, int mes, int dia, int hora = 10, int minuto = 0, int segundo = 0)
        {
            tiempo = new DateTime(año, mes, dia, hora, minuto, segundo);
        }
    }
}