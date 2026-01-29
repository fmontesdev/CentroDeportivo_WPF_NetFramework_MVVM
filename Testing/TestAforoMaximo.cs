using ViewModel.Services;
using Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Testing
{
    [TestClass]
    public sealed class TestAforoMaximo
    {
        // Datos de prueba
        private Actividad actividad;
        private Socio socio1;
        private Socio socio2;

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

        // Método auxiliar simple para eliminar con manejo de errores
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
