# SistemaHorarios Frontend UI

Interfaz gráfica de escritorio para el sistema de gestión y generación de horarios académicos de la UAM.  
El frontend está desarrollado en **.NET 8 WPF** y consume la API del backend mediante servicios HTTP.

## 1. Objetivo del frontend

La aplicación permite a los usuarios interactuar visualmente con los módulos del sistema:

- Inicio de sesión.
- Dashboard principal.
- Gestión de docentes.
- Gestión de materias.
- Gestión de grupos académicos.
- Gestión de coordinadores.
- Gestión de plan académico.
- Generación y consulta de horarios.
- Consulta de horarios por grupo.
- Consulta de horarios por docente.
- Aprobación y rechazo de horarios.
- Consulta de reportes.

## 2. Tecnologías utilizadas

- .NET 8
- WPF
- C#
- XAML
- HttpClient
- Arquitectura separada por proyectos

## 3. Estructura del proyecto

```text
UAMappFront
│
├── SistemaHorario.UI
│   ├── Views
│   ├── ViewModels
│   ├── Models
│   ├── Services
│   ├── Resources
│   └── App.xaml
│
├── SistemaHorarios.Application
│
└── SistemaHorarios.Infrastructure
```

## 4. Arquitectura general

La UI mantiene separación entre vistas, modelos, servicios y lógica de presentación.

```text
Views → ViewModels → Services → Backend API
```

### Views

Contiene las pantallas visuales en XAML.

### ViewModels

Contiene la lógica de presentación y comandos usados por las vistas.

### Services

Contiene las clases que consumen la API del backend.

### Models

Contiene los modelos usados por la interfaz para mostrar información.

## 5. Requisitos previos

Antes de ejecutar la UI se debe tener instalado:

- .NET SDK 8
- Windows
- Backend API ejecutándose
- Base de datos MySQL configurada para el backend

Para verificar .NET:

```powershell
dotnet --version
```

## 6. Configurar URL de la API

La UI debe apuntar al backend.

Buscar en los servicios del frontend la URL base de la API.  
Puede estar en archivos como:

```text
SistemaHorario.UI/Services/ApiService.cs
SistemaHorario.UI/Services/*ApiService.cs
```

Ejemplo de URL en desarrollo:

```csharp
private readonly string _baseUrl = "https://localhost:7208/api";
```

Ejemplo de URL para ejecutable local publicado:

```csharp
private readonly string _baseUrl = "http://localhost:5000/api";
```

La URL debe coincidir con el puerto real donde esté corriendo el backend.

## 7. Restaurar paquetes

Desde la raíz del frontend:

```powershell
dotnet restore
```

## 8. Compilar el frontend

```powershell
dotnet build
```

Si la compilación termina correctamente, se mostrará un mensaje similar a:

```text
Compilación realizada correctamente.
```

## 9. Ejecutar la aplicación

Desde la raíz del frontend:

```powershell
dotnet run --project SistemaHorario.UI/SistemaHorario.UI.csproj
```

Orden recomendado antes de abrir la UI:

```text
1. Iniciar MySQL.
2. Ejecutar backend API.
3. Ejecutar frontend UI.
```

## 10. Módulos de la aplicación

### Login

Permite iniciar sesión en el sistema.

Validaciones principales:

- El usuario debe existir.
- La contraseña debe ser correcta.
- El usuario debe estar activo.
- Los coordinadores inactivos no pueden iniciar sesión.

### Inicio / Dashboard

Muestra información general del sistema, como resumen de módulos, horarios y datos académicos relevantes.

### Docentes

Permite:

- Crear docentes.
- Editar docentes.
- Inactivar docentes.
- Activar docentes.
- Ver disponibilidad docente.
- Ver materias asignadas al docente.
- Ver horario del docente.

Los docentes inactivos se muestran visualmente con un estilo diferente para identificarlos fácilmente.

### Materias

Permite:

- Crear materias.
- Editar materias.
- Inactivar materias.
- Consultar intensidad horaria.
- Consultar créditos y estado.

### Grupos académicos

Permite:

- Crear grupos.
- Editar grupos.
- Inactivar grupos.
- Consultar jornada.
- Consultar semestre.
- Consultar plan académico asociado.

Los grupos representan conjuntos de estudiantes que ven clases juntos.

Ejemplo:

```text
SIST-D-1A
```

Puede representar:

```text
Sistemas · Diurna · Semestre 1 · Grupo A
```

### Plan académico

Permite consultar y administrar la organización del plan académico, semestres, materias y malla curricular.

### Horarios

El módulo de horarios permite generar y administrar el horario académico por grupo.

La pantalla debe mostrar el horario del estudiante, es decir, el horario del grupo académico.

Funcionalidades principales:

- Generar horario por grupo.
- Regenerar horario si ya existe.
- Ver horario del grupo.
- Ver horario del docente.
- Editar bloques de horario.
- Aprobar horario.
- Rechazar horario con motivo.

Reglas visuales y funcionales:

- Los bloques son de 2 horas.
- Se muestran materias, docentes, grupos y franjas.
- No se permite mover una clase a una franja inválida.
- Si una edición no se puede realizar, la UI muestra el motivo.
- Se bloquean las franjas de descanso.
- El horario docente se deriva del horario generado para los grupos.

### Reportes

Permite consultar reportes académicos y exportarlos.

Reportes disponibles:

- Horarios generados.
- Horario por grupo.
- Horario por docente.
- Carga docente.
- Conflictos de horarios.

Funcionalidades principales:

- Vista previa.
- Descargar PDF.
- Descargar CSV.
- Filtrar información.

### Coordinadores

Permite administrar coordinadores del sistema.

Funcionalidades principales:

- Crear coordinador.
- Editar coordinador.
- Activar coordinador.
- Inactivar coordinador.
- Visualizar estado activo/inactivo.

Si un coordinador está inactivo, el backend no permite que inicie sesión.

## 11. Flujo recomendado para probar la UI

1. Ejecutar MySQL.
2. Ejecutar backend.
3. Ejecutar frontend.
4. Iniciar sesión.
5. Verificar dashboard.
6. Crear o revisar docentes.
7. Crear o revisar materias.
8. Crear o revisar grupos académicos.
9. Generar horario por grupo.
10. Ver horario del grupo.
11. Ver horario del docente.
12. Aprobar o rechazar horario.
13. Consultar reportes.

## 12. Publicar frontend como ejecutable

Desde la raíz del frontend:

```powershell
dotnet publish SistemaHorario.UI/SistemaHorario.UI.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true -o publish/ui
```

El ejecutable quedará en:

```text
publish/ui
```

Archivo principal:

```text
SistemaHorario.UI.exe
```

## 13. Ejecutar versión publicada

Antes de abrir la UI publicada, se debe tener:

- MySQL iniciado.
- Backend API ejecutándose.
- Base de datos importada.
- URL del backend correctamente configurada en la UI.

Luego ejecutar:

```text
SistemaHorario.UI.exe
```

## 14. Entrega en otro computador

Para entregar la aplicación a otra persona, se recomienda usar esta estructura:

```text
SistemaHorarios_Entrega
│
├── API
│   ├── SistemaHorarios.API.exe
│   ├── appsettings.json
│   └── demás archivos publicados
│
├── UI
│   ├── SistemaHorario.UI.exe
│   └── demás archivos publicados
│
├── BaseDatos
│   └── sistema_horarios.sql
│
└── LEEME_INSTALACION.txt
```

La UI sola no funciona sin la API y sin la base de datos.

## 15. Problemas comunes

### La UI abre pero no carga datos

Revisar:

- Que la API esté ejecutándose.
- Que la URL de la API en la UI sea correcta.
- Que MySQL esté iniciado.
- Que la base de datos exista.

### Error al iniciar sesión

Revisar:

- Usuario y contraseña.
- Estado del usuario.
- Conexión con backend.
- Respuesta del endpoint de login.

### No aparecen horarios

Revisar:

- Que existan grupos académicos.
- Que existan materias en el semestre.
- Que existan docentes disponibles.
- Que los docentes puedan dictar las materias.
- Que la API esté generando horarios correctamente.

### No aparecen reportes

Primero se deben generar horarios.  
Los reportes dependen de la información existente en la base de datos.

## 16. Consideraciones importantes

- No modificar la URL de API sin recompilar si está quemada en código.
- No ejecutar la UI antes del backend.
- No borrar archivos generados en la carpeta `publish`.
- No cambiar nombres de carpetas internas si la aplicación publicada depende de ellas.

## 17. Autores

Proyecto académico desarrollado para la gestión y generación de horarios académicos.
