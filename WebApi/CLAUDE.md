# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and Development Commands

### Building the Solution
```bash
msbuild WebApi.sln /p:Configuration=Debug
msbuild WebApi.sln /p:Configuration=Release
```

### Running the Web API
The WebApi.Misc project is configured to run on IIS Express:
- Development URL: http://localhost:54340/
- SSL URL: https://localhost:44342/

Use Visual Studio or IIS Express to run the project locally.

### NuGet Package Restore
```bash
nuget restore WebApi.sln
```

## Architecture Overview

This is a .NET Framework 4.7.2 Web API solution for price calculation services. The solution follows a multi-tier architecture:

### Project Structure
- **WebApi.Misc** - Main Web API project containing controllers and API configuration
  - Controllers handle HTTP requests
  - Uses attribute routing for API endpoints
  - Configuration split between Web.config and WebConfigurationManager.AppSettings

- **Omnitech.Prezzi** - Business logic layer for price calculations
  - Contains pricing algorithms and business rules
  - Static methods for price calculation with various contribution factors

- **Omnitech.DTO** - Data Transfer Objects
  - Contains model classes for API communication
  - BusinessCentral.Alexide namespace contains ConfigItem with extensive product configuration properties

- **Omnitech.Database** - Data access layer
  - Uses Dapper ORM for database operations
  - Contains DAL classes for different business areas:
    - `BusinessCentral.Prezzi.DAL` - Price data access (currently empty, needs implementation)
    - `NAV.NavDAL` - NAV system integration (hardcoded connection strings)
  - Currently uses hardcoded connection strings in code (needs refactoring)

- **Omnitech.Enum** - Shared enumerations

- **HI.Libs.Utility** - Utility libraries

- **Omnitech.Prezzi.Core** (NEW) - Core business logic estratto da PsR
  - Modelli: Order, Customer, Dimensions, Stamp, PriceResult
  - Enumerazioni: Material, Texture, ArticleType, PriceListType
  - Service: PriceCalculator con logica di calcolo prezzi
  - Repository interfaces per accesso dati

- **Omnitech.Prezzi.Infrastructure** (NEW) - Implementazione repository
  - PriceRepository - Accesso dati prezzi/listini
  - CustomerRepository - Accesso dati clienti
  - OrderRepository - Accesso dati ordini
  - DiscountRepository - Accesso dati sconti
  - QueryManager - Sistema per esecuzione query parametriche con timeout configurabili
  - Usa Dapper per query SQL Server

### Key API Endpoints

#### v1 - Sistema Legacy
- `POST api/v1/price/evaluate/{CustomerNo}` - Evaluates price for a customer with ConfigItem payload

#### v2 - Sistema Core (Nuovo - Estratto da PsR)
- `GET api/v2/price/calculate/{barcode}` - Calcola prezzo per barcode (recupera ordine dal DB)
- `POST api/v2/price/calculate` - Calcola prezzo con dati Order completi
- `GET api/v2/price/details/{barcode}` - Ottiene dettagli ordine senza ricalcolare
- `POST api/v2/price/calculate/batch` - Calcola prezzi in batch per più barcode

#### Health & Management Endpoints
- `GET api/health/sql` - Test SQL Server connection
- `GET api/health/api` - Test Web API health
- `GET api/health/statistics` - Get API and database statistics
- `GET api/health/query/{queryId}/template` - Get query template with parameters
- `POST api/health/query` - Execute parameterized queries

### Connection String Configuration
The application uses connection strings configured in code:
- `CONNSTR_PREZZI` - Connection string for price database (currently hardcoded as "Test_ConnectionString")
- Configuration values read from WebConfigurationManager.AppSettings

### Logging Configuration
- `LogModeDebug` - Enable/disable debug logging (AppSettings)
- `LogFilename` - Log file path (AppSettings)
- In Debug mode, logs to: `C:\WebApiLog\Price\LogWebApiPrice.txt`

## Database Configuration

### Setting Up Local Database
1. **SQL Server Requirements**:
   - SQL Server 2016 or later (Express edition is sufficient)
   - Windows Authentication enabled

2. **Connection String Configuration**:
   Currently the connection string is hardcoded in `PriceController.cs`:
   ```csharp
   CONNSTR_PREZZI = "Test_ConnectionString";  // Need to replace with actual connection string
   ```

   **Proper connection string format**:
   ```xml
   <!-- Add to Web.config appSettings -->
   <add key="ConnectionStringPrezzi" value="Data Source=localhost\SQLEXPRESS;Initial Catalog=PrezziDB;Integrated Security=True;" />
   ```

3. **Database Structure** (needs to be created):
   ```sql
   CREATE DATABASE PrezziDB;
   USE PrezziDB;
   
   -- Create tables for price calculations
   CREATE TABLE PriceRules (
       Id INT PRIMARY KEY IDENTITY,
       CustomerNo VARCHAR(50),
       BasePrice DECIMAL(10,2),
       -- Add other fields as needed
   );
   ```

4. **Implementing DAL Methods**:
   The `Omnitech.Database.BusinessCentral.Prezzi.DAL` class is empty and needs implementation:
   ```csharp
   public static decimal GetPriceForCustomer(string connectionString, string customerNo)
   {
       using (IDbConnection db = new SqlConnection(connectionString))
       {
           // Implement Dapper queries here
       }
   }
   ```

### Current Database Issues to Fix
1. Replace hardcoded connection string in `PriceController.cs`
2. Implement `Omnitech.Database.BusinessCentral.Prezzi.DAL` methods
3. Move connection strings to Web.config
4. Create database schema for price calculations

## Dashboard Interface

The project includes a comprehensive web dashboard accessible at `http://localhost:54340/` with the following features:

### Dashboard Features (v.4)
- **Collapsible Sections** - All dashboard sections can be expanded/collapsed for better organization
- **Real-time Status Monitoring** - Live SQL Server and Web API health status
- **Query Manager (2-Step System)**:
  - Step 1: Load query template and identify required parameters ({0}, {1}, etc.)
  - Step 2: Fill parameters dynamically and execute with configurable timeout
  - Special handling for Query 516 with 180-second timeout
- **Custom SQL Query Execution** - Execute arbitrary SQL queries with real-time timer
- **Statistics Tracking** - API calls, database queries, success rates, and execution times
- **Health Endpoint Testing** - Test all API endpoints with status indicators
- **Request Generation** - Generate test requests for statistics

### QueryManager System
The `QueryManager` class provides:
- **Parameter Replacement**: Two-phase system mimicking PsR's cQueryManager
  - Phase 1: Indexed parameters using `string.Format` ({0}, {1}, etc.)
  - Phase 2: Dictionary-based replacements for advanced substitutions
- **Configurable Timeouts**: Default 90s, Query 516 gets 180s automatically
- **NOLOCK Hints**: Automatically added to Query 516 to prevent blocking
- **Retry Logic**: 3 attempts with exponential backoff for timeout errors
- **Thread-safe Logging**: All operations logged to `C:\WebApiLog\QueryManagerDebug.log`

## Important Notes
- The solution uses IIS Express for local development
- Dependencies managed via NuGet packages (including Dapper for data access)
- Uses Newtonsoft.Json for JSON serialization
- Web API configured with attribute routing
- Current implementation contains hardcoded test connection string that needs to be properly configured
- Database layer uses Dapper ORM for data access
- Dashboard provides comprehensive monitoring and testing capabilities
- QueryManager system extracted from PsR maintains full compatibility