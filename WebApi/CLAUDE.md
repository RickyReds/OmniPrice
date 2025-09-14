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
  - Usa Dapper per query SQL Server

### Key API Endpoints

#### v1 - Sistema Legacy
- `POST api/v1/price/evaluate/{CustomerNo}` - Evaluates price for a customer with ConfigItem payload

#### v2 - Sistema Core (Nuovo - Estratto da PsR)
- `GET api/v2/price/calculate/{barcode}` - Calcola prezzo per barcode (recupera ordine dal DB)
- `POST api/v2/price/calculate` - Calcola prezzo con dati Order completi
- `GET api/v2/price/details/{barcode}` - Ottiene dettagli ordine senza ricalcolare
- `POST api/v2/price/calculate/batch` - Calcola prezzi in batch per più barcode

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

## Important Notes
- The solution uses IIS Express for local development
- Dependencies managed via NuGet packages (including Dapper for data access)
- Uses Newtonsoft.Json for JSON serialization
- Web API configured with attribute routing
- Current implementation contains hardcoded test connection string that needs to be properly configured
- Database layer uses Dapper ORM for data access