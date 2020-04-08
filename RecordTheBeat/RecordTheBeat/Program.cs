using System;
using System.IO;
using CircleHelper.Data;
using CircleHelper.Parsing;
using RecordTheBeat.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace RecordTheBeat
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create logging level switch to allow for logger level to be changed to what is inside config, after logger initialization
            LoggingLevelSwitch lls = new LoggingLevelSwitch();
            
            //Initialize logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(lls)
                .WriteTo.Console()
                .WriteTo.File("logs/logfile.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Debug("Starting up");
            Log.Debug("Shutting down");
            
            //Load config file
            ConfigLoader cfgLoader = new ConfigLoader();
            Config cfg = cfgLoader.Load();

            //Change logger depth to that stated in config
            lls.MinimumLevel = cfg.LogDepth;
            
            Database dm = DatabaseParser.Parse(cfg.DatabasePath);

            string file = "";
            
            while(!File.Exists(file)){
                Log.Information("Drag and drop the replay file, or input the file path: ");
                file = Console.ReadLine();
                
                if(!File.Exists(file)) Log.Error("File not found.");
            }
            
            Replay r = ReplayParser.Parse(file);
            
        }
    }
}