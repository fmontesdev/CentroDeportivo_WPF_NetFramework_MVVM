using ViewModel;
using Model;
using System.Collections.Generic;

namespace Testing
{
    /// <summary>
    /// Clase de pruebas unitarias para validar el formato de emails de socios.
    /// Verifica que la validación de emails acepte formatos válidos y rechace inválidos
    /// </summary>
    [TestClass]
    public sealed class TestFormatoEmail
    {
        /// <summary>
        /// Lista de socios con emails en formato válido para pruebas positivas
        /// </summary>
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
        
        /// <summary>
        /// Lista de socios con emails en formato inválido para pruebas negativas
        /// </summary>
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

        /// <summary>
        /// Prueba que verifica que emails con formato válido son aceptados por la validación.
        /// Recorre la lista de emails válidos y comprueba que ValidarFormulario retorna true
        /// </summary>
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

        /// <summary>
        /// Prueba que verifica que emails con formato inválido son rechazados por la validación.
        /// Recorre la lista de emails inválidos y comprueba que ValidarFormulario retorna false
        /// </summary>
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
