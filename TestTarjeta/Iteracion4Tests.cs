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

        // MANTENER: Método de la rama feature/8-restricciones-horarias
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
            // MANTENER: Validación de horario de la rama feature/8-restricciones-horarias
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

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

        #region Tests Issue #8: Restricciones horarias para franquicias

        [Test]
        public void TestMedioBoletoNoPermiteViajesFueraDeHorario()
        {
            DateTime ahora = DateTime.Now;
            
            // Solo ejecutar si estamos fuera del horario permitido
            if (ahora.DayOfWeek != DayOfWeek.Saturday && 
                ahora.DayOfWeek != DayOfWeek.Sunday && 
                ahora.Hour >= 6 && ahora.Hour < 22)
            {
                Assert.Ignore("Test solo válido fuera del horario L-V 6-22");
            }

            MedioBoleto medioBoleto = new MedioBoleto();
            medioBoleto.Cargar(5000);

            // Intentar pagar debería fallar fuera del horario
            bool resultado = colectivo.TryPagarCon(medioBoleto, out Boleto boleto);
            Assert.IsFalse(resultado);
            Assert.IsNull(boleto);
        }

        [Test]
        public void TestBoletoGratuitoNoPermiteViajesFueraDeHorario()
        {
            DateTime ahora = DateTime.Now;
            
            if (ahora.DayOfWeek != DayOfWeek.Saturday && 
                ahora.DayOfWeek != DayOfWeek.Sunday && 
                ahora.Hour >= 6 && ahora.Hour < 22)
            {
                Assert.Ignore("Test solo válido fuera del horario L-V 6-22");
            }

            BoletoGratuito boletoGratuito = new BoletoGratuito();

            bool resultado = colectivo.TryPagarCon(boletoGratuito, out Boleto boleto);
            Assert.IsFalse(resultado);
            Assert.IsNull(boleto);
        }

        [Test]
        public void TestFranquiciaCompletaNoPermiteViajesFueraDeHorario()
        {
            DateTime ahora = DateTime.Now;
            
            if (ahora.DayOfWeek != DayOfWeek.Saturday && 
                ahora.DayOfWeek != DayOfWeek.Sunday && 
                ahora.Hour >= 6 && ahora.Hour < 22)
            {
                Assert.Ignore("Test solo válido fuera del horario L-V 6-22");
            }

            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            bool resultado = colectivo.TryPagarCon(franquicia, out Boleto boleto);
            Assert.IsFalse(resultado);
            Assert.IsNull(boleto);
        }

        [Test]
        public void TestFranquiciasPermitenViajesDentroDeHorario()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            MedioBoleto medioBoleto = new MedioBoleto();
            BoletoGratuito boletoGratuito = new BoletoGratuito();
            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            medioBoleto.Cargar(5000);

            // Todos deberían poder viajar dentro del horario
            Boleto b1 = colectivo.PagarCon(medioBoleto);
            Boleto b2 = colectivo.PagarCon(boletoGratuito);
            Boleto b3 = colectivo.PagarCon(franquicia);

            Assert.IsNotNull(b1);
            Assert.IsNotNull(b2);
            Assert.IsNotNull(b3);
        }

        #endregion

        #region Tests Issue #9: Líneas interurbanas

        [Test]
        public void TestColectivoInterurbanoTarifaNormal()
        {
            ColectivoInterurbano interurbano = new ColectivoInterurbano("Gálvez");
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            Boleto boleto = interurbano.PagarCon(tarjeta);

            Assert.AreEqual(3000, boleto.ObtenerTarifa());
            Assert.AreEqual(2000, tarjeta.ObtenerSaldo());
            Assert.AreEqual("Gálvez", boleto.ObtenerLinea());
        }

        [Test]
        public void TestColectivoInterurbanoMedioBoleto()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            ColectivoInterurbano interurbano = new ColectivoInterurbano("Baigorria");
            MedioBoleto medioBoleto = new MedioBoleto();
            medioBoleto.Cargar(5000);

            Boleto boleto = interurbano.PagarCon(medioBoleto);

            // MedioBoleto paga 50% de $3000 = $1500
            Assert.AreEqual(1500, boleto.ObtenerTarifa());
            Assert.AreEqual(3500, medioBoleto.ObtenerSaldo());
        }

        [Test]
        public void TestColectivoInterurbanoFranquiciaCompleta()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            ColectivoInterurbano interurbano = new ColectivoInterurbano("Villa Gobernador Gálvez");
            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            Boleto boleto = interurbano.PagarCon(franquicia);

            // FranquiciaCompleta no paga
            Assert.AreEqual(0, boleto.ObtenerTarifa());
            Assert.AreEqual(0, franquicia.ObtenerSaldo());
        }

        [Test]
        public void TestColectivoInterurbanoBoletoGratuito()
        {
            if (!EsHorarioValidoParaFranquicias())
            {
                Assert.Ignore("Test solo válido L-V entre 6:00 y 22:00");
            }

            ColectivoInterurbano interurbano = new ColectivoInterurbano("Capitán Bermúdez");
            BoletoGratuito boletoGratuito = new BoletoGratuito();
            boletoGratuito.Cargar(5000);

            // Primer viaje: gratis
            Boleto boleto1 = interurbano.PagarCon(boletoGratuito);
            Assert.AreEqual(0, boleto1.ObtenerTarifa());

            // Segundo viaje: gratis
            Boleto boleto2 = interurbano.PagarCon(boletoGratuito);
            Assert.AreEqual(0, boleto2.ObtenerTarifa());

            // Tercer viaje: paga tarifa completa interurbana
            Boleto boleto3 = interurbano.PagarCon(boletoGratuito);
            Assert.AreEqual(3000, boleto3.ObtenerTarifa());
            Assert.AreEqual(2000, boletoGratuito.ObtenerSaldo());
        }

        [Test]
        public void TestColectivoInterurbanoUsoFrecuente()
        {
            ColectivoInterurbano interurbano = new ColectivoInterurbano("Funes");
            Tarjeta tarjeta = new Tarjeta();
            
            // Cargar solo montos válidos
            // 29 viajes * 3000 = 87000
            // Usando cargas válidas: 30000 + 30000 + 30000 = 90000
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000); // Total: 90000

            // Hacer 29 viajes interurbanos
            for (int i = 1; i <= 29; i++)
            {
                Boleto boleto = interurbano.PagarCon(tarjeta);
                Assert.AreEqual(3000, boleto.ObtenerTarifa(), $"Viaje {i}");
            }

            // Viaje 30: 20% descuento sobre $3000 = $2400
            Boleto boleto30 = interurbano.PagarCon(tarjeta);
            Assert.AreEqual(2400, boleto30.ObtenerTarifa());
        }

        [Test]
        public void TestColectivoInterurbanoConSaldoNegativo()
        {
            ColectivoInterurbano interurbano = new ColectivoInterurbano("Roldán");
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(4000);

            // Primer viaje: 4000 - 3000 = 1000
            interurbano.PagarCon(tarjeta);
            Assert.AreEqual(1000, tarjeta.ObtenerSaldo());

            // Segundo viaje: 1000 - 3000 = -2000
            // Esto excede el límite de -1200
            // El test debe verificar que NO se puede realizar el viaje
            bool resultado = interurbano.TryPagarCon(tarjeta, out Boleto boleto);
            Assert.IsFalse(resultado, "No debería poder pagar porque excede el límite de saldo negativo");
            Assert.IsNull(boleto);
            Assert.AreEqual(1000, tarjeta.ObtenerSaldo(), "El saldo no debería cambiar si el pago falla");
        }

        #endregion
    }
}