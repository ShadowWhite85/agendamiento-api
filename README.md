# 📅 AgendamientoApi

[![CI](https://github.com/ShadowWhite85/agendamiento-api/actions/workflows/ci.yml/badge.svg)](https://github.com/ShadowWhite85/agendamiento-api/actions/workflows/ci.yml)

**API de agendamiento de citas** para clínicas, peluquerías y negocios de servicios.
CRUD completo con roles, autenticación JWT y documentación Swagger pública.
API REST en **.NET 10** con **Minimal APIs**, EF Core 10 y SQLite.

> Proyecto de portafolio #2 — desarrollado en Riobamba, Ecuador 🇪🇨

## ✨ Funcionalidades

- **CRUD de citas:** cliente, teléfono, servicio, fecha/hora, notas y estados
  (Pendiente → Confirmada → Completada / Cancelada) con filtros por estado y rango de fechas
- **Autenticación JWT con roles:** la recepción crea/edita citas; solo el admin puede eliminar
- **Swagger UI público** con botón *Authorize* para probar con token
- **Seed de demostración:** 2 usuarios + 4 citas al primer arranque
- **6 pruebas automatizadas** (xUnit + WebApplicationFactory) ejecutándose en cada push

## 🔑 Credenciales de demostración

| Rol | Email | Contraseña | Permisos |
|---|---|---|---|
| Admin | `admin@agendamiento.app` | `admin123` | Todo + eliminar citas |
| Recepcionista | `recepcion@agendamiento.app` | `recepcion123` | Crear y editar citas |

## 🔌 Endpoints

| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| POST | `/api/auth/login` | — | Devuelve token JWT |
| GET | `/api/citas?estado=&desde=&hasta=` | Bearer | Lista con filtros opcionales |
| GET | `/api/citas/{id}` | Bearer | Detalle de una cita |
| POST | `/api/citas` | Bearer | Crea una cita |
| PUT | `/api/citas/{id}` | Bearer | Actualiza cita (incluye estado) |
| DELETE | `/api/citas/{id}` | Bearer + Admin | Elimina una cita |

## 🛠️ Stack

| Capa | Tecnología |
|---|---|
| Backend | .NET 10, Minimal APIs, EF Core 10, SQLite |
| Auth | JWT (JwtBearer) + BCrypt.Net-Next |
| Documentación | Swashbuckle 10 (Swagger UI) |
| Tests | xUnit + Microsoft.AspNetCore.Mvc.Testing |
| Contenedores | Docker multi-stage |
| CI/CD | GitHub Actions (build + tests en cada push) |

## 🌐 Demo en vivo

| | URL |
|---|---|
| **API + Swagger** | https://agendamiento-api-jxfj.onrender.com/swagger |

*La API puede tardar ~50 s en despertar la primera vez (plan gratuito de Render).*

## 🚀 Ejecutar en local

```bash
dotnet restore
dotnet run --project Agendamiento.Api   # API en http://localhost:5088
dotnet test                             # 6 pruebas
```

Swagger: `http://localhost:5088/swagger`

## 📂 Estructura

```
AgendamientoApi/
├── Agendamiento.Api/       # API (Models, Data, Dtos, Services, Migrations)
├── Agendamiento.Tests/     # 6 pruebas xUnit
├── Dockerfile              # multi-stage (SDK → aspnet)
└── .github/workflows/      # CI en GitHub Actions
```

## 📖 Proyecto relacionado

Sistema de facturación ecuatoriano (.NET 10 + Angular 22):
https://github.com/ShadowWhite85/facturacion-app
