using System;
using System.IO;

namespace HI.Libs.Utility
{
    /// <summary>
    /// Test class to verify FileLogger behavior
    /// </summary>
    public static class TestFileLogger
    {
        public static void RunTest()
        {
            Console.WriteLine("=== Testing FileLogger ===");
            
            // Test configuration
            string testDir = @"C:\WebApiLog\Test\";
            string baseLogFile = Path.Combine(testDir, "TestLog.txt");
            
            // Clean test directory
            if (Directory.Exists(testDir))
            {
                Directory.Delete(testDir, true);
            }
            
            Console.WriteLine($"Base log file: {baseLogFile}");
            
            // Test 1: First log to empty directory
            Console.WriteLine("\nTest 1: First log to empty directory");
            var actualFile1 = FileLogger.RotateLogIfNeeded(baseLogFile, 100);
            Console.WriteLine($"  RotateLogIfNeeded returned: {Path.GetFileName(actualFile1)}");
            FileLogger.Log(actualFile1, "First log entry", LogLevel.INFO);
            Console.WriteLine($"  File exists: {File.Exists(actualFile1)}");
            
            // Test 2: Second log (should append to same file)
            Console.WriteLine("\nTest 2: Second log (should append)");
            var actualFile2 = FileLogger.RotateLogIfNeeded(baseLogFile, 100);
            Console.WriteLine($"  RotateLogIfNeeded returned: {actualFile2}");
            FileLogger.Log(actualFile2, "Second log entry", LogLevel.INFO);
            
            // Test 3: Force rotation (small size limit)
            Console.WriteLine("\nTest 3: Force rotation");
            var actualFile3 = FileLogger.RotateLogIfNeeded(baseLogFile, 10);
            Console.WriteLine($"  RotateLogIfNeeded returned: {actualFile3}");
            FileLogger.Log(actualFile3, "After rotation", LogLevel.INFO);
            
            // Test 4: Using LogDebugWithTimestamp
            Console.WriteLine("\nTest 4: Using LogDebugWithTimestamp");
            bool result = FileLogger.LogDebugWithTimestamp(baseLogFile, "Debug message", true, 50);
            Console.WriteLine($"  LogDebugWithTimestamp result: {result}");
            
            // List all files created
            Console.WriteLine("\nFiles created:");
            if (Directory.Exists(testDir))
            {
                foreach (var file in Directory.GetFiles(testDir))
                {
                    var info = new FileInfo(file);
                    Console.WriteLine($"  {Path.GetFileName(file)} - Size: {info.Length} bytes");
                }
            }
            
            Console.WriteLine("\n=== Test Complete ===");
            
            // Salva output in file per evitare problemi clipboard
            string outputFile = Path.Combine(testDir, "TestOutput.txt");
            using (var writer = new System.IO.StreamWriter(outputFile))
            {
                writer.WriteLine("Test Results:");
                foreach (var file in Directory.GetFiles(testDir, "*.txt"))
                {
                    writer.WriteLine($"File: {Path.GetFileName(file)} - Size: {new FileInfo(file).Length} bytes");
                }
            }
            Console.WriteLine($"\nOutput saved to: {outputFile}");
        }
    }
}