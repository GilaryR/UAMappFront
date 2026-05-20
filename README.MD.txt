# SistemaHorarios Backend

Backend del sistema de gestión y generación de horarios académicos universitarios.  
El proyecto está desarrollado en **.NET 8**, usando arquitectura por capas, Entity Framework Core, SQLite, autenticación JWT y documentación automática con Swagger.

---

## Descripción del proyecto

SistemaHorarios es una aplicación backend orientada a la administración académica de horarios universitarios.

Permite gestionar:

- Usuarios y autenticación.
- Roles y permisos.
- Materias.
- Docentes.
- Disponibilidad docente.
- Franjas horarias.
- Planes académicos.
- Semestres del plan.
- Materias asociadas a cada semestre.
- Grupos académicos.
- Generación automática de horarios.

El objetivo principal del backend es permitir que, a partir de las materias de un semestre, la disponibilidad de los docentes y las franjas horarias disponibles, se pueda generar un horario válido sin cruces de docentes ni grupos.

---

## Tecnologías utilizadas

| Tecnología | Descripción |
|---|---|
| .NET 8 | Plataforma principal de desarrollo backend |
| ASP.NET Core Web API | Construcción de API REST |
| Entity Framework Core | ORM para acceso a datos |
| SQLite | Base de datos local basada en archivo |
| JWT | Autenticación y autorización |
| Swagger / OpenAPI | Documentación y pruebas de endpoints |
| BCrypt.Net | Hash seguro de contraseñas |
| C# | Lenguaje principal del backend |

---

## Arquitectura del backend

El backend está organizado por capas:

```txt
Sistema-de-horarios-UAM
│
├── SistemaHorarios.API
│   ├── Controllers
│   ├── Extensiones
│   ├── Middlewares
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
│
├── SistemaHorarios.Logica
│   ├── Negocio
│   ├── Servicios
│   └── Validaciones
│
├── SistemaHorarios.Datos
│   ├── Contexto
│   ├── Repositorios
│   └── Migrations
│
├── SistemaHorarios.Modelos
│   ├── DTOs
│   ├── Entidades
│   ├── Constantes
│   └── Respuestas
│
└── SistemaHorarios.Pruebas