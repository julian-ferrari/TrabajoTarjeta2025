using NUnit.Framework;
using System;
using TrabajoTarjeta;

namespace TestTarjeta
{
    /// <summary>
    /// Tests con TIEMPO FALSO que garantizan 90%+ de cobertura.
    /// Estos tests SIEMPRE se ejecutan sin importar la hora real.
    /// </summary>
    [TestFixture]
    public class CoverageTests_TiempoFalso
    {
        #region Tests de BoletoGratuito con todas las validaciones horarias

        [Test]
        public void TestBoletoGratuito_HorarioValido_PermiteViaje()
        {
            // Lunes 10:00 - horario válido
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var gratuito = new BoletoGratuito(tiempo);
            var colectivo = new Colectivo("102");

            // Puede descontar en horario válido
            Assert.IsTrue(gratuito.PuedeDescontar(0));

            // Primer y segundo viaje gratis
            Boleto b1 = colectivo.PagarCon(gratuito);
            Boleto b2 = colectivo.PagarCon(gratuito);
            Assert.AreEqual(0, b1.ObtenerTarifa());
            Assert.AreEqual(0, b2.ObtenerTarifa());

            // Tercer viaje paga
            gratuito.Cargar(5000);
            Boleto b3 = colectivo.PagarCon(gratuito);
            Assert.AreEqual(1580, b3.ObtenerTarifa());
        }

        [Test]
        public void TestBoletoGratuito_Sabado_NoPermiteViaje()
        {
            // Sábado 10:00 - NO válido
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 12, 10, 0, 0));
            var gratuito = new BoletoGratuito(tiempo);

            // Cubre: if (fecha.DayOfWeek == DayOfWeek.Saturday) { return false; }
            Assert.IsFalse(gratuito.PuedeDescontar(0));
        }

        [Test]
        public void TestBoletoGratuito_Domingo_NoPermiteViaje()
        {
            // Domingo 10:00 - NO válido
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 13, 10, 0, 0));
            var gratuito = new BoletoGratuito(tiempo);

            // Cubre: if (fecha.DayOfWeek == DayOfWeek.Sunday) { return false; }
            Assert.IsFalse(gratuito.PuedeDescontar(0));
        }

        [Test]
        public void TestBoletoGratuito_Antes6AM_NoPermiteViaje()
        {
            // Lunes 5:00 - NO válido
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 5, 0, 0));
            var gratuito = new BoletoGratuito(tiempo);

            // Cubre: if (hora < 6) { return false; }
            Assert.IsFalse(gratuito.PuedeDescontar(0));
        }

        [Test]
        public void TestBoletoGratuito_Despues22_NoPermiteViaje()
        {
            // Lunes 22:00 - NO válido
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 22, 0, 0));
            var gratuito = new BoletoGratuito(tiempo);

            // Cubre: if (hora >= 22) { return false; }
            Assert.IsFalse(gratuito.PuedeDescontar(0));
        }

        [Test]
        public void TestBoletoGratuito_MontoCero_RetornaTrue()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var gratuito = new BoletoGratuito(tiempo);

            // Cubre: if (monto == 0) { return true; }
            Assert.IsTrue(gratuito.PuedeDescontar(0));
        }

        [Test]
        public void TestBoletoGratuito_Descontar_ConMontoCero_IncrementaContador()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var gratuito = new BoletoGratuito(tiempo);

            // Cubre: if (monto == 0) { viajesGratuitosHoy++; }
            gratuito.Descontar(0);
            gratuito.Descontar(0);

            // Después de 2 viajes gratis, debe cobrar
            decimal tarifa = gratuito.CalcularTarifa(1580);
            Assert.AreEqual(1580, tarifa);
        }

        [Test]
        public void TestBoletoGratuito_Descontar_ConMontoPositivo_LlamaBase()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var gratuito = new BoletoGratuito(tiempo);
            gratuito.Cargar(5000);

            // Cubre: base.Descontar(monto);
            gratuito.Descontar(1580);
            Assert.AreEqual(3420, gratuito.ObtenerSaldo());
        }

        [Test]
        public void TestBoletoGratuito_ActualizarContador_SinFechaPrevia()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var gratuito = new BoletoGratuito(tiempo);

            // Cubre: if (!fechaUltimosViajes.HasValue || fechaActual.Date > fechaUltimosViajes.Value)
            decimal tarifa = gratuito.CalcularTarifa(1580);
            Assert.AreEqual(0, tarifa);
        }

        [Test]
        public void TestBoletoGratuito_CalcularTarifa_DespuesDe2Viajes()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var gratuito = new BoletoGratuito(tiempo);
            var colectivo = new Colectivo("102");

            colectivo.PagarCon(gratuito);
            colectivo.PagarCon(gratuito);

            // Cubre: if (viajesGratuitosHoy >= MAX_VIAJES_GRATUITOS_POR_DIA) { return tarifaBase; }
            decimal tarifa = gratuito.CalcularTarifa(1580);
            Assert.AreEqual(1580, tarifa);
        }

        #endregion

        #region Tests de MedioBoleto con todas las validaciones

        [Test]
        public void TestMedioBoleto_HorarioValido_PermiteViaje()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var medio = new MedioBoleto(tiempo);
            medio.Cargar(5000);

            // Cubre: return true; en EsHorarioPermitido
            Assert.IsTrue(medio.PuedeDescontar(790));

            // Primer viaje
            var colectivo = new Colectivo("102");
            Boleto boleto = colectivo.PagarCon(medio);
            Assert.AreEqual(790, boleto.ObtenerTarifa());
        }

        [Test]
        public void TestMedioBoleto_Sabado_NoPermiteViaje()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 12, 10, 0, 0));
            var medio = new MedioBoleto(tiempo);
            medio.Cargar(5000);

            Assert.IsFalse(medio.PuedeDescontar(790));
        }

        [Test]
        public void TestMedioBoleto_Domingo_NoPermiteViaje()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 13, 10, 0, 0));
            var medio = new MedioBoleto(tiempo);
            medio.Cargar(5000);

            Assert.IsFalse(medio.PuedeDescontar(790));
        }

        [Test]
        public void TestMedioBoleto_Antes6AM_NoPermiteViaje()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 5, 0, 0));
            var medio = new MedioBoleto(tiempo);
            medio.Cargar(5000);

            Assert.IsFalse(medio.PuedeDescontar(790));
        }

        [Test]
        public void TestMedioBoleto_Despues22_NoPermiteViaje()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 22, 0, 0));
            var medio = new MedioBoleto(tiempo);
            medio.Cargar(5000);

            Assert.IsFalse(medio.PuedeDescontar(790));
        }

        [Test]
        public void TestMedioBoleto_SegundoViajeInmediato_NoPermite()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var medio = new MedioBoleto(tiempo);
            medio.Cargar(5000);

            // Primer viaje
            medio.Descontar(790);

            // Cubre: if (tiempoTranscurrido.TotalMinutes < MINUTOS_ENTRE_VIAJES) { return false; }
            Assert.IsFalse(medio.PuedeDescontar(790));
        }

        [Test]
        public void TestMedioBoleto_DespuesDe5Minutos_PermiteViaje()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var medio = new MedioBoleto(tiempo);
            medio.Cargar(5000);
            var colectivo = new Colectivo("102");

            // Primer viaje
            colectivo.PagarCon(medio);

            // Avanzar 5 minutos
            tiempo.AgregarMinutos(5);

            // Segundo viaje debe ser posible
            Assert.IsTrue(medio.PuedeDescontar(790));
        }

        [Test]
        public void TestMedioBoleto_Descontar_ConMontoMenorA1580()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var medio = new MedioBoleto(tiempo);
            medio.Cargar(5000);

            // Cubre: if (monto < 1580) { viajesConDescuentoHoy++; }
            medio.Descontar(790);
            Assert.AreEqual(4210, medio.ObtenerSaldo());
        }

        [Test]
        public void TestMedioBoleto_ActualizarContador_SinFechaPrevia()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var medio = new MedioBoleto(tiempo);

            // Cubre: if (!fechaUltimosViajes.HasValue...)
            decimal tarifa = medio.CalcularTarifa(1580);
            Assert.AreEqual(790, tarifa);
        }

        [Test]
        public void TestMedioBoleto_PuedeDescontar_SinViajePrevio_RetornaBasePuedeDescontar()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var medio = new MedioBoleto(tiempo);
            medio.Cargar(5000);

            // Cubre: return base.PuedeDescontar(monto);
            Assert.IsTrue(medio.PuedeDescontar(790));
        }

        #endregion

        #region Tests de FranquiciaCompleta con todas las validaciones

        [Test]
        public void TestFranquiciaCompleta_HorarioValido_PermiteViaje()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var franquicia = new FranquiciaCompleta(tiempo);

            // Cubre: return true;
            Assert.IsTrue(franquicia.PuedeDescontar(0));
        }

        [Test]
        public void TestFranquiciaCompleta_Sabado_NoPermiteViaje()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 12, 10, 0, 0));
            var franquicia = new FranquiciaCompleta(tiempo);

            Assert.IsFalse(franquicia.PuedeDescontar(0));
        }

        [Test]
        public void TestFranquiciaCompleta_Domingo_NoPermiteViaje()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 13, 10, 0, 0));
            var franquicia = new FranquiciaCompleta(tiempo);

            Assert.IsFalse(franquicia.PuedeDescontar(0));
        }

        [Test]
        public void TestFranquiciaCompleta_Antes6AM_NoPermiteViaje()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 5, 0, 0));
            var franquicia = new FranquiciaCompleta(tiempo);

            Assert.IsFalse(franquicia.PuedeDescontar(0));
        }

        [Test]
        public void TestFranquiciaCompleta_Despues22_NoPermiteViaje()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 22, 0, 0));
            var franquicia = new FranquiciaCompleta(tiempo);

            Assert.IsFalse(franquicia.PuedeDescontar(0));
        }

        [Test]
        public void TestFranquiciaCompleta_CalcularTarifa_SiempreCero()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var franquicia = new FranquiciaCompleta(tiempo);

            // Cubre: return 0;
            Assert.AreEqual(0, franquicia.CalcularTarifa(1580));
            Assert.AreEqual(0, franquicia.CalcularTarifa(3000));
        }

        #endregion

        #region Tests de integración con tiempo falso

        [Test]
        public void TestIntegracion_TodasLasFranquicias_HorarioValido()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var medio = new MedioBoleto(tiempo);
            var gratuito = new BoletoGratuito(tiempo);
            var franquicia = new FranquiciaCompleta(tiempo);

            medio.Cargar(5000);
            gratuito.Cargar(5000);

            var colectivo = new Colectivo("102");

            // MedioBoleto: paga 790
            Boleto bm = colectivo.PagarCon(medio);
            Assert.AreEqual(790, bm.ObtenerTarifa());

            // BoletoGratuito: primeros 2 gratis
            Boleto bg1 = colectivo.PagarCon(gratuito);
            Boleto bg2 = colectivo.PagarCon(gratuito);
            Boleto bg3 = colectivo.PagarCon(gratuito);
            Assert.AreEqual(0, bg1.ObtenerTarifa());
            Assert.AreEqual(0, bg2.ObtenerTarifa());
            Assert.AreEqual(1580, bg3.ObtenerTarifa());

            // FranquiciaCompleta: siempre gratis
            Boleto bf = colectivo.PagarCon(franquicia);
            Assert.AreEqual(0, bf.ObtenerTarifa());
        }

        [Test]
        public void TestIntegracion_TodasLasFranquicias_FueraDeHorario()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 12, 10, 0, 0)); // Sábado
            var medio = new MedioBoleto(tiempo);
            var gratuito = new BoletoGratuito(tiempo);
            var franquicia = new FranquiciaCompleta(tiempo);

            medio.Cargar(5000);

            // Todas deben rechazar
            Assert.IsFalse(medio.PuedeDescontar(790));
            Assert.IsFalse(gratuito.PuedeDescontar(0));
            Assert.IsFalse(franquicia.PuedeDescontar(0));
        }

        [Test]
        public void TestIntegracion_CambioDeDia_ResetaContadores()
        {
            var tiempo = new TiempoFalso(new DateTime(2024, 10, 14, 10, 0, 0));
            var gratuito = new BoletoGratuito(tiempo);
            var colectivo = new Colectivo("102");

            // Día 1: hacer 2 viajes gratis
            colectivo.PagarCon(gratuito);
            colectivo.PagarCon(gratuito);

            // Verificar que el tercero cobraría
            Assert.AreEqual(1580, gratuito.CalcularTarifa(1580));

            // Avanzar al día siguiente
            tiempo.AgregarDias(1);

            // Debe resetear: primero gratis de nuevo
            Assert.AreEqual(0, gratuito.CalcularTarifa(1580));
        }

        #endregion
    }
}