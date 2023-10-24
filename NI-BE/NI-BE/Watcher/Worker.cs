using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
                try
                {

                //Register Event Handler
                watcher.Changed += new FileSystemEventHandler(OnChanged);
                watcher.Created += new FileSystemEventHandler(OnChanged);
                watcher.Deleted += new FileSystemEventHandler(OnChanged);
                watcher.Renamed += new RenamedEventHandler(OnRenamed);
                }
                catch(IOException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                //Start Monitoring
                watcher.EnableRaisingEvents = true;



            }
        }
        public void OnChanged(object source, FileSystemEventArgs e)
        {
            string fileName = Path.GetFileName(e.FullPath);
           
            if(e.ChangeType == WatcherChangeTypes.Created)
            {
        


                string targetPath = "";
                Console.WriteLine("path: "+ Path.GetExtension(e.FullPath.ToString()));
                if (Path.GetExtension(e.FullPath.ToString())== ".txt")
                {
//                    string csvFile = Path.ChangeExtension(e.FullPath,"csv");
                    string csvFile = Path.Combine(@"C:\Users\User\Desktop\G\Baby NI Project\Code\NI-BE\NI-BE\NI-BE\Data\ParsedData", Path.ChangeExtension(Path.GetFileName(e.FullPath), "csv"));



                    try
                    {
                        //Reading from the file
                        List<string> lines = File.ReadAllLines(e.FullPath).ToList();


                        using (StreamWriter csvWriter = new StreamWriter(csvFile))
                        {

                            for (int i = 0; i < lines.Count; i++)
                            {

                                //Add two new fields to the list
                                try
                                {


                                    if (i == 0)
                                    {
                                        lines[i] = "Network_SID," + "DateTime_Key," + lines[i];
                                    }
                                    else
                                    {

                                        string[] splitEntires = lines[i].Split(',');
                                        string concatValues = splitEntires[6] + splitEntires[7];

                                        // hashing as int the values and storing it under the field Network_SID
                                        string hashed = concatValues.GetHashCode().ToString();

                                        // extracting the date from the file name and storing it under the field DateTime_key
                                        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(e.FullPath);
                                        string[] fileDate = fileNameWithoutExt.Split('_');
                                        string dateExtract = fileDate[fileDate.Length - 2] + fileDate[fileDate.Length - 1];

                                        DateTime date = DateTime.ParseExact(dateExtract, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);

                                        lines[i] = hashed + "," + date.ToString("yyyy/MM/dd HH:mm:ss") + "," + lines[i];
                                    }
                                }
                                catch (Exception exep)
                                {

                                    Console.WriteLine("Error adding fields: " + exep.Message);
                                }
                                finally
                                {


                                    string[] lineEntries = lines[i].Split(',');

                                    if (fileName.Contains("RADIO_LINK_POWER"))
                                    {

                                        //// discard the record if failed field has value other than "-"
                                        //string faildDescField = lineEntries[lineEntries.Length - 1];

                                        //if (faildDescField != "-")
                                        //    //lineEntries[lineEntries.Length - 1] = "-";
                                        //    lines.RemoveAt(i);

                                        // Discarding failed records and 
                                        // Discard the record if failed field has value other than "-"
                                        for (int j = lines.Count - 1; j >= 0; j--)
                                        {
                                            string faildDescField = lines[j].Split(',').Last();

                                            if (lines[j].Trim().IndexOf("Unreachable Bulk FC", StringComparison.OrdinalIgnoreCase) >= 0|| faildDescField != "-")
                                                lines.RemoveAt(j);
                                        }


                                        // Remove disabled fields
                                        //  int[] removedColumns = { 0, 8, 16 };
                                        int[] removedColumns = { 2, 10, 18 };

                                        lineEntries = lineEntries
                                            .Select((x, Index) => removedColumns.Contains(Index) ? "" : x)
                                            .Where(x => !string.IsNullOrWhiteSpace(x))
                                            .ToArray();


                                        // Reconstruct the list
                                        lines[i] = string.Join(",", lineEntries);






                                    }

                                    else if (fileName.Contains("TN_RFInputPower"))
                                    {



                                        int[] removedColumns = { 8, 10, 11, 14 };
                                        // Remove disabled fields
                                        lineEntries = lineEntries
                                            .Select((x, Index) => removedColumns.Contains(Index) ? "" : x)
                                            .Where(x => !string.IsNullOrWhiteSpace(x))
                                            .ToArray();

                                        // Reconstruct the list
                                        lines[i] = string.Join(",", lineEntries);

                                    }

                                }
                                csvWriter.WriteLine(lines[i]);
                            }


                        }

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

                    if(File.Exists(targetPath))
                    {
                     File.Delete(e.FullPath.ToString());
                    }
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

        //public static int Combine<T1,T2>(string value1, string value2)
        //{
        //    string concatValues = value1 + value2;
        //    return concatValues.GetHashCode();
        //}

    }
}
