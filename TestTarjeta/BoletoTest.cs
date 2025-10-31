using NUnit.Framework;
using System;
using TrabajoTarjeta;

namespace TestTarjeta
{
    [TestFixture]
    public class BoletoTest
    {
        private DateTime fechaHoraPrueba;
        private Boleto boleto;

        [SetUp]
        public void SetUp()
        {
            fechaHoraPrueba = new DateTime(2024, 9, 16, 14, 30, 0);

            // ============================================================
            // ACTUALIZADO: Constructor ahora requiere más parámetros
            // ============================================================
            boleto = new Boleto(
                fechaHora: fechaHoraPrueba,
                tarifa: 1580,
                saldoRestante: 3420,
                linea: "102 Rojo",
                tipoTarjeta: "Tarjeta",      // NUEVO
                totalAbonado: 1580,          // NUEVO
                idTarjeta: 1                 // NUEVO
            );
        }

        [Test]
        public void TestCrearBoletoConParametrosValidos()
        {
            Assert.AreEqual(fechaHoraPrueba, boleto.ObtenerFechaHora());
            Assert.AreEqual(1580, boleto.ObtenerTarifa());
            Assert.AreEqual(3420, boleto.ObtenerSaldoRestante());
            Assert.AreEqual("102 Rojo", boleto.ObtenerLinea());

            // ============================================================
            // NUEVOS ASSERTS: Verificar nuevas propiedades
            // ============================================================
            Assert.AreEqual("Tarjeta", boleto.ObtenerTipoTarjeta());
            Assert.AreEqual(1580, boleto.ObtenerTotalAbonado());
            Assert.AreEqual(1, boleto.ObtenerIdTarjeta());
        }

        [Test]
        public void TestCrearBoletoConLineaNulaLanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Boleto(fechaHoraPrueba, 1580, 3420, null, "Tarjeta", 1580, 1));
        }

        [Test]
        public void TestObtenerFechaHora()
        {
            DateTime fechaEsperada = new DateTime(2024, 12, 25, 10, 15, 30);
            Boleto boletoConFecha = new Boleto(
                fechaEsperada, 1580, 2000, "144", "MedioBoleto", 790, 2);

            Assert.AreEqual(fechaEsperada, boletoConFecha.ObtenerFechaHora());
        }

        [Test]
        public void TestObtenerTarifa()
        {
            Assert.AreEqual(1580, boleto.ObtenerTarifa());
        }

        [Test]
        public void TestObtenerSaldoRestante()
        {
            Assert.AreEqual(3420, boleto.ObtenerSaldoRestante());
        }

        [Test]
        public void TestObtenerLinea()
        {
            Assert.AreEqual("102 Rojo", boleto.ObtenerLinea());
        }

        // ============================================================
        // NUEVOS TESTS: Para las nuevas propiedades
        // ============================================================

        [Test]
        public void TestObtenerTipoTarjeta()
        {
            Assert.AreEqual("Tarjeta", boleto.ObtenerTipoTarjeta());
        }

        [Test]
        public void TestObtenerTotalAbonado()
        {
            Assert.AreEqual(1580, boleto.ObtenerTotalAbonado());
        }

        [Test]
        public void TestObtenerIdTarjeta()
        {
            Assert.AreEqual(1, boleto.ObtenerIdTarjeta());
        }

        [Test]
        public void TestBoletoConMedioBoleto()
        {
            Boleto boletoMedio = new Boleto(
                fechaHoraPrueba, 790, 1210, "121", "MedioBoleto", 790, 5);

            Assert.AreEqual("MedioBoleto", boletoMedio.ObtenerTipoTarjeta());
            Assert.AreEqual(790, boletoMedio.ObtenerTarifa());
            Assert.AreEqual(5, boletoMedio.ObtenerIdTarjeta());
        }

        [Test]
        public void TestBoletoConFranquiciaCompleta()
        {
            Boleto boletoGratis = new Boleto(
                fechaHoraPrueba, 0, 0, "144", "FranquiciaCompleta", 0, 10);

            Assert.AreEqual("FranquiciaCompleta", boletoGratis.ObtenerTipoTarjeta());
            Assert.AreEqual(0, boletoGratis.ObtenerTarifa());
            Assert.AreEqual(0, boletoGratis.ObtenerTotalAbonado());
        }

        [Test]
        public void TestBoletoConSaldoNegativoYTotalAbonado()
        {
            // Caso: Tenía saldo negativo, total abonado > tarifa
            Boleto boletoConDeuda = new Boleto(
                fechaHoraPrueba, 1580, -1160, "102 Rojo", "Tarjeta", 1580, 3);

            Assert.AreEqual(1580, boletoConDeuda.ObtenerTarifa());
            Assert.AreEqual(1580, boletoConDeuda.ObtenerTotalAbonado());
            Assert.AreEqual(-1160, boletoConDeuda.ObtenerSaldoRestante());
        }

        [Test]
        public void TestToStringContieneTodosLosDatos()
        {
            string resultado = boleto.ToString();

            // Datos originales
            Assert.That(resultado, Does.Contain("102 Rojo"));
            Assert.That(resultado, Does.Contain("1580"));
            Assert.That(resultado, Does.Contain("3420"));
            Assert.That(resultado, Does.Contain("16/09/2024"));
            Assert.That(resultado, Does.Contain("14:30:00"));

            // ============================================================
            // NUEVOS CHECKS: Verificar que incluye nueva info
            // ============================================================
            Assert.That(resultado, Does.Contain("Tarjeta"));
            Assert.That(resultado, Does.Contain("ID Tarjeta: 1"));
        }

        [Test]
        public void TestBoletoConDiferentesTiposDeTarjeta()
        {
            Boleto boletoNormal = new Boleto(
                fechaHoraPrueba, 1580, 8420, "121", "Tarjeta", 1580, 1);

            Boleto boletoMedio = new Boleto(
                fechaHoraPrueba, 790, 1210, "121", "MedioBoleto", 790, 2);

            Boleto boletoGratuito = new Boleto(
                fechaHoraPrueba, 0, 2000, "121", "BoletoGratuito", 0, 3);

            Assert.AreEqual("Tarjeta", boletoNormal.ObtenerTipoTarjeta());
            Assert.AreEqual("MedioBoleto", boletoMedio.ObtenerTipoTarjeta());
            Assert.AreEqual("BoletoGratuito", boletoGratuito.ObtenerTipoTarjeta());
        }

        [Test]
        public void TestBoletoConSaldoCero()
        {
            Boleto boletoSinSaldo = new Boleto(
                fechaHoraPrueba, 1580, 0, "144 Negro", "Tarjeta", 1580, 7);

            Assert.AreEqual(0, boletoSinSaldo.ObtenerSaldoRestante());
        }
    }
}