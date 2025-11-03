using NUnit.Framework;
using System;
using TrabajoTarjeta;

namespace TestTarjeta
{
    /// <summary>
    /// Tests adicionales para alcanzar 85% de cobertura.
    /// Cubren casos edge y métodos específicos que faltaban.
    /// </summary>
    [TestFixture]
    public class CoverageBoostTests
    {
        #region Tests para métodos getter de Tarjeta

        [Test]
        public void TestObtenerLimiteSaldoNegativo()
        {
            Tarjeta tarjeta = new Tarjeta();
            Assert.AreEqual(-1200, tarjeta.ObtenerLimiteSaldoNegativo());
        }

        [Test]
        public void TestObtenerFechaUltimoViajeInicial()
        {
            Tarjeta tarjeta = new Tarjeta();
            Assert.IsNull(tarjeta.ObtenerFechaUltimoViaje());
        }

        [Test]
        public void TestObtenerLineaUltimoViajeInicial()
        {
            Tarjeta tarjeta = new Tarjeta();
            Assert.IsNull(tarjeta.ObtenerLineaUltimoViaje());
        }

        [Test]
        public void TestRegistrarViajeTarjetaNormal()
        {
            Tarjeta tarjeta = new Tarjeta();
            Colectivo colectivo = new Colectivo("102");

            tarjeta.Cargar(5000);
            colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(tarjeta.ObtenerFechaUltimoViaje());
            Assert.AreEqual("102", tarjeta.ObtenerLineaUltimoViaje());
        }

        #endregion

        #region Tests para AcreditarCarga manual

        [Test]
        public void TestAcreditarCargaSinSaldoPendiente()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            // Llamar AcreditarCarga cuando no hay saldo pendiente
            tarjeta.AcreditarCarga();

            // No debería cambiar nada
            Assert.AreEqual(5000, tarjeta.ObtenerSaldo());
            Assert.AreEqual(0, tarjeta.ObtenerSaldoPendiente());
        }

        [Test]
        public void TestAcreditarCargaConSaldoEnLimite()
        {
            Tarjeta tarjeta = new Tarjeta();

            // Llenar hasta el límite
            tarjeta.Cargar(30000);
            tarjeta.Cargar(20000);
            tarjeta.Cargar(3000);
            tarjeta.Cargar(3000); // 56000

            // Intentar cargar más
            tarjeta.Cargar(5000); // Queda pendiente

            // Llamar AcreditarCarga no hace nada porque está en el límite
            tarjeta.AcreditarCarga();

            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());
            Assert.AreEqual(5000, tarjeta.ObtenerSaldoPendiente());
        }

        #endregion

        #region Tests para casos edge de descuento

        [Test]
        public void TestDescontarMontoExactoAlLimite()
        {
            Tarjeta tarjeta = new Tarjeta();

            // Descontar exactamente hasta el límite
            tarjeta.Descontar(1200);

            Assert.AreEqual(-1200, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestPuedeDescontarMontoExactoAlLimite()
        {
            Tarjeta tarjeta = new Tarjeta();

            Assert.IsTrue(tarjeta.PuedeDescontar(1200));
        }

        [Test]
        public void TestPuedeDescontarMontoCero()
        {
            Tarjeta tarjeta = new Tarjeta();

            Assert.IsTrue(tarjeta.PuedeDescontar(0));
        }

        #endregion

        #region Tests para CalcularTarifa con diferentes cantidades de viajes

        [Test]
        public void TestCalcularTarifaViaje1()
        {
            Tarjeta tarjeta = new Tarjeta();
            Assert.AreEqual(1580, tarjeta.CalcularTarifa(1580));
        }

        [Test]
        public void TestCalcularTarifaViaje29()
        {
            Tarjeta tarjeta = new Tarjeta();
            Colectivo colectivo = new Colectivo("102");
            tarjeta.Cargar(30000);
            tarjeta.Cargar(20000);

            // Hacer 28 viajes
            for (int i = 0; i < 28; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // El viaje 29 todavía es tarifa normal
            Assert.AreEqual(1580, tarjeta.CalcularTarifa(1580));
        }

        [Test]
        public void TestCalcularTarifaViaje30()
        {
            Tarjeta tarjeta = new Tarjeta();
            Colectivo colectivo = new Colectivo("102");
            tarjeta.Cargar(30000);
            tarjeta.Cargar(20000);

            // Hacer 29 viajes
            for (int i = 0; i < 29; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // El viaje 30 tiene 20% descuento
            Assert.AreEqual(1264, tarjeta.CalcularTarifa(1580));
        }

        [Test]
        public void TestCalcularTarifaViaje60()
        {
            Tarjeta tarjeta = new Tarjeta();
            Colectivo colectivo = new Colectivo("102");
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);

            // Hacer 59 viajes
            for (int i = 0; i < 59; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // El viaje 60 tiene 25% descuento
            Assert.AreEqual(1185, tarjeta.CalcularTarifa(1580));
        }

        [Test]
        public void TestCalcularTarifaViaje81()
        {
            Tarjeta tarjeta = new Tarjeta();
            Colectivo colectivo = new Colectivo("102");
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);

            // Hacer 80 viajes
            for (int i = 0; i < 80; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // El viaje 81 vuelve a tarifa normal
            Assert.AreEqual(1580, tarjeta.CalcularTarifa(1580));
        }

        #endregion

        #region Tests para ActualizarContadorMensual

        [Test]
        public void TestActualizarContadorMensualAlInicioDelMes()
        {
            Tarjeta tarjeta = new Tarjeta();
            Colectivo colectivo = new Colectivo("102");

            tarjeta.Cargar(5000);
            colectivo.PagarCon(tarjeta);

            Assert.AreEqual(1, tarjeta.ObtenerViajesMesActual());
        }

        [Test]
        public void TestObtenerViajesMesActualLlamaActualizarContador()
        {
            Tarjeta tarjeta = new Tarjeta();

            // Llamar sin viajes previos
            int viajes = tarjeta.ObtenerViajesMesActual();
            Assert.AreEqual(0, viajes);
        }

        #endregion

        #region Tests para franquicias con diferentes tarifas base

        [Test]
        public void TestMedioBoletoConTarifaDiferente()
        {
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
            }

            MedioBoleto medioBoleto = new MedioBoleto();

            // Tarifa base diferente (ej: interurbano)
            Assert.AreEqual(1500, medioBoleto.CalcularTarifa(3000));
        }

        [Test]
        public void TestBoletoGratuitoConTarifaDiferente()
        {
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
            }

            BoletoGratuito boletoGratuito = new BoletoGratuito();

            // Siempre gratis los primeros 2 viajes
            Assert.AreEqual(0, boletoGratuito.CalcularTarifa(3000));
            Assert.AreEqual(0, boletoGratuito.CalcularTarifa(1580));
        }

        [Test]
        public void TestFranquiciaCompletaConTarifaDiferente()
        {
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
            }

            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            // Siempre gratis
            Assert.AreEqual(0, franquicia.CalcularTarifa(3000));
            Assert.AreEqual(0, franquicia.CalcularTarifa(1580));
            Assert.AreEqual(0, franquicia.CalcularTarifa(5000));
        }

        #endregion

        #region Tests para ColectivoInterurbano específicos

        [Test]
        public void TestColectivoInterurbanoObtenerTarifaBasica()
        {
            ColectivoInterurbano interurbano = new ColectivoInterurbano("Gálvez");
            Assert.AreEqual(3000, interurbano.ObtenerTarifaBasica());
        }

        [Test]
        public void TestColectivoInterurbanoConTarjetaNula()
        {
            ColectivoInterurbano interurbano = new ColectivoInterurbano("Gálvez");
            Assert.Throws<ArgumentNullException>(() => interurbano.PagarCon(null));
        }

        [Test]
        public void TestColectivoInterurbanoTryPagarConTarjetaNula()
        {
            ColectivoInterurbano interurbano = new ColectivoInterurbano("Gálvez");
            bool resultado = interurbano.TryPagarCon(null, out Boleto boleto);

            Assert.IsFalse(resultado);
            Assert.IsNull(boleto);
        }

        #endregion

        #region Tests para cobertura de ToString

        [Test]
        public void TestBoletoToStringConTrasbordo()
        {
            Boleto boleto = new Boleto(
                DateTime.Now,
                0,
                5000,
                "102",
                "Tarjeta",
                0,
                1,
                true
            );

            string str = boleto.ToString();
            Assert.That(str, Does.Contain("TRASBORDO"));
        }

        [Test]
        public void TestBoletoToStringSinTrasbordo()
        {
            Boleto boleto = new Boleto(
                DateTime.Now,
                1580,
                3420,
                "102",
                "Tarjeta",
                1580,
                1,
                false
            );

            string str = boleto.ToString();
            Assert.That(str, Does.Not.Contain("TRASBORDO"));
        }

        #endregion

        #region Tests para Descontar con saldo pendiente

        [Test]
        public void TestDescontarAcreditaSaldoPendienteAutomaticamente()
        {
            Tarjeta tarjeta = new Tarjeta();
            Colectivo colectivo = new Colectivo("102");

            // Cargar con excedente
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000); // Pendiente: 4000

            // Descontar llama automáticamente a AcreditarCarga
            colectivo.PagarCon(tarjeta);

            // Verifica que se acreditó algo del pendiente
            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());
            Assert.AreEqual(2420, tarjeta.ObtenerSaldoPendiente());
        }

        #endregion

        #region Tests para ID únicos

        [Test]
        public void TestTarjetasConIdsUnicos()
        {
            Tarjeta t1 = new Tarjeta();
            Tarjeta t2 = new Tarjeta();
            Tarjeta t3 = new Tarjeta();

            Assert.AreNotEqual(t1.Id, t2.Id);
            Assert.AreNotEqual(t2.Id, t3.Id);
            Assert.AreNotEqual(t1.Id, t3.Id);
        }

        [Test]
        public void TestFranquiciasConIdsUnicos()
        {
            MedioBoleto mb1 = new MedioBoleto();
            MedioBoleto mb2 = new MedioBoleto();
            BoletoGratuito bg = new BoletoGratuito();
            FranquiciaCompleta fc = new FranquiciaCompleta();

            Assert.AreNotEqual(mb1.Id, mb2.Id);
            Assert.AreNotEqual(mb1.Id, bg.Id);
            Assert.AreNotEqual(mb1.Id, fc.Id);
            Assert.AreNotEqual(bg.Id, fc.Id);
        }

        #endregion

        #region Tests para recarga después de saldo negativo

        [Test]
        public void TestRecargaDespuesDeSaldoNegativoCubreSaldoNegativoCompleto()
        {
            Tarjeta tarjeta = new Tarjeta();
            Colectivo colectivo = new Colectivo("102");

            tarjeta.Cargar(2000);
            colectivo.PagarCon(tarjeta); // 420
            colectivo.PagarCon(tarjeta); // -1160

            // Cargar menos de lo que debe
            tarjeta.Cargar(2000);

            // Debería quedar: 2000 - 1160 = 840
            Assert.AreEqual(840, tarjeta.ObtenerSaldo());
        }

        #endregion

        #region Tests edge cases combinados

        [Test]
        public void TestMedioBoletoConUsoFrecuenteYDescuentoMedioBoleto()
        {
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
            }

            MedioBoleto medioBoleto = new MedioBoleto();

            // MedioBoleto NO se beneficia del uso frecuente
            // Siempre es 50% de la tarifa base
            decimal tarifa = medioBoleto.CalcularTarifa(1580);
            Assert.AreEqual(790, tarifa);
        }

        [Test]
        public void TestBoletoGratuitoDespuesDelSegundoViajeUsaTarifaNormal()
        {
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                Assert.Ignore("Test requiere horario L-V 6-22");
            }

            BoletoGratuito boletoGratuito = new BoletoGratuito();
            Colectivo colectivo = new Colectivo("102");

            // Hacer 2 viajes gratuitos
            colectivo.PagarCon(boletoGratuito);
            colectivo.PagarCon(boletoGratuito);

            // El tercero debería usar tarifa normal (no uso frecuente)
            decimal tarifa = boletoGratuito.CalcularTarifa(1580);
            Assert.AreEqual(1580, tarifa);
        }

        #endregion
    }
}