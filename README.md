# SacraScriptura

A RESTful API developed with C# and .NET Core, using a clean architecture and a PostgreSQL database.

## Project Structure

The project follows the principles of Clean Architecture with the following layers:

- **API**: Contains the controllers and API configuration.
- **Application**: Contains use cases and application logic.
- **Domain**: Contains entities and business rules.
- **Infrastructure**: Contains concrete implementations (database, external services, etc.).

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) (version 9.0 or higher)
- [Docker](https://www.docker.com/products/docker-desktop)
- [Docker Compose](https://docs.docker.com/compose/install/)

## Local Development

1. Clone this repository
2. Navigate to the project folder
3. Restore dependencies:

    ```bash
    make dependencies
    ```

4. Start the database:

    ```bash
    make db
    ```

5. Run the database migrations:

    ```bash
    make migrations
    ```

6. Launch the application:

    ```bash
    make app
    ```

The API will be available at `http://localhost:8080/health` to check the status.

## API Documentation

The API documentation is available through Swagger UI when the application is running in development mode:

```
http://localhost:8080/swagger
```

## License

This project is licensed under the MIT license.
