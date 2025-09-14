# Changelog

All notable changes to the OmniPrice Web API project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [4.0.0] - 2025-01-14

### Added
- **Collapsible Dashboard Sections** - All dashboard sections can now be expanded/collapsed for better organization
  - Smooth CSS animations with opacity and height transitions
  - Toggle icons (▼/▶) with visual feedback
  - Persistent section states during session
  - Dashboard version updated to v.4

### Changed
- **Horizontal Scroll Enhancement** - Improved table scrolling behavior
  - Horizontal scrollbars now apply only to result tables, not query text areas
  - Query text areas use word wrapping for better readability
  - Maintained table minimum width for proper column display

### Fixed
- **Query Text Display** - Fixed horizontal scroll issues in query preview areas
  - Applied `white-space: pre-wrap` and `word-wrap: break-word` to query text elements
  - Prevented horizontal scroll bars on text areas while maintaining table scroll functionality

## [3.0.0] - 2025-01-14

### Added
- **Real-Time Execution Timers** - Live execution time tracking for all query operations
  - Visual timer updates every 100ms during query execution
  - Server-side execution time display after completion
  - Consistent timer format across Query Manager and Custom SQL sections

- **Enhanced Query Manager System** - Two-step query execution process
  - **Step 1**: Load query template and identify required parameters ({0}, {1}, etc.)
  - **Step 2**: Dynamic parameter form generation and query execution
  - Real-time parameter detection and form creation
  - Query preview functionality with syntax highlighting

### Changed
- **QueryManager Parameter Handling** - Improved parameter preservation logic
  - Args collection no longer cleared when barcode is null
  - Maintains parameter state between template loading and execution
  - Better error handling for parameter mismatches

### Fixed
- **Query Parameter Index Errors** - Resolved "L'indice deve essere maggiore o uguale a zero" errors
  - Fixed Args collection clearing when using custom parameters
  - Proper parameter state management in two-step execution process

## [2.0.0] - 2025-01-13

### Added
- **QueryManager Infrastructure** - Complete query management system extracted from PsR
  - Two-phase parameter replacement system (indexed + dictionary-based)
  - Configurable timeouts with special handling for Query 516 (180 seconds)
  - NOLOCK hints automatically applied to Query 516 for performance optimization
  - Retry logic with exponential backoff for timeout scenarios
  - Thread-safe logging to `C:\WebApiLog\QueryManagerDebug.log`

- **Health Management API Endpoints**
  - `GET /api/health/sql` - SQL Server connection testing
  - `GET /api/health/api` - Web API health verification
  - `GET /api/health/statistics` - Execution statistics and metrics
  - `GET /api/health/query/{id}/template` - Query template retrieval
  - `POST /api/health/query` - Parameterized query execution

- **Interactive Dashboard Interface** - Comprehensive monitoring and testing interface
  - Real-time status monitoring for SQL Server and Web API
  - Statistics tracking with API calls, query metrics, and response times
  - Health endpoint testing with status indicators
  - Request generation tools for statistics population

### Changed
- **Project Architecture** - Enhanced multi-tier structure
  - Added `Omnitech.Prezzi.Infrastructure` project with QueryManager
  - Improved separation of concerns between layers
  - Enhanced logging and error handling throughout

## [1.0.0] - Initial Release

### Added
- **Core Price Calculation API** - Multi-version price calculation system
  - v1 Legacy system with ConfigItem-based evaluation
  - v2 Modern system extracted from PsR with Order-based calculations
  - Support for barcode-based price calculations
  - Batch processing capabilities

- **Multi-Tier Architecture** - Comprehensive project structure
  - `WebApi.Misc` - Main Web API project with controllers
  - `Omnitech.Prezzi.Core` - Core business logic
  - `Omnitech.DTO` - Data Transfer Objects
  - `Omnitech.Database` - Data access layer with Dapper
  - `Omnitech.Enum` - Shared enumerations

- **Basic Dashboard** - Initial monitoring interface
  - SQL Server and API status monitoring
  - Basic statistics display
  - Configuration management

### Technical Details

#### Database Enhancements
- **Connection String Management** - Improved configuration handling
- **Query Optimization** - NOLOCK hints for performance-critical queries
- **Thread-Safe Operations** - Concurrent access protection for logging and query execution

#### API Improvements
- **Error Handling** - Comprehensive error messages and logging
- **Performance Monitoring** - Execution time tracking and statistics
- **Health Checks** - Automated system health verification

#### Dashboard Features
- **Responsive Design** - Mobile-friendly interface with smooth animations
- **Real-Time Updates** - Live status indicators and execution timers
- **Interactive Testing** - Built-in tools for API and query testing
- **Statistics Visualization** - Comprehensive metrics display

---

### Development Notes

**Framework**: .NET Framework 4.7.2
**Database**: SQL Server 2016+ with Dapper ORM
**Frontend**: HTML5 + Vanilla JavaScript with CSS Grid/Flexbox
**Deployment**: IIS Express for development, configurable for production

### Migration Notes

When upgrading from previous versions:
1. Update connection strings in `Web.config`
2. Ensure `C:\WebApiLog\` directory exists for logging
3. Test QueryManager functionality with existing queries
4. Verify dashboard accessibility at `http://localhost:54340/`