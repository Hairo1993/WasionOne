# Wasion One — Backend (Fase 0)

Este paquete NO es un proyecto .NET completo (mi entorno no tiene acceso a NuGet para generarlo y compilarlo aquí). Es el conjunto de archivos que van **sobre** un proyecto generado por el propio `dotnet new`, que sí sabe resolver las versiones correctas de los paquetes.

## 1. Generar el proyecto base

En tu máquina, donde quieras tener el backend:

```
dotnet new webapi -n WasionOne.API -o WasionOne.API --use-controllers
cd WasionOne.API
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Swashbuckle.AspNetCore
```

## 2. Borrar los archivos de ejemplo del template

```
WeatherForecast.cs
Controllers/WeatherForecastController.cs
```

## 3. Copiar estos archivos dentro de `WasionOne.API/`

Todo lo que trae esta carpeta `WasionOne.API/` se copia tal cual, respetando la misma estructura de subcarpetas (`Models/`, `DTOs/`, `Data/`, `Interfaces/`, `Services/`, `Controllers/`). `Program.cs` y `appsettings.json` **reemplazan** a los que generó el template.

## 4. Configurar tu conexión a SQL Server

En `appsettings.json`, ajusta `ConnectionStrings:DefaultConnection` con tu servidor/instancia real (usuario/password si no usas autenticación de Windows).

## 5. Crear la base de datos con las migraciones de EF Core

```
dotnet ef migrations add InicialCatalogos
dotnet ef database update
```

Esto crea las tablas `Direccion`, `Area`, `SubArea`, `Ubicacion`, `SubAreaUbicacion` y las deja pobladas con la estructura organizacional que ya definimos (Administración Centralizada → Seguridad/RH, las 5 Ubicaciones, e IT como la única Sub-área ya confirmada en las 5 ubicaciones).

## 6. Correr la API

```
dotnet run
```

Abre `https://localhost:<puerto>/swagger` (el puerto lo indica la consola al arrancar) para ver la documentación interactiva.

## 7. Probar el flujo login → JWT → endpoint protegido

1. `POST /api/auth/login` con body `{ "usuario": "admin", "password": "admin123" }` → regresa un token JWT.

   ⚠️ **Esto es temporal.** No hay todavía tabla de Usuarios ni roles reales — es solo para dejar probado el mecanismo completo (login, emisión de JWT, protección de endpoints). En la Fase 1 del plan esto se reemplaza por la validación real contra la tabla `Usuario` con roles (CEO, Director, Responsable de Área, Responsable de Sub-área) y su posición en la jerarquía.

2. Copia el `token` de la respuesta y en Swagger da clic en **Authorize**, pega `Bearer <token>`.
3. Prueba `GET /api/catalogos/direcciones` — sin el token da 401; con el token, regresa las 2 Direcciones sembradas.

## Notas

- La clave (`Jwt:ClaveSecreta`) que dejé en `appsettings.json` es un valor de ejemplo — sirve para desarrollo local, pero no debe quedar así en un ambiente compartido o en producción (mover a `dotnet user-secrets` o variables de entorno, como pide el estándar).
- El origen `http://localhost:4200` / `https://localhost:4200` ya está habilitado en CORS para que el frontend Angular (que corre en ese puerto con `npm start`) pueda consumir esta API sin bloqueos.
- Si algo no compila al hacer `dotnet build`, mándame el error exacto y lo corregimos — no pude compilar esto yo mismo porque mi entorno en la nube no tiene acceso a NuGet.
