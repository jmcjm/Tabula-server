# Tabula Server 🛒

Tabula Server is the backend API for a shopping list management application. It allows users to create and manage shopping lists, add items, categorize them with tags, and share lists with other users.

The project is built with .NET 9, ASP.NET Core, Entity Framework Core, and PostgreSQL, orchestrated using .NET Aspire for development and Docker for deployment.

## Features ✨

*   **👤 User Management:** Registration, login, and role-based access control (admin, user) using ASP.NET Core Identity.
*   **📝 Shopping List CRUD:** Create, read, update, and delete shopping lists.
*   **🛍️ Item Management:** Add, update, and remove items within shopping lists.
*   **🏷️ Tagging System:** Create custom tags (with names and colors) and assign them to shopping lists for better organization (max 5 tags per list).
*   **🤝 List Sharing:** Securely share shopping lists with other users, granting either `ReadOnly` or `ReadWrite` permissions.
*   **🔐 JWT Authentication:** Secure API endpoints using JSON Web Tokens.
*   **🐳 Dockerized:** Ready for containerized deployment with Docker and Docker Compose.
*   **🚀 .NET Aspire Orchestration:** Simplified development setup and service discovery.

## Tech Stack 🛠️

*   **.NET 9** & **ASP.NET Core**
*   **Entity Framework Core 9**
*   **PostgreSQL** 
*   **.NET Aspire 9.3.1**
*   **Docker** & **Docker Compose**
*   **ASP.NET Core Identity** 
*   **JWT** 
*   **Swagger/OpenAPI** 
*   **ErrorOr**
*   **OpenTelemetry** 

## Project Structure 📁

The solution follows **Domain-Driven Design (DDD)** principles and is organized into the following layers:

### 🏗️ Architecture Layers
*   `Tabula.Domain/`: **Pure domain layer** - Contains domain entities, value objects, aggregates, and business rules (no external dependencies).
*   `Tabula.Application/`: **Application layer** - CQRS commands/queries, MediatR handlers, and application services.
*   `Tabula.Infrastructure/`: **Infrastructure layer** - Data access, external services, EF Core repositories, and Identity.
*   `Tabula.Presentation/`: **Presentation layer** - ASP.NET Core Web API with Minimal API endpoints.

### 🚀 Host & Configuration
*   `Tabula.AppHost/`: .NET Aspire application host for orchestrating services during development.
*   `Tabula.ServiceDefaults/`: Common service configurations (logging, telemetry, health checks).

### 📦 Legacy Projects (Phase 2+ Migration)
*   `Tabula.Infrastructure.DataAccess/`: *[Being migrated to Tabula.Infrastructure]*
*   `Tabula.Infrastructure.Identity/`: *[Being migrated to Tabula.Infrastructure]*

**🔄 Migration Status:** Currently in Phase 1 - Solution restructure complete. Domain entities and business logic will be moved to their respective DDD layers in subsequent phases.

## Getting Started 🚀

### Prerequisites 📋

*   .NET 9 SDK
*   Docker Desktop (or Docker Engine + Docker Compose) 
*   A code editor like Visual Studio 2022+ or JetBrains Rider 

### Configuration ⚙️

1.  **📥 Clone the repository:**
    ```bash
    git clone https://github.com/your-username/Tabula-server-clean.git
    cd Tabula-server-clean
    ```

2.  **🔧 Set up environment variables:**
    The `Tabula.AppHost` project uses an `.env` file for configuration, especially when running with Docker Compose directly or for the Aspire-generated Docker Compose.
    *   Navigate to the `Tabula.AppHost` directory:
        ```bash
        cd Tabula.AppHost
        ```
    *   Create a `.env` file by copying the example:
        ```bash
        cp .env.example .env
        ```
    *   Edit the `.env` file and provide values for the following variables:
        ```env
        # Parameter DbUsername
        DBUSERNAME=your_db_user # e.g., tabula_user

        # Parameter DbPasswd
        DBPASSWD=your_strong_db_password

        # Container image name for apiservice
        APISERVICE_IMAGE=apiservice:latest

        # Parameter AdminUsername
        ADMINUSERNAME=admin_username # e.g., admin

        # Parameter AdminEmail
        ADMINEMAIL=admin_email@example.com

        # Parameter AdminPassword (must be strong: min 12 chars, upper, lower, digit, special)
        ADMINPASSWORD=your_strong_admin_password

        # Parameter JwtKey (a long, random, secret string for signing JWTs)
        JWTKEY=your_very_strong_and_secret_jwt_key
        ```
    **⚠️ Important:**
    *   The `DBUSERNAME` and `DBPASSWD` will be used to create a new user and database in PostgreSQL.
    *   The `ADMINPASSWORD` must meet complexity requirements: at least 12 characters, including uppercase, lowercase, a digit, and a special character.
    *   The `JWTKEY` should be a strong, randomly generated secret.

### Running with .NET Aspire (Development) 🏃‍♂️

This is the recommended way to run the application during development.

1.  **📁 Ensure you are in the root directory of the solution.**
2.  **🔐 Set User Secrets for Aspire Parameters (if not using `.env` directly for Aspire dev):**
    For parameters marked `secret: true` in `Tabula.AppHost/Program.cs` (`DbPasswd`, `AdminPassword`, `JwtKey`), you can also set them using .NET User Secrets for the `Tabula.AppHost` project. Aspire will prioritize `.env` file values if both are present for a given parameter.
    To set user secrets (from the `Tabula.AppHost` directory):
    ```bash
    dotnet user-secrets init
    dotnet user-secrets set "DbPasswd" "your_strong_db_password"
    dotnet user-secrets set "AdminPassword" "your_strong_admin_password"
    dotnet user-secrets set "JwtKey" "your_very_strong_and_secret_jwt_key"
    # For non-secret parameters (optional, as .env is primary for these for compose)
    dotnet user-secrets set "DbUsername" "your_db_user"
    dotnet user-secrets set "AdminUsername" "admin_username"
    dotnet user-secrets set "AdminEmail" "admin_email@example.com"
    ```

3.  **▶️ Run the AppHost project:**
    You can run it from your IDE (Visual Studio, Rider) by setting `Tabula.AppHost` as the startup project and running it.
    Alternatively, from the command line in the `Tabula.AppHost` directory:
    ```bash
    dotnet run
    ```
4.  **📊 Access the Aspire Dashboard:**
    Once running, the Aspire Dashboard will be available (typically at `http://localhost:15091` or `https://localhost:17051`). It shows the status of services, logs, and connection strings.
5.  **🌐 API Access:**
    The `apiservice` will be running on the port assigned by Aspire. Check the Aspire Dashboard for the exact URL (e.g., `http://localhost:XXXXX`).
    Swagger UI will be available at `/swagger` on the `apiservice` URL (e.g., `http://localhost:XXXXX/swagger`).

### Running with Docker Compose (Production-like) 🐳

The `Tabula.AppHost/docker-compose.yaml` file is designed to be used by Aspire for generating compose files for deployment, or can be used with `docker-compose up` if you build the `apiservice` image manually with the tag specified in `.env`.

1.  **🔨 Build the `apiservice` Docker image:**
    From the root of the solution:
    ```bash
    # Ensure APISERVICE_IMAGE in Tabula.AppHost/.env matches the tag you use here
    # e.g., if APISERVICE_IMAGE=apiservice:latest
    docker build -t apiservice:latest -f Tabula.Presentation/Dockerfile .
    ```

2.  **✅ Ensure your `Tabula.AppHost/.env` file is correctly configured (see Configuration section).**

3.  **🏃‍♂️ Run Docker Compose:**
    From the `Tabula.AppHost` directory:
    ```bash
    docker-compose up -d
    ```

4.  **🌐 Access the API:**
    The API service will be available on the port mapped in `docker-compose.yaml` (e.g., `http://localhost:8002` as per `ports: - "8002:8001"`).
    Swagger UI will be available at `/swagger` (e.g., `http://localhost:8002/swagger`).
    PgAdmin (if included in your compose setup or run separately) can be used to connect to the PostgreSQL instance on port `8000`.

### Database Initialization 🗄️

On the first run, the application will:
*   Apply Entity Framework Core migrations to create the database schema for both `TabulaDb` (application data) and `IdentityDb` (user data).
*   Create an admin user with the credentials specified in your `.env` file (or user secrets for Aspire).

## API Endpoints 🌐

The API provides endpoints for managing:
*   **🔐 Authentication:** `/auth/login`, `/auth/register`
*   **📝 Shopping Lists:** `/shoppinglists`
*   **🛍️ Items:** `/items`
*   **🏷️ Tags:** `/tags`, `/shoppinglists/{listId}/tags`
*   **🤝 Sharing:** `/sharing`

Refer to the Swagger UI (`/swagger`) for detailed API documentation and to try out the endpoints.

## License 📄

This project is licensed under the **GNU General Public License v3.0**. See the [LICENSE](LICENSE) file for details.