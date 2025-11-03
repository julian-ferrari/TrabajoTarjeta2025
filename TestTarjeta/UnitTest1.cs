using NUnit.Framework;
using System;
using TrabajoTarjeta;

namespace TestTarjeta
{
    [TestFixture]
    public class UnitTest
    {
        private Tarjeta tarjeta;
        private Colectivo colectivo;

        [SetUp]
        public void SetUp()
        {
            tarjeta = new Tarjeta();
            colectivo = new Colectivo("102 Rojo");
        }

        [Test]
        public void TestFlujoCompletoViajeConTarjeta()
        {
            // Cargar tarjeta
            tarjeta.Cargar(5000);
            Assert.AreEqual(5000, tarjeta.ObtenerSaldo());

            // Pagar pasaje
            Boleto boleto = colectivo.PagarCon(tarjeta);

            // Verificar boleto generado
            Assert.IsNotNull(boleto);
            Assert.AreEqual(1580, boleto.ObtenerTarifa());
            Assert.AreEqual(3420, boleto.ObtenerSaldoRestante());
            Assert.AreEqual("102 Rojo", boleto.ObtenerLinea());
            Assert.That(boleto.ObtenerFechaHora(), Is.EqualTo(DateTime.Now).Within(TimeSpan.FromSeconds(5)));

            // Verificar saldo actualizado en tarjeta
            Assert.AreEqual(3420, tarjeta.ObtenerSaldo());
        }

        [TestCase(2000, 420)]   // 2000 - 1580 = 420
        [TestCase(3000, 1420)]  // 3000 - 1580 = 1420
        [TestCase(4000, 2420)]  // 4000 - 1580 = 2420
        [TestCase(5000, 3420)]  // 5000 - 1580 = 3420
        [TestCase(8000, 6420)]  // 8000 - 1580 = 6420
        [TestCase(10000, 8420)] // 10000 - 1580 = 8420
        [TestCase(15000, 13420)] // 15000 - 1580 = 13420
        [TestCase(20000, 18420)] // 20000 - 1580 = 18420
        [TestCase(25000, 23420)] // 25000 - 1580 = 23420
        [TestCase(30000, 28420)] // 30000 - 1580 = 28420
        public void TestPagoConTodosLosMontosValidos(decimal montoCarga, decimal saldoEsperado)
        {
            tarjeta.Cargar(montoCarga);
            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.AreEqual(saldoEsperado, boleto.ObtenerSaldoRestante());
            Assert.AreEqual(saldoEsperado, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestMultiplesViajesHastaAgotarSaldo()
        {
            tarjeta.Cargar(5000); // Permite 3 viajes (5000 / 1580 = 3.16...)

            // Primer viaje
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(3420, boleto1.ObtenerSaldoRestante());

            // Segundo viaje
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(1840, boleto2.ObtenerSaldoRestante());

            // Tercer viaje
            Boleto boleto3 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(260, boleto3.ObtenerSaldoRestante());

            // Cuarto viaje debería fallar
            Assert.Throws<InvalidOperationException>(() => colectivo.PagarCon(tarjeta));
        }

        [Test]
        public void TestRecargaTarjetaYContinuaViajando()
        {
            // Primera carga
            tarjeta.Cargar(2000);
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(420, boleto1.ObtenerSaldoRestante());

            // Segunda carga
            tarjeta.Cargar(3000);
            Assert.AreEqual(3420, tarjeta.ObtenerSaldo());

            // Nuevo viaje después de recargar
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(1840, boleto2.ObtenerSaldoRestante());
        }

        [Test]
        public void TestLimiteSaldoMaximoYViajeCompleto()
        {
            // Cargar hasta el límite máximo
            tarjeta.Cargar(30000);
            tarjeta.Cargar(20000);
            tarjeta.Cargar(3000);
            tarjeta.Cargar(3000);
            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());

            // Realizar viaje
            Boleto boleto = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(54420, boleto.ObtenerSaldoRestante());
            Assert.AreEqual(54420, tarjeta.ObtenerSaldo());
        }
    }
}