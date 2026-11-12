# Order Intake & Tracking

A full-stack order management application built with **ASP.NET Core Web API**, **Entity Framework Core**, **SQLite**, and **Angular**.

The application allows users to create orders, view existing orders, inspect order details, and manage order statuses. It also includes server-side validation, order total calculations, and duplicate order reference protection.

## Technology Stack

### Backend
- .NET 10 / ASP.NET Core Web API
- C#
- Entity Framework Core
- SQLite
- xUnit

### Frontend
- Angular
- TypeScript
- Standalone components
- Reactive Forms
- Angular Signals
- HttpClient

## Project Structure

```text
order-management/
│
├── backend/
│   ├── OrderManagement.sln
│   ├── OrderManagement.Domain/
│   ├── OrderManagement.Application/
│   ├── OrderManagement.Infrastructure/
│   ├── OrderManagement.API/
│   └── OrderManagement.Tests/
│
├── frontend/
│   └── order-management-ui/
│
├── README.md
└── SOLUTION.md
```

## Running the Backend

### 1. Navigate to the backend directory

```bash
cd backend
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Apply database migrations

The application uses SQLite for local data persistence.

If migrations have already been included in the repository, run:

```bash
dotnet tool install --global dotnet-ef
```

Then apply the migrations:

```bash
dotnet ef database update --project OrderManagement.Infrastructure --startup-project OrderManagement.API
```

If the EF Core CLI is already installed, you can skip the installation command.

If no migrations have been created yet, create the initial migration first:

```bash
dotnet ef migrations add InitialCreate --project OrderManagement.Infrastructure --startup-project OrderManagement.API
```

Then run the database update command.

### 4. Run the API

```bash
dotnet run --project OrderManagement.API
```

The API will start using the URLs configured in `OrderManagement.API/Properties/launchSettings.json`.

For example:

```text
https://localhost:7001
http://localhost:5000
```

Your actual URLs may differ. Check the terminal output when the API starts.

### 5. Open Swagger

Open the Swagger URL shown in the terminal, usually:

```text
https://localhost:7001/swagger
```

Swagger can be used to explore and test the available API endpoints.

## Running the Angular Frontend

### 1. Open a second terminal

From the repository root, navigate to the Angular application:

```bash
cd frontend/order-management-ui
```

### 2. Install dependencies

```bash
npm install
```

### 3. Configure the API URL

Open the environment configuration file:

```text
src/environments/environment.ts
```

Ensure the API URL matches the URL where your backend is running:

```typescript
export const environment = {
  apiUrl: 'https://localhost:7001/api'
};
```

Update the URL if your backend uses a different port or protocol.

### 4. Start Angular

```bash
npm start
```

Open the application in your browser:

```text
http://localhost:4200
```

## Application Features

- **Create orders:** Capture customer details, external references, and one or more order lines.
- **View orders:** Display existing orders, with the newest orders shown first.
- **View order details:** Inspect order information, individual line totals, and the overall order total.
- **Calculate totals:** Calculate line totals and order totals on the server to ensure data integrity.
- **Prevent duplicate orders:** Prevent duplicate external references from creating unintended duplicate orders.
- **Manage order statuses:** Update order statuses according to the permitted business transitions.
- **Validation:** Validate required fields, positive quantities, and non-negative unit prices.
- **Error handling:** Return meaningful API errors for invalid requests and business rule violations.

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/orders` | Create an order |
| GET | `/api/orders` | Retrieve all orders |
| GET | `/api/orders/{id}` | Retrieve an order by ID |
| PATCH | `/api/orders/{id}/status` | Update an order's status |

### Database or migration errors

- Confirm that the SQLite connection string is configured correctly.
- Ensure the EF Core tools are installed.
- Confirm that the EF Core package versions are compatible with the backend target framework.
- Run the database migration command again.

## Design and Architecture

The backend follows a layered architecture:

- **Domain:** Contains core business entities, enums, and business rules.
- **Application:** Contains application services, DTOs, and repository abstractions.
- **Infrastructure:** Implements data persistence and repository operations using Entity Framework Core and SQLite.
- **API:** Exposes REST endpoints and handles HTTP requests and responses.

The Angular frontend is organised around application features and uses standalone components, reactive forms, and services to communicate with the backend API.

For additional architectural details, implementation decisions, and trade-offs, see [SOLUTION.md](SOLUTION.md).

