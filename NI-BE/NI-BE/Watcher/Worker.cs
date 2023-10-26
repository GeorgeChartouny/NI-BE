using System;
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
                await Task.Delay(10000, stoppingToken);

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
                catch (IOException ex)
                {
                    Console.WriteLine(ex.Message);
                    watcher.Dispose();
                    Console.WriteLine("Disposed");
                }

                //Start Monitoring
                watcher.EnableRaisingEvents = true;



            }
        }
        public void OnChanged(object source, FileSystemEventArgs e)
        {
            string fileName = Path.GetFileName(e.FullPath);

            if (e.ChangeType == WatcherChangeTypes.Created)
            {



                string targetPath = "";
                Console.WriteLine("path: " + Path.GetExtension(e.FullPath.ToString()));
                if (Path.GetExtension(e.FullPath.ToString()) == ".txt")
                {
                    //                    string csvFile = Path.ChangeExtension(e.FullPath,"csv");
                    string csvFile = Path.Combine(@"C:\Users\User\Desktop\G\Baby NI Project\Code\NI-BE\NI-BE\NI-BE\Data\ParsedData", Path.ChangeExtension(Path.GetFileName(e.FullPath), "csv"));


                    try
                    {
                        //Reading from the file
                        List<string> lines = File.ReadAllLines(e.FullPath).ToList();


                        using (StreamWriter csvWriter = new StreamWriter(csvFile))
                        {

                            // Discarding failed records and 
                            // Discard the record if failed field has value other than "-"
                            Console.WriteLine("before:" + lines.Count);

                            for (int j = lines.Count - 1; j >= 1; j--)
                            {
                                string[] faildDescFieldarray = lines[j].Split(',');


                                string failedDescField = lines[j].Split(',').Last();
                 
                                if (lines[j].Trim().IndexOf("Unreachable Bulk FC", StringComparison.OrdinalIgnoreCase) >= 0 || failedDescField != "-")
                                    lines.RemoveAt(j);
                            }

                            for (int i = 0; i < lines.Count; i++)
                            {

                                //Add two new fields to the list
                                try
                                {


                                    if (i == 0 && fileName.Contains("RADIO_LINK_POWER"))
                                    {
                                        Console.WriteLine("line[0]: " + lines[0]);
                                        lines[i] = "Network_SID," + "DateTime_Key," + lines[i] + ",Link,TID,FARENDTID,SLOT,PORT"; 
                                    }
                                    else if (i == 0 && fileName.Contains("TN_RFInputPower"))
                                    {
                                        lines[i] = "Network_SID," + "DateTime_Key," + lines[i] + "SLOT," + "PORT";
                                    }
                                    else
                                    {

                                        string[] splitEntires = lines[i].Split(',');




                                        //string hashed = concatValues.GetHashCode().ToString();

                                        // hashing as int the values and storing it under the field Network_SID
                                        string concatValues = splitEntires[6] + splitEntires[7];
                                        int hashed = 0;
                                        using (SHA256 sha256 = SHA256.Create())
                                        {
                                            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(concatValues));
                                            hashed = BitConverter.ToInt32(hashedBytes, 0);

                                        }
                                        // extracting the date from the file name and storing it under the field DateTime_key
                                        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(e.FullPath);
                                        string[] fileDate = fileNameWithoutExt.Split('_');
                                        string dateExtract = fileDate[fileDate.Length - 2] + fileDate[fileDate.Length - 1];

                                        DateTime date = DateTime.ParseExact(dateExtract, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);

                                        lines[i] = hashed.ToString() + "," + date.ToString("yyyy/MM/dd HH:mm:ss") + "," + lines[i];




                                    }
                                }
                                catch (Exception exep)
                                {

                                    Console.WriteLine("Error adding fields: " + exep.Message);
                                }
                                finally
                                {


                                    string[] lineEntriesArray = lines[i].Split(',');
                                    List<string> lineEntries = new List<string>(lineEntriesArray);


                                    if (fileName.Contains("RADIO_LINK_POWER"))
                                    {


                                  

                                        //Console.WriteLine("after:"+lines.Count);
                                        // Remove disabled fields
                                        //  int[] removedColumns = { 0, 8, 16 };
                                        int[] removedColumns = { 2, 10, 18 };

                                        lineEntries = lineEntries
                                            .Select((x, Index) => removedColumns.Contains(Index) ? "" : x)
                                            .Where(x => !string.IsNullOrWhiteSpace(x))
                                            .ToList();


                                        //List<string> lineEntriesList = new List<string>(lineEntries);
                                        // Extract the trailing info of the object field
                                        if (i != 0)
                                        {

                                            string objectValue = lineEntries[3];
                                            string trailingInfo = objectValue.Split("_").First();
                                            string[] splitTrailing = trailingInfo.Split("/");
                                            Console.WriteLine("trailingInfo: " + trailingInfo);
                                            string slot = "";
                                            string slot1 = "";
                                            string slot2 = "";
                                            string port = "";
                                            
                                            

                                            // Case 1 where the trailing info has one spot where it is decimal
                                            if (splitTrailing[1].Contains(".") && !splitTrailing[1].Contains("+"))
                                            {
                                                slot = splitTrailing[1].Split(".")[0];
                                                port = splitTrailing[1].Split(".")[1];
                                                string linkValue = " " + slot + "/" + port ;

                                                lineEntries.Add(linkValue);

                                              


                                            }
                                            // Case 2 where there is one trailing info and it is integer
                                            else if (splitTrailing[1].Contains("+") && !splitTrailing[1].Contains("."))
                                            {
                                                slot1 = splitTrailing[1].Split("+")[0];
                                                slot2 = splitTrailing[1].Split('+')[1];
                                                port = splitTrailing[2];
                                                //string linkValue1 = slot1 + "/" + port;
                                                //string linkValue2 = slot2 + "/" + port;
                                                string linkValue = " "+splitTrailing[1] + "/" + port;
                
                                                lineEntries.Add(linkValue);

                                            // Case 3 where there are two slots and a decimal
                                            }else if (splitTrailing[1].Contains("+") && splitTrailing[1].Contains("."))
                                            {
                                                slot1 = splitTrailing[1].Split("+")[0].Split(".")[0];
                                                slot2 = splitTrailing[1].Split("+")[1].Split(".")[1];
                                                port = splitTrailing[2];

                                                string linkValue = " " + slot1 + "+" + slot2 + "/" + port;
                                                lineEntries.Add(linkValue);

                                            // Case default where there is only one integer slot
                                            }else if (!splitTrailing[1].Contains("+") && !splitTrailing[1].Contains("."))
                                            {
                                                slot = splitTrailing[1];
                                                port = splitTrailing[2];
                                                string linkValue = " " + slot + "/" + port;
                                                lineEntries.Add(linkValue);
                                            }


                                            //TID Field
                                            string tid = objectValue.Split("_")[2];
                                            lineEntries.Add(tid);

                                            //FARENDTID
                                            string farendtid = objectValue.Split("_").Last();
                                            lineEntries.Add(farendtid);

                                            //SLOT
                                            if(!string.IsNullOrEmpty(slot))
                                            {

                                            lineEntries.Add(slot);
                                            lineEntries.Add(port);
                                            }
                                            //else if (!string.IsNullOrEmpty(slot1))
                                            //{
                                            //    lineEntries.Add(slot1);
                                            //}



                                        }


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
                                            .ToList();

                                        // Reconstruct the list
                                        lines[i] = string.Join(",", lineEntries);

                                    }

                                }
                                csvWriter.WriteLine(lines[i]);
                            }


                        }

                    }catch(IndexOutOfRangeException exception)
                    {
                        Console.WriteLine(exception.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                    finally
                    {
                        targetPath = @"C:\Users\User\Desktop\G\Baby NI Project\Code\NI-BE\NI-BE\NI-BE\Data\OldData";

                    }


                }

                // removing the file form the folder and adding it to the OldData folder
                try
                {
                    targetPath = Path.Combine(targetPath, Path.GetFileName(e.FullPath.ToString()));
                    File.Copy(e.FullPath.ToString(), targetPath, true);

                    if (File.Exists(targetPath))
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

    }
}
