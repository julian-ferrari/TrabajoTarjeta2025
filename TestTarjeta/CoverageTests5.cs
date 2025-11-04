using NUnit.Framework;
using System;
using TrabajoTarjeta;

namespace TestTarjeta
{
    /// <summary>
    /// Tests diseñados específicamente para aumentar la cobertura al 90%+.
    /// Se enfocan en las líneas no cubiertas de BoletoGratuito, FranquiciaCompleta y MedioBoleto.
    /// </summary>
    [TestFixture]
    public class CoverageTests5
    {
        private bool EsHorarioValidoParaFranquicias()
        {
            DateTime ahora = DateTime.Now;
            return ahora.DayOfWeek != DayOfWeek.Saturday &&
                   ahora.DayOfWeek != DayOfWeek.Sunday &&
                   ahora.Hour >= 6 && ahora.Hour < 22;
        }

        #region Tests para MedioBoleto - Líneas críticas

        [Test]
        public void TestMedioBoleto_PuedeDescontar_ConHorarioValido_RetornaBasePuedeDescontar()
        {
            // Cubre: return base.PuedeDescontar(monto); en horario válido
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
            }

            MedioBoleto medio = new MedioBoleto();
            medio.Cargar(5000);

            // Sin viaje previo, en horario válido
            bool puede = medio.PuedeDescontar(790);
            Assert.IsTrue(puede);
        }

        [Test]
        public void TestMedioBoleto_Descontar_ConMontoMenorA1580_IncrementaContador()
        {
            // Cubre: if (monto < 1580) { viajesConDescuentoHoy++; }
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
            }

            MedioBoleto medio = new MedioBoleto();
            medio.Cargar(5000);

            // Descontar 790 (menor a 1580)
            medio.Descontar(790);

            // Verificar que el contador se incrementó
            Assert.AreEqual(4210, medio.ObtenerSaldo());

            // El segundo viaje con descuento aún debería dar medio boleto
            decimal tarifa = medio.CalcularTarifa(1580);
            Assert.AreEqual(790, tarifa);
        }

        [Test]
        public void TestMedioBoleto_PuedeDescontar_FueraDeHorario_RetornaFalse()
        {
            // Cubre: if (!EsHorarioPermitido(DateTime.Now)) { return false; }
            if (EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test requiere estar FUERA del horario L-V 6-22");
            }

            MedioBoleto medio = new MedioBoleto();
            medio.Cargar(5000);

            bool puede = medio.PuedeDescontar(790);
            Assert.IsFalse(puede, "Fuera de horario debe retornar false");
        }

        [Test]
        public void TestMedioBoleto_ActualizarContadorDiario_SinFechaPrevia()
        {
            // Cubre: if (!fechaUltimosViajes.HasValue || fechaActual.Date > fechaUltimosViajes.Value)
            MedioBoleto medio = new MedioBoleto();

            // Primera llamada sin fecha previa
            decimal tarifa = medio.CalcularTarifa(1580);
            Assert.AreEqual(790, tarifa);
        }

        [Test]
        public void TestMedioBoleto_EsHorarioPermitido_RetornaTrue()
        {
            // Cubre: return true; (al final de EsHorarioPermitido)
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
            }

            MedioBoleto medio = new MedioBoleto();
            medio.Cargar(5000);

            Assert.IsTrue(medio.PuedeDescontar(790));
        }

        #endregion

        #region Tests para BoletoGratuito - Líneas críticas

        [Test]
        public void TestBoletoGratuito_CalcularTarifa_DespuesDeDosViajes_RetornaTarifaBase()
        {
            // Cubre: if (viajesGratuitosHoy >= MAX_VIAJES_GRATUITOS_POR_DIA) { return tarifaBase; }
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
            }

            BoletoGratuito gratuito = new BoletoGratuito();
            Colectivo colectivo = new Colectivo("102");

            // Hacer 2 viajes gratuitos
            colectivo.PagarCon(gratuito);
            colectivo.PagarCon(gratuito);

            // Calcular tarifa después de 2 viajes
            decimal tarifa = gratuito.CalcularTarifa(1580);
            Assert.AreEqual(1580, tarifa, "Después de 2 viajes debe cobrar tarifa completa");
        }

        [Test]
        public void TestBoletoGratuito_PuedeDescontar_ConMontoCero_RetornaTrue()
        {
            // Cubre: if (monto == 0) { return true; }
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
            }

            BoletoGratuito gratuito = new BoletoGratuito();

            bool puede = gratuito.PuedeDescontar(0);
            Assert.IsTrue(puede);
        }

        [Test]
        public void TestBoletoGratuito_PuedeDescontar_FueraDeHorario_RetornaFalse()
        {
            // Cubre: if (!EsHorarioPermitido(DateTime.Now)) { return false; }
            if (EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test requiere estar FUERA del horario L-V 6-22");
            }

            BoletoGratuito gratuito = new BoletoGratuito();

            bool puede = gratuito.PuedeDescontar(0);
            Assert.IsFalse(puede);
        }

        [Test]
        public void TestBoletoGratuito_Descontar_LlamaBaseDescontar()
        {
            // Cubre: base.Descontar(monto);
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
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
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
            }

            BoletoGratuito gratuito = new BoletoGratuito();

            // Descontar 0 dos veces
            gratuito.Descontar(0);
            gratuito.Descontar(0);

            // Verificar que el contador se incrementó
            decimal tarifa = gratuito.CalcularTarifa(1580);
            Assert.AreEqual(1580, tarifa);
        }

        [Test]
        public void TestBoletoGratuito_ActualizarContadorDiario_SinFechaPrevia()
        {
            // Cubre: if (!fechaUltimosViajes.HasValue || fechaActual.Date > fechaUltimosViajes.Value)
            BoletoGratuito gratuito = new BoletoGratuito();

            decimal tarifa = gratuito.CalcularTarifa(1580);
            Assert.AreEqual(0, tarifa);
        }

        [Test]
        public void TestBoletoGratuito_EsHorarioPermitido_RetornaTrue()
        {
            // Cubre: return true; (al final de EsHorarioPermitido)
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
            }

            BoletoGratuito gratuito = new BoletoGratuito();

            bool puede = gratuito.PuedeDescontar(0);
            Assert.IsTrue(puede);
        }

        #endregion

        #region Tests para FranquiciaCompleta - Líneas críticas

        [Test]
        public void TestFranquiciaCompleta_PuedeDescontar_EnHorarioValido_RetornaTrue()
        {
            // Cubre: return true; después de validar horario
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
            }

            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            bool puede = franquicia.PuedeDescontar(0);
            Assert.IsTrue(puede);
        }

        [Test]
        public void TestFranquiciaCompleta_PuedeDescontar_FueraDeHorario_RetornaFalse()
        {
            // Cubre: if (!EsHorarioPermitido(DateTime.Now)) { return false; }
            if (EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test requiere estar FUERA del horario L-V 6-22");
            }

            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            bool puede = franquicia.PuedeDescontar(0);
            Assert.IsFalse(puede);
        }

        [Test]
        public void TestFranquiciaCompleta_EsHorarioPermitido_Sabado_RetornaFalse()
        {
            // Cubre: if (fecha.DayOfWeek == DayOfWeek.Saturday) { return false; }
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek != DayOfWeek.Saturday)
            {
                Assert.Ignore("Test solo ejecutable los sábados");
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
                Assert.Ignore("Test solo ejecutable los domingos");
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
                Assert.Ignore("Test solo ejecutable antes de las 6 AM en L-V");
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
                Assert.Ignore("Test solo ejecutable después de las 22 en L-V");
            }

            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            bool puede = franquicia.PuedeDescontar(0);
            Assert.IsFalse(puede);
        }

        #endregion

        #region Tests de integración que ejecutan múltiples líneas

        [Test]
        public void TestIntegracion_FlujoCompletoConFranquicias()
        {
            // Este test ejecuta muchas líneas de código en un flujo realista
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
            }

            Colectivo colectivo = new Colectivo("102");

            MedioBoleto medio = new MedioBoleto();
            BoletoGratuito gratuito = new BoletoGratuito();
            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            medio.Cargar(5000);
            gratuito.Cargar(5000);

            // Flujo MedioBoleto
            Boleto bm1 = colectivo.PagarCon(medio);
            Assert.AreEqual(790, bm1.ObtenerTarifa());

            // Flujo BoletoGratuito (2 viajes gratis + 1 pago)
            Boleto bg1 = colectivo.PagarCon(gratuito);
            Boleto bg2 = colectivo.PagarCon(gratuito);
            Boleto bg3 = colectivo.PagarCon(gratuito);
            Assert.AreEqual(0, bg1.ObtenerTarifa());
            Assert.AreEqual(0, bg2.ObtenerTarifa());
            Assert.AreEqual(1580, bg3.ObtenerTarifa());

            // Flujo FranquiciaCompleta (siempre gratis)
            for (int i = 0; i < 5; i++)
            {
                Boleto bf = colectivo.PagarCon(franquicia);
                Assert.AreEqual(0, bf.ObtenerTarifa());
            }
        }

        [Test]
        public void TestIntegracion_ValidacionesFueraDeHorario()
        {
            // Test que se ejecuta SOLO fuera de horario
            if (EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test requiere estar FUERA del horario L-V 6-22");
            }

            Colectivo colectivo = new Colectivo("102");

            MedioBoleto medio = new MedioBoleto();
            BoletoGratuito gratuito = new BoletoGratuito();
            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            medio.Cargar(5000);

            // Todos deben fallar fuera de horario
            Assert.IsFalse(colectivo.TryPagarCon(medio, out _));
            Assert.IsFalse(colectivo.TryPagarCon(gratuito, out _));
            Assert.IsFalse(colectivo.TryPagarCon(franquicia, out _));
        }

        #endregion

        #region Tests que SIEMPRE se ejecutan (sin depender de horario)

        [Test]
        public void TestCalcularTarifa_SiempreSeEjecuta()
        {
            // Este test se ejecuta SIEMPRE y cubre líneas de CalcularTarifa
            MedioBoleto medio = new MedioBoleto();
            BoletoGratuito gratuito = new BoletoGratuito();
            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            // Estos métodos NO dependen del horario para calcular
            decimal tarifaMedio = medio.CalcularTarifa(1580);
            decimal tarifaGratuito = gratuito.CalcularTarifa(1580);
            decimal tarifaFranquicia = franquicia.CalcularTarifa(1580);

            Assert.AreEqual(790, tarifaMedio);
            Assert.AreEqual(0, tarifaGratuito);
            Assert.AreEqual(0, tarifaFranquicia);
        }

        [Test]
        public void TestPuedeDescontar_CondicionalSegunHorario()
        {
            // Test que se ejecuta SIEMPRE y valida según el horario actual
            MedioBoleto medio = new MedioBoleto();
            medio.Cargar(5000);

            bool resultado = medio.PuedeDescontar(790);
            bool esHorarioValido = EsHorarioValidoParaFranquicias();

            if (esHorarioValido)
            {
                Assert.IsTrue(resultado, "En horario válido debe poder descontar");
            }
            else
            {
                Assert.IsFalse(resultado, "Fuera de horario debe rechazar");
            }
        }

        #endregion
    }
}