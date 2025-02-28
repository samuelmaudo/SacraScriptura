# SacraScriptura API

Una API RESTful desarrollada con C# y .NET Core, utilizando una arquitectura limpia y una base de datos PostgreSQL.

## Estructura del Proyecto

El proyecto sigue los principios de Clean Architecture con las siguientes capas:

- **Domain**: Contiene las entidades y reglas de negocio.
- **Application**: Contiene los casos de uso y lógica de aplicación.
- **Infrastructure**: Contiene implementaciones concretas (base de datos, servicios externos, etc.).
- **API**: Contiene los controladores y configuración de la API.

## Requisitos Previos

- [.NET Core SDK](https://dotnet.microsoft.com/download) (versión 9.0 o superior)
- [Docker](https://www.docker.com/products/docker-desktop)
- [Docker Compose](https://docs.docker.com/compose/install/)

## Cómo Ejecutar

### Usando Docker Compose

1. Clona este repositorio
2. Navega a la carpeta del proyecto
3. Ejecuta el siguiente comando:

```bash
docker-compose up -d
```

La API estará disponible en `http://localhost:8080/health` para verificar el estado.

### Desarrollo Local

1. Clona este repositorio
2. Navega a la carpeta del proyecto
3. Restaura las dependencias:

```bash
dotnet restore
```

4. Ejecuta la aplicación:

```bash
dotnet run --project src/SacraScriptura.API/SacraScriptura.API.csproj
```

## Endpoints

- **GET /health**: Endpoint de healthcheck que verifica el estado de la API y la conexión a la base de datos.

## Documentación de la API

La documentación de la API está disponible a través de Swagger UI cuando la aplicación se ejecuta en modo de desarrollo:

```
http://localhost:8080/swagger
```

## Licencia

Este proyecto está licenciado bajo la licencia MIT.
