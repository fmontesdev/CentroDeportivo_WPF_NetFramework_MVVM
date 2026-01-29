# Centro Deportivo - Sistema de Gestión

Aplicación de escritorio para la gestión de un centro deportivo desarrollada con WPF y .NET Framework 4.8.

## Tecnologías

- **WPF** (Windows Presentation Foundation)
- **.NET Framework 4.8**
- **Entity Framework 6** (Database First)
- **Crystal Reports** para informes
- **Patrón MVVM** (Model-View-ViewModel)

## Estructura del Proyecto

```
CentroDeportivo/
├── View/				# Vista (WPF)
├── ViewModel/          # Lógica de presentación
├── Model/              # Acceso a datos y entidades
├── Reports/            # Informes Crystal Reports
└── Testing/            # Tests unitarios (MSTest)
```

## Funcionalidades

### 📋 Gestión de Socios
- Crear, editar y eliminar socios
- Activar/desactivar socios
- Listado completo con búsqueda

### 🏃 Gestión de Actividades
- Crear, editar y eliminar actividades
- Control de aforo máximo
- Gestión de horarios

### 📅 Gestión de Reservas
- Crear y eliminar reservas
- Validación de aforo
- Filtrado por socio y actividad

### 📊 Informes
1. **Listado de Socios**: Reporte completo de socios registrados
2. **Reservas por Actividad**: Hoja de asistencia con cálculo de ocupación
3. **Historial de Reservas**: Historial completo agrupado por socio

## Testing

El proyecto incluye un conjunto de **tests unitarios** (MSTest) que validan la lógica de negocio y las reglas de validación:

### 🧪 Tests Implementados

#### 1. **TestFormatoEmail** - Validación de Emails
Verifica que el sistema solo acepte direcciones de correo electrónico válidas al crear socios.

- **TestEmailsValidos_RetornaTrue**: Valida emails con formato correcto
  - Ejemplos: `ana.gomez@dominio.com`, `usuario@mail.dominio.com`
- **TestEmailsInvalidos_RetornaFalse**: Rechaza emails con formato incorrecto
  - Ejemplos: `usuario.com`, `usuario@`, `usuario@dominio`, `usuario @dominio.com`

#### 2. **TestFechaReserva** - Validación de Fechas
Garantiza que las reservas solo se puedan crear para fechas futuras o el día actual.

- **TestFechasValidas_RetornaTrue**: Acepta fechas de hoy o futuras
  - Ejemplos: Hoy, mañana, próxima semana, próximo mes
- **TestFechasInvalidas_RetornaFalse**: Rechaza fechas pasadas
  - Ejemplos: Ayer, semana pasada, mes pasado
- **TestFechaNull_RetornaFalse**: Rechaza fechas nulas
- **TestFechaAyer_MensajeError**: Verifica que se muestre el mensaje de error correcto

#### 3. **TestAforoMaximo** - Control de Aforo
Valida que el sistema respete el aforo máximo de las actividades y no permita sobrecupo.

- **TestControlAforo_ActividadConAforoUno_SegundaReservaDenegada**: 
  - Crea una actividad con aforo máximo de 1 persona
  - Permite la primera reserva exitosamente
  - Rechaza la segunda reserva para la misma actividad y fecha
  - Verifica que se lance `InvalidOperationException` con mensaje apropiado
  - Asegura que no se persista la reserva rechazada en la base de datos

### 📊 Ejecución de Tests

**En Visual Studio:**
```
Pruebas → Ejecutar todas las pruebas
```

**Desde la terminal:**
```powershell
dotnet test
```

### ✅ Cobertura de Tests

Los tests cubren:
- ✔️ Validación de formularios (emails, fechas)
- ✔️ Reglas de negocio (control de aforo)
- ✔️ Integración con base de datos (CRUD completo)
- ✔️ Manejo de excepciones
- ✔️ Limpieza automática de datos de prueba

---

## Requisitos

- Windows 10 o superior
- .NET Framework 4.8
- SQL Server / SQL Server Express
- Crystal Reports Runtime (para visualizar informes)

## Instalación

1. Clona el repositorio
2. Abre `CentroDeportivo.sln` en Visual Studio
3. Restaura los paquetes NuGet
4. Configura la cadena de conexión en `App.config`
5. Compila y ejecuta

## Base de Datos

La aplicación usa **Entity Framework Database First**. El modelo de datos se genera automáticamente desde la base de datos `CentroDeportivo`.

### Tablas principales:
- **Socio**: Información de los socios
- **Actividad**: Actividades del centro deportivo
- **Reserva**: Reservas realizadas por los socios

## Arquitectura

El proyecto sigue el patrón **MVVM** con separación clara de responsabilidades:

- **Model**: Entidades, Repositorios y DataSets
- **ViewModel**: Lógica de negocio y Commands
- **View**: Interfaz de usuario (XAML + Code-behind)
- **Reports**: Informes Crystal Reports

## Autor

Francisco Montes
