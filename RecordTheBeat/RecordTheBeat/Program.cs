using System;
using RecordTheBeat.Data;
using Serilog;

namespace RecordTheBeat
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/logfile.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Debug("Starting up");
            Log.Debug("Shutting down");
            
            ConfigLoader cfgloader = new ConfigLoader();
            cfgloader.Load();
        }
    }
}