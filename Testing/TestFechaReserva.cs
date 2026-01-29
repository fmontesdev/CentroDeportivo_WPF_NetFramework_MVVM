using ViewModel;
using Model;
using System;
using System.Collections.Generic;

namespace Testing
{
    [TestClass]
    public sealed class TestFechaReserva
    {
        // Socio y actividad de prueba
        private readonly Socio socio = new Socio
        {
            IdSocio = 1,
            Nombre = "Juan Perez"
        };
        private readonly Actividad actividad = new Actividad
        {
            IdActividad = 1,
            Nombre = "Yoga"
        };

        // Fechas de prueba
        private readonly List<DateTime?> fechasValidas = new List<DateTime?>
        {
            DateTime.Today,
            DateTime.Today.AddDays(1),
            DateTime.Today.AddDays(7),
            DateTime.Today.AddMonths(1)
        };
        private readonly List<DateTime?> fechasInvalidas = new List<DateTime?>
        {
            DateTime.Today.AddDays(-1),
            DateTime.Today.AddDays(-7),
            DateTime.Today.AddMonths(-1),
            DateTime.Today.AddYears(-1)
        };

        // Test que comprueba fechas de reserva válidas
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

        // Test que comprueba fechas de reserva inválidas
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

        // Test que comprueba el caso de fecha null
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

        // Test que comprueba que se lance un mensaje de error para fecha anterior a hoy
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
