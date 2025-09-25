# Fanex NAP SPU DCS - AI Development Guidelines

## Architecture Overview

This is a .NET 8.0 microservices solution for Data Collection System (DCS) processing with two main projects:

- **`Fanex.NAP.SPU.DCS`** - Core domain library with feature-based organization
- **`Fanex.NAP.SPU.DCS.JobService`** - ASP.NET Core Web API for job processing and orchestration

## Key Architectural Patterns

### Feature-Based Organization
Code is organized by business domains in `src/Fanex.NAP.SPU.DCS/Features/`:
- `Transactions/` - Core transaction processing
- `UserAgent/` - User agent parsing and device detection
- `DataTrace/` - Data tracing and monitoring
- `ELK/` - Elasticsearch/Logstash integration
- `MBDevice/`, `MBTransactions/` - Mobile banking specifics
- `Monitoring/`, `SystemSetting/` - Infrastructure concerns

Each feature typically contains:
- `Entities/` - Domain models
- `Criteria/` - Query/filter objects
- `[Feature]Services.cs` - Business logic interface/implementation
- `[Feature]DataAccess.cs` - Data access layer

### Service Composition Pattern
The `DCSTransactionJobService` class demonstrates the composition pattern - it aggregates multiple related services:
```csharp
public class DCSTransactionJobService
{
    public ITransactionJobService TransactionJobService { get; }
    public ISumTransactionJobService SumTransactionJobService { get; }
    // ... other services
}
```

### Fluent Interface Pattern
Services use method chaining for operations:
```csharp
return this.transactionDataAccess.InsertTransaction(fromId, toId, isReadyDbExtra);
return this;
```

## Environment Configuration

### Multi-Environment Setup
The project supports multiple environments through appsettings files:
- `appsettings.json` (LOCAL)
- `appsettings.Development.json`
- `appsettings.UAT.json`
- `appsettings.PROD.json`
- `appsettings.AWS.json`
- `appsettings.KLPROD.json`

### Connection String Pattern
Connection strings follow a specific naming convention:
```json
"DCSMain_WEB_LAYER.DCS_DataCenter.ctsService"
```
Format: `[Database]_WEB_LAYER.[Schema].[Username]`

### Database Support
- **MySQL** - Primary database (DCS_DataCenter, DCS_DataTrace)
- **SQL Server** - Secondary databases (bodb_VR2Model, bodb_DWRS, bodb_Account)
- Uses Fanex.Data abstraction layer with provider-specific implementations

## Development Patterns

### Dependency Injection Registration
Services are registered through `InitService.RegisterService()` extension method, organized by feature area.

### Job Processing Controllers
Controllers in `JobService` follow the pattern:
- `[HttpPost]` endpoints for job execution with size/batch parameters
- `[HttpGet]` endpoints for utility/conversion operations
- Return `Task<bool>` for async job operations

### Logging and Monitoring
- **Sentry** integration for error tracking
- **Telegram** notifications for monitoring alerts
- **ELK Stack** integration via Logstash TCP
- Custom logging through Fanex.Logging extensions

### Testing Structure
Tests mirror the src structure:
- `test/Fanex.NAP.SPU.DCS.Test/Features/` - Unit tests by feature
- `test/Fanex.NAP.SPU.DCS.Test/JobService/` - Integration tests for job services

## Development Workflow

### Building
```bash
dotnet restore
dotnet build
```

### Running Locally
The JobService runs on port 5000 (HTTP) with SSL termination handled by load balancer.

### Docker Deployment
Uses multi-stage Dockerfile with:
- Alpine-based .NET 8.0 runtime
- Non-root user (appuser:appgroup)
- Custom NuGet source: `https://nuget.nexdev.net/api/v2`

### Stored Procedures
XML-based stored procedure definitions in `App_Data/[Environment]/SPNames.xml` files.

## Code Quality Standards

- **SonarLint** integration with custom ruleset
- **ExcludeFromCodeCoverage** attributes on startup/configuration classes
- **NOSONAR** comments for justified warnings (e.g., HTTP URLs in containerized environments)

## External Dependencies

- **UAParser** - User agent parsing
- **RestSharp** - HTTP client operations  
- **EPPlus** - Excel file processing
- **Fanex.*** - Internal framework libraries for data access, logging, notifications

When adding new features, follow the established patterns: create feature folders with Services/DataAccess/Entities, register in InitService, and expose through appropriate controllers.