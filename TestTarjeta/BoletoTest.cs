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

            // Constructor actualizado con parámetro esTrasbordo
            boleto = new Boleto(
                fechaHora: fechaHoraPrueba,
                tarifa: 1580,
                saldoRestante: 3420,
                linea: "102 Rojo",
                tipoTarjeta: "Tarjeta",
                totalAbonado: 1580,
                idTarjeta: 1,
                esTrasbordo: false  // NUEVO parámetro
            );
        }

        [Test]
        public void TestCrearBoletoConParametrosValidos()
        {
            Assert.AreEqual(fechaHoraPrueba, boleto.ObtenerFechaHora());
            Assert.AreEqual(1580, boleto.ObtenerTarifa());
            Assert.AreEqual(3420, boleto.ObtenerSaldoRestante());
            Assert.AreEqual("102 Rojo", boleto.ObtenerLinea());
            Assert.AreEqual("Tarjeta", boleto.ObtenerTipoTarjeta());
            Assert.AreEqual(1580, boleto.ObtenerTotalAbonado());
            Assert.AreEqual(1, boleto.ObtenerIdTarjeta());
            Assert.IsFalse(boleto.EsTrasbordo()); // NUEVO assert
        }

        [Test]
        public void TestCrearBoletoConLineaNulaLanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Boleto(fechaHoraPrueba, 1580, 3420, null, "Tarjeta", 1580, 1, false));
        }

        [Test]
        public void TestObtenerFechaHora()
        {
            DateTime fechaEsperada = new DateTime(2024, 12, 25, 10, 15, 30);
            Boleto boletoConFecha = new Boleto(
                fechaEsperada, 1580, 2000, "144", "MedioBoleto", 790, 2, false);

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
                fechaHoraPrueba, 790, 1210, "121", "MedioBoleto", 790, 5, false);

            Assert.AreEqual("MedioBoleto", boletoMedio.ObtenerTipoTarjeta());
            Assert.AreEqual(790, boletoMedio.ObtenerTarifa());
            Assert.AreEqual(5, boletoMedio.ObtenerIdTarjeta());
            Assert.IsFalse(boletoMedio.EsTrasbordo());
        }

        [Test]
        public void TestBoletoConFranquiciaCompleta()
        {
            Boleto boletoGratis = new Boleto(
                fechaHoraPrueba, 0, 0, "144", "FranquiciaCompleta", 0, 10, false);

            Assert.AreEqual("FranquiciaCompleta", boletoGratis.ObtenerTipoTarjeta());
            Assert.AreEqual(0, boletoGratis.ObtenerTarifa());
            Assert.AreEqual(0, boletoGratis.ObtenerTotalAbonado());
        }

        [Test]
        public void TestBoletoConSaldoNegativoYTotalAbonado()
        {
            Boleto boletoConDeuda = new Boleto(
                fechaHoraPrueba, 1580, -1160, "102 Rojo", "Tarjeta", 1580, 3, false);

            Assert.AreEqual(1580, boletoConDeuda.ObtenerTarifa());
            Assert.AreEqual(1580, boletoConDeuda.ObtenerTotalAbonado());
            Assert.AreEqual(-1160, boletoConDeuda.ObtenerSaldoRestante());
        }

        [Test]
        public void TestToStringContieneTodosLosDatos()
        {
            string resultado = boleto.ToString();

            Assert.That(resultado, Does.Contain("102 Rojo"));
            Assert.That(resultado, Does.Contain("1580"));
            Assert.That(resultado, Does.Contain("3420"));
            Assert.That(resultado, Does.Contain("16/09/2024"));
            Assert.That(resultado, Does.Contain("14:30:00"));
            Assert.That(resultado, Does.Contain("Tarjeta"));
            Assert.That(resultado, Does.Contain("ID Tarjeta: 1"));
        }

        [Test]
        public void TestBoletoConDiferentesTiposDeTarjeta()
        {
            Boleto boletoNormal = new Boleto(
                fechaHoraPrueba, 1580, 8420, "121", "Tarjeta", 1580, 1, false);

            Boleto boletoMedio = new Boleto(
                fechaHoraPrueba, 790, 1210, "121", "MedioBoleto", 790, 2, false);

            Boleto boletoGratuito = new Boleto(
                fechaHoraPrueba, 0, 2000, "121", "BoletoGratuito", 0, 3, false);

            Assert.AreEqual("Tarjeta", boletoNormal.ObtenerTipoTarjeta());
            Assert.AreEqual("MedioBoleto", boletoMedio.ObtenerTipoTarjeta());
            Assert.AreEqual("BoletoGratuito", boletoGratuito.ObtenerTipoTarjeta());
        }

        [Test]
        public void TestBoletoConSaldoCero()
        {
            Boleto boletoSinSaldo = new Boleto(
                fechaHoraPrueba, 1580, 0, "144 Negro", "Tarjeta", 1580, 7, false);

            Assert.AreEqual(0, boletoSinSaldo.ObtenerSaldoRestante());
        }

        // ============================================================
        // NUEVOS TESTS: Para trasbordos
        // ============================================================

        [Test]
        public void TestBoletoTrasbordo()
        {
            Boleto boletoTrasbordo = new Boleto(
                fechaHoraPrueba, 0, 5000, "121", "Tarjeta", 0, 1, true);

            Assert.IsTrue(boletoTrasbordo.EsTrasbordo());
            Assert.AreEqual(0, boletoTrasbordo.ObtenerTarifa());
            Assert.AreEqual(0, boletoTrasbordo.ObtenerTotalAbonado());
        }

        [Test]
        public void TestBoletoNoTrasbordo()
        {
            Boleto boletoNormal = new Boleto(
                fechaHoraPrueba, 1580, 3420, "102", "Tarjeta", 1580, 1, false);

            Assert.IsFalse(boletoNormal.EsTrasbordo());
        }

        [Test]
        public void TestToStringIndicaTrasbordo()
        {
            Boleto boletoTrasbordo = new Boleto(
                fechaHoraPrueba, 0, 5000, "121", "Tarjeta", 0, 1, true);

            string resultado = boletoTrasbordo.ToString();
            Assert.That(resultado, Does.Contain("TRASBORDO"));
        }

        [Test]
        public void TestBoletoTrasbordoConParametroOpcional()
        {
            // El parámetro esTrasbordo es opcional, por defecto es false
            Boleto boletoSinParametro = new Boleto(
                fechaHoraPrueba, 1580, 3420, "102", "Tarjeta", 1580, 1);

            Assert.IsFalse(boletoSinParametro.EsTrasbordo());
        }
    }
}