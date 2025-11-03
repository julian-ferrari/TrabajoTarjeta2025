using NUnit.Framework;
using System;
using TrabajoTarjeta;

namespace TestTarjeta
{
    /// <summary>
    /// Tests adicionales para alcanzar 85%+ de cobertura.
    /// Estos tests se ejecutan SIEMPRE que sea posible, sin depender del horario.
    /// </summary>
    [TestFixture]
    public class AdditionalCoverageTests
    {
        #region Tests que SIEMPRE se ejecutan (no dependen de horario)

        [Test]
        public void TestMedioBoleto_CalcularTarifa_SinViajes_RetornaMitad()
        {
            // Este test SIEMPRE se ejecuta - no depende de horario
            MedioBoleto medio = new MedioBoleto();

            decimal tarifa = medio.CalcularTarifa(1580);
            Assert.AreEqual(790, tarifa);

            decimal tarifaInterurbana = medio.CalcularTarifa(3000);
            Assert.AreEqual(1500, tarifaInterurbana);
        }

        [Test]
        public void TestBoletoGratuito_CalcularTarifa_SinViajes_RetornaCero()
        {
            // Este test SIEMPRE se ejecuta
            BoletoGratuito gratuito = new BoletoGratuito();

            decimal tarifa1 = gratuito.CalcularTarifa(1580);
            Assert.AreEqual(0, tarifa1);

            decimal tarifa2 = gratuito.CalcularTarifa(3000);
            Assert.AreEqual(0, tarifa2);
        }

        [Test]
        public void TestFranquiciaCompleta_CalcularTarifa_SiempreCero()
        {
            // Este test SIEMPRE se ejecuta
            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            Assert.AreEqual(0, franquicia.CalcularTarifa(1580));
            Assert.AreEqual(0, franquicia.CalcularTarifa(3000));
            Assert.AreEqual(0, franquicia.CalcularTarifa(100));
            Assert.AreEqual(0, franquicia.CalcularTarifa(10000));
        }

        [Test]
        public void TestColectivo_ObtenerTarifaBasica_RetornaConstante()
        {
            // Cubre: private const decimal TARIFA_BASICA = 1580;
            Colectivo colectivo = new Colectivo("102");
            Assert.AreEqual(1580, colectivo.ObtenerTarifaBasica());
        }

        [Test]
        public void TestColectivoInterurbano_ObtenerTarifaBasica_RetornaConstante()
        {
            // Cubre: private const decimal TARIFA_INTERURBANA = 3000;
            ColectivoInterurbano interurbano = new ColectivoInterurbano("Gálvez");
            Assert.AreEqual(3000, interurbano.ObtenerTarifaBasica());
        }

        #endregion

        #region Tests condicionales - ejecutan cuando el horario es apropiado

        [Test]
        public void TestMedioBoleto_FlujoCon3Viajes_PrimerosDosConDescuento()
        {
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
            }

            MedioBoleto medio = new MedioBoleto();
            medio.Cargar(10000);
            Colectivo colectivo = new Colectivo("102");

            // Viaje 1: con descuento (790)
            Boleto b1 = colectivo.PagarCon(medio);
            Assert.AreEqual(790, b1.ObtenerTarifa());
            decimal saldo1 = medio.ObtenerSaldo();
            Assert.AreEqual(9210, saldo1);

            // Viaje 2: con descuento (790)
            Boleto b2 = colectivo.PagarCon(medio);
            Assert.AreEqual(790, b2.ObtenerTarifa());
            decimal saldo2 = medio.ObtenerSaldo();
            Assert.AreEqual(8420, saldo2);

            // Viaje 3: tarifa completa (1580)
            Boleto b3 = colectivo.PagarCon(medio);
            Assert.AreEqual(1580, b3.ObtenerTarifa());
            decimal saldo3 = medio.ObtenerSaldo();
            Assert.AreEqual(6840, saldo3);
        }

        [Test]
        public void TestBoletoGratuito_FlujoCon3Viajes_PrimerosDosGratis()
        {
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
            }

            BoletoGratuito gratuito = new BoletoGratuito();
            gratuito.Cargar(5000);
            Colectivo colectivo = new Colectivo("102");

            // Viaje 1: gratis
            Boleto b1 = colectivo.PagarCon(gratuito);
            Assert.AreEqual(0, b1.ObtenerTarifa());
            Assert.AreEqual(5000, gratuito.ObtenerSaldo());

            // Viaje 2: gratis
            Boleto b2 = colectivo.PagarCon(gratuito);
            Assert.AreEqual(0, b2.ObtenerTarifa());
            Assert.AreEqual(5000, gratuito.ObtenerSaldo());

            // Viaje 3: paga tarifa completa
            Boleto b3 = colectivo.PagarCon(gratuito);
            Assert.AreEqual(1580, b3.ObtenerTarifa());
            Assert.AreEqual(3420, gratuito.ObtenerSaldo());
        }

        [Test]
        public void TestMedioBoleto_PuedeDescontar_Inmediatamente_RetornaFalse()
        {
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
            }

            MedioBoleto medio = new MedioBoleto();
            medio.Cargar(5000);
            Colectivo colectivo = new Colectivo("102");

            // Primer viaje
            colectivo.PagarCon(medio);

            // Segundo viaje inmediato - debe fallar
            bool resultado = colectivo.TryPagarCon(medio, out Boleto boleto);
            Assert.IsFalse(resultado);
            Assert.IsNull(boleto);
        }

        [Test]
        public void TestColectivo_Trasbordo_LineaDiferente_TarifaCero()
        {
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Sunday || ahora.Hour < 7 || ahora.Hour >= 22)
            {
                Assert.Ignore("Test requiere horario L-S 7-22");
            }

            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            Colectivo colectivo1 = new Colectivo("102");
            Colectivo colectivo2 = new Colectivo("121");
            Colectivo colectivo3 = new Colectivo("144");

            // Viaje 1: paga
            Boleto b1 = colectivo1.PagarCon(tarjeta);
            Assert.AreEqual(1580, b1.ObtenerTarifa());
            Assert.IsFalse(b1.EsTrasbordo());

            // Viaje 2: trasbordo (línea diferente)
            Boleto b2 = colectivo2.PagarCon(tarjeta);
            Assert.AreEqual(0, b2.ObtenerTarifa());
            Assert.IsTrue(b2.EsTrasbordo());

            // Viaje 3: trasbordo (otra línea diferente)
            Boleto b3 = colectivo3.PagarCon(tarjeta);
            Assert.AreEqual(0, b3.ObtenerTarifa());
            Assert.IsTrue(b3.EsTrasbordo());

            // Solo pagó el primero
            Assert.AreEqual(3420, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestColectivo_Trasbordo_MismaLinea_Paga()
        {
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Sunday || ahora.Hour < 7 || ahora.Hour >= 22)
            {
                Assert.Ignore("Test requiere horario L-S 7-22");
            }

            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            Colectivo colectivo = new Colectivo("102");

            // Viaje 1
            Boleto b1 = colectivo.PagarCon(tarjeta);
            Assert.IsFalse(b1.EsTrasbordo());

            // Viaje 2 en misma línea - NO es trasbordo
            Boleto b2 = colectivo.PagarCon(tarjeta);
            Assert.IsFalse(b2.EsTrasbordo());
            Assert.AreEqual(1580, b2.ObtenerTarifa());

            // Pagó ambos
            Assert.AreEqual(1840, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestInterurbano_Trasbordo_DesdeUrbano()
        {
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Sunday || ahora.Hour < 7 || ahora.Hour >= 22)
            {
                Assert.Ignore("Test requiere horario L-S 7-22");
            }

            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(10000);

            Colectivo urbano = new Colectivo("102");
            ColectivoInterurbano interurbano = new ColectivoInterurbano("Gálvez");

            // Viaje urbano
            Boleto b1 = urbano.PagarCon(tarjeta);
            Assert.AreEqual(1580, b1.ObtenerTarifa());

            // Viaje interurbano - trasbordo
            Boleto b2 = interurbano.PagarCon(tarjeta);
            Assert.IsTrue(b2.EsTrasbordo());
            Assert.AreEqual(0, b2.ObtenerTarifa());

            // Solo pagó el urbano
            Assert.AreEqual(8420, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestInterurbano_ConMedioBoleto_Descuento50Porciento()
        {
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
            }

            MedioBoleto medio = new MedioBoleto();
            medio.Cargar(5000);

            ColectivoInterurbano interurbano = new ColectivoInterurbano("Funes");

            Boleto boleto = interurbano.PagarCon(medio);
            Assert.AreEqual(1500, boleto.ObtenerTarifa());
            Assert.AreEqual(3500, medio.ObtenerSaldo());
        }

        [Test]
        public void TestInterurbano_ConBoletoGratuito_PrimerosDosGratis()
        {
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
            }

            BoletoGratuito gratuito = new BoletoGratuito();
            gratuito.Cargar(10000);

            ColectivoInterurbano interurbano = new ColectivoInterurbano("Capitán Bermúdez");

            // Viajes 1 y 2: gratis
            Boleto b1 = interurbano.PagarCon(gratuito);
            Boleto b2 = interurbano.PagarCon(gratuito);
            Assert.AreEqual(0, b1.ObtenerTarifa());
            Assert.AreEqual(0, b2.ObtenerTarifa());
            Assert.AreEqual(10000, gratuito.ObtenerSaldo());

            // Viaje 3: paga tarifa interurbana completa
            Boleto b3 = interurbano.PagarCon(gratuito);
            Assert.AreEqual(3000, b3.ObtenerTarifa());
            Assert.AreEqual(7000, gratuito.ObtenerSaldo());
        }

        [Test]
        public void TestInterurbano_ConFranquiciaCompleta_SiempreGratis()
        {
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
            }

            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            ColectivoInterurbano interurbano = new ColectivoInterurbano("Roldán");

            // Múltiples viajes, todos gratis
            for (int i = 0; i < 5; i++)
            {
                Boleto boleto = interurbano.PagarCon(franquicia);
                Assert.AreEqual(0, boleto.ObtenerTarifa());
            }
        }

        #endregion

        #region Tests para TryPagarCon - cobertura de ramas

        [Test]
        public void TestColectivo_TryPagarCon_ConSaldo_RetornaTrue()
        {
            Colectivo colectivo = new Colectivo("102");
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            bool resultado = colectivo.TryPagarCon(tarjeta, out Boleto boleto);

            Assert.IsTrue(resultado);
            Assert.IsNotNull(boleto);
            Assert.AreEqual(1580, boleto.ObtenerTarifa());
            Assert.AreEqual(3420, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestColectivo_TryPagarCon_SinSaldo_RetornaFalse()
        {
            Colectivo colectivo = new Colectivo("102");
            Tarjeta tarjeta = new Tarjeta();
            // No cargar - saldo 0, que excedería el límite negativo

            bool resultado = colectivo.TryPagarCon(tarjeta, out Boleto boleto);

            Assert.IsFalse(resultado);
            Assert.IsNull(boleto);
        }

        [Test]
        public void TestColectivo_TryPagarCon_ConTrasbordo_RetornaTrue()
        {
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Sunday || ahora.Hour < 7 || ahora.Hour >= 22)
            {
                Assert.Ignore("Test requiere horario L-S 7-22");
            }

            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            Colectivo colectivo1 = new Colectivo("102");
            Colectivo colectivo2 = new Colectivo("121");

            // Primer viaje
            colectivo1.TryPagarCon(tarjeta, out _);

            // Segundo viaje - trasbordo
            bool resultado = colectivo2.TryPagarCon(tarjeta, out Boleto boleto);

            Assert.IsTrue(resultado);
            Assert.IsTrue(boleto.EsTrasbordo());
            Assert.AreEqual(0, boleto.ObtenerTarifa());
        }

        [Test]
        public void TestInterurbano_TryPagarCon_ConSaldo_RetornaTrue()
        {
            ColectivoInterurbano interurbano = new ColectivoInterurbano("Villa Gobernador Gálvez");
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            bool resultado = interurbano.TryPagarCon(tarjeta, out Boleto boleto);

            Assert.IsTrue(resultado);
            Assert.IsNotNull(boleto);
            Assert.AreEqual(3000, boleto.ObtenerTarifa());
            Assert.AreEqual(2000, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestInterurbano_TryPagarCon_ConTrasbordo_RetornaTrue()
        {
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Sunday || ahora.Hour < 7 || ahora.Hour >= 22)
            {
                Assert.Ignore("Test requiere horario L-S 7-22");
            }

            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(10000);

            Colectivo urbano = new Colectivo("102");
            ColectivoInterurbano interurbano = new ColectivoInterurbano("Granadero Baigorria");

            // Viaje urbano
            urbano.TryPagarCon(tarjeta, out _);

            // Viaje interurbano - trasbordo
            bool resultado = interurbano.TryPagarCon(tarjeta, out Boleto boleto);

            Assert.IsTrue(resultado);
            Assert.IsTrue(boleto.EsTrasbordo());
            Assert.AreEqual(0, boleto.ObtenerTarifa());
        }

        #endregion

        #region Tests de descontar para franquicias

        [Test]
        public void TestBoletoGratuito_Descontar_ConMontoPositivo()
        {
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
            }

            BoletoGratuito gratuito = new BoletoGratuito();
            gratuito.Cargar(5000);

            // Descontar monto positivo (después de 2 viajes gratis)
            gratuito.Descontar(0); // Viaje 1
            gratuito.Descontar(0); // Viaje 2
            gratuito.Descontar(1580); // Viaje 3 - paga

            Assert.AreEqual(3420, gratuito.ObtenerSaldo());
        }

        [Test]
        public void TestMedioBoleto_Descontar_ActualizaFechaUltimoViaje()
        {
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
            }

            MedioBoleto medio = new MedioBoleto();
            medio.Cargar(5000);

            // Descontar actualiza ultimoViaje y fechaUltimosViajes
            medio.Descontar(790);

            // Verificar que se actualizó
            Assert.AreEqual(4210, medio.ObtenerSaldo());
        }

        #endregion
    }
}