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
└── Reports/            # Informes Crystal Reports
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
