using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HI.Libs.Utility
{
    /// <summary>
    /// Simple file logger utility for logging messages to file
    /// </summary>
    public static class FileLogger
    {
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Generates a timestamped log file name
        /// </summary>
        /// <param name="baseLogFilePath">Base log file path (e.g., C:\Logs\MyApp.txt)</param>
        /// <param name="includeTime">If true, includes time in the timestamp, otherwise only date</param>
        /// <returns>Log file path with timestamp (e.g., C:\Logs\MyApp_20240912_143022.txt)</returns>
        public static string GetTimestampedLogFileName(string baseLogFilePath, bool includeTime = true)
        {
            if (string.IsNullOrEmpty(baseLogFilePath))
                return baseLogFilePath;

            try
            {
                var directory = Path.GetDirectoryName(baseLogFilePath);
                var fileName = Path.GetFileNameWithoutExtension(baseLogFilePath);
                var extension = Path.GetExtension(baseLogFilePath);

                // Remove existing timestamp if present (pattern: _YYYYMMDD or _YYYYMMDD_HHMMSS)
                var timestampPattern = @"_\d{8}(_\d{6})?$";
                fileName = System.Text.RegularExpressions.Regex.Replace(fileName, timestampPattern, "");

                // Add new timestamp
                var timestamp = includeTime 
                    ? DateTime.Now.ToString("yyyyMMdd_HHmmss")
                    : DateTime.Now.ToString("yyyyMMdd");

                return Path.Combine(directory, $"{fileName}_{timestamp}{extension}");
            }
            catch
            {
                // Return original path if something goes wrong
                return baseLogFilePath;
            }
        }

        /// <summary>
        /// Gets today's log file name (one file per day)
        /// </summary>
        /// <param name="baseLogFilePath">Base log file path</param>
        /// <returns>Log file path with today's date</returns>
        public static string GetDailyLogFileName(string baseLogFilePath)
        {
            return GetTimestampedLogFileName(baseLogFilePath, false);
        }

        /// <summary>
        /// Logs a message to file with timestamp
        /// </summary>
        /// <param name="logFilePath">Full path to the log file</param>
        /// <param name="message">Message to log</param>
        /// <param name="logLevel">Log level (INFO, WARNING, ERROR, DEBUG)</param>
        /// <returns>True if logged successfully, false otherwise</returns>
        public static bool Log(string logFilePath, string message, LogLevel logLevel = LogLevel.INFO)
        {
            if (string.IsNullOrEmpty(logFilePath) || string.IsNullOrEmpty(message))
                return false;

            try
            {
                // Ensure directory exists
                var directory = Path.GetDirectoryName(logFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Format log message
                var logMessage = FormatLogMessage(message, logLevel);

                // Thread-safe write to file
                lock (_lockObject)
                {
                    File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
                }

                return true;
            }
            catch
            {
                // Silently fail to avoid disrupting application flow
                return false;
            }
        }

        /// <summary>
        /// Logs a message asynchronously
        /// </summary>
        public static async Task<bool> LogAsync(string logFilePath, string message, LogLevel logLevel = LogLevel.INFO)
        {
            return await Task.Run(() => Log(logFilePath, message, logLevel));
        }

        /// <summary>
        /// Logs an exception with stack trace
        /// </summary>
        public static bool LogException(string logFilePath, Exception ex, string additionalMessage = null)
        {
            if (ex == null) return false;

            var message = $"EXCEPTION: {ex.Message}";
            if (!string.IsNullOrEmpty(additionalMessage))
                message = $"{additionalMessage} - {message}";
            
            message += $"\nStackTrace: {ex.StackTrace}";
            
            if (ex.InnerException != null)
                message += $"\nInner Exception: {ex.InnerException.Message}";

            return Log(logFilePath, message, LogLevel.ERROR);
        }

        /// <summary>
        /// Logs debug information (only if debug mode is enabled)
        /// </summary>
        public static bool LogDebug(string logFilePath, string message, bool debugMode)
        {
            if (!debugMode) return false;
            return Log(logFilePath, message, LogLevel.DEBUG);
        }

        /// <summary>
        /// Logs API request information
        /// </summary>
        public static bool LogApiRequest(string logFilePath, string method, string endpoint, string customerNo, object requestData = null)
        {
            var message = $"API Request - Method: {method}, Endpoint: {endpoint}, Customer: {customerNo}";
            
            if (requestData != null)
            {
                try
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
                    message += $", Data: {json}";
                }
                catch
                {
                    message += $", Data: {requestData}";
                }
            }

            return Log(logFilePath, message, LogLevel.INFO);
        }

        /// <summary>
        /// Logs API response information
        /// </summary>
        public static bool LogApiResponse(string logFilePath, int statusCode, object responseData = null, long? elapsedMs = null)
        {
            var message = $"API Response - Status: {statusCode}";
            
            if (elapsedMs.HasValue)
                message += $", Elapsed: {elapsedMs}ms";
            
            if (responseData != null)
            {
                try
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(responseData);
                    message += $", Data: {json}";
                }
                catch
                {
                    message += $", Data: {responseData}";
                }
            }

            return Log(logFilePath, message, LogLevel.INFO);
        }

        /// <summary>
        /// Cleans old log files
        /// </summary>
        public static void CleanOldLogs(string logDirectory, int daysToKeep = 30)
        {
            try
            {
                if (!Directory.Exists(logDirectory)) return;

                var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
                var files = Directory.GetFiles(logDirectory, "*.txt");

                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.LastWriteTime < cutoffDate)
                    {
                        File.Delete(file);
                    }
                }
            }
            catch
            {
                // Silently fail
            }
        }

        /// <summary>
        /// Gets the size of the log file
        /// </summary>
        public static long GetLogFileSize(string logFilePath)
        {
            try
            {
                if (File.Exists(logFilePath))
                {
                    var fileInfo = new FileInfo(logFilePath);
                    return fileInfo.Length;
                }
            }
            catch
            {
                // Silently fail
            }
            return 0;
        }

        /// <summary>
        /// Gets the most recent log file or creates a new one if needed
        /// </summary>
        private static string GetCurrentLogFile(string baseLogFilePath, bool debugOutput = false)
        {
            try
            {
                var directory = Path.GetDirectoryName(baseLogFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                var baseFileName = Path.GetFileNameWithoutExtension(baseLogFilePath);
                var extension = Path.GetExtension(baseLogFilePath);
                
                // First check if base file exists without timestamp (legacy/error case)
                if (File.Exists(baseLogFilePath))
                {
                    if (debugOutput) System.Diagnostics.Debug.WriteLine($"Found base file without timestamp: {baseLogFilePath}");
                    
                    // Rename it to include timestamp
                    var timestampedName = GetDailyLogFileName(baseLogFilePath);
                    if (!File.Exists(timestampedName))
                    {
                        File.Move(baseLogFilePath, timestampedName);
                        if (debugOutput) System.Diagnostics.Debug.WriteLine($"Renamed to: {timestampedName}");
                    }
                    else
                    {
                        // If timestamped version exists, archive the non-timestamped one
                        var archivedPath = GetTimestampedLogFileName(baseLogFilePath, true);
                        File.Move(baseLogFilePath, archivedPath);
                        if (debugOutput) System.Diagnostics.Debug.WriteLine($"Archived to: {archivedPath}");
                    }
                }
                
                // Pattern to match log files with date (and optionally time)
                var pattern = $"{baseFileName}_*{extension}";
                
                // Find all matching log files, ordered by name (which includes timestamp)
                var files = Directory.GetFiles(directory, pattern)
                    .Where(f => System.Text.RegularExpressions.Regex.IsMatch(
                        Path.GetFileName(f), 
                        $@"{baseFileName}_\d{{8}}(_\d{{6}}(_\d{{3}})?)?\{extension}$"))
                    .OrderByDescending(f => f) // File names with timestamps sort correctly as strings
                    .ToArray();
                
                if (debugOutput) System.Diagnostics.Debug.WriteLine($"Found {files.Length} existing timestamped files");
                
                // Return the most recent file if it exists
                if (files.Length > 0)
                {
                    if (debugOutput) System.Diagnostics.Debug.WriteLine($"Returning existing file: {files[0]}");
                    return files[0];
                }
                
                // No existing file, create new daily log file name
                var newFileName = GetDailyLogFileName(baseLogFilePath);
                if (debugOutput) System.Diagnostics.Debug.WriteLine($"Creating new file name: {newFileName}");
                return newFileName;
            }
            catch (Exception ex)
            {
                if (debugOutput) System.Diagnostics.Debug.WriteLine($"Error in GetCurrentLogFile: {ex.Message}");
                // Fallback to daily log file name
                return GetDailyLogFileName(baseLogFilePath);
            }
        }

        /// <summary>
        /// Rotates log file if it exceeds maximum size by creating a new timestamped file
        /// </summary>
        /// <param name="baseLogFilePath">Base log file path (without timestamp)</param>
        /// <param name="maxSizeInBytes">Maximum size before rotation (default 40 bytes for testing)</param>
        /// <returns>The actual log file path to use (new if rotated, existing if not)</returns>
        public static string RotateLogIfNeeded(string baseLogFilePath, long maxSizeInBytes = 40)
        {
            try
            {
                // Get the current log file (most recent or new if none exists)
                var currentLogFile = GetCurrentLogFile(baseLogFilePath);
                
                // Check if current file needs rotation
                if (File.Exists(currentLogFile) && GetLogFileSize(currentLogFile) > maxSizeInBytes)
                {
                    // When rotating, rename current file to include full timestamp
                    // and create a new file with timestamp (hour/minute/second)
                    var archivedPath = currentLogFile;
                    
                    // If the current file only has date, add time to it when archiving
                    if (!System.Text.RegularExpressions.Regex.IsMatch(Path.GetFileName(currentLogFile), @"_\d{8}_\d{6}"))
                    {
                        // Current file has only date, archive it with the current time added
                        var dir = Path.GetDirectoryName(currentLogFile);
                        var fileName = Path.GetFileNameWithoutExtension(currentLogFile);
                        var ext = Path.GetExtension(currentLogFile);
                        var timeStamp = DateTime.Now.ToString("HHmmss");
                        archivedPath = Path.Combine(dir, $"{fileName}_{timeStamp}{ext}");
                    }
                    
                    // Ensure archived filename is unique
                    int counter = 1;
                    var originalArchived = archivedPath;
                    while (File.Exists(archivedPath))
                    {
                        var dir = Path.GetDirectoryName(originalArchived);
                        var name = Path.GetFileNameWithoutExtension(originalArchived);
                        var ext = Path.GetExtension(originalArchived);
                        archivedPath = Path.Combine(dir, $"{name}_{counter:D3}{ext}");
                        counter++;
                    }
                    
                    if (currentLogFile != archivedPath)
                    {
                        File.Move(currentLogFile, archivedPath);
                    }
                    
                    // Create new file with full timestamp (date + time)
                    return GetTimestampedLogFileName(baseLogFilePath, true);
                }
                
                // Current file is fine, return it
                return currentLogFile;
            }
            catch
            {
                // Return current or new log file even if rotation fails
                return GetCurrentLogFile(baseLogFilePath);
            }
        }

        /// <summary>
        /// Logs with automatic timestamped file name and rotation
        /// </summary>
        public static bool LogWithTimestamp(string baseLogFilePath, string message, LogLevel logLevel = LogLevel.INFO, long maxSizeInBytes = 10485760)
        {
            // Gestisce automaticamente rotazione e ottiene il file corretto
            var actualLogFile = RotateLogIfNeeded(baseLogFilePath, maxSizeInBytes);
            return Log(actualLogFile, message, logLevel);
        }

        /// <summary>
        /// Logs debug information with automatic timestamped file name and rotation
        /// </summary>
        public static bool LogDebugWithTimestamp(string baseLogFilePath, string message, bool debugMode, long maxSizeInBytes = 10485760)
        {
            if (!debugMode) return false;
            // Gestisce automaticamente rotazione e ottiene il file corretto
            var actualLogFile = RotateLogIfNeeded(baseLogFilePath, maxSizeInBytes);
            return Log(actualLogFile, message, LogLevel.DEBUG);
        }

        private static string FormatLogMessage(string message, LogLevel logLevel)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            return $"[{timestamp}] [{logLevel}] {message}";
        }
    }

    /// <summary>
    /// Log levels
    /// </summary>
    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARNING,
        ERROR
    }
}