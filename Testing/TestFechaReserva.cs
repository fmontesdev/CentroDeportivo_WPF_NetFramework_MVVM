using ViewModel;
using Model;
using System;
using System.Collections.Generic;

namespace Testing
{
    /// <summary>
    /// Clase de pruebas unitarias para validar las fechas de reserva.
    /// Verifica que la validación de fechas acepte fechas futuras y hoy, y rechace fechas pasadas
    /// </summary>
    [TestClass]
    public sealed class TestFechaReserva
    {
        /// <summary>
        /// Socio de prueba para las validaciones de fecha
        /// </summary>
        private readonly Socio socio = new Socio
        {
            IdSocio = 1,
            Nombre = "Juan Perez"
        };
        
        /// <summary>
        /// Actividad de prueba para las validaciones de fecha
        /// </summary>
        private readonly Actividad actividad = new Actividad
        {
            IdActividad = 1,
            Nombre = "Yoga"
        };

        /// <summary>
        /// Lista de fechas válidas para pruebas positivas (hoy y fechas futuras)
        /// </summary>
        private readonly List<DateTime?> fechasValidas = new List<DateTime?>
        {
            DateTime.Today,
            DateTime.Today.AddDays(1),
            DateTime.Today.AddDays(7),
            DateTime.Today.AddMonths(1)
        };
        
        /// <summary>
        /// Lista de fechas inválidas para pruebas negativas (fechas pasadas)
        /// </summary>
        private readonly List<DateTime?> fechasInvalidas = new List<DateTime?>
        {
            DateTime.Today.AddDays(-1),
            DateTime.Today.AddDays(-7),
            DateTime.Today.AddMonths(-1),
            DateTime.Today.AddYears(-1)
        };

        /// <summary>
        /// Prueba que verifica que fechas válidas (hoy y futuras) son aceptadas por la validación.
        /// Recorre la lista de fechas válidas y comprueba que ValidarFormulario retorna true
        /// </summary>
        [TestMethod]
        public void TestFechasValidas_RetornaTrue()
        {
            foreach (var fecha in fechasValidas)
            {
                // Arrange
                var viewModel = new NuevaReservaViewModel();
                viewModel.SocioSeleccionado = socio;
                viewModel.ActividadSeleccionada = actividad;
                viewModel.FechaReserva = fecha;

                // Act
                bool resultado = viewModel.ValidarFormulario();

                // Assert
                Assert.IsTrue(resultado, $"La fecha '{fecha:dd/MM/yyyy}' debería ser válida");
            }
        }

        /// <summary>
        /// Prueba que verifica que fechas inválidas (pasadas) son rechazadas por la validación.
        /// Recorre la lista de fechas inválidas y comprueba que ValidarFormulario retorna false
        /// </summary>
        [TestMethod]
        public void TestFechasInvalidas_RetornaFalse()
        {
            foreach (var fecha in fechasInvalidas)
            {
                // Arrange
                var viewModel = new NuevaReservaViewModel();
                viewModel.SocioSeleccionado = socio;
                viewModel.ActividadSeleccionada = actividad;
                viewModel.FechaReserva = fecha;

                // Act
                bool resultado = viewModel.ValidarFormulario();

                // Assert
                Assert.IsFalse(resultado, $"La fecha '{fecha:dd/MM/yyyy}' debería ser inválida (anterior a hoy)");
            }
        }

        /// <summary>
        /// Prueba que verifica que una fecha null es rechazada por la validación.
        /// Comprueba que ValidarFormulario retorna false cuando la fecha es null
        /// </summary>
        [TestMethod]
        public void TestFechaNull_RetornaFalse()
        {
            // Arrange
            var viewModel = new NuevaReservaViewModel();
            viewModel.SocioSeleccionado = socio;
            viewModel.ActividadSeleccionada = actividad;
            viewModel.FechaReserva = null;

            // Act
            bool resultado = viewModel.ValidarFormulario();

            // Assert
            Assert.IsFalse(resultado, "La fecha null debería ser inválida");
        }

        /// <summary>
        /// Prueba que verifica que se genera el mensaje de error correcto para fechas pasadas.
        /// Comprueba que el mensaje de error es específico para fechas anteriores a hoy
        /// </summary>
        [TestMethod]
        public void TestFechaAyer_MensajeError()
        {
            // Arrange
            var viewModel = new NuevaReservaViewModel();
            viewModel.SocioSeleccionado = socio;
            viewModel.ActividadSeleccionada = actividad;
            viewModel.FechaReserva = DateTime.Today.AddDays(-1);

            // Act
            bool resultado = viewModel.ValidarFormulario();

            // Assert
            Assert.IsFalse(resultado);
            Assert.AreEqual("La fecha de reserva no puede ser anterior a hoy", viewModel.ErrorMessage);
        }
    }
}
