# OmniPrice Web API

A comprehensive .NET Framework 4.7.2 Web API solution for price calculation services, extracted from the PsR (ProductionStepRecorder) system with enhanced query management capabilities.

## 🚀 Features

- **Multi-tier Price Calculation** - Legacy and modern price calculation systems
- **QueryManager System** - Advanced parameterized query execution with timeout management
- **Interactive Dashboard** - Real-time monitoring and testing interface
- **Health Monitoring** - Comprehensive API and database health checks
- **Statistics Tracking** - Detailed execution metrics and performance monitoring

## 🏗️ Architecture

### Project Structure

```
WebApi.sln
├── WebApi.Misc/                    # Main Web API project
│   ├── Controllers/                # API controllers
│   ├── Default.html               # Interactive dashboard
│   └── Web.config                 # Configuration
├── Omnitech.Prezzi.Core/          # Core business logic (from PsR)
├── Omnitech.Prezzi.Infrastructure/ # Repository implementations
│   └── QueryManager.cs           # Advanced query execution
├── Omnitech.DTO/                  # Data Transfer Objects
├── Omnitech.Database/             # Data access layer
├── Omnitech.Enum/                 # Shared enumerations
└── HI.Libs.Utility/               # Utility libraries
```

## 🔧 Quick Start

### Prerequisites

- .NET Framework 4.7.2 or later
- SQL Server 2016+ (Express edition supported)
- Visual Studio 2019+ or Visual Studio Code
- IIS Express (included with Visual Studio)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd WebApiPrezzi/WebApi
   ```

2. **Restore NuGet packages**
   ```bash
   nuget restore WebApi.sln
   ```

3. **Build the solution**
   ```bash
   msbuild WebApi.sln /p:Configuration=Debug
   ```

4. **Configure connection strings**
   Update `Web.config` with your SQL Server connection string:
   ```xml
   <appSettings>
     <add key="ConnectionStringPrezzi" value="Data Source=localhost\SQLEXPRESS;Initial Catalog=YourDB;Integrated Security=True;" />
   </appSettings>
   ```

5. **Run the application**
   - Open in Visual Studio and press F5, or
   - Use IIS Express directly
   - Access dashboard at `http://localhost:54340/`

## 📊 Dashboard Interface

The interactive dashboard (`http://localhost:54340/`) provides:

### 🎛️ Main Features
- **Real-time Status Monitoring** - SQL Server and API health
- **Collapsible Sections** - Organized interface for better UX
- **Statistics Dashboard** - API calls, query metrics, response times
- **Health Endpoint Testing** - Test all API endpoints with status indicators

### 🔢 Query Manager (2-Step System)
1. **Step 1: Template Loading**
   - Enter Query ID (default: 516)
   - Load query template and identify parameters ({0}, {1}, etc.)
   - Preview raw query structure

2. **Step 2: Parameter Input & Execution**
   - Dynamic form generation for required parameters
   - Real-time execution timer
   - Configurable timeouts (180s for Query 516, 90s default)
   - Results display with horizontal scrolling

### 🔍 Custom SQL Query Execution
- Execute arbitrary SQL queries
- Real-time execution timer
- Sample query insertion
- Results formatting with pagination
- Error handling and logging

## 🔌 API Endpoints

### Price Calculation APIs

#### v1 - Legacy System
```
POST /api/v1/price/evaluate/{CustomerNo}
Content-Type: application/json

{
  "ConfigItem": {
    // Product configuration object
  }
}
```

#### v2 - Modern System (from PsR)
```
GET  /api/v2/price/calculate/{barcode}     # Calculate price by barcode
POST /api/v2/price/calculate               # Calculate with full Order object
GET  /api/v2/price/details/{barcode}       # Get order details without calculation
POST /api/v2/price/calculate/batch         # Batch calculation
```

### Health & Management APIs
```
GET /api/health/sql                        # Test SQL connection
GET /api/health/api                        # Test API health
GET /api/health/statistics                 # Get execution statistics
GET /api/health/query/{id}/template        # Get query template
POST /api/health/query                     # Execute parameterized query
```

## 🗄️ QueryManager System

Advanced query execution system extracted from PsR with enhancements:

### Key Features
- **Two-Phase Parameter Replacement**:
  - Phase 1: Indexed parameters (`{0}`, `{1}`) using `string.Format`
  - Phase 2: Dictionary-based replacements for advanced substitutions
- **Configurable Timeouts**: 90s default, 180s for Query 516
- **NOLOCK Hints**: Automatically applied to Query 516 for performance
- **Retry Logic**: 3 attempts with exponential backoff for timeout errors
- **Thread-Safe Logging**: All operations logged to `C:\WebApiLog\QueryManagerDebug.log`

### Usage Example
```csharp
var queryManager = new QueryManager();
queryManager.IdQuery = 516;
queryManager.Args.Add("your-barcode-here");
queryManager.Replace.Add("{CUSTOM_FIELD}", "replacement-value");

if (queryManager.GetQuery())
{
    var results = queryManager.DT; // DataTable with results
    var firstRow = queryManager.DR; // First row shortcut
}
```

## 📈 Monitoring & Logging

### Log Files
- **Query Manager**: `C:\WebApiLog\QueryManagerDebug.log`
- **Price API**: `C:\WebApiLog\Price\LogWebApiPrice.txt`
- **Query Output**: `C:\WebApiLog\query516_our_output_{timestamp}.sql`

### Statistics Available
- Total API requests
- Successful database queries
- Query timeout count
- Average response times
- Success rates and ratios
- Last request timestamp

## 🛠️ Development

### Building
```bash
# Debug build
msbuild WebApi.sln /p:Configuration=Debug

# Release build
msbuild WebApi.sln /p:Configuration=Release
```

### Testing
Access the dashboard at `http://localhost:54340/` and use:
- Health endpoint tests
- Query Manager with sample queries
- Custom SQL execution
- Bulk request generation for statistics

### Configuration
Key configuration settings in `Web.config`:
```xml
<appSettings>
  <add key="ConnectionStringPrezzi" value="..." />
  <add key="LogModeDebug" value="true" />
  <add key="LogFilename" value="C:\WebApiLog\Price\LogWebApiPrice.txt" />
</appSettings>
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🏢 About

Developed as part of the Omnitech price calculation system, extracted and enhanced from the PsR (ProductionStepRecorder) application for standalone API usage.

---

**Dashboard Version**: v.4 (with collapsible sections)
**API Version**: v.20
**Framework**: .NET Framework 4.7.2