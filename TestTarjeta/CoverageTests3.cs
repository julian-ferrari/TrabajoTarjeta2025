using NUnit.Framework;
using System;
using TrabajoTarjeta;

namespace TestTarjeta
{
  
    [TestFixture]
    public class FinalCoverageBoostTests
    {
        #region Tests para cubrir líneas específicas de MedioBoleto

        [Test]
        public void TestMedioBoleto_PuedeDescontar_FueraDeHorario_RetornaFalse()
        {
            // Cubre: if (!EsHorarioPermitido(DateTime.Now)) { return false; }
            DateTime ahora = DateTime.Now;

            // Este test solo se ejecuta fuera de horario
            if (ahora.DayOfWeek != DayOfWeek.Saturday && ahora.DayOfWeek != DayOfWeek.Sunday &&
                ahora.Hour >= 6 && ahora.Hour < 22)
            {
                Assert.Ignore("Requiere estar FUERA del horario L-V 6-22");
            }

            MedioBoleto medio = new MedioBoleto();
            medio.Cargar(5000);

            // Fuera de horario, PuedeDescontar debe retornar false
            bool puede = medio.PuedeDescontar(790);
            Assert.IsFalse(puede, "Fuera de horario debe retornar false");
        }

        [Test]
        public void TestMedioBoleto_PuedeDescontar_ConViajePrevioReciente_RetornaFalse()
        {
            // Cubre: if (tiempoTranscurrido.TotalMinutes < MINUTOS_ENTRE_VIAJES) { return false; }
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Requiere L-V 6-22");
            }

            MedioBoleto medio = new MedioBoleto();
            medio.Cargar(5000);

            // Hacer un viaje
            medio.Descontar(790);

            // Inmediatamente intentar otro - debe retornar false
            bool puede = medio.PuedeDescontar(790);
            Assert.IsFalse(puede, "No debe poder descontar antes de 5 minutos");
        }

        [Test]
        public void TestMedioBoleto_PuedeDescontar_SinViajePrevio_RetornaBasePuedeDescontar()
        {
            // Cubre: return base.PuedeDescontar(monto);
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Requiere L-V 6-22");
            }

            MedioBoleto medio = new MedioBoleto();
            medio.Cargar(5000);

            // Sin viaje previo, debe llamar a base.PuedeDescontar
            bool puede = medio.PuedeDescontar(790);
            Assert.IsTrue(puede);
        }

        [Test]
        public void TestMedioBoleto_Descontar_LlamaBaseDescontar()
        {
            // Cubre: base.Descontar(monto);
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Requiere L-V 6-22");
            }

            MedioBoleto medio = new MedioBoleto();
            medio.Cargar(5000);

            // Descontar ejecuta base.Descontar
            medio.Descontar(790);
            Assert.AreEqual(4210, medio.ObtenerSaldo());
        }

        [Test]
        public void TestMedioBoleto_ActualizarContadorDiario_SinFechaPrevia()
        {
            // Cubre: if (!fechaUltimosViajes.HasValue || fechaActual.Date > fechaUltimosViajes.Value)
            MedioBoleto medio = new MedioBoleto();

            // Primera vez que se llama - no hay fecha previa
            decimal tarifa = medio.CalcularTarifa(1580);
            Assert.AreEqual(790, tarifa);
        }

        [Test]
        public void TestMedioBoleto_EsHorarioPermitido_Sabado_RetornaFalse()
        {
            // Cubre: if (fecha.DayOfWeek == DayOfWeek.Saturday) { return false; }
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek != DayOfWeek.Saturday)
            {
                Assert.Ignore("Solo ejecutable los sábados");
            }

            MedioBoleto medio = new MedioBoleto();
            medio.Cargar(5000);

            bool puede = medio.PuedeDescontar(790);
            Assert.IsFalse(puede, "Los sábados no está permitido");
        }

        [Test]
        public void TestMedioBoleto_EsHorarioPermitido_Domingo_RetornaFalse()
        {
            // Cubre: if (fecha.DayOfWeek == DayOfWeek.Sunday) { return false; }
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek != DayOfWeek.Sunday)
            {
                Assert.Ignore("Solo ejecutable los domingos");
            }

            MedioBoleto medio = new MedioBoleto();
            medio.Cargar(5000);

            bool puede = medio.PuedeDescontar(790);
            Assert.IsFalse(puede, "Los domingos no está permitido");
        }

        [Test]
        public void TestMedioBoleto_EsHorarioPermitido_Antes6AM_RetornaFalse()
        {
            // Cubre: if (hora < 6) { return false; }
            DateTime ahora = DateTime.Now;
            if (ahora.Hour >= 6 || ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Ignore("Solo ejecutable antes de las 6 AM en L-V");
            }

            MedioBoleto medio = new MedioBoleto();
            medio.Cargar(5000);

            bool puede = medio.PuedeDescontar(790);
            Assert.IsFalse(puede, "Antes de las 6 AM no está permitido");
        }

        [Test]
        public void TestMedioBoleto_EsHorarioPermitido_Despues22_RetornaFalse()
        {
            // Cubre: if (hora >= 22) { return false; }
            DateTime ahora = DateTime.Now;
            if (ahora.Hour < 22 || ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Ignore("Solo ejecutable después de las 22 en L-V");
            }

            MedioBoleto medio = new MedioBoleto();
            medio.Cargar(5000);

            bool puede = medio.PuedeDescontar(790);
            Assert.IsFalse(puede, "Después de las 22 no está permitido");
        }

        [Test]
        public void TestMedioBoleto_EsHorarioPermitido_HorarioValido_RetornaTrue()
        {
            // Cubre: return true; (al final de EsHorarioPermitido)
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Requiere L-V 6-22");
            }

            MedioBoleto medio = new MedioBoleto();
            medio.Cargar(5000);

            bool puede = medio.PuedeDescontar(790);
            Assert.IsTrue(puede, "En horario válido debe permitir");
        }

        #endregion

        #region Tests para cubrir líneas específicas de BoletoGratuito

        [Test]
        public void TestBoletoGratuito_CalcularTarifa_ConDosViajesGratuitos_RetornaTarifaCompleta()
        {
            // Cubre: if (viajesGratuitosHoy >= MAX_VIAJES_GRATUITOS_POR_DIA) { return tarifaBase; }
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Requiere L-V 6-22");
            }

            BoletoGratuito gratuito = new BoletoGratuito();
            Colectivo colectivo = new Colectivo("102");

            // Hacer 2 viajes gratuitos
            colectivo.PagarCon(gratuito);
            colectivo.PagarCon(gratuito);

            // Calcular tarifa debe retornar tarifaBase
            decimal tarifa = gratuito.CalcularTarifa(1580);
            Assert.AreEqual(1580, tarifa, "Después de 2 viajes gratis debe cobrar");
        }

        [Test]
        public void TestBoletoGratuito_PuedeDescontar_FueraDeHorario_RetornaFalse()
        {
            // Cubre: if (!EsHorarioPermitido(DateTime.Now)) { return false; }
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek != DayOfWeek.Saturday && ahora.DayOfWeek != DayOfWeek.Sunday &&
                ahora.Hour >= 6 && ahora.Hour < 22)
            {
                Assert.Ignore("Requiere estar FUERA del horario L-V 6-22");
            }

            BoletoGratuito gratuito = new BoletoGratuito();

            bool puede = gratuito.PuedeDescontar(0);
            Assert.IsFalse(puede, "Fuera de horario debe retornar false");
        }

        [Test]
        public void TestBoletoGratuito_PuedeDescontar_ConMontoCero_EnHorario_RetornaTrue()
        {
            // Cubre: if (monto == 0) { return true; }
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Requiere L-V 6-22");
            }

            BoletoGratuito gratuito = new BoletoGratuito();

            bool puede = gratuito.PuedeDescontar(0);
            Assert.IsTrue(puede, "Con monto 0 en horario válido debe retornar true");
        }

        [Test]
        public void TestBoletoGratuito_Descontar_ConMontoPositivo_LlamaBase()
        {
            // Cubre: base.Descontar(monto); ActualizarContadorDiario();
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Requiere L-V 6-22");
            }

            BoletoGratuito gratuito = new BoletoGratuito();
            gratuito.Cargar(5000);

            // Descontar monto > 0
            gratuito.Descontar(1580);
            Assert.AreEqual(3420, gratuito.ObtenerSaldo());
        }

        [Test]
        public void TestBoletoGratuito_Descontar_ConMontoCero_IncrementaContador()
        {
            // Cubre: if (monto == 0) { viajesGratuitosHoy++; }
            // Y también: fechaUltimosViajes = DateTime.Now.Date;
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Requiere L-V 6-22");
            }

            BoletoGratuito gratuito = new BoletoGratuito();

            // Descontar 0 dos veces
            gratuito.Descontar(0);
            gratuito.Descontar(0);

            // Verificar que el contador se incrementó
            decimal tarifa = gratuito.CalcularTarifa(1580);
            Assert.AreEqual(1580, tarifa, "Después de 2 viajes con monto 0, debe cobrar");
        }

        [Test]
        public void TestBoletoGratuito_ActualizarContadorDiario_SinFechaPrevia()
        {
            // Cubre: if (!fechaUltimosViajes.HasValue || fechaActual.Date > fechaUltimosViajes.Value)
            BoletoGratuito gratuito = new BoletoGratuito();

            decimal tarifa = gratuito.CalcularTarifa(1580);
            Assert.AreEqual(0, tarifa, "Primera vez debe ser gratis");
        }

        [Test]
        public void TestBoletoGratuito_EsHorarioPermitido_Sabado_RetornaFalse()
        {
            // Cubre: if (fecha.DayOfWeek == DayOfWeek.Saturday) { return false; }
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek != DayOfWeek.Saturday)
            {
                Assert.Ignore("Solo ejecutable los sábados");
            }

            BoletoGratuito gratuito = new BoletoGratuito();

            bool puede = gratuito.PuedeDescontar(0);
            Assert.IsFalse(puede);
        }

        [Test]
        public void TestBoletoGratuito_EsHorarioPermitido_Domingo_RetornaFalse()
        {
            // Cubre: if (fecha.DayOfWeek == DayOfWeek.Sunday) { return false; }
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek != DayOfWeek.Sunday)
            {
                Assert.Ignore("Solo ejecutable los domingos");
            }

            BoletoGratuito gratuito = new BoletoGratuito();

            bool puede = gratuito.PuedeDescontar(0);
            Assert.IsFalse(puede);
        }

        [Test]
        public void TestBoletoGratuito_EsHorarioPermitido_Antes6AM_RetornaFalse()
        {
            // Cubre: if (hora < 6) { return false; }
            DateTime ahora = DateTime.Now;
            if (ahora.Hour >= 6 || ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Ignore("Solo ejecutable antes de las 6 AM en L-V");
            }

            BoletoGratuito gratuito = new BoletoGratuito();

            bool puede = gratuito.PuedeDescontar(0);
            Assert.IsFalse(puede);
        }

        [Test]
        public void TestBoletoGratuito_EsHorarioPermitido_Despues22_RetornaFalse()
        {
            // Cubre: if (hora >= 22) { return false; }
            DateTime ahora = DateTime.Now;
            if (ahora.Hour < 22 || ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Ignore("Solo ejecutable después de las 22 en L-V");
            }

            BoletoGratuito gratuito = new BoletoGratuito();

            bool puede = gratuito.PuedeDescontar(0);
            Assert.IsFalse(puede);
        }

        [Test]
        public void TestBoletoGratuito_EsHorarioPermitido_HorarioValido_RetornaTrue()
        {
            // Cubre: return true; (al final de EsHorarioPermitido)
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Requiere L-V 6-22");
            }

            BoletoGratuito gratuito = new BoletoGratuito();

            bool puede = gratuito.PuedeDescontar(0);
            Assert.IsTrue(puede);
        }

        #endregion

        #region Tests para cubrir líneas específicas de FranquiciaCompleta

        [Test]
        public void TestFranquiciaCompleta_PuedeDescontar_FueraDeHorario_RetornaFalse()
        {
            // Cubre: if (!EsHorarioPermitido(DateTime.Now)) { return false; }
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek != DayOfWeek.Saturday && ahora.DayOfWeek != DayOfWeek.Sunday &&
                ahora.Hour >= 6 && ahora.Hour < 22)
            {
                Assert.Ignore("Requiere estar FUERA del horario L-V 6-22");
            }

            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            bool puede = franquicia.PuedeDescontar(0);
            Assert.IsFalse(puede, "Fuera de horario debe retornar false");
        }

        [Test]
        public void TestFranquiciaCompleta_PuedeDescontar_EnHorario_RetornaTrue()
        {
            // Cubre: return true;
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Requiere L-V 6-22");
            }

            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            bool puede = franquicia.PuedeDescontar(0);
            Assert.IsTrue(puede, "En horario válido debe retornar true");
        }

        [Test]
        public void TestFranquiciaCompleta_EsHorarioPermitido_Sabado_RetornaFalse()
        {
            // Cubre: if (fecha.DayOfWeek == DayOfWeek.Saturday) { return false; }
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek != DayOfWeek.Saturday)
            {
                Assert.Ignore("Solo ejecutable los sábados");
            }

            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            bool puede = franquicia.PuedeDescontar(0);
            Assert.IsFalse(puede);
        }

        [Test]
        public void TestFranquiciaCompleta_EsHorarioPermitido_Domingo_RetornaFalse()
        {
            // Cubre: if (fecha.DayOfWeek == DayOfWeek.Sunday) { return false; }
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek != DayOfWeek.Sunday)
            {
                Assert.Ignore("Solo ejecutable los domingos");
            }

            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            bool puede = franquicia.PuedeDescontar(0);
            Assert.IsFalse(puede);
        }

        [Test]
        public void TestFranquiciaCompleta_EsHorarioPermitido_Antes6AM_RetornaFalse()
        {
            // Cubre: if (hora < 6) { return false; }
            DateTime ahora = DateTime.Now;
            if (ahora.Hour >= 6 || ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Ignore("Solo ejecutable antes de las 6 AM en L-V");
            }

            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            bool puede = franquicia.PuedeDescontar(0);
            Assert.IsFalse(puede);
        }

        [Test]
        public void TestFranquiciaCompleta_EsHorarioPermitido_Despues22_RetornaFalse()
        {
            // Cubre: if (hora >= 22) { return false; }
            DateTime ahora = DateTime.Now;
            if (ahora.Hour < 22 || ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Ignore("Solo ejecutable después de las 22 en L-V");
            }

            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            bool puede = franquicia.PuedeDescontar(0);
            Assert.IsFalse(puede);
        }

        [Test]
        public void TestFranquiciaCompleta_EsHorarioPermitido_HorarioValido_RetornaTrue()
        {
            // Cubre: return true; (al final de EsHorarioPermitido)
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Requiere L-V 6-22");
            }

            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            bool puede = franquicia.PuedeDescontar(0);
            Assert.IsTrue(puede);
        }

        #endregion
    }
}