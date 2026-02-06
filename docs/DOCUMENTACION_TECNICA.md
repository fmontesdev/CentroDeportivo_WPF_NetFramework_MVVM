# 📘 Documentación Técnica - Centro Deportivo

## 📑 Tabla de Contenidos

1. [Introducción](#1-introducción)
2. [Arquitectura del Sistema](#2-arquitectura-del-sistema)
3. [Patrón MVVM](#3-patrón-mvvm)
4. [Diagrama de Capas](#4-diagrama-de-capas)
5. [Estructura de Carpetas](#5-estructura-de-carpetas)
6. [Componentes del Proyecto](#6-componentes-del-proyecto)
7. [Base de Datos](#7-base-de-datos)
8. [Tecnologías y Frameworks](#8-tecnologías-y-frameworks)

---

## 1. Introducción

### 1.1 Propósito del Documento

Este documento proporciona una descripción técnica completa del sistema de gestión para centros deportivos, incluyendo su arquitectura, diseño, componentes y flujo de datos.

### 1.2 Alcance

El sistema permite:
- Gestión completa de socios (CRUD)
- Administración de actividades deportivas
- Sistema de reservas con control de aforo
- Generación de informes con Crystal Reports
- Validación de datos en tiempo real
- Persistencia en base de datos SQL Server

## 2. Arquitectura del Sistema

### 2.1 Visión General

El sistema implementa una **arquitectura en capas** basada en el patrón **MVVM (Model-View-ViewModel)**, específicamente diseñada para aplicaciones WPF.

```
┌──────────────────────────────────────────────────────────┐
│                  CAPA DE PRESENTACIÓN                    │
│                          (WPF)                           │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐  │
│  │ Socios   │  │Actividad │  │ Reservas │  │ Informes │  │
│  │  View    │  │  View    │  │  View    │  │  View    │  │
│  └────┬─────┘  └────┬─────┘  └────┬─────┘  └────┬─────┘  │
└───────┼─────────────┼─────────────┼─────────────┼────────┘
        │             │             │             │
        │        Data Binding       │             │
        ▼             ▼             ▼             ▼
┌──────────────────────────────────────────────────────────┐
│              CAPA DE LÓGICA DE PRESENTACIÓN              │
│                       (ViewModels)                       │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐  │
│  │  Socio   │  │Actividad │  │ Reserva  │  │ Informe  │  │
│  │ViewModel │  │ViewModel │  │ViewModel │  │ViewModel │  │
│  └────┬─────┘  └────┬─────┘  └────┬─────┘  └────┬─────┘  │
│       │             │             │             │        │
│  ┌────▼─────┐  ┌────▼─────┐  ┌────▼─────┐  ┌────▼─────┐  │
│  │  Socio   │  │Actividad │  │ Reserva  │  │ Informe  │  │
│  │ Service  │  │ Service  │  │ Service  │  │ Service  │  │
│  └────┬─────┘  └────┬─────┘  └────┬─────┘  └────┬─────┘  │
└───────┼─────────────┼─────────────┼─────────────┼────────┘
        │             │             │             │
        │     Business Logic        │             │
        ▼             ▼             ▼             ▼
      ┌─────────────────────────────────────────────┐
      │            CAPA DE ACCESO A DATOS           │
      │                    (Model)                  │
      │  ┌──────────┐  ┌──────────┐  ┌──────────┐   │
      │  │  Socio   │  │Actividad │  │ Reserva  │   │
      │  │   Repo   │  │   Repo   │  │   Repo   │   │
      │  └────┬─────┘  └────┬─────┘  └────┬─────┘   │
      │       │             │             │         │
      │       └─────────────┴─────────────┘         │
      │                     │                       │
      │           ┌─────────▼─────────┐             │
      │           │  Entity Framework │             │
      │           │     DbContext     │             │
      │           └─────────┬─────────┘             │
      └─────────────────────┼───────────────────────┘
                            │
                            ▼
                    ┌───────────────┐
                    │  SQL Server   │
                    │   Database    │
                    └───────────────┘
```

### 2.2 Principios Arquitectónicos

#### **Separación de Responsabilidades**
- Cada capa tiene una responsabilidad única y bien definida
- Las vistas no contienen lógica de negocio
- Los ViewModels no tienen dependencias directas con las vistas
- El acceso a datos está completamente abstraído

#### **Inversión de Dependencias**
- Las capas superiores dependen de abstracciones, no de implementaciones concretas
- Los repositorios abstraen el acceso a datos
- Los servicios coordinan la lógica de negocio

#### **Testabilidad**
- La separación de capas permite probar cada componente de forma independiente
- Los ViewModels pueden ser probados sin interfaz de usuario
- Los servicios tienen lógica desacoplada de la infraestructura

---

## 3. Patrón MVVM

### 3.1 ¿Qué es MVVM?

**Model-View-ViewModel** es un patrón arquitectónico diseñado específicamente para aplicaciones con interfaces de usuario, especialmente WPF y XAML.

### 3.2 Componentes del Patrón

#### **Model (Modelo)**
```
Responsabilidades:
├── Definir entidades de negocio
├── Acceso a datos (Repositorios)
├── Lógica de persistencia
└── Validaciones de entidad
```

**Ubicación:** `Model/`

**Ejemplo:**
```csharp
public partial class Socio
{
    public int IdSocio { get; set; }
    public string Nombre { get; set; }
    public string Email { get; set; }
    public bool Activo { get; set; }
    public virtual ICollection<Reserva> Reserva { get; set; }
}
```

#### **View (Vista)**
```
Responsabilidades:
├── Presentación visual (XAML)
├── Data Binding con ViewModel
├── Eventos de UI
└── Navegación
```

**Ubicación:** `CentroDeportivo/Views/` y `CentroDeportivo/Windows/`

**Ejemplo:**
```xml
<DataGrid ItemsSource="{Binding Socios}" 
          SelectedItem="{Binding SocioSeleccionado}"/>
```

#### **ViewModel (Modelo de Vista)**
```
Responsabilidades:
├── Exponer datos para la vista
├── Implementar INotifyPropertyChanged
├── Comandos (ICommand)
├── Validación de formularios
├── Coordinación con Services
└── Transformación de datos
```

**Ubicación:** `ViewModel/`

**Ejemplo:**
```csharp
public class SocioViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Socio> Socios { get; set; }
    public ICommand NuevoCommand { get; }
    public ICommand EditarCommand { get; }
    public ICommand EliminarCommand { get; }
}
```

### 3.3 Comunicación entre Componentes

```
┌─────────┐                ┌──────────────┐                ┌─────────┐
│  VIEW   │                │  VIEWMODEL   │                │  MODEL  │
└────┬────┘                └──────┬───────┘                └────┬────┘
     │                            │                             │
     │  1. Evento de UI           │                             │
     │───────────────────────────>│                             │
     │  (ej: click en botón)      │                             │
     │                            │                             │
     │                            │  2. Ejecuta Command         │
     │                            │────────────────────────────>│
     │                            │  (ej: CrearSocio)           │
     │                            │                             │
     │                            │  3. Retorna datos           │
     │                            │<────────────────────────────│
     │                            │                             │
     │  4. PropertyChanged        │                             │
     │<───────────────────────────│                             │
     │  (notifica cambios)        │                             │
     │                            │                             │
     │  5. Actualiza UI           │                             │
     │  (Data Binding)            │                             │
     │                            │                             │
```

### 3.4 Data Binding

El Data Binding conecta automáticamente las propiedades del ViewModel con los elementos de la vista:

```csharp
// ViewModel
public string Nombre 
{ 
    get => _nombre; 
    set 
    { 
        _nombre = value; 
        OnPropertyChanged(nameof(Nombre)); 
    } 
}
```

```xml
<!-- XAML -->
<TextBox Text="{Binding Nombre, UpdateSourceTrigger=PropertyChanged}"/>
```

## 4. Diagrama de Capas

### 4.1 Arquitectura en 5 Capas

```
╔═══════════════════════════════════════════════════════════════╗
║                    CAPA 1: PRESENTACIÓN                       ║
║                         (View - WPF)                          ║
╠═══════════════════════════════════════════════════════════════╣
║  • MainWindow.xaml          • NuevoSocioWindow.xaml           ║
║  • SociosView.xaml          • NuevaActividadWindow.xaml       ║
║  • ActividadesView.xaml     • NuevaReservaWindow.xaml         ║
║  • ReservasView.xaml                                          ║
║  • InformesView.xaml                                          ║
╚═══════════════════════════════════════════════════════════════╝
                            ▼ Data Binding
╔═══════════════════════════════════════════════════════════════╗
║               CAPA 2: LÓGICA DE PRESENTACIÓN                  ║
║                        (ViewModels)                           ║
╠═══════════════════════════════════════════════════════════════╣
║  • MainViewModel            • NuevoSocioViewModel             ║
║  • SocioViewModel           • NuevaActividadViewModel         ║
║  • ActividadViewModel       • NuevaReservaViewModel           ║
║  • ReservaViewModel         • RelayCommand                    ║
║  • InformeViewModel                                           ║
╚═══════════════════════════════════════════════════════════════╝
                            ▼ Llama a
╔═══════════════════════════════════════════════════════════════╗
║                 CAPA 3: LÓGICA DE NEGOCIO                     ║
║                         (Services)                            ║
╠═══════════════════════════════════════════════════════════════╣
║  • SocioService                                               ║
║  • ActividadService                                           ║
║  • ReservaService                                             ║
║  • InformeService                                             ║
║                                                               ║
║  Responsabilidades:                                           ║
║  - Validaciones de negocio                                    ║
║  - Coordinación entre repositorios                            ║
║  - Transacciones                                              ║
║  - Transformación de datos                                    ║
╚═══════════════════════════════════════════════════════════════╝
                            ▼ Usa
╔═══════════════════════════════════════════════════════════════╗
║                   CAPA 4: ACCESO A DATOS                      ║
║                       (Repositories)                          ║
╠═══════════════════════════════════════════════════════════════╣
║  • SocioRepositorio                                           ║
║  • ActividadRepositorio                                       ║
║  • ReservaRepositorio                                         ║
║                                                               ║
║  Operaciones:                                                 ║
║  - SeleccionarAsync()                                         ║
║  - CrearAsync(entity)                                         ║
║  - GuardarAsync()                                             ║
║  - EliminarAsync(entity)                                      ║
╚═══════════════════════════════════════════════════════════════╝
                            ▼ Utiliza
╔═══════════════════════════════════════════════════════════════╗
║                     CAPA 5: PERSISTENCIA                      ║
║                (Entity Framework + SQL Server)                ║
╠═══════════════════════════════════════════════════════════════╣
║  • CentroDeportivoEntities (DbContext)                        ║
║  • Entity Framework 6                                         ║
║  • SQL Server Database                                        ║
║                                                               ║
║  Entidades:                                                   ║
║  - Socio                                                      ║
║  - Actividad                                                  ║
║  - Reserva                                                    ║
╚═══════════════════════════════════════════════════════════════╝
```

### 4.2 Flujo de Dependencias

```
VIEW → ViewModel → Service → Repository → DbContext → Database
  ↑                   ↑          ↑
  └── Data Binding ───┴──────────┘
```

**Reglas:**
- ❌ Las vistas NO acceden directamente a los servicios
- ❌ Los ViewModels NO acceden directamente a los repositorios
- ❌ Las capas inferiores NO conocen las capas superiores
- ✅ Cada capa solo conoce la capa inmediatamente inferior

---

## 5. Estructura de Carpetas

### 5.1 Árbol de Directorios Completo

```
CentroDeportivo/
│
├── 📂 View/                                # Proyecto de Presentación (WPF)
│   ├── 📂 Views/                           # Vistas principales
│   │   ├── SociosView.xaml                 # Vista de gestión de socios
│   │   ├── SociosView.xaml.cs              # Code-behind
│   │   ├── ActividadesView.xaml            # Vista de gestión de actividades
│   │   ├── ActividadesView.xaml.cs
│   │   ├── ReservasView.xaml               # Vista de gestión de reservas
│   │   ├── ReservasView.xaml.cs
│   │   ├── InformesView.xaml               # Vista de informes
│   │   └── InformesView.xaml.cs
│   │
│   ├── 📂 Windows/                         # Ventanas modales
│   │   ├── NuevoSocioWindow.xaml           # Modal para crear socio
│   │   ├── NuevoSocioWindow.xaml.cs
│   │   ├── NuevaActividadWindow.xaml       # Modal para crear actividad
│   │   ├── NuevaActividadWindow.xaml.cs
│   │   ├── NuevaReservaWindow.xaml         # Modal para crear reserva
│   │   └── NuevaReservaWindow.xaml.cs
│   │
│   ├── MainWindow.xaml                     # Ventana principal
│   ├── MainWindow.xaml.cs
│   ├── App.xaml                            # Configuración de aplicación
│   ├── App.xaml.cs
│   └── App.config                          # Configuración (cadenas de conexión)
│
├── 📂 ViewModel/                           # Proyecto de Lógica de Presentación
│   ├── 📂 Services/                        # Servicios de negocio
│   │   ├── SocioService.cs                 # Lógica de negocio de socios
│   │   ├── ActividadService.cs             # Lógica de negocio de actividades
│   │   ├── ReservaService.cs               # Lógica de negocio de reservas
│   │   └── InformeService.cs               # Lógica de negocio de informes
│   │
│   ├── 📂 Command/                         # Implementación de comandos
│   │   └── RelayCommand.cs                 # Implementación de ICommand
│   │
│   ├── 📂 Models/                          # Modelos auxiliares
│   │   ├── Informe.cs                      # Modelo de informe
│   │   ├── TipoInforme.cs                  # Enum de tipos de informe
│   │   └── ListaInformes.cs                # Lista de informes disponibles
│   │
│   ├── MainViewModel.cs                    # ViewModel de ventana principal
│   ├── SocioViewModel.cs                   # ViewModel de gestión de socios
│   ├── ActividadViewModel.cs               # ViewModel de gestión de actividades
│   ├── ReservaViewModel.cs                 # ViewModel de gestión de reservas
│   ├── InformeViewModel.cs                 # ViewModel de informes
│   ├── NuevoSocioViewModel.cs              # ViewModel de modal nuevo socio
│   ├── NuevaActividadViewModel.cs          # ViewModel de modal nueva actividad
│   └── NuevaReservaViewModel.cs            # ViewModel de modal nueva reserva
│
├── 📂 Model/                               # Proyecto de Acceso a Datos
│   ├── 📂 Repositorios/                    # Capa de repositorios
│   │   ├── SocioRepositorio.cs             # Repositorio de socios
│   │   ├── ActividadRepositorio.cs         # Repositorio de actividades
│   │   └── ReservaRepositorio.cs           # Repositorio de reservas
│   │
│   ├── 📂 DataSets/                        # DataSets tipados para informes
│   │   ├── dsSocios.xsd                    # DataSet de socios
│   │   ├── dsReservasPorActividad.xsd      # DataSet de reservas por actividad
│   │   └── dsReservasHistorial.xsd         # DataSet de historial de reservas
│   │
│   ├── Socio.cs                            # Entidad Socio (EF)
│   ├── Actividad.cs                        # Entidad Actividad (EF)
│   ├── Reserva.cs                          # Entidad Reserva (EF)
│   ├── Model1.Context.cs                   # DbContext de Entity Framework
│   └── CentroDeportivo.edmx                # Modelo de Entity Framework
│
├── 📂 Reports/                                 # Proyecto de Informes
│   ├── 📂 Reports/                             # Archivos .rpt de Crystal Reports
│   │   ├── ListadoSocios.rpt                   # Informe de listado de socios
│   │   ├── ListadoSocios.cs                    # Clase generada del informe
│   │   ├── HistorialReservas.rpt               # Informe de historial de reservas
│   │   ├── HistorialReservas.cs                # Clase generada del informe
│   │   ├── ListadoReserevasPorActividad.rpt    # Informe de reservas por actividad
│   │   └── ListadoReserevasPorActividad.cs     # Clase generada del informe
│   │
│   └── 📂 Windows/                                     # Ventanas de visualización
│       ├── ListadoSociosWindow.xaml                    # Visor de informe de socios
│       ├── ListadoSociosWindow.xaml.cs
│       ├── HistorialReservasWindow.xaml                # Visor de historial
│       ├── HistorialReservasWindow.xaml.cs
│       ├── ListadoReservasPorActividadWindow.xaml      # Visor de reservas
│       └── ListadoReservasPorActividadWindow.xaml.cs
│
├── 📂 Testing/                             # Proyecto de Pruebas Unitarias
│   ├── TestFormatoEmail.cs                 # Pruebas de validación de email
│   ├── TestFechaReserva.cs                 # Pruebas de validación de fechas
│   ├── TestAforoMaximo.cs                  # Pruebas de control de aforo
│   └── MSTestSettings.cs                   # Configuración de MSTest
│
├── 📂 docs/                                # Documentación del proyecto
│   ├── 📂 screenshots/                     # Capturas de pantalla
│   └── 📂 html/                            # Documentación en Doxygen
│
├── README.md                               # Documentación principal
├── LICENSE                                 # Licencia MIT
└── CentroDeportivo.sln                     # Archivo de solución
```

### 5.2 Descripción de Proyectos

| Proyecto | Tipo | Descripción | Dependencias |
|----------|------|-------------|--------------|
| **CentroDeportivo** | WPF Application | Capa de presentación con vistas y ventanas | ViewModel |
| **ViewModel** | Class Library | Lógica de presentación, ViewModels y Services | Model |
| **Model** | Class Library | Acceso a datos, entidades y repositorios | Entity Framework 6 |
| **Reports** | WPF Library | Informes con Crystal Reports | Model |
| **Testing** | MSTest Project | Pruebas unitarias | ViewModel, Model |

---

## 6. Componentes del Proyecto

### 6.1 Models (Entidades y Repositorios)

#### **6.1.1 Entidades**

Las entidades representan las tablas de la base de datos y son generadas automáticamente por Entity Framework (Database First).

**Ubicación:** `Model/`

##### **Socio.cs**
```csharp
/// <summary>
/// Representa un socio del centro deportivo
/// </summary>
public partial class Socio
{
    public int IdSocio { get; set; }
    public string Nombre { get; set; }
    public string Email { get; set; }
    public bool Activo { get; set; }
    public virtual ICollection<Reserva> Reserva { get; set; }
}
```

**Propiedades:**
- `IdSocio`: Clave primaria
- `Nombre`: Nombre completo del socio
- `Email`: Correo electrónico (validado)
- `Activo`: Estado del socio (activo/inactivo)
- `Reserva`: Navegación a las reservas del socio

##### **Actividad.cs**
```csharp
/// <summary>
/// Representa una actividad deportiva del centro
/// </summary>
public partial class Actividad
{
    public int IdActividad { get; set; }
    public string Nombre { get; set; }
    public int AforoMaximo { get; set; }
    public virtual ICollection<Reserva> Reserva { get; set; }
}
```

**Propiedades:**
- `IdActividad`: Clave primaria
- `Nombre`: Nombre de la actividad
- `AforoMaximo`: Capacidad máxima de personas
- `Reserva`: Navegación a las reservas de la actividad

##### **Reserva.cs**
```csharp
/// <summary>
/// Representa una reserva realizada por un socio para una actividad
/// </summary>
public partial class Reserva
{
    public int IdReserva { get; set; }
    public int IdSocio { get; set; }
    public int IdActividad { get; set; }
    public DateTime Fecha { get; set; }
    public virtual Actividad Actividad { get; set; }
    public virtual Socio Socio { get; set; }
}
```

**Propiedades:**
- `IdReserva`: Clave primaria
- `IdSocio`: Clave foránea a Socio
- `IdActividad`: Clave foránea a Actividad
- `Fecha`: Fecha de la reserva
- `Actividad`: Navegación a la actividad
- `Socio`: Navegación al socio

#### **6.1.2 Repositorios**

Los repositorios implementan el patrón Repository para abstraer el acceso a datos.

**Ubicación:** `Model/Repositorios/`

##### **Estructura de Repositorio**
```csharp
public class SocioRepositorio
{
    private readonly CentroDeportivoEntities _context;
    
    // Métodos CRUD
    public async Task<List<Socio>> SeleccionarAsync()
    public async Task CrearAsync(Socio entity)
    public async Task GuardarAsync()
    public async Task EliminarAsync(Socio entity)
    
    // Gestión de transacciones
    public DbContextTransaction IniciarTransaccion()
    public CentroDeportivoEntities ObtenerContexto()
    
    // DataSets para informes
    public async Task<dsSocios> ObtenerDataSetSociosAsync()
}
```

**Responsabilidades:**
- ✅ Operaciones CRUD asíncronas
- ✅ Gestión de transacciones
- ✅ Generación de DataSets para informes
- ✅ Eager loading de relaciones
- ✅ Abstracción de Entity Framework

#### **6.1.3 DbContext**

**Ubicación:** `Model/Model1.Context.cs`

```csharp
/// <summary>
/// Contexto de Entity Framework para la base de datos del centro deportivo
/// </summary>
public partial class CentroDeportivoEntities : DbContext
{
    public CentroDeportivoEntities() 
        : base("name=CentroDeportivoEntities") { }
    
    public virtual DbSet<Actividad> Actividad { get; set; }
    public virtual DbSet<Reserva> Reserva { get; set; }
    public virtual DbSet<Socio> Socio { get; set; }
}
```

**Configuración:**
- Cadena de conexión en `App.config`
- Database First (modelo generado desde BD)
- Lazy Loading habilitado
- Change Tracking automático

---

### 6.2 ViewModels

Los ViewModels son el puente entre la Vista y el Modelo, implementando la lógica de presentación.

**Ubicación:** `ViewModel/`

#### **6.2.1 Estructura de un ViewModel**

```csharp
public class SocioViewModel : INotifyPropertyChanged
{
    // 1. PROPIEDADES PRIVADAS
    private readonly SocioService _socioService;
    private Socio _socioSeleccionado;
    private string _errorMessage;
    
    // 2. PROPIEDADES PÚBLICAS (Data Binding)
    public ObservableCollection<Socio> Socios { get; set; }
    public Socio SocioSeleccionado { get; set; }
    public string ErrorMessage { get; set; }
    
    // 3. COMANDOS
    public ICommand NuevoCommand { get; }
    public ICommand EditarCommand { get; }
    public ICommand EliminarCommand { get; }
    
    // 4. CONSTRUCTOR
    public SocioViewModel()
    {
        _socioService = new SocioService();
        Socios = new ObservableCollection<Socio>();
        
        // Inicializar comandos
        NuevoCommand = new RelayCommand(NuevoSocio);
        EditarCommand = new RelayCommand(EditarSocio);
        EliminarCommand = new RelayCommand(EliminarSocio);
        
        // Cargar datos
        InicializarAsync();
    }
    
    // 5. MÉTODOS DE NEGOCIO
    private async void NuevoSocio() { }
    private async void EditarSocio() { }
    private async void EliminarSocio() { }
    private bool ValidarFormulario() { }
    
    // 6. INOTIFYPROPERTYCHANGED
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, 
            new PropertyChangedEventArgs(propertyName));
    }
}
```

#### **6.2.2 ViewModels Principales**

| ViewModel | Propósito | Comandos | Propiedades Clave |
|-----------|-----------|----------|-------------------|
| **MainViewModel** | Navegación entre vistas | MostrarReservas, MostrarSocios, MostrarActividades, MostrarInformes, Salir | Titulo |
| **SocioViewModel** | Gestión de socios | Nuevo, Editar, Eliminar | Socios, SocioSeleccionado |
| **ActividadViewModel** | Gestión de actividades | Nuevo, Editar, Eliminar | Actividades, ActividadSeleccionada |
| **ReservaViewModel** | Gestión de reservas | Nueva, Editar, Cancelar, LimpiarBusqueda | Reservas, ReservaSeleccionada, BuscarSocio, BuscarActividad |
| **InformeViewModel** | Generación de informes | GenerarInforme | InformesDisponibles, InformeSeleccionado, ActividadSeleccionada |
| **NuevoSocioViewModel** | Modal crear socio | Crear, Cancelar | Nombre, Email, Activo |
| **NuevaActividadViewModel** | Modal crear actividad | Crear, Cancelar | Nombre, AforoMaximo |
| **NuevaReservaViewModel** | Modal crear reserva | Crear, Cancelar | SocioSeleccionado, ActividadSeleccionada, FechaReserva |

#### **6.2.3 INotifyPropertyChanged**

Todos los ViewModels implementan `INotifyPropertyChanged` para notificar a la vista cuando cambian las propiedades:

```csharp
private string _nombre;
public string Nombre
{
    get => _nombre;
    set
    {
        _nombre = value;
        OnPropertyChanged(nameof(Nombre)); // Notifica el cambio
    }
}
```

**Beneficios:**
- ✅ Actualización automática de la UI
- ✅ Sincronización bidireccional
- ✅ No requiere código en code-behind

---

### 6.3 Commands (Comandos)

Los comandos encapsulan acciones que se pueden ejecutar desde la vista.

**Ubicación:** `ViewModel/Command/`

#### **6.3.1 RelayCommand**

Implementación personalizada de `ICommand`:

```csharp
/// <summary>
/// Implementación de ICommand para MVVM
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool> _canExecute;
    
    public RelayCommand(Action execute, Func<bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }
    
    public bool CanExecute(object parameter)
    {
        return _canExecute == null || _canExecute();
    }
    
    public void Execute(object parameter)
    {
        _execute();
    }
    
    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
}
```

#### **6.3.2 Uso de Comandos**

**En el ViewModel:**
```csharp
public ICommand NuevoCommand { get; }

public SocioViewModel()
{
    NuevoCommand = new RelayCommand(NuevoSocio);
}

private void NuevoSocio()
{
    // Lógica del comando
}
```

**En XAML:**
```xml
<Button Content="Nuevo" Command="{Binding NuevoCommand}"/>
```

**Ventajas:**
- ✅ No hay código en code-behind
- ✅ Lógica centralizada en ViewModel
- ✅ Fácilmente testeable
- ✅ Desacoplamiento total

---

### 6.4 Services (Servicios de Negocio)

Los servicios contienen la lógica de negocio y coordinan los repositorios.

**Ubicación:** `ViewModel/Services/`

#### **6.4.1 Estructura de un Service**

```csharp
/// <summary>
/// Servicio para la gestión de socios con validaciones de negocio
/// </summary>
public class SocioService
{
    private readonly SocioRepositorio _socioRepo;
    
    public SocioService()
    {
        _socioRepo = new SocioRepositorio();
    }
    
    // Operaciones CRUD con validaciones
    public async Task<List<Socio>> ObtenerSociosAsync()
    public async Task<List<Socio>> ObtenerSociosActivosAsync()
    public async Task CrearSocioAsync(Socio socio)
    public async Task ActualizarSocioAsync(Socio socio)
    public async Task EliminarSocioAsync(Socio socio)
    
    // Validaciones privadas
    private void ValidarSocio(Socio socio)
    private bool EmailValido(string email)
}
```

#### **6.4.2 Responsabilidades de Services**

```
Services
├── Validación de reglas de negocio
├── Coordinación entre repositorios
├── Gestión de transacciones
├── Transformación de datos
├── Manejo de excepciones de negocio
└── Logging (si aplica)
```

#### **6.4.3 Ejemplo de Validación**

```csharp
public async Task CrearSocioAsync(Socio socio)
{
    // 1. Validar entidad
    if (string.IsNullOrWhiteSpace(socio.Nombre))
        throw new ArgumentException("El nombre es obligatorio");
    
    if (!EmailValido(socio.Email))
        throw new ArgumentException("El email no es válido");
    
    // 2. Verificar duplicados
    var existe = await _socioRepo.ExisteEmailAsync(socio.Email);
    if (existe)
        throw new InvalidOperationException("El email ya está registrado");
    
    // 3. Persistir
    await _socioRepo.CrearAsync(socio);
}
```

#### **6.4.4 Services Disponibles**

| Service | Responsabilidad | Métodos Principales |
|---------|----------------|---------------------|
| **SocioService** | Gestión de socios con validación de email | ObtenerSocios, ObtenerSociosActivos, Crear, Actualizar, Eliminar |
| **ActividadService** | Gestión de actividades con validación de aforo | ObtenerActividades, Crear, Actualizar, Eliminar |
| **ReservaService** | Gestión de reservas con control de aforo y fechas | ObtenerReservas, Crear, Actualizar, Eliminar, VerificarDisponibilidad |
| **InformeService** | Generación de DataSets para informes | GenerarDataSetSocios, GenerarDataSetReservasPorActividad, GenerarDataSetHistorialReservas |

---

### 6.5 Acceso a Datos

#### **6.5.1 Entity Framework 6 (Database First)**

El proyecto utiliza **Entity Framework 6** con el enfoque **Database First**:

**Flujo de trabajo:**
```
1. Diseño de base de datos en SQL Server
2. Conexión a la base de datos en Visual Studio
3. Agregar nuevo elemento → Modelo de datos ADO.NET Entity Data Model
4. Seleccionar "EF Designer from database"
5. Configurar conexión
6. Seleccionar tablas, vistas y procedimientos
7. Generar modelo (.edmx)
```

**Archivos generados:**
- `CentroDeportivo.edmx` - Modelo visual
- `CentroDeportivo.Designer.cs` - Clases de entidades
- `Model1.Context.cs` - DbContext
- `Socio.cs`, `Actividad.cs`, `Reserva.cs` - Entidades parciales

#### **6.5.2 Cadena de Conexión**

**Ubicación:** `CentroDeportivo/App.config`

```xml
<connectionStrings>
  <add name="CentroDeportivoEntities" 
       connectionString="metadata=res://*/CentroDeportivo.csdl|
                         res://*/CentroDeportivo.ssdl|
                         res://*/CentroDeportivo.msl;
       provider=System.Data.SqlClient;
       provider connection string=&quot;
       data source=IP_SERVIDOR,1313;
       initial catalog=CentroDeportivo;
       user id=USUARIO;
       password=CONTRASEÑA;
       MultipleActiveResultSets=True;
       Encrypt=True;
       TrustServerCertificate=True;
       App=EntityFramework&quot;" 
       providerName="System.Data.EntityClient" />
</connectionStrings>
```

#### **6.5.3 Operaciones Asíncronas**

Todas las operaciones de base de datos son **asíncronas** para no bloquear la UI:

```csharp
public async Task<List<Socio>> SeleccionarAsync()
{
    return await _context.Socio
        .Include("Reserva")
        .ToListAsync();
}
```

**Ventajas:**
- ✅ Aplicación responsiva
- ✅ No congela la UI durante operaciones largas
- ✅ Mejor experiencia de usuario
- ✅ Escalabilidad

#### **6.5.4 Eager Loading**

Se utiliza **Eager Loading** para cargar relaciones y evitar múltiples consultas:

```csharp
// Cargar reservas con socio y actividad
await _context.Reserva
    .Include("Socio")
    .Include("Actividad")
    .ToListAsync();
```

---

## 7. Base de Datos

### 7.1 Diagrama Entidad-Relación (ER)

```
┌──────────────────────────────────────────────────────────┐
│                       BASE DE DATOS                      │
│                      CentroDeportivo                     │
└──────────────────────────────────────────────────────────┘

┌───────────────────┐               ┌──────────────────────┐
│       Socio       │               │      Actividad       │
├───────────────────┤               ├──────────────────────┤
│    IdSocio (PK)   │               │    IdActividad (PK)  │
│    Nombre         │               │    Nombre            │
│    Email          │               │    AforoMaximo       │
│    Activo         │               └──────────────────────┘
└───────────────────┘                          │
         │                                     │
         │ 1                                 1 │
         │                                     │
         │                                     │
         │       ┌─────────────────────┐       │
         │       │      Reserva        │       │
         │       ├─────────────────────┤       │
         │       │   IdReserva (PK)    │       │
         └─── N  │   IdSocio (FK)      │  N ───┘
                 │   IdActividad (FK)  │
                 │   Fecha             │
                 └─────────────────────┘

Leyenda:
PK = Clave Primaria (Primary Key)
FK = Clave Foránea (Foreign Key)
1  = Relación uno
N  = Relación muchos
```

### 7.2 Descripción de Tablas

#### **7.2.1 Tabla: Socio**

**Propósito:** Almacenar información de los socios del centro deportivo.

| Columna | Tipo | Restricciones | Descripción |
|---------|------|---------------|-------------|
| `IdSocio` | INT | PRIMARY KEY, IDENTITY(1,1) | Identificador único autoincrementable |
| `Nombre` | NVARCHAR(100) | NOT NULL | Nombre completo del socio |
| `Email` | NVARCHAR(100) | NOT NULL, UNIQUE | Correo electrónico único |
| `Activo` | BIT | NOT NULL, DEFAULT(1) | Estado del socio (1=Activo, 0=Inactivo) |

#### **7.2.2 Tabla: Actividad**

**Propósito:** Almacenar las actividades deportivas disponibles.

| Columna | Tipo | Restricciones | Descripción |
|---------|------|---------------|-------------|
| `IdActividad` | INT | PRIMARY KEY, IDENTITY(1,1) | Identificador único autoincrementable |
| `Nombre` | NVARCHAR(100) | NOT NULL | Nombre de la actividad deportiva |
| `AforoMaximo` | INT | NOT NULL, CHECK(AforoMaximo > 0) | Capacidad máxima de personas |

#### **7.2.3 Tabla: Reserva**

**Propósito:** Registrar las reservas realizadas por socios para actividades.

| Columna | Tipo | Restricciones | Descripción |
|---------|------|---------------|-------------|
| `IdReserva` | INT | PRIMARY KEY, IDENTITY(1,1) | Identificador único autoincrementable |
| `IdSocio` | INT | FOREIGN KEY → Socio(IdSocio), NOT NULL | Referencia al socio que reserva |
| `IdActividad` | INT | FOREIGN KEY → Actividad(IdActividad), NOT NULL | Referencia a la actividad reservada |
| `Fecha` | DATETIME | NOT NULL | Fecha y hora de la reserva |

### 7.3 Relaciones

#### **7.3.1 Socio ↔ Reserva (1:N)**

- **Cardinalidad:** Un socio puede tener múltiples reservas
- **Navegación:** `Socio.Reserva` (colección)
- **Restricción:** No se permite eliminar un socio con reservas activas

#### **7.3.2 Actividad ↔ Reserva (1:N)**

- **Cardinalidad:** Una actividad puede tener múltiples reservas
- **Navegación:** `Actividad.Reserva` (colección)
- **Restricción:** No se permite eliminar una actividad con reservas activas

#### **7.3.3 Restricciones de Negocio**

**Control de Aforo:**
```csharp
// Validación en ReservaService
var reservasExistentes = await _reservaRepo
    .ContarReservasPorActividadYFechaAsync(idActividad, fecha);

if (reservasExistentes >= actividad.AforoMaximo)
    throw new InvalidOperationException("Aforo completo");
```

**Validación de Fechas:**
```csharp
// Solo se permiten reservas para hoy o fechas futuras
if (fecha.Date < DateTime.Today)
    throw new ArgumentException("La fecha no puede ser anterior a hoy");
```

**Unicidad de Email:**
```sql
-- Restricción en base de datos
ALTER TABLE Socio
ADD CONSTRAINT UQ_Socio_Email UNIQUE (Email);
```

## 8. Tecnologías y Frameworks

### 8.1 Stack Tecnológico

```
┌─────────────────────────────────────────────────────────────┐
│                    TECNOLOGÍAS UTILIZADAS                   │
└─────────────────────────────────────────────────────────────┘

FRONTEND
├── WPF (Windows Presentation Foundation)
│   ├── XAML (Diseño declarativo)
│   ├── Data Binding
│   └── Styles y Templates
│
BACKEND
├── .NET Framework 4.8
├── C# 7.3
├── LINQ (Language Integrated Query)
├── Async/Await Pattern
│
ACCESO A DATOS
├── Entity Framework 6.4.4
│   ├── Database First
│   ├── LINQ to Entities
│   └── Lazy Loading
│
BASE DE DATOS
├── SQL Server 2022
│
INFORMES
├── Crystal Reports
│   ├── .rpt files
│   ├── DataSets tipados
│   └── Crystal Reports Viewer
│
TESTING
├── MSTest Framework
├── Assert Library
│
HERRAMIENTAS
├── Visual Studio 2022
├── SQL Server Management Studio 2022
├── NuGet Package Manager
└── Git / GitHub
```

## 📝 Historial de Versiones

| Versión | Fecha | Cambios |
|---------|-------|---------|
| 1.0 | Febrero 2026 | Versión inicial del documento técnico |

---

## 👨‍💻 Información del Autor

**Francisco Montes Doria**  
Desarrollador de Software  
📧 Email: f.montesdoria@gmail.com  
🔗 GitHub: [@fmontesdev](https://github.com/fmontesdev)

---

<div align="center">

[⬆ Volver al inicio](#-documentación-técnica---centro-deportivo)

</div>
