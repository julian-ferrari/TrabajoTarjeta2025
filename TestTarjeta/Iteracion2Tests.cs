using NUnit.Framework;
using System;
using TrabajoTarjeta;

namespace TestTarjeta
{
    [TestFixture]
    public class Iteracion2Tests
    {
        private Tarjeta tarjeta;
        private Colectivo colectivo;

        [SetUp]
        public void SetUp()
        {
            tarjeta = new Tarjeta();
            colectivo = new Colectivo("102 Rojo");
        }

        #region Tests Saldo Negativo

        [Test]
        public void TestTarjetaNoPuedeQuedarConMenosSaldoQueElPermitido()
        {
            // La tarjeta empieza con saldo 0
            // Intentar descontar más del límite negativo permitido (-1200)
            Assert.Throws<InvalidOperationException>(() => tarjeta.Descontar(1201));
        }

        [Test]
        public void TestTarjetaPuedeQuedarEnSaldoNegativoHastaElLimite()
        {
            tarjeta.Cargar(2000);
            // Usar todo el saldo y quedar en negativo
            tarjeta.Descontar(1580); // Saldo: 420
            tarjeta.Descontar(1580); // Saldo: -1160 (dentro del límite de -1200)

            Assert.AreEqual(-1160, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestSaldoDescuentaCorrectamenteViajesPlus()
        {
            tarjeta.Cargar(2000); // Saldo inicial: 2000

            // Primer viaje: 2000 - 1580 = 420
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(420, tarjeta.ObtenerSaldo());

            // Segundo viaje (plus): 420 - 1580 = -1160
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(-1160, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestRecargaDescuentaSaldoNegativo()
        {
            tarjeta.Cargar(2000);

            // Dos viajes que dejan saldo negativo
            colectivo.PagarCon(tarjeta);
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(-1160, tarjeta.ObtenerSaldo());

            // Recargar y verificar que se descuenta el saldo negativo
            tarjeta.Cargar(3000);
            Assert.AreEqual(1840, tarjeta.ObtenerSaldo()); // 3000 - 1160 = 1840
        }

        [Test]
        public void TestColectivoPagarConDevuelveFalseConSaldoInsuficiente()
        {
            // Tarjeta con saldo insuficiente para viaje plus
            tarjeta.Cargar(2000);
            colectivo.PagarCon(tarjeta); // Saldo: 420
            colectivo.PagarCon(tarjeta); // Saldo: -1160

            // Intento de tercer viaje debería fallar
            bool resultado = colectivo.TryPagarCon(tarjeta, out Boleto boleto);
            Assert.IsFalse(resultado);
            Assert.IsNull(boleto);
        }

        #endregion

        #region Tests Franquicias

        [Test]
        public void TestMedioBoletoTarifaEsLaMitad()
        {
            MedioBoleto medioBoleto = new MedioBoleto();
            medioBoleto.Cargar(2000);

            Boleto boleto = colectivo.PagarCon(medioBoleto);

            Assert.AreEqual(790, boleto.ObtenerTarifa()); // 1580 / 2 = 790
            Assert.AreEqual(1210, medioBoleto.ObtenerSaldo()); // 2000 - 790 = 1210
        }

        [Test]
        public void TestMedioBoletoNoPermiteSegundoViajeInmediato()
        {
            MedioBoleto medioBoleto = new MedioBoleto();
            medioBoleto.Cargar(2000);

            // Primer viaje exitoso
            Boleto boleto1 = colectivo.PagarCon(medioBoleto);
            Assert.AreEqual(790, boleto1.ObtenerTarifa());
            Assert.AreEqual(1210, medioBoleto.ObtenerSaldo());

            // Segundo viaje inmediato debería fallar por restricción de 5 minutos
            bool resultado = colectivo.TryPagarCon(medioBoleto, out Boleto boleto2);
            Assert.IsFalse(resultado);
            Assert.IsNull(boleto2);

            // El saldo no cambió
            Assert.AreEqual(1210, medioBoleto.ObtenerSaldo());
        }

        [Test]
        public void TestBoletoGratuitoNoDescuentaSaldo()
        {
            BoletoGratuito boletoGratuito = new BoletoGratuito();
            boletoGratuito.Cargar(2000);

            Boleto boleto = colectivo.PagarCon(boletoGratuito);

            Assert.AreEqual(0, boleto.ObtenerTarifa());
            Assert.AreEqual(2000, boletoGratuito.ObtenerSaldo()); // Saldo no cambia
        }

        [Test]
        public void TestBoletoGratuitoPrimerYSegundoViajeGratis()
        {
            BoletoGratuito boletoGratuito = new BoletoGratuito();
            boletoGratuito.Cargar(2000);

            // Primer viaje gratis
            Boleto boleto1 = colectivo.PagarCon(boletoGratuito);
            Assert.AreEqual(0, boleto1.ObtenerTarifa());
            Assert.AreEqual(2000, boletoGratuito.ObtenerSaldo());

            // Segundo viaje gratis
            Boleto boleto2 = colectivo.PagarCon(boletoGratuito);
            Assert.AreEqual(0, boleto2.ObtenerTarifa());
            Assert.AreEqual(2000, boletoGratuito.ObtenerSaldo());
        }

        [Test]
        public void TestFranquiciaCompletaSiemprePuedePagar()
        {
            FranquiciaCompleta franquiciaCompleta = new FranquiciaCompleta();
            // No cargar saldo

            // Debería poder pagar múltiples viajes sin saldo
            for (int i = 0; i < 10; i++)
            {
                Boleto boleto = colectivo.PagarCon(franquiciaCompleta);
                Assert.AreEqual(0, boleto.ObtenerTarifa());
                Assert.AreEqual(0, franquiciaCompleta.ObtenerSaldo());
            }
        }

        [Test]
        public void TestTiposDeTarjetasConMismoColectivo()
        {
            Tarjeta normal = new Tarjeta();
            MedioBoleto medio = new MedioBoleto();
            FranquiciaCompleta gratuita = new FranquiciaCompleta();

            normal.Cargar(2000);
            medio.Cargar(2000);

            Boleto boletoNormal = colectivo.PagarCon(normal);
            Boleto boletoMedio = colectivo.PagarCon(medio);
            Boleto boletoGratuito = colectivo.PagarCon(gratuita);

            Assert.AreEqual(1580, boletoNormal.ObtenerTarifa());
            Assert.AreEqual(790, boletoMedio.ObtenerTarifa());
            Assert.AreEqual(0, boletoGratuito.ObtenerTarifa());

            Assert.AreEqual(420, normal.ObtenerSaldo());
            Assert.AreEqual(1210, medio.ObtenerSaldo());
            Assert.AreEqual(0, gratuita.ObtenerSaldo());
        }

        #endregion
    }
}