using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
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
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at time: {time}", DateTimeOffset.Now);
                await Task.Delay(10000,stoppingToken);

                FileSystemWatcher watcher = new FileSystemWatcher();

                // The path to monitor the files
                watcher.Path = @"C:\Users\User\Desktop\G\Baby NI Project\Code\NI-BE\NI-BE\NI-BE\Data\NewData";

                // Disable monitoring sub directories
                watcher.IncludeSubdirectories = false;

                //Notify Filters
                watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.Size;

                //Include all files in the folder
                watcher.Filter = "*";

                //Register Event Handler
                watcher.Changed += new FileSystemEventHandler(OnChanged);
                watcher.Created += new FileSystemEventHandler(OnChanged);
                watcher.Deleted += new FileSystemEventHandler(OnChanged);
                watcher.Renamed += new RenamedEventHandler(OnRenamed);

                //Start Monitoring
                watcher.EnableRaisingEvents = true;



            }
        }
        public void OnChanged(object source, FileSystemEventArgs e)
        {
            if(e.ChangeType == WatcherChangeTypes.Created)
            {
        


                string targetPath = "";
                Console.WriteLine("path: "+ Path.GetExtension(e.FullPath.ToString()));
                if (Path.GetExtension(e.FullPath.ToString())== ".txt")
                {
                    string csvFile = Path.ChangeExtension(e.FullPath,"csv");
                 

                    try
                    {
                        //Reading from the file
                        List<string> lines = File.ReadAllLines(e.FullPath).ToList();

                        List<string>headers = null;
                        List<string> entries = null;

                        using (StreamWriter csvWriter = new StreamWriter(csvFile))
                        {

                            foreach (string line in lines)
                             {
                            string[] lineEntries = line.Split(',');

                                csvWriter.WriteLine(line);
                            Console.WriteLine(line);
                            // specify the fields headers
                            if (headers == null)
                            {
                                headers = lineEntries.ToList(); 
                            } else
                            {
                                 entries  = lineEntries.ToList();
                            }
                            }


                        }

                        Console.WriteLine("headers: ", headers);
                        Console.WriteLine("entries: ", entries);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Error: "+ ex.Message);
                    }finally
                    {
                        targetPath = @"C:\Users\User\Desktop\G\Baby NI Project\Code\NI-BE\NI-BE\NI-BE\Data\OldData";

                    }


                }

                // removing the file form the folder and adding it to the OldData folder
                try { 
                targetPath = Path.Combine(targetPath, Path.GetFileName(e.FullPath.ToString()));
                File.Copy(e.FullPath.ToString(), targetPath, true);

                    // if(File.Exists(targetPath))
                    //{
                    // File.Delete(e.FullPath.ToString());
                    //}
                Console.WriteLine(e.Name + " File moved successfully to organizer folder");
                    }
                catch (Exception ex)
                { 
                    Console.WriteLine(ex.Message); 
                }
            }

            Console.WriteLine(e.Name + " " + e.ChangeType);
        }

        public void OnRenamed(object source, RenamedEventArgs e)
        {
            Console.WriteLine(e.OldName + " is changed to " + e.Name);
        }


    }
}
