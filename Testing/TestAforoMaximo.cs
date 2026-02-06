using ViewModel.Services;
using Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Testing
{
    /// <summary>
    /// Clase de pruebas unitarias para validar el control de aforo máximo en actividades.
    /// Verifica que el sistema rechace reservas cuando se alcanza el aforo máximo de una actividad
    /// </summary>
    [TestClass]
    public sealed class TestAforoMaximo
    {
        /// <summary>
        /// Actividad de prueba con aforo máximo de 1 persona
        /// </summary>
        private Actividad actividad;
        
        /// <summary>
        /// Primer socio de prueba para las reservas
        /// </summary>
        private Socio socio1;
        
        /// <summary>
        /// Segundo socio de prueba para las reservas
        /// </summary>
        private Socio socio2;

        /// <summary>
        /// Método de inicialización que se ejecuta antes de cada prueba.
        /// Configura los datos de prueba con una actividad de aforo 1 y dos socios
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            actividad = new Actividad
            {
                Nombre = "Bailes de Salón",
                AforoMaximo = 1
            };

            socio1 = new Socio
            {
                Nombre = "Sofía Martínez",
                Email = "sofia.martinez@test.com",
                Activo = true
            };

            socio2 = new Socio
            {
                Nombre = "Pablo Calvo",
                Email = "pablo.calvo@test.com",
                Activo = true
            };
        }

        /// <summary>
        /// Prueba de integración que verifica el control de aforo máximo en actividades.
        /// Crea una actividad con aforo 1, realiza una primera reserva (debe tener éxito),
        /// e intenta una segunda reserva (debe ser rechazada con excepción de aforo completo).
        /// Al finalizar, limpia todos los datos de prueba de la base de datos
        /// </summary>
        [TestMethod]
        public async Task TestControlAforo_ActividadConAforoUno_SegundaReservaDenegada()
        {
            // Arrange
            ReservaService reservaService = new ReservaService();
            ActividadService actividadService = new ActividadService();
            SocioService socioService = new SocioService();
            Actividad actividadCreada = null;
            Socio socio1Creado = null;
            Socio socio2Creado = null;
            Reserva reserva1Creada = null;
            Reserva reserva2Creada = null;

            try
            {
                // Crear una actividad de prueba con aforo 1
                await actividadService.CrearActividadAsync(actividad);
                var actividades = await actividadService.ObtenerActividadesAsync();
                actividadCreada = actividades.FirstOrDefault(a =>
                    a.Nombre == actividad.Nombre && a.AforoMaximo == actividad.AforoMaximo);
                Assert.IsNotNull(actividadCreada, "No se pudo recuperar la actividad creada de la BD");

                // Crear dos socios de prueba
                await socioService.CrearSocioAsync(socio1);
                var socios = await socioService.ObtenerSociosAsync();
                socio1Creado = socios.FirstOrDefault(s =>
                    s.Nombre == socio1.Nombre && s.Email == socio1.Email);
                Assert.IsNotNull(socio1Creado, "No se pudo recuperar el socio1 creado de la BD");

                await socioService.CrearSocioAsync(socio2);
                socios = await socioService.ObtenerSociosAsync();
                socio2Creado = socios.FirstOrDefault(s =>
                    s.Nombre == socio2.Nombre && s.Email == socio2.Email);
                Assert.IsNotNull(socio2Creado, "No se pudo recuperar el socio2 creado de la BD");

                // Act 1: Crear la primera reserva
                var reserva1 = new Reserva
                {
                    IdSocio = socio1Creado.IdSocio,
                    IdActividad = actividadCreada.IdActividad,
                    Fecha = DateTime.Today.AddDays(1)
                };

                await reservaService.CrearReservaAsync(reserva1);
                var reservas = await reservaService.ObtenerReservasAsync();
                reserva1Creada = reservas.FirstOrDefault(r =>
                    r.IdSocio == socio1Creado.IdSocio &&
                    r.IdActividad == actividadCreada.IdActividad &&
                    r.Fecha.Date == reserva1.Fecha.Date);

                // Assert 1: La primera reserva debe haberse creado exitosamente
                Assert.IsNotNull(reserva1Creada, "La primera reserva debería haberse creado");
                Assert.IsTrue(reserva1Creada.IdReserva > 0, "La primera reserva debería tener un ID válido");

                // Act 2: Intentar crear la segunda reserva (misma actividad, misma fecha, diferente socio)
                var reserva2 = new Reserva
                {
                    IdSocio = socio2Creado.IdSocio,
                    IdActividad = actividadCreada.IdActividad,
                    Fecha = DateTime.Today.AddDays(1)
                };

                InvalidOperationException excepcionCapturada = null;
                try
                {
                    await reservaService.CrearReservaAsync(reserva2);

                    // Si llegamos aquí, verificar que NO se creó en la BD
                    reservas = await reservaService.ObtenerReservasAsync();
                    reserva2Creada = reservas.FirstOrDefault(r =>
                        r.IdSocio == socio2Creado.IdSocio &&
                        r.IdActividad == actividadCreada.IdActividad &&
                        r.Fecha.Date == reserva2.Fecha.Date);
                }
                catch (InvalidOperationException ex)
                {
                    excepcionCapturada = ex;
                }

                // Assert 2: La segunda reserva debe haber sido rechazada
                Assert.IsNotNull(excepcionCapturada, "Debería lanzarse una InvalidOperationException al intentar crear la segunda reserva");
                Assert.IsTrue(excepcionCapturada.Message.Contains("aforo máximo"),
                    "El mensaje de error debería mencionar el aforo máximo");
                Assert.IsTrue(excepcionCapturada.Message.Contains("(1)"),
                    "El mensaje de error debería indicar que el aforo es 1");
                Assert.IsNull(reserva2Creada, "La segunda reserva NO debería haberse creado en la BD");
            }
            finally
            {
                // Eliminar datos de prueba en orden inverso
                await EliminarDatos(reserva2Creada, reservaService.EliminarReservaAsync);
                await EliminarDatos(reserva1Creada, reservaService.EliminarReservaAsync);
                await EliminarDatos(socio1Creado, socioService.EliminarSocioAsync);
                await EliminarDatos(socio2Creado, socioService.EliminarSocioAsync);
                await EliminarDatos(actividadCreada, actividadService.EliminarActividadAsync);
            }
        }

        /// <summary>
        /// Método auxiliar genérico para eliminar datos de prueba de la base de datos.
        /// Ignora errores durante la eliminación para no interferir con las pruebas
        /// </summary>
        /// <typeparam name="T">Tipo de entidad a eliminar</typeparam>
        /// <param name="entidad">Entidad a eliminar</param>
        /// <param name="eliminar">Función de eliminación del servicio</param>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        private async Task EliminarDatos<T>(T entidad, Func<T, Task> eliminar) where T : class
        {
            try
            {
                if (entidad != null)
                {
                    await eliminar(entidad);
                }
            }
            catch { /* Ignorar errores */ }
        }
    }
}
