using NUnit.Framework;
using System;
using TrabajoTarjeta;

namespace TestTarjeta
{
    [TestFixture]
    public class Iteracion4Tests
    {
        private Colectivo colectivo;

        [SetUp]
        public void SetUp()
        {
            colectivo = new Colectivo("102 Rojo");
        }

        #region Tests Issue #7: Boleto de uso frecuente

        [Test]
        public void TestTarifaNormalPrimeros29Viajes()
        {
            Tarjeta tarjeta = new Tarjeta();

            // Cargar suficiente saldo para 29 viajes
            // 29 * 1580 = 45820
            tarjeta.Cargar(30000);
            tarjeta.Cargar(20000); // Total: 50000

            // Primeros 29 viajes: tarifa normal $1580
            for (int i = 1; i <= 29; i++)
            {
                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.AreEqual(1580, boleto.ObtenerTarifa(), $"Viaje {i} debería costar $1580");
            }
        }

        [Test]
        public void TestDescuento20PorCientoViajes30A59()
        {
            Tarjeta tarjeta = new Tarjeta();

            // Cargar saldo suficiente
            // 29 viajes normales: 29 * 1580 = 45820
            // 30 viajes con descuento: 30 * 1264 = 37920
            // Total: 83740
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);
            tarjeta.Cargar(25000); // Total: 85000

            // Hacer 29 viajes para llegar al rango de descuento
            for (int i = 1; i <= 29; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // Viaje 30: 20% descuento = $1264
            Boleto boleto30 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(1264, boleto30.ObtenerTarifa());

            // Viaje 50: todavía 20% descuento
            for (int i = 31; i <= 49; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            Boleto boleto50 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(1264, boleto50.ObtenerTarifa());

            // Viaje 59: último con 20% descuento
            for (int i = 51; i <= 58; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            Boleto boleto59 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(1264, boleto59.ObtenerTarifa());
        }

        [Test]
        public void TestDescuento25PorCientoViajes60A80()
        {
            Tarjeta tarjeta = new Tarjeta();

            // Cargar saldo muy grande
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000); // Total: 120000 (pendiente: 64000)

            // Hacer 59 viajes
            for (int i = 1; i <= 59; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // Viaje 60: 25% descuento = $1185
            Boleto boleto60 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(1185, boleto60.ObtenerTarifa());

            // Viaje 70: todavía 25% descuento
            for (int i = 61; i <= 69; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            Boleto boleto70 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(1185, boleto70.ObtenerTarifa());

            // Viaje 80: último con 25% descuento
            for (int i = 71; i <= 79; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            Boleto boleto80 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(1185, boleto80.ObtenerTarifa());
        }

        [Test]
        public void TestTarifaNormalDespuesDeViaje80()
        {
            Tarjeta tarjeta = new Tarjeta();

            // Cargar saldo muy grande
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000); // Total: 120000 (pendiente: 64000)

            // Hacer 80 viajes
            for (int i = 1; i <= 80; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // Viaje 81: vuelve a tarifa normal $1580
            Boleto boleto81 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(1580, boleto81.ObtenerTarifa());

            // Viaje 85: sigue tarifa normal
            for (int i = 82; i <= 84; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            Boleto boleto85 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(1580, boleto85.ObtenerTarifa());
        }

        [Test]
        public void TestObtenerViajesMesActual()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(10000);

            Assert.AreEqual(0, tarjeta.ObtenerViajesMesActual());

            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(1, tarjeta.ObtenerViajesMesActual());

            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(2, tarjeta.ObtenerViajesMesActual());
        }

        [Test]
        public void TestUsoFrecuenteNoAplicaAFranquicias()
        {
            MedioBoleto medioBoleto = new MedioBoleto();
            FranquiciaCompleta franquicia = new FranquiciaCompleta();
            BoletoGratuito boletoGratuito = new BoletoGratuito();

            // Cargar saldo grande
            medioBoleto.Cargar(30000);
            medioBoleto.Cargar(30000);
            boletoGratuito.Cargar(30000);
            boletoGratuito.Cargar(30000);

            // MedioBoleto: primer viaje siempre $790 (no se aplica descuento por uso frecuente)
            Boleto boletoMedio1 = colectivo.PagarCon(medioBoleto);
            Assert.AreEqual(790, boletoMedio1.ObtenerTarifa());

            // FranquiciaCompleta: siempre $0
            Boleto boletoFranq = colectivo.PagarCon(franquicia);
            Assert.AreEqual(0, boletoFranq.ObtenerTarifa());

            // BoletoGratuito: primeros 2 gratis
            Boleto boletoGrat1 = colectivo.PagarCon(boletoGratuito);
            Assert.AreEqual(0, boletoGrat1.ObtenerTarifa());

            Boleto boletoGrat2 = colectivo.PagarCon(boletoGratuito);
            Assert.AreEqual(0, boletoGrat2.ObtenerTarifa());

            // Tercer viaje: $1580 (sin descuento por uso frecuente)
            Boleto boletoGrat3 = colectivo.PagarCon(boletoGratuito);
            Assert.AreEqual(1580, boletoGrat3.ObtenerTarifa());
        }

        #endregion
    }
}