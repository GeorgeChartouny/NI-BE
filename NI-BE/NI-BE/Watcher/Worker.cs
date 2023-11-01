using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.FileSystemGlobbing;
using NI_BE.DataDb;
using NI_BE.Parser;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace babyNI_BE.Watcher
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();

            // The path to monitor the files
            watcher.Path = @"C:\Users\User\Desktop\G\Baby NI Project\Code\NI-BE\NI-BE\NI-BE\Data\NewData";

            // Disable monitoring sub directories
            watcher.IncludeSubdirectories = false;

            //Notify Filters
            watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.Size;

            //Include all files in the folder
            watcher.Filter = "*";


            try
            {
                var onChanged = new ParsingFile();
                //Register Event Handler
                watcher.Changed += new FileSystemEventHandler(onChanged.OnChanged);
                watcher.Created += new FileSystemEventHandler(onChanged.OnChanged);
                watcher.Deleted += new FileSystemEventHandler(onChanged.OnChanged);
                watcher.Renamed += new RenamedEventHandler(OnRenamed);

            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);

            }

            //Start Monitoring
            watcher.EnableRaisingEvents = true;

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at time: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }

        public void OnRenamed(object source, RenamedEventArgs e)
        {
            Console.WriteLine(e.OldName + " is changed to " + e.Name);
        }

    }
}
