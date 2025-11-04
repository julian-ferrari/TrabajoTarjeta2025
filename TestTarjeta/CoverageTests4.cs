using NUnit.Framework;
using System;
using TrabajoTarjeta;

namespace TestTarjeta
{
    /// <summary>
    /// Tests que SIEMPRE se ejecutan sin importar el horario.
    /// Usan lógica condicional DENTRO del test para cubrir diferentes casos.
    /// </summary>
    [TestFixture]
    public class GuaranteedExecutionTests
    {
        #region Tests que SIEMPRE se ejecutan para FranquiciaCompleta

        [Test]
        public void TestFranquiciaCompleta_PuedeDescontar_CubreTodosLosCasos()
        {
            // Este test SIEMPRE se ejecuta y cubre múltiples ramas
            FranquiciaCompleta franquicia = new FranquiciaCompleta();
            DateTime ahora = DateTime.Now;

            // Llamar a PuedeDescontar ejecuta EsHorarioPermitido
            bool resultado = franquicia.PuedeDescontar(0);

            // Verificar resultado según el horario actual
            bool esHorarioValido = ahora.DayOfWeek != DayOfWeek.Saturday &&
                                   ahora.DayOfWeek != DayOfWeek.Sunday &&
                                   ahora.Hour >= 6 && ahora.Hour < 22;

            if (esHorarioValido)
            {
                // En horario válido debe retornar true
                Assert.IsTrue(resultado, "En horario L-V 6-22 debe retornar true");
            }
            else
            {
                // Fuera de horario debe retornar false
                Assert.IsFalse(resultado, "Fuera de horario debe retornar false");
            }
        }

        [Test]
        public void TestFranquiciaCompleta_PuedeDescontar_ConDiferentesMontos()
        {
            // Este test ejecuta PuedeDescontar múltiples veces
            FranquiciaCompleta franquicia = new FranquiciaCompleta();
            DateTime ahora = DateTime.Now;

            // Probar con diferentes montos
            bool r1 = franquicia.PuedeDescontar(0);
            bool r2 = franquicia.PuedeDescontar(1580);
            bool r3 = franquicia.PuedeDescontar(3000);

            bool esHorarioValido = ahora.DayOfWeek != DayOfWeek.Saturday &&
                                   ahora.DayOfWeek != DayOfWeek.Sunday &&
                                   ahora.Hour >= 6 && ahora.Hour < 22;

            // Todos deben dar el mismo resultado según el horario
            Assert.AreEqual(r1, r2);
            Assert.AreEqual(r2, r3);

            if (esHorarioValido)
            {
                Assert.IsTrue(r1 && r2 && r3);
            }
            else
            {
                Assert.IsFalse(r1 || r2 || r3);
            }
        }

        [Test]
        public void TestFranquiciaCompleta_MultiplesPagosSeguidos()
        {
            // Test de integración que ejecuta múltiples métodos
            DateTime ahora = DateTime.Now;
            bool esHorarioValido = ahora.DayOfWeek != DayOfWeek.Saturday &&
                                   ahora.DayOfWeek != DayOfWeek.Sunday &&
                                   ahora.Hour >= 6 && ahora.Hour < 22;

            if (!esHorarioValido)
            {
                // Si no es horario válido, verificar que PuedeDescontar retorna false
                FranquiciaCompleta franquicia = new FranquiciaCompleta();
                Assert.IsFalse(franquicia.PuedeDescontar(0));
                return;
            }

            // Si es horario válido, hacer múltiples pagos
            FranquiciaCompleta franquicia2 = new FranquiciaCompleta();
            Colectivo colectivo = new Colectivo("102");

            // Múltiples viajes
            for (int i = 0; i < 5; i++)
            {
                Boleto boleto = colectivo.PagarCon(franquicia2);
                Assert.AreEqual(0, boleto.ObtenerTarifa());
            }
        }

        #endregion

        #region Tests que SIEMPRE se ejecutan para MedioBoleto

        [Test]
        public void TestMedioBoleto_PuedeDescontar_CubreTodosLosCasos()
        {
            MedioBoleto medio = new MedioBoleto();
            medio.Cargar(5000);
            DateTime ahora = DateTime.Now;

            bool resultado = medio.PuedeDescontar(790);

            bool esHorarioValido = ahora.DayOfWeek != DayOfWeek.Saturday &&
                                   ahora.DayOfWeek != DayOfWeek.Sunday &&
                                   ahora.Hour >= 6 && ahora.Hour < 22;

            if (esHorarioValido)
            {
                Assert.IsTrue(resultado, "Primer viaje en horario válido debe permitir");
            }
            else
            {
                Assert.IsFalse(resultado, "Fuera de horario debe rechazar");
            }
        }

        #endregion

        #region Tests que SIEMPRE se ejecutan para BoletoGratuito

        [Test]
        public void TestBoletoGratuito_PuedeDescontar_CubreTodosLosCasos()
        {
            BoletoGratuito gratuito = new BoletoGratuito();
            DateTime ahora = DateTime.Now;

            bool resultado = gratuito.PuedeDescontar(0);

            bool esHorarioValido = ahora.DayOfWeek != DayOfWeek.Saturday &&
                                   ahora.DayOfWeek != DayOfWeek.Sunday &&
                                   ahora.Hour >= 6 && ahora.Hour < 22;

            if (esHorarioValido)
            {
                Assert.IsTrue(resultado, "Monto 0 en horario válido debe retornar true");
            }
            else
            {
                Assert.IsFalse(resultado, "Fuera de horario debe rechazar");
            }
        }

        [Test]
        public void TestBoletoGratuito_FlujoCompleto_ConValidaciones()
        {
            BoletoGratuito gratuito = new BoletoGratuito();
            gratuito.Cargar(10000);
            DateTime ahora = DateTime.Now;

            bool esHorarioValido = ahora.DayOfWeek != DayOfWeek.Saturday &&
                                   ahora.DayOfWeek != DayOfWeek.Sunday &&
                                   ahora.Hour >= 6 && ahora.Hour < 22;

            if (!esHorarioValido)
            {
                Assert.IsFalse(gratuito.PuedeDescontar(0));
                return;
            }

            Colectivo colectivo = new Colectivo("102");

            // Viajes 1 y 2: gratis
            Boleto b1 = colectivo.PagarCon(gratuito);
            Boleto b2 = colectivo.PagarCon(gratuito);
            Assert.AreEqual(0, b1.ObtenerTarifa());
            Assert.AreEqual(0, b2.ObtenerTarifa());
            Assert.AreEqual(10000, gratuito.ObtenerSaldo());

            // Viaje 3: paga
            Boleto b3 = colectivo.PagarCon(gratuito);
            Assert.AreEqual(1580, b3.ObtenerTarifa());
            Assert.AreEqual(8420, gratuito.ObtenerSaldo());
        }

        [Test]
        public void TestBoletoGratuito_Descontar_ConMontoCeroYPositivo()
        {
            DateTime ahora = DateTime.Now;
            bool esHorarioValido = ahora.DayOfWeek != DayOfWeek.Saturday &&
                                   ahora.DayOfWeek != DayOfWeek.Sunday &&
                                   ahora.Hour >= 6 && ahora.Hour < 22;

            if (!esHorarioValido)
            {
                BoletoGratuito gratuito = new BoletoGratuito();
                Assert.IsFalse(gratuito.PuedeDescontar(0));
                return;
            }

            BoletoGratuito gratuito2 = new BoletoGratuito();
            gratuito2.Cargar(5000);

            // Descontar 0 (viajes gratuitos)
            gratuito2.Descontar(0);
            Assert.AreEqual(5000, gratuito2.ObtenerSaldo());

            gratuito2.Descontar(0);
            Assert.AreEqual(5000, gratuito2.ObtenerSaldo());

            // Descontar monto positivo
            gratuito2.Descontar(1580);
            Assert.AreEqual(3420, gratuito2.ObtenerSaldo());
        }

        [Test]
        public void TestBoletoGratuito_CalcularTarifa_TodasLasRamas()
        {
            BoletoGratuito gratuito = new BoletoGratuito();

            // Primeras 2 tarifas: gratis
            decimal tarifa1 = gratuito.CalcularTarifa(1580);
            Assert.AreEqual(0, tarifa1);

            DateTime ahora = DateTime.Now;
            bool esHorarioValido = ahora.DayOfWeek != DayOfWeek.Saturday &&
                                   ahora.DayOfWeek != DayOfWeek.Sunday &&
                                   ahora.Hour >= 6 && ahora.Hour < 22;

            if (!esHorarioValido)
            {
                return;
            }

            Colectivo colectivo = new Colectivo("102");

            // Hacer 2 viajes gratis
            colectivo.PagarCon(gratuito);
            colectivo.PagarCon(gratuito);

            // Tercera tarifa: completa
            decimal tarifa3 = gratuito.CalcularTarifa(1580);
            Assert.AreEqual(1580, tarifa3);
        }

        #endregion

        #region Tests de integración que ejecutan MÚLTIPLES líneas

        [Test]
        public void TestIntegracion_TodasLasFranquicias_EnColectivo()
        {
            DateTime ahora = DateTime.Now;
            bool esHorarioValido = ahora.DayOfWeek != DayOfWeek.Saturday &&
                                   ahora.DayOfWeek != DayOfWeek.Sunday &&
                                   ahora.Hour >= 6 && ahora.Hour < 22;

            if (!esHorarioValido)
            {
                // Verificar que todas las franquicias rechazan fuera de horario
                MedioBoleto medio = new MedioBoleto();
                BoletoGratuito gratuito = new BoletoGratuito();
                FranquiciaCompleta franquicia = new FranquiciaCompleta();

                medio.Cargar(5000);

                Assert.IsFalse(medio.PuedeDescontar(790));
                Assert.IsFalse(gratuito.PuedeDescontar(0));
                Assert.IsFalse(franquicia.PuedeDescontar(0));
                return;
            }

            // Si es horario válido, probar todas las franquicias
            MedioBoleto medio2 = new MedioBoleto();
            BoletoGratuito gratuito2 = new BoletoGratuito();
            FranquiciaCompleta franquicia2 = new FranquiciaCompleta();

            medio2.Cargar(5000);
            gratuito2.Cargar(5000);

            Colectivo colectivo = new Colectivo("102");

            // Medio boleto: paga 790
            Boleto bm = colectivo.PagarCon(medio2);
            Assert.AreEqual(790, bm.ObtenerTarifa());

            // Boleto gratuito: gratis
            Boleto bg = colectivo.PagarCon(gratuito2);
            Assert.AreEqual(0, bg.ObtenerTarifa());

            // Franquicia completa: gratis
            Boleto bf = colectivo.PagarCon(franquicia2);
            Assert.AreEqual(0, bf.ObtenerTarifa());
        }

        [Test]
        public void TestIntegracion_FranquiciasConInterurbano()
        {
            DateTime ahora = DateTime.Now;
            bool esHorarioValido = ahora.DayOfWeek != DayOfWeek.Saturday &&
                                   ahora.DayOfWeek != DayOfWeek.Sunday &&
                                   ahora.Hour >= 6 && ahora.Hour < 22;

            if (!esHorarioValido)
            {
                return;
            }

            ColectivoInterurbano interurbano = new ColectivoInterurbano("Gálvez");

            MedioBoleto medio = new MedioBoleto();
            BoletoGratuito gratuito = new BoletoGratuito();
            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            medio.Cargar(5000);
            gratuito.Cargar(10000);

            // Medio boleto: 1500
            Boleto bm = interurbano.PagarCon(medio);
            Assert.AreEqual(1500, bm.ObtenerTarifa());

            // Boleto gratuito: gratis
            Boleto bg1 = interurbano.PagarCon(gratuito);
            Boleto bg2 = interurbano.PagarCon(gratuito);
            Assert.AreEqual(0, bg1.ObtenerTarifa());
            Assert.AreEqual(0, bg2.ObtenerTarifa());

            // Tercer viaje de gratuito: paga
            Boleto bg3 = interurbano.PagarCon(gratuito);
            Assert.AreEqual(3000, bg3.ObtenerTarifa());

            // Franquicia: gratis
            Boleto bf = interurbano.PagarCon(franquicia);
            Assert.AreEqual(0, bf.ObtenerTarifa());
        }

        [Test]
        public void TestIntegracion_MultiplesLlamadasAPuedeDescontar()
        {
            // Este test hace MUCHAS llamadas a PuedeDescontar para maximizar cobertura
            DateTime ahora = DateTime.Now;
            bool esHorarioValido = ahora.DayOfWeek != DayOfWeek.Saturday &&
                                   ahora.DayOfWeek != DayOfWeek.Sunday &&
                                   ahora.Hour >= 6 && ahora.Hour < 22;

            MedioBoleto medio = new MedioBoleto();
            BoletoGratuito gratuito = new BoletoGratuito();
            FranquiciaCompleta franquicia = new FranquiciaCompleta();

            medio.Cargar(10000);
            gratuito.Cargar(10000);

            // Hacer múltiples llamadas a PuedeDescontar
            for (int i = 0; i < 5; i++)
            {
                bool pm = medio.PuedeDescontar(790);
                bool pg = gratuito.PuedeDescontar(0);
                bool pf = franquicia.PuedeDescontar(0);

                if (esHorarioValido)
                {
                    // En horario válido, el primero debe poder
                    if (i == 0)
                    {
                        Assert.IsTrue(pm);
                    }
                    Assert.IsTrue(pg);
                    Assert.IsTrue(pf);
                }
                else
                {
                    Assert.IsFalse(pm);
                    Assert.IsFalse(pg);
                    Assert.IsFalse(pf);
                }
            }
        }

        #endregion
    }
}