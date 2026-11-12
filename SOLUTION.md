# Solution Design – Order Intake & Tracking

## 1. Overview

The Order Intake & Tracking application was designed to provide a simple, maintainable solution for creating, viewing, and managing customer orders.

The primary focus was to ensure:

- Clear separation of responsibilities.
- Consistent enforcement of business rules.
- Reliable order calculations.
- Protection against duplicate order submissions.
- Meaningful validation and error responses.
- A straightforward user experience.
- Testability and maintainability.

The solution consists of an ASP.NET Core Web API backend and an Angular frontend, with SQLite used for data persistence.

## 2. Technology Choices

| Technology | Purpose | Reason for selection |
|---|---|---|
| ASP.NET Core | Backend API | Provides a robust framework for building RESTful APIs. |
| C# | Backend language | Strong typing and support for object-oriented domain modelling. |
| Entity Framework Core | Data access | Simplifies persistence and provides database constraints and migrations. |
| SQLite | Database | Lightweight, requires minimal local setup, and supports relational constraints. |
| Angular | Frontend | Provides a structured component-based framework for building maintainable applications. |
| TypeScript | Frontend language | Provides static typing and improves maintainability. |
| xUnit | Unit testing | Provides a straightforward framework for automated backend testing. |

## 3. Backend Architecture

The backend follows a layered architecture to separate business logic, application workflows, persistence, and HTTP concerns.

### Domain Layer

The Domain layer contains the core business entities and rules.

The main entities are:

- `Order`
- `OrderLine`

The `Order` entity is responsible for maintaining order-related business rules, including valid status transitions.

The `OrderLine` entity ensures that quantities and unit prices meet the required constraints.

Business rules are encapsulated within the domain rather than being implemented exclusively in API controllers. This ensures that the rules remain consistent regardless of how the application is accessed.

### Application Layer

The Application layer coordinates the application's use cases.

It contains:

- Application services.
- Request and response DTOs.
- Repository interfaces.
- Order creation and retrieval workflows.
- Status update operations.

The application service coordinates the creation of orders, checks for existing external references, interacts with repository abstractions, and maps domain entities to response DTOs.

This keeps application workflows separate from persistence and HTTP implementation details.

### Infrastructure Layer

The Infrastructure layer handles data persistence.

Entity Framework Core is used to communicate with SQLite.

The repository implementation is responsible for retrieving and persisting orders, while the database context configures entity relationships, indexes, and persistence behaviour.

A unique database index is configured for the external order reference to provide an additional level of protection against duplicate records.

### API Layer

The API layer exposes RESTful endpoints for the frontend.

Controllers handle HTTP requests and delegate business operations to application services.

Centralised exception handling provides consistent error responses and maps common failures to appropriate HTTP status codes.

This prevents controllers from becoming responsible for business logic and keeps HTTP-specific concerns isolated.

## 4. Frontend Architecture

The frontend is implemented using Angular and TypeScript.

The application uses standalone components and a feature-oriented structure to keep the code organised and easier to maintain.

The main responsibilities are separated as follows:

- **Components:** Responsible for displaying information and handling user interactions.
- **Services:** Responsible for communicating with the backend API.
- **Models:** Define strongly typed request and response structures.
- **Reactive Forms:** Handle user input and client-side validation.
- **Signals:** Manage reactive UI state where appropriate.

The frontend provides functionality for:

- Viewing the order list.
- Creating new orders with multiple line items.
- Viewing individual order details.
- Updating order statuses.
- Displaying validation and API errors.

Client-side validation improves the user experience, but the backend remains the source of truth for business rules and calculated values.

## 5. Key Design Decisions

### 5.1 Server-side order calculations

Order totals are calculated by the backend rather than accepted from the frontend.

Each order line calculates its total using:

`Quantity × Unit Price`

The overall order total is calculated by summing the line totals.

This prevents clients from manipulating submitted totals and ensures that calculations remain consistent regardless of the client application.

The frontend may display an estimated total for convenience, but the persisted and returned total is calculated by the backend.

### 5.2 Duplicate order prevention

The external reference is used to identify potential duplicate submissions.

The application checks for an existing reference before creating a new order. A unique database index provides an additional safeguard against concurrent requests attempting to create the same reference.

This is important because an application-level check alone is not sufficient: two requests could pass the check at nearly the same time.

The database constraint therefore acts as the final protection against duplicate records.

A duplicate submission is treated as a conflict rather than silently creating another order.

### 5.3 Order status transitions

Order statuses are represented using an enum.

The application enforces the following transitions:

- Pending → Confirmed
- Pending → Cancelled
- Confirmed → Fulfilled
- Confirmed → Cancelled

Fulfilled and Cancelled are terminal states.

The transition rules are enforced within the domain entity rather than relying solely on the frontend to restrict available actions.

This ensures invalid transitions are rejected even when requests are submitted directly to the API.

### 5.4 Validation

Validation is performed at multiple levels.

**Frontend validation:**

- Required customer and reference fields.
- At least one order line.
- Positive whole-number quantities.
- Non-negative unit prices.
- Clear validation feedback.

**Backend and domain validation:**

- Required customer and external reference values.
- At least one order line.
- Positive quantities.
- Non-negative unit prices.
- Valid status transitions.

The backend validation is authoritative because client-side validation can be bypassed.

### 5.5 Error handling

A centralised exception-handling middleware is used to return consistent API errors.

Expected failures are translated into appropriate HTTP responses, such as:

- `400 Bad Request` for invalid input.
- `404 Not Found` when an order does not exist.
- `409 Conflict` for duplicate references or invalid state transitions.

This allows the Angular application to display meaningful error messages without needing to understand backend implementation details.

## 6. API Design

The API follows REST conventions.

| Method | Endpoint | Responsibility |
|---|---|---|
| POST | `/api/orders` | Create an order |
| GET | `/api/orders` | Retrieve all orders |
| GET | `/api/orders/{id}` | Retrieve a specific order |
| PATCH | `/api/orders/{id}/status` | Update an order's status |

The order listing returns the newest orders first, making recently created orders easier to locate.

Request and response DTOs are used to avoid exposing domain entities directly through the API.

## 7. Testing Strategy

The testing approach focuses on business rules and important application behaviour.

Key scenarios include:

- Calculating individual line totals.
- Calculating overall order totals.
- Rejecting invalid quantities.
- Rejecting negative unit prices.
- Preventing duplicate external references.
- Allowing valid status transitions.
- Rejecting invalid status transitions.
- Handling orders that do not exist.

The intention is to prioritise tests around business-critical behaviour rather than testing implementation details.

## 8. Trade-offs and Considerations

### SQLite instead of a full database server

SQLite was selected because the assessment does not require a complex database environment.

**Benefits:**
- Minimal local setup.
- Lightweight and easy to run.
- Supports relational constraints and transactions.
- Suitable for demonstrating persistence.

**Trade-off:**

For a larger production application with significant concurrent workloads, a database such as SQL Server or PostgreSQL may be more appropriate.

The repository abstraction makes it easier to change the persistence implementation without significantly affecting the application layer.

### Layered architecture instead of a single project

A layered structure introduces additional projects and interfaces compared with placing everything in one API project.

However, it provides clearer separation of responsibilities and makes business logic easier to test independently.

For this assessment, the additional structure was considered worthwhile because maintainability and testability are important requirements.

### No dedicated frontend state management library

Angular services and Signals are used instead of introducing NgRx.

This keeps the frontend implementation relatively simple while still providing reactive state management.

A more complex application with extensive shared state and workflows could justify a dedicated state management solution.

### No authentication or authorisation

Authentication and role-based access control were not included because they were outside the scope of the assessment.

In a production environment, these would need to be considered before exposing order management functionality to multiple users or external systems.

### No distributed architecture

The solution uses a single backend application rather than microservices or event-driven infrastructure.

This avoids unnecessary operational complexity for the current requirements.

A distributed architecture could be considered if the system later needed independent scaling, external integrations, or asynchronous order processing.

## 9. Potential Future Improvements

If the application were extended beyond the assessment scope, the following improvements would be worth considering:

- Add authentication and role-based authorisation.
- Introduce pagination and more advanced order filtering.
- Add structured logging and application monitoring.
- Add integration tests covering API and database interactions.
- Improve duplicate submission handling to distinguish identical retries from conflicting payloads.
- Introduce automated frontend tests.
- Add audit history for order status changes.
- Support additional database providers such as SQL Server or PostgreSQL.
- Introduce API versioning if the API becomes externally consumed.

## 10. Conclusion

The solution aims to balance clean architecture, practical implementation, and the requirements of the assessment.

The main design priorities were to keep business rules within the backend, protect data integrity through both application logic and database constraints, maintain clear separation of responsibilities, and provide a usable Angular frontend.

The implementation deliberately avoids unnecessary complexity while leaving room for future enhancements as the application grows.
