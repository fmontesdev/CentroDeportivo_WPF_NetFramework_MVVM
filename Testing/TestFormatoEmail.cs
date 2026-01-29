using ViewModel;
using Model;
using System.Collections.Generic;

namespace Testing
{
    [TestClass]
    public sealed class TestFormatoEmail
    {
        // Socios de prueba con emails válidos e inválidos
        private readonly List<Socio> sociosValidos = new List<Socio>
        {
            new Socio
            {
                Nombre = "Ana Gomez",
                Email = "ana.gomez@dominio.com"
            },
            new Socio
            {
                Nombre = "Juan Perez",
                Email = "usuario@dominio.com"
            },
            new Socio
            {
                Nombre = "Maria Lopez",
                Email = "usuario@mail.dominio.com"
            }
        };
        private readonly List<Socio> sociosInvalidos = new List<Socio>
        {
            new Socio
            {
                Nombre = "Carlos Ruiz",
                Email = "usuario.com"
            },
            new Socio
            {
                Nombre = "Laura Martinez",
                Email = "usuario@"
            },
            new Socio
            {
                Nombre = "Pedro Sanchez",
                Email = "usuario@dominio"
            },
            new Socio
            {
                Nombre = "Sofia Garcia",
                Email = "usuario @dominio.com"
            }
        };

        // Test que comprueba emails válidos
        [TestMethod]
        public void TestEmailsValidos_RetornaTrue()
        {
            foreach (var socio in sociosValidos)
            {
                // Arrange
                var viewModel = new NuevoSocioViewModel();
                viewModel.Nombre = socio.Nombre;
                viewModel.Email = socio.Email;

                // Act
                bool resultado = viewModel.ValidarFormulario();

                // Assert
                Assert.IsTrue(resultado, $"El email '{socio.Email}' debería ser válido");
            }
        }

        // Test que comprueba emails inválidos
        [TestMethod]
        public void TestEmailsInvalidos_RetornaFalse()
        {
            foreach (var socio in sociosInvalidos)
            {
                // Arrange
                var viewModel = new NuevoSocioViewModel();
                viewModel.Nombre = socio.Nombre;
                viewModel.Email = socio.Email;

                // Act
                bool resultado = viewModel.ValidarFormulario();

                // Assert
                Assert.IsFalse(resultado, $"El email '{socio.Email}' debería ser inválido");
            }
        }
    }
}
