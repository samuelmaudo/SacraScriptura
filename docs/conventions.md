# SacraScriptura Project Conventions and Style Guide

This document describes the main conventions, architecture, and best practices found in the `src/` folder of the SacraScriptura project. It is intended as a practical guide for agents and developers who want to build new projects following the same standards.

## Table of Contents
- [Folder Structure](#folder-structure)
- [Naming Conventions](#naming-conventions)
- [Dependency Injection](#dependency-injection)
- [Controllers and Services](#controllers-and-services)
- [Repositories](#repositories)
- [Documentation and Comments](#documentation-and-comments)
- [Error Handling](#error-handling)
- [Admin vs Web Contexts](#admin-vs-web-contexts)

---

## Folder Structure
- **Separation by Context and Layer:**
  - Each functional context (`Admin`, `Web`, `Shared`) is divided into four main subfolders:
    - `API`: Contains controllers and entry points for HTTP APIs.
    - `Application`: Application services, use cases, and orchestration logic.
    - `Domain`: Domain models, interfaces, business rules, and core logic.
    - `Infrastructure`: Data access, external integrations, and implementation details.
  - Inside each layer, main entities have their own subfolders, grouping related files (models, services, repositories, DTOs, etc.).
  - Controllers are placed in `API/Controllers`, and further grouped by entity for clarity and scalability.
- **Configuration and Project Files:**
  - Files like `appsettings.json` and `.csproj` are in the root of each project/layer.

## Naming Conventions
- **Classes and Files:**
  - Use PascalCase for all class, file, and folder names.
  - Name classes according to their responsibility: e.g., `BibleCreator`, `BookFinder`, `DivisionSearcher`.
  - Interfaces always start with `I`, e.g., `IBibleRepository`, `IEmbeddingsGenerator`.
  - Folders and namespaces follow `[Project].[Context].[Layer]`, such as `SacraScriptura.Admin.Domain.Bibles`.
  - File names should match the main class or interface they contain.
- **Controllers:**
  - Controller classes end with `Controller` (e.g., `GetBibleController`).
  - Use `[ApiController]` and `[Route("api/[entity]")]` attributes for routing and API conventions.
  - Group controllers by entity in subfolders for clarity.
- **Services:**
  - Service classes use suffixes to indicate their main action: `Creator`, `Deleter`, `Finder`, `Searcher`, `Updater`.

## Dependency Injection
- Each layer has a static `DependencyInjection.cs` file responsible for registering all services, repositories, and configuration for that layer.
- The registration method is named following the pattern: `Add[Context][Layer]` (e.g., `AddAdminInfrastructure`, `AddWebApplication`).
- Use `IServiceCollection` for dependency registration and `IConfiguration` for configuration values when needed.
- Register services and repositories as `Scoped` (one instance per request), unless a different lifetime is required for technical reasons.
- Register interfaces to their concrete implementations to enable easy substitution and testing.

## Controllers and Services
- Controllers receive all dependencies via constructor injection (DI).
- All controller actions that access data or external resources should be asynchronous (`async`/`await`).
- Use Data Transfer Objects (DTOs) for all input and output in API endpoints.
- Controllers should be thin: delegate business logic to services, avoid direct data access or complex logic in controllers.
- Services encapsulate business logic, validation, and orchestration. Application services coordinate use cases; domain services encapsulate domain-specific rules.

## Repositories
Repositories are a key pattern in this project for data access and encapsulation. Follow these conventions:

- **Interface and Implementation:**
  - Each main entity (Bible, Book, Division) has a repository interface (e.g., `IBibleRepository`) in the `Domain` layer and a concrete implementation (e.g., `BibleRepository`) in the `Infrastructure` layer.
  - Interfaces define all CRUD and query operations, using async methods that return `Task` or `Task<T>`.
- **Naming and Structure:**
  - Use clear, descriptive method names: `GetAllAsync`, `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`, etc.
  - For hierarchical data (like divisions), include methods for tree operations: `GetHierarchyByBookIdAsync`, `MoveToChildOfAsync`, `DeleteAsync` (with children), etc.
- **Persistence:**
  - Use Entity Framework (with `DbContext`) or direct SQL (with parameterized queries) depending on the context.
  - Always save changes asynchronously (`await context.SaveChangesAsync()`).
- **Registration:**
  - Register repositories in `DependencyInjection.cs` as `Scoped`.
  - Use interface-to-implementation mapping: `services.AddScoped<IBibleRepository, BibleRepository>()`.
- **Error Handling:**
  - Throw meaningful exceptions when entities are not found or operations are invalid.
  - Catch exceptions in controllers and return appropriate HTTP responses.

## Documentation and Comments
- Use XML comments (`/// <summary> ... </summary>`) for all public classes, interfaces, methods, and properties, especially in controllers, services, and repositories.
- Document the purpose, parameters, return values, and exceptions for each method.
- Use `<param>`, `<returns>`, and `<exception>` tags for clarity.
- Keep comments up-to-date and concise, focusing on what the code does and why.

## Error Handling
- Throw specific exceptions (e.g., `KeyNotFoundException`, `ArgumentException`) in services and repositories to signal errors or invalid operations.
- In controllers, catch exceptions and translate them into appropriate HTTP responses: `NotFound()` for missing resources, `BadRequest()` for validation errors, `CreatedAtAction()` for successful creation, etc.
- Always provide clear, actionable error messages in API responses, avoiding exposure of sensitive details.
- Use global exception handling middleware if needed to centralize error handling and logging.

---

## Admin vs Web Contexts

The SacraScriptura project is organized into two main application contexts: **Admin** and **Web**. Each has a distinct role and follows different architectural principles:

### Admin Context
- **Purpose:** Contains the core business logic and domain rules of the system.
- **Layers:** Follows a full layered architecture: `API`, `Application`, `Domain`, and `Infrastructure`.
- **Business Rules:** Business logic is implemented in Application and Domain services (e.g., `BookFinder`, `DivisionSearcher`). These services coordinate validation, entity consistency, and complex operations.
- **Persistence:** Uses Entity Framework and a rich domain model for data access and manipulation.
- **Use Cases:** Designed for administrative tools, data management, and operations that require enforcing business rules and workflows.
- **Testing:** Business logic and rules are testable independently of the API or infrastructure.

### Web Context (Backend for Frontend, BFF)
- **Purpose:** Acts as a Backend for Frontend (BFF), providing a simplified API tailored to the needs of the frontend application.
- **Layers:** Uses the same folder structure but with lighter Application and Domain layers. Often, the domain is just a set of DTOs or record types.
- **Business Rules:** Contains minimal or no business logic. It delegates complex operations to the Admin context or exposes data in a frontend-friendly way.
- **Persistence:** Often uses direct SQL queries or thin repositories, focusing on data retrieval and simple transformations.
- **Use Cases:** Designed to optimize frontend performance and developer experience, not to enforce core business rules.
- **Testing:** Focuses on endpoint correctness and data shaping, not on business logic.

### Summary Table
| Aspect              | Admin Context           | Web Context (BFF)     |
|---------------------|-------------------------|-----------------------|
| Main Role           | Business logic & rules  | API for frontend      |
| Business Logic      | Rich, validated         | Minimal, delegated    |
| Data Access         | EF Core, domain model   | Direct SQL/DTOs       |
| Use Cases           | Admin tools, workflows  | UI/API consumption    |
| Testing Focus       | Logic & validation      | Endpoint/data shape   |

> **Guideline:** Use the Admin context for any operation that changes data or must enforce business rules. Use the Web context only for exposing data and simple actions needed by the frontend, keeping it as thin as possible.

> **Note:** These conventions are based on the current structure and code. Please keep and expand them as the project evolves.
