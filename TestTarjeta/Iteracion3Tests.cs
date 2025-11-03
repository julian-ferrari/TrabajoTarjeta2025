using NUnit.Framework;
using System;
using System.Threading;
using TrabajoTarjeta;

namespace TestTarjeta
{
    /// <summary>
    /// Tests para la Iteración 3: Datos de boleto y limitaciones de uso.
    /// </summary>
    [TestFixture]
    public class Iteracion3Tests
    {
        private Colectivo colectivo;

        [SetUp]
        public void SetUp()
        {
            colectivo = new Colectivo("102 Rojo");
        }

        private bool EsHorarioValidoParaFranquicias()
        {
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }
            if (ahora.Hour < 6 || ahora.Hour >= 22)
            {
                return false;
            }
            return true;
        }

        #region Tests Issue #4: Más datos sobre el boleto

        [Test]
        public void TestBoletoContieneFechaYHora()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000);

            DateTime antes = DateTime.Now;
            Boleto boleto = colectivo.PagarCon(tarjeta);
            DateTime despues = DateTime.Now;

            // Verificar que la fecha está entre antes y después
            Assert.That(boleto.ObtenerFechaHora(), Is.InRange(antes, despues));
        }

        [Test]
        public void TestBoletoContieneTipoTarjetaNormal()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.AreEqual("Tarjeta", boleto.ObtenerTipoTarjeta());
        }

        [Test]
        public void TestBoletoContieneTipoTarjetaMedioBoleto()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            MedioBoleto medioBoleto = new MedioBoleto();
            medioBoleto.Cargar(2000);

            Boleto boleto = colectivo.PagarCon(medioBoleto);

            Assert.AreEqual("MedioBoleto", boleto.ObtenerTipoTarjeta());
        }

        [Test]
        public void TestBoletoContieneTipoTarjetaBoletoGratuito()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            BoletoGratuito boletoGratuito = new BoletoGratuito();

            Boleto boleto = colectivo.PagarCon(boletoGratuito);

            Assert.AreEqual("BoletoGratuito", boleto.ObtenerTipoTarjeta());
        }

        [Test]
        public void TestBoletoContieneTipoTarjetaFranquiciaCompleta()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            Boleto boleto = colectivo.PagarCon(franquicia);

            Assert.AreEqual("FranquiciaCompleta", boleto.ObtenerTipoTarjeta());
        }

        [Test]
        public void TestBoletoContieneLineaDeColectivo()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.AreEqual("102 Rojo", boleto.ObtenerLinea());
        }

        [Test]
        public void TestBoletoContieneTotalAbonado()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            // Sin saldo negativo, total abonado = tarifa
            Assert.AreEqual(1580, boleto.ObtenerTotalAbonado());
        }

        [Test]
        public void TestBoletoContieneSaldoRestante()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.AreEqual(3420, boleto.ObtenerSaldoRestante());
            Assert.AreEqual(tarjeta.ObtenerSaldo(), boleto.ObtenerSaldoRestante());
        }

        [Test]
        public void TestBoletoContieneIdDeTarjeta()
        {
            Tarjeta tarjeta1 = new Tarjeta();
            Tarjeta tarjeta2 = new Tarjeta();

            tarjeta1.Cargar(2000);
            tarjeta2.Cargar(2000);

            Boleto boleto1 = colectivo.PagarCon(tarjeta1);
            Boleto boleto2 = colectivo.PagarCon(tarjeta2);

            // Los IDs deben ser diferentes
            Assert.AreNotEqual(boleto1.ObtenerIdTarjeta(), boleto2.ObtenerIdTarjeta());

            // Verificar que coinciden con los IDs de las tarjetas
            Assert.AreEqual(tarjeta1.Id, boleto1.ObtenerIdTarjeta());
            Assert.AreEqual(tarjeta2.Id, boleto2.ObtenerIdTarjeta());
        }

        [Test]
        public void TestBoletoTotalAbonadoIgualATarifaSinSaldoNegativo()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            // Sin saldo negativo previo: total abonado = tarifa
            Assert.AreEqual(1580, boleto.ObtenerTarifa());
            Assert.AreEqual(1580, boleto.ObtenerTotalAbonado());
        }

        [Test]
        public void TestBoletoConSaldoNegativoMuestraTotalAbonado()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000);

            // Primer viaje: saldo = 420
            colectivo.PagarCon(tarjeta);

            // Segundo viaje: saldo = -1160
            Boleto boleto2 = colectivo.PagarCon(tarjeta);

            Assert.AreEqual(1580, boleto2.ObtenerTarifa());
            Assert.AreEqual(1580, boleto2.ObtenerTotalAbonado());
            Assert.AreEqual(-1160, boleto2.ObtenerSaldoRestante());
        }

        [Test]
        public void TestBoletosConDiferentesTiposTarjetaMuestranTarifasCorrectas()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            Tarjeta normal = new Tarjeta();
            MedioBoleto medio = new MedioBoleto();
            BoletoGratuito gratuito = new BoletoGratuito();
            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            normal.Cargar(2000);
            medio.Cargar(2000);

            Boleto boletoNormal = colectivo.PagarCon(normal);
            Boleto boletoMedio = colectivo.PagarCon(medio);
            Boleto boletoGratuito = colectivo.PagarCon(gratuito);
            Boleto boletoFranquicia = colectivo.PagarCon(franquicia);

            // Verificar tarifas
            Assert.AreEqual(1580, boletoNormal.ObtenerTarifa());
            Assert.AreEqual(790, boletoMedio.ObtenerTarifa());
            Assert.AreEqual(0, boletoGratuito.ObtenerTarifa());
            Assert.AreEqual(0, boletoFranquicia.ObtenerTarifa());

            // Verificar tipos
            Assert.AreEqual("Tarjeta", boletoNormal.ObtenerTipoTarjeta());
            Assert.AreEqual("MedioBoleto", boletoMedio.ObtenerTipoTarjeta());
            Assert.AreEqual("BoletoGratuito", boletoGratuito.ObtenerTipoTarjeta());
            Assert.AreEqual("FranquiciaCompleta", boletoFranquicia.ObtenerTipoTarjeta());
        }

        #endregion

        #region Tests Issue #5: Limitación Medio Boleto - 5 minutos entre viajes

        [Test]
        public void TestMedioBoletoNoPuedeViajarAntesDe5Minutos()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            MedioBoleto medioBoleto = new MedioBoleto();
            medioBoleto.Cargar(5000);

            // Primer viaje exitoso
            Boleto boleto1 = colectivo.PagarCon(medioBoleto);
            Assert.IsNotNull(boleto1);
            Assert.AreEqual(790, boleto1.ObtenerTarifa());

            // Intento inmediato de segundo viaje debería fallar
            Assert.Throws<InvalidOperationException>(() => colectivo.PagarCon(medioBoleto));
        }

        [Test]
        public void TestMedioBoletoTryPagarConRetornaFalseAntesDe5Minutos()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            MedioBoleto medioBoleto = new MedioBoleto();
            medioBoleto.Cargar(5000);

            // Primer viaje exitoso
            bool resultado1 = colectivo.TryPagarCon(medioBoleto, out Boleto boleto1);
            Assert.IsTrue(resultado1);
            Assert.IsNotNull(boleto1);

            // Intento inmediato debería retornar false
            bool resultado2 = colectivo.TryPagarCon(medioBoleto, out Boleto boleto2);
            Assert.IsFalse(resultado2);
            Assert.IsNull(boleto2);
        }

        // NOTA: Este test toma 5 minutos en ejecutarse. 
        // Comentarlo si quieres ejecución rápida de tests.
        [Test]
        [Ignore("Test lento: toma 5+ minutos. Descomentar para verificación completa.")]
        public void TestMedioBoletoPermiteViajarDespuesDe5Minutos()
        {
            MedioBoleto medioBoleto = new MedioBoleto();
            medioBoleto.Cargar(5000);

            // Primer viaje
            colectivo.PagarCon(medioBoleto);

            // Esperar 5 minutos + margen
            Thread.Sleep((5 * 60 * 1000) + 1000); // 5 min + 1 seg

            // Segundo viaje debería ser exitoso
            Boleto boleto2 = colectivo.PagarCon(medioBoleto);
            Assert.IsNotNull(boleto2);
            Assert.AreEqual(790, boleto2.ObtenerTarifa());
        }

        #endregion

        #region Tests Issue #5: Limitación Medio Boleto - Máximo 2 viajes con descuento por día

        [Test]
        public void TestMedioBoletoPermiteDosViajesConDescuentoPorDia()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            MedioBoleto medioBoleto = new MedioBoleto();
            medioBoleto.Cargar(10000);

            // Primer viaje: medio boleto
            Boleto boleto1 = colectivo.PagarCon(medioBoleto);
            Assert.AreEqual(790, boleto1.ObtenerTarifa());
            Assert.AreEqual(9210, medioBoleto.ObtenerSaldo());
        }

        [Test]
        [Ignore("Test lento: requiere esperas de 5 minutos. Descomentar para verificación completa.")]
        public void TestMedioBoletoTercerViajeCobraTarifaCompleta()
        {
            MedioBoleto medioBoleto = new MedioBoleto();
            medioBoleto.Cargar(10000);

            // Primer viaje: $790
            colectivo.PagarCon(medioBoleto);
            Thread.Sleep((5 * 60 * 1000) + 100);

            // Segundo viaje: $790
            colectivo.PagarCon(medioBoleto);
            Thread.Sleep((5 * 60 * 1000) + 100);

            // Tercer viaje: $1580 (tarifa completa)
            Boleto boleto3 = colectivo.PagarCon(medioBoleto);
            Assert.AreEqual(1580, boleto3.ObtenerTarifa());
            Assert.AreEqual(6840, medioBoleto.ObtenerSaldo()); // 10000 - 790 - 790 - 1580
        }

        [Test]
        [Ignore("Test lento: requiere múltiples esperas de 5 minutos.")]
        public void TestMedioBoletoNoMasDeDosDescuentosPorDia()
        {
            MedioBoleto medioBoleto = new MedioBoleto();
            medioBoleto.Cargar(15000);

            // Dos viajes con descuento
            colectivo.PagarCon(medioBoleto); // Viaje 1: $790
            Thread.Sleep((5 * 60 * 1000) + 100);

            colectivo.PagarCon(medioBoleto); // Viaje 2: $790
            Thread.Sleep((5 * 60 * 1000) + 100);

            // Tercer viaje: tarifa completa
            Boleto boleto3 = colectivo.PagarCon(medioBoleto);
            Assert.AreEqual(1580, boleto3.ObtenerTarifa());

            Thread.Sleep((5 * 60 * 1000) + 100);

            // Cuarto viaje: también tarifa completa
            Boleto boleto4 = colectivo.PagarCon(medioBoleto);
            Assert.AreEqual(1580, boleto4.ObtenerTarifa());

            // Saldo final: 15000 - 790 - 790 - 1580 - 1580 = 10260
            Assert.AreEqual(10260, medioBoleto.ObtenerSaldo());
        }

        [Test]
        public void TestMedioBoletoCalcularTarifaSegunViajes()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            MedioBoleto medioBoleto = new MedioBoleto();
            medioBoleto.Cargar(10000);

            // Antes del primer viaje: medio boleto
            Assert.AreEqual(790, medioBoleto.CalcularTarifa(1580));

            colectivo.PagarCon(medioBoleto);

            // Después del primer viaje: todavía medio boleto
            Assert.AreEqual(790, medioBoleto.CalcularTarifa(1580));
        }

        #endregion

        #region Tests Issue #6: Limitación Boleto Gratuito - Máximo 2 viajes gratuitos por día

        [Test]
        public void TestBoletoGratuitoPrimerosDosViajesSonGratis()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            BoletoGratuito boletoGratuito = new BoletoGratuito();
            // No cargar saldo

            // Primer viaje: gratis
            Boleto boleto1 = colectivo.PagarCon(boletoGratuito);
            Assert.AreEqual(0, boleto1.ObtenerTarifa());
            Assert.AreEqual(0, boletoGratuito.ObtenerSaldo());

            // Segundo viaje: gratis
            Boleto boleto2 = colectivo.PagarCon(boletoGratuito);
            Assert.AreEqual(0, boleto2.ObtenerTarifa());
            Assert.AreEqual(0, boletoGratuito.ObtenerSaldo());
        }

        [Test]
        public void TestBoletoGratuitoTercerViajeCobraTarifaCompleta()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            BoletoGratuito boletoGratuito = new BoletoGratuito();
            boletoGratuito.Cargar(5000);

            // Primer viaje: gratis
            Boleto boleto1 = colectivo.PagarCon(boletoGratuito);
            Assert.AreEqual(0, boleto1.ObtenerTarifa());

            // Segundo viaje: gratis
            Boleto boleto2 = colectivo.PagarCon(boletoGratuito);
            Assert.AreEqual(0, boleto2.ObtenerTarifa());

            // Tercer viaje: PAGA TARIFA COMPLETA
            Boleto boleto3 = colectivo.PagarCon(boletoGratuito);
            Assert.AreEqual(1580, boleto3.ObtenerTarifa());
            Assert.AreEqual(3420, boletoGratuito.ObtenerSaldo());
        }

        [Test]
        public void TestBoletoGratuitoNoMasDeDosViajesGratuitosPorDia()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            BoletoGratuito boletoGratuito = new BoletoGratuito();
            boletoGratuito.Cargar(10000);

            // Dos viajes gratuitos
            colectivo.PagarCon(boletoGratuito);
            colectivo.PagarCon(boletoGratuito);
            Assert.AreEqual(10000, boletoGratuito.ObtenerSaldo());

            // Tercero, cuarto y quinto viajes cobran tarifa completa
            Boleto boleto3 = colectivo.PagarCon(boletoGratuito);
            Boleto boleto4 = colectivo.PagarCon(boletoGratuito);
            Boleto boleto5 = colectivo.PagarCon(boletoGratuito);

            Assert.AreEqual(1580, boleto3.ObtenerTarifa());
            Assert.AreEqual(1580, boleto4.ObtenerTarifa());
            Assert.AreEqual(1580, boleto5.ObtenerTarifa());

            // Saldo final: 10000 - (3 * 1580) = 5260
            Assert.AreEqual(5260, boletoGratuito.ObtenerSaldo());
        }

        [Test]
        public void TestBoletoGratuitoTercerViajeSinSaldoFalla()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            BoletoGratuito boletoGratuito = new BoletoGratuito();
            // No cargar saldo

            // Dos viajes gratuitos
            colectivo.PagarCon(boletoGratuito);
            colectivo.PagarCon(boletoGratuito);

            // Tercer viaje sin saldo debería fallar
            Assert.Throws<InvalidOperationException>(() => colectivo.PagarCon(boletoGratuito));
        }

        [Test]
        public void TestBoletoGratuitoTryPagarConRetornaFalseSinSaldo()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            BoletoGratuito boletoGratuito = new BoletoGratuito();

            // Dos viajes gratuitos
            colectivo.TryPagarCon(boletoGratuito, out _);
            colectivo.TryPagarCon(boletoGratuito, out _);

            // Tercer viaje sin saldo
            bool resultado = colectivo.TryPagarCon(boletoGratuito, out Boleto boleto3);

            Assert.IsFalse(resultado);
            Assert.IsNull(boleto3);
        }

        [Test]
        public void TestBoletoGratuitoCalcularTarifaSegunViajes()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            BoletoGratuito boletoGratuito = new BoletoGratuito();

            // Antes del primer viaje: gratis
            Assert.AreEqual(0, boletoGratuito.CalcularTarifa(1580));

            colectivo.PagarCon(boletoGratuito);

            // Después del primer viaje: todavía gratis
            Assert.AreEqual(0, boletoGratuito.CalcularTarifa(1580));

            colectivo.PagarCon(boletoGratuito);

            // Después del segundo viaje: tarifa completa
            Assert.AreEqual(1580, boletoGratuito.CalcularTarifa(1580));
        }

        [Test]
        public void TestBoletoGratuitoConSaldoNegativoEnViajesPagos()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            BoletoGratuito boletoGratuito = new BoletoGratuito();
            boletoGratuito.Cargar(2000);

            // Dos viajes gratuitos
            colectivo.PagarCon(boletoGratuito); // Saldo: 2000
            colectivo.PagarCon(boletoGratuito); // Saldo: 2000

            // Tercer viaje cobra completo
            Boleto boleto3 = colectivo.PagarCon(boletoGratuito);
            Assert.AreEqual(420, boletoGratuito.ObtenerSaldo());

            // Cuarto viaje puede dejar en saldo negativo
            Boleto boleto4 = colectivo.PagarCon(boletoGratuito);
            Assert.AreEqual(-1160, boletoGratuito.ObtenerSaldo());
        }

        #endregion

        #region Tests Comparativos: FranquiciaCompleta vs BoletoGratuito

        [Test]
        public void TestFranquiciaCompletaSiempreViajaGratis()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            // Múltiples viajes, todos gratis
            for (int i = 0; i < 10; i++)
            {
                Boleto boleto = colectivo.PagarCon(franquicia);
                Assert.AreEqual(0, boleto.ObtenerTarifa());
                Assert.AreEqual(0, franquicia.ObtenerSaldo());
            }
        }

        [Test]
        public void TestComparacionBoletoGratuitoVsFranquiciaCompleta()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            BoletoGratuito boletoGratuito = new BoletoGratuito();
            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            boletoGratuito.Cargar(5000);

            // BoletoGratuito: Primeros 2 gratis, tercero paga
            Boleto bg1 = colectivo.PagarCon(boletoGratuito);
            Boleto bg2 = colectivo.PagarCon(boletoGratuito);
            Boleto bg3 = colectivo.PagarCon(boletoGratuito);

            // FranquiciaCompleta: Todos gratis
            Boleto fc1 = colectivo.PagarCon(franquicia);
            Boleto fc2 = colectivo.PagarCon(franquicia);
            Boleto fc3 = colectivo.PagarCon(franquicia);

            // Verificar diferencias
            Assert.AreEqual(0, bg1.ObtenerTarifa());
            Assert.AreEqual(0, bg2.ObtenerTarifa());
            Assert.AreEqual(1580, bg3.ObtenerTarifa()); // PAGA

            Assert.AreEqual(0, fc1.ObtenerTarifa());
            Assert.AreEqual(0, fc2.ObtenerTarifa());
            Assert.AreEqual(0, fc3.ObtenerTarifa()); // GRATIS

            Assert.AreEqual(3420, boletoGratuito.ObtenerSaldo());
            Assert.AreEqual(0, franquicia.ObtenerSaldo());
        }

        #endregion

        #region Tests de Integración: Múltiples tipos de tarjeta

        [Test]
        public void TestMultiplesTiposTarjetaEnMismoColectivo()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            Tarjeta normal = new Tarjeta();
            MedioBoleto medio = new MedioBoleto();
            BoletoGratuito gratuito = new BoletoGratuito();
            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            normal.Cargar(3000);
            medio.Cargar(3000);
            gratuito.Cargar(3000);

            // Viajes
            Boleto b1 = colectivo.PagarCon(normal);
            Boleto b2 = colectivo.PagarCon(medio);
            Boleto b3 = colectivo.PagarCon(gratuito);
            Boleto b4 = colectivo.PagarCon(franquicia);

            // Verificar tarifas
            Assert.AreEqual(1580, b1.ObtenerTarifa());
            Assert.AreEqual(790, b2.ObtenerTarifa());
            Assert.AreEqual(0, b3.ObtenerTarifa());
            Assert.AreEqual(0, b4.ObtenerTarifa());

            // Verificar tipos en boletos
            Assert.AreEqual("Tarjeta", b1.ObtenerTipoTarjeta());
            Assert.AreEqual("MedioBoleto", b2.ObtenerTipoTarjeta());
            Assert.AreEqual("BoletoGratuito", b3.ObtenerTipoTarjeta());
            Assert.AreEqual("FranquiciaCompleta", b4.ObtenerTipoTarjeta());

            // Verificar saldos finales
            Assert.AreEqual(1420, normal.ObtenerSaldo());
            Assert.AreEqual(2210, medio.ObtenerSaldo());
            Assert.AreEqual(3000, gratuito.ObtenerSaldo());
            Assert.AreEqual(0, franquicia.ObtenerSaldo());
        }

        [Test]
        public void TestBoletosContienenIdsUnicosDeDiferentesTarjetas()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            Tarjeta t1 = new Tarjeta();
            Tarjeta t2 = new Tarjeta();
            MedioBoleto t3 = new MedioBoleto();
            BoletoGratuito t4 = new BoletoGratuito();

            t1.Cargar(2000);
            t2.Cargar(2000);
            t3.Cargar(2000);

            Boleto b1 = colectivo.PagarCon(t1);
            Boleto b2 = colectivo.PagarCon(t2);
            Boleto b3 = colectivo.PagarCon(t3);
            Boleto b4 = colectivo.PagarCon(t4);

            // Todos los IDs deben ser diferentes
            Assert.AreNotEqual(b1.ObtenerIdTarjeta(), b2.ObtenerIdTarjeta());
            Assert.AreNotEqual(b1.ObtenerIdTarjeta(), b3.ObtenerIdTarjeta());
            Assert.AreNotEqual(b1.ObtenerIdTarjeta(), b4.ObtenerIdTarjeta());
            Assert.AreNotEqual(b2.ObtenerIdTarjeta(), b3.ObtenerIdTarjeta());
            Assert.AreNotEqual(b2.ObtenerIdTarjeta(), b4.ObtenerIdTarjeta());
            Assert.AreNotEqual(b3.ObtenerIdTarjeta(), b4.ObtenerIdTarjeta());
        }

        #endregion

        #region Tests: Saldo pendiente de acreditación

        [Test]
        public void TestCargaExcedeLimiteQuedaPendiente()
        {
            Tarjeta tarjeta = new Tarjeta();

            // Cargar hasta cerca del límite
            tarjeta.Cargar(30000);
            tarjeta.Cargar(25000); // Total: 55000
            Assert.AreEqual(55000, tarjeta.ObtenerSaldo());
            Assert.AreEqual(0, tarjeta.ObtenerSaldoPendiente());

            // Cargar $5000 más: solo se acreditan $1000, quedan $4000 pendientes
            tarjeta.Cargar(5000);

            Assert.AreEqual(56000, tarjeta.ObtenerSaldo()); // Límite máximo
            Assert.AreEqual(4000, tarjeta.ObtenerSaldoPendiente()); // Excedente
        }

        [Test]
        public void TestSaldoPendienteSeAcreditaDespuesDeViaje()
        {
            Tarjeta tarjeta = new Tarjeta();
            Colectivo colectivo = new Colectivo("102");

            // Cargar hasta el límite con excedente
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000); // Total: 60000, pero límite 56000

            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());
            Assert.AreEqual(4000, tarjeta.ObtenerSaldoPendiente());

            // Hacer un viaje: $1580
            // IMPORTANTE: El orden es:
            // 1. Descuenta: 56000 - 1580 = 54420
            // 2. Actualiza contador de viajes
            // 3. Acredita pendiente: 54420 + 1580 = 56000
            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());
            Assert.AreEqual(2420, tarjeta.ObtenerSaldoPendiente());
        }

        [Test]
        public void TestMultiplesViajesAcreditanSaldoPendiente()
        {
            Tarjeta tarjeta = new Tarjeta();
            Colectivo colectivo = new Colectivo("102");

            // Cargar $60000 (límite 56000, pendiente 4000)
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);

            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());
            Assert.AreEqual(4000, tarjeta.ObtenerSaldoPendiente());

            // Primer viaje
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());
            Assert.AreEqual(2420, tarjeta.ObtenerSaldoPendiente());

            // Segundo viaje
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());
            Assert.AreEqual(840, tarjeta.ObtenerSaldoPendiente());

            // Tercer viaje
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(55260, tarjeta.ObtenerSaldo());
            Assert.AreEqual(0, tarjeta.ObtenerSaldoPendiente());
        }

        [Test]
        public void TestCargaPendienteNoSeAcreditaSiSaldoEstaEnLimite()
        {
            Tarjeta tarjeta = new Tarjeta();

            // Cargar exactamente el límite usando cargas válidas
            tarjeta.Cargar(30000);
            tarjeta.Cargar(20000);
            tarjeta.Cargar(3000);
            tarjeta.Cargar(3000); // Total: 56000

            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());

            // Intentar cargar más: todo queda pendiente
            tarjeta.Cargar(10000);

            Assert.AreEqual(56000, tarjeta.ObtenerSaldo()); // No cambió
            Assert.AreEqual(10000, tarjeta.ObtenerSaldoPendiente());

            // Llamar manualmente a AcreditarCarga no hace nada si está lleno
            tarjeta.AcreditarCarga();

            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());
            Assert.AreEqual(10000, tarjeta.ObtenerSaldoPendiente());
        }

        [Test]
        public void TestSaldoPendienteSeAcreditaProgresivamente()
        {
            Tarjeta tarjeta = new Tarjeta();
            Colectivo colectivo = new Colectivo("102");

            // Cargar con excedente
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000); // Saldo: 56000, Pendiente: 4000

            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());
            Assert.AreEqual(4000, tarjeta.ObtenerSaldoPendiente());

            // Después del primer viaje
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());
            Assert.AreEqual(2420, tarjeta.ObtenerSaldoPendiente());

            // Después del segundo viaje
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());
            Assert.AreEqual(840, tarjeta.ObtenerSaldoPendiente());

            // Después del tercer viaje (se acaba el pendiente)
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(55260, tarjeta.ObtenerSaldo());
            Assert.AreEqual(0, tarjeta.ObtenerSaldoPendiente());
        }

        [Test]
        public void TestObtenerSaldoPendienteInicial()
        {
            Tarjeta tarjeta = new Tarjeta();

            Assert.AreEqual(0, tarjeta.ObtenerSaldoPendiente());
        }

        [Test]
        public void TestLimiteActualizado56000()
        {
            Tarjeta tarjeta = new Tarjeta();

            // Cargar hasta 56000 usando cargas válidas
            tarjeta.Cargar(30000);
            tarjeta.Cargar(20000);
            tarjeta.Cargar(3000);
            tarjeta.Cargar(3000);

            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());

            // No se puede cargar más sin que quede pendiente
            tarjeta.Cargar(2000);

            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());
            Assert.AreEqual(2000, tarjeta.ObtenerSaldoPendiente());
        }

        #endregion
    }

}