using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace GitCommit.Shared.Utilities
{
    public static class Logger
    {
        private static readonly object _lock = new object();
        
        public static void LogTransmit(string logFilePath, object data)
        {
            Log(logFilePath, "TRANSMIT", data);
        }
        
        public static void LogReceive(string logFilePath, object data)
        {
            Log(logFilePath, "RECEIVE", data);
        }
        
        public static void LogMatch(string logFilePath, object matchData)
        {
            Log(logFilePath, "MATCH", matchData);
        }
        
        private static void Log(string logFilePath, string action, object data)
        {
            try
            {
                var logEntry = new
                {
                    Timestamp = DateTime.Now,
                    Action = action,
                    Data = data
                };
                
                string json = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                
                lock (_lock)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
                    File.AppendAllText(logFilePath, json + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging to {logFilePath}: {ex.Message}");
            }
        }
        
        public static async Task<string> ExportLogsAsCsv(string logFilePath)
        {
            try
            {
                string csvFilePath = Path.ChangeExtension(logFilePath, ".csv");
                
                if (!File.Exists(logFilePath))
                {
                    return $"Log file {logFilePath} does not exist.";
                }
                
                string[] lines = await File.ReadAllLinesAsync(logFilePath);
                
                using (StreamWriter writer = new StreamWriter(csvFilePath))
                {
                    await writer.WriteLineAsync("Timestamp,Action,Data");
                    
                    foreach (string line in lines)
                    {
                        try
                        {
                            var logEntry = JsonSerializer.Deserialize<dynamic>(line);
                            string timestamp = logEntry.GetProperty("Timestamp").GetString();
                            string action = logEntry.GetProperty("Action").GetString();
                            string data = logEntry.GetProperty("Data").ToString();
                            
                            await writer.WriteLineAsync($"{timestamp},{action},{data}");
                        }
                        catch
                        {
                            // Skip invalid JSON lines
                        }
                    }
                }
                
                return $"Logs exported to {csvFilePath}";
            }
            catch (Exception ex)
            {
                return $"Error exporting logs: {ex.Message}";
            }
        }
    }
}
