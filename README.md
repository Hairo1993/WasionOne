# Wasion One

Aplicación centralizada de operación y administración. Arquitectura: **Angular → HTTP/REST API → ASP.NET Core → Entity Framework Core → SQL Server** (ver `estandar-tecnico-general.md` en el proyecto de Claude).

Este zip trae las dos partes de la aplicación en un solo lugar para que abras la carpeta `WasionOne` completa en VS Code y trabajes ambas desde ahí.

```
WasionOne/
├── frontend/    → Proyecto Angular 21 completo, listo para correr (npm install && npm start)
└── backend/     → Archivos del backend ASP.NET Core (requiere un paso de generación local, ver backend/LEEME.md)
```

## frontend/

Ya es un proyecto Angular completo y verificado (compila y pasa pruebas). Para correrlo:

```
cd frontend
npm install
npm start
```

Abre `http://localhost:4200`.

## backend/

**Importante:** esta carpeta NO es un proyecto .NET ejecutable todavía — son los archivos de código (Models, DTOs, Data, Services, Controllers, Program.cs, appsettings.json) que van sobre un proyecto generado con `dotnet new` en tu máquina, porque mi entorno en la nube no tiene acceso a NuGet para generarlo y compilarlo por ti.

Sigue `backend/LEEME.md` paso a paso: genera el proyecto con `dotnet new webapi`, copia estos archivos encima, corre las migraciones de Entity Framework Core, y prueba el login. Ahí mismo se explica todo con el detalle de comandos.

## Cómo abrir esto en VS Code

1. Descomprime el zip.
2. En VS Code: **Archivo → Abrir carpeta...** y selecciona la carpeta `WasionOne` (la que contiene `frontend/` y `backend/`).
3. Verás ambas carpetas en el explorador de archivos, cada una como su propio proyecto. Puedes abrir dos terminales integradas (una para `frontend`, otra para `backend/WasionOne.API` una vez generado) y correr ambos al mismo tiempo mientras desarrollas.

## Estado actual (Fase 0 del plan)

- [x] Frontend Angular con estructura `core/shared/features`, login y JWT interceptor/guard.
- [x] Backend: catálogos dinámicos (Dirección/Área/Sub-área/Ubicación), CRUD, login JWT temporal (`admin`/`admin123`).
- [ ] Pendiente: correr el backend contra tu SQL Server real y confirmar el flujo completo.
- [ ] Después: Fase 1 (usuarios/roles reales) y Fase 2 (Sub-área piloto: IT).
