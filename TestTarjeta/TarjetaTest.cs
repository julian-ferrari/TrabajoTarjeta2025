using NUnit.Framework;
using System;
using TrabajoTarjeta;

namespace TestTarjeta
{
    [TestFixture]
    public class TarjetaTest
    {
        private Tarjeta tarjeta;

        [SetUp]
        public void SetUp()
        {
            tarjeta = new Tarjeta();
        }

        [Test]
        public void TestTarjetaNuevaTieneSaldoCero()
        {
            Assert.AreEqual(0, tarjeta.ObtenerSaldo());
        }

        [TestCase(2000)]
        [TestCase(3000)]
        [TestCase(4000)]
        [TestCase(5000)]
        [TestCase(8000)]
        [TestCase(10000)]
        [TestCase(15000)]
        [TestCase(20000)]
        [TestCase(25000)]
        [TestCase(30000)]
        public void TestCargarMontosValidosActualizaSaldo(decimal monto)
        {
            tarjeta.Cargar(monto);
            Assert.AreEqual(monto, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestCargarMontosValidosMultiples()
        {
            tarjeta.Cargar(5000);
            tarjeta.Cargar(3000);
            tarjeta.Cargar(2000);
            Assert.AreEqual(10000, tarjeta.ObtenerSaldo());
        }

        [TestCase(1000)]
        [TestCase(1500)]
        [TestCase(6000)]
        [TestCase(12000)]
        [TestCase(35000)]
        public void TestCargarMontoInvalidoLanzaExcepcion(decimal montoInvalido)
        {
            Assert.Throws<ArgumentException>(() => tarjeta.Cargar(montoInvalido));
        }

        [Test]
        public void TestCargarExcedeLimiteSaldoQuedaPendiente()
        {
            // Cargar hasta el límite usando cargas válidas
            tarjeta.Cargar(30000);
            tarjeta.Cargar(20000);
            tarjeta.Cargar(3000);
            tarjeta.Cargar(3000);

            // El saldo ahora es 56000 (límite máximo)
            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());

            // Cargar más: queda pendiente en lugar de lanzar excepción
            tarjeta.Cargar(2000);
            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());
            Assert.AreEqual(2000, tarjeta.ObtenerSaldoPendiente());
        }

        [Test]
        public void TestCargarHastaLimiteSaldo()
        {
            tarjeta.Cargar(30000);
            tarjeta.Cargar(20000);
            tarjeta.Cargar(3000);
            tarjeta.Cargar(3000);
            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestPuedeDescontarConSaldoSuficiente()
        {
            tarjeta.Cargar(5000);
            Assert.IsTrue(tarjeta.PuedeDescontar(1580));
            Assert.IsTrue(tarjeta.PuedeDescontar(3000));
            Assert.IsTrue(tarjeta.PuedeDescontar(5000));
        }

        [Test]
        public void TestNoPuedeDescontarConSaldoInsuficiente()
        {
            tarjeta.Cargar(2000);
            // Con saldo negativo permitido hasta -1200, sí puede descontar más del saldo actual
            // pero no puede exceder el límite de -1200
            Assert.IsTrue(tarjeta.PuedeDescontar(3000)); // 2000 - 3000 = -1000 (permitido)
            Assert.IsFalse(tarjeta.PuedeDescontar(3201)); // 2000 - 3201 = -1201 (no permitido)
        }

        [Test]
        public void TestDescontarConSaldoSuficiente()
        {
            tarjeta.Cargar(5000);
            tarjeta.Descontar(1580);
            Assert.AreEqual(3420, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestDescontarConSaldoInsuficienteLanzaExcepcion()
        {
            tarjeta.Cargar(2000);
            // Ahora puede descontar hasta quedar en -1200
            // Solo lanza excepción si excede el límite
            Assert.Throws<InvalidOperationException>(() => tarjeta.Descontar(3201));
        }

        [Test]
        public void TestDescontarTarifaBasicaMultiplesVeces()
        {
            tarjeta.Cargar(10000);
            tarjeta.Descontar(1580); // Saldo: 8420
            tarjeta.Descontar(1580); // Saldo: 6840
            tarjeta.Descontar(1580); // Saldo: 5260
            Assert.AreEqual(5260, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestDescontarHastaSaldoNegativoPermitido()
        {
            tarjeta.Cargar(2000);
            tarjeta.Descontar(3000); // Saldo: -1000 (permitido)
            Assert.AreEqual(-1000, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestDescontarExcedeLimiteNegativoLanzaExcepcion()
        {
            // Intentar descontar más del límite permitido desde saldo 0
            Assert.Throws<InvalidOperationException>(() => tarjeta.Descontar(1201));
        }
    }
}