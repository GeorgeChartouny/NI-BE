using NI_BE.DataDb;
using System.Security.Cryptography;
using System.Text;

namespace NI_BE.Parser
{
    public class ParsingFile
    {

        public ParsingFile()
        {
            
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
                    //  string csvFile = Path.ChangeExtension(e.FullPath,"csv");
                    string csvFile = Path.Combine(@"C:\Users\User\Desktop\G\Baby NI Project\Code\NI-BE\NI-BE\NI-BE\Data\ParsedData", Path.ChangeExtension(Path.GetFileName(e.FullPath), "csv"));
                    string slot = "";
                    string slot1 = "";
                    string slot2 = "";
                    string port = "";
                    List<string> copyEntry = new List<string>();

                    int tries = 10;
                    int delayTime = 5000;
                    for (int y = 0; y < tries; y++)
                    {


                        try
                        {
                            //Reading from the file
                            List<string> lines = File.ReadAllLines(e.FullPath).ToList();


                            using (StreamWriter csvWriter = new StreamWriter(csvFile))
                            {
                                // Discarding failed records and 
                                // Discard the record if failed field has value other than "-"
                                if (fileName.Contains("RADIO_LINK_POWER"))
                                {

                                    for (int j = lines.Count - 1; j >= 1; j--)
                                    {
                                        string failedDescField = lines[j].Split(',').Last();

                                        if (lines[j].Trim().IndexOf("Unreachable Bulk FC", StringComparison.OrdinalIgnoreCase) >= 0 || failedDescField != "-")
                                            lines.RemoveAt(j);
                                    }
                                }
                                //Discard records with values "---"
                                else if (fileName.Contains("TN_RFInputPower"))
                                {
                                    for (int j = lines.Count - 1; j >= 1; j--)
                                    {
                                        if (lines[j].Trim().IndexOf("---", StringComparison.OrdinalIgnoreCase) >= 0)
                                            lines.RemoveAt(j);
                                    }
                                }

                                for (int i = 0; i < lines.Count; i++)
                                {

                                    //Add two new fields to the list
                                    try
                                    {
                                        if (i == 0 && fileName.Contains("RADIO_LINK_POWER"))
                                        {
                                            lines[i] = "Network_SID," + "DateTime_Key," + lines[i] + ",Link,TID,FARENDTID,SLOT,PORT";
                                        }
                                        else if (i == 0 && fileName.Contains("TN_RFInputPower"))
                                        {
                                            lines[i] = "Network_SID," + "DateTime_Key," + lines[i] + ",SLOT," + "PORT";
                                        }
                                        else
                                        {

                                            string[] splitEntires = lines[i].Split(',');


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

                                            // Remove disabled fields
                                            //  int[] removedColumns = { 0, 8, 16 };
                                            int[] removedColumns = { 2, 10, 18 };

                                            lineEntries = lineEntries
                                                .Select((x, Index) => removedColumns.Contains(Index) ? "" : x)
                                                .Where(x => !string.IsNullOrWhiteSpace(x))
                                                .ToList();


                                            // Extract the trailing info of the object field
                                            if (i != 0)
                                            {

                                                string objectValue = lineEntries[3];
                                                string trailingInfo = objectValue.Split("_").First();
                                                string[] splitTrailing = trailingInfo.Split("/");


                                                // Case 1 where the trailing info has one spot where it is decimal
                                                if (splitTrailing[1].Contains(".") && !splitTrailing[1].Contains("+"))
                                                {
                                                    slot = splitTrailing[1].Split(".")[0];
                                                    port = splitTrailing[1].Split(".")[1];
                                                    string linkValue = " " + slot + "/" + port;

                                                    lineEntries.Add(linkValue);

                                                }
                                                // Case 2 where there is one trailing info and it is integer
                                                else if (splitTrailing[1].Contains("+") && !splitTrailing[1].Contains("."))
                                                {
                                                    slot1 = splitTrailing[1].Split("+")[0];
                                                    slot2 = splitTrailing[1].Split('+')[1];
                                                    port = splitTrailing[2];

                                                    string linkValue = " " + splitTrailing[1] + "/" + port;

                                                    lineEntries.Add(linkValue);

                                                    // Case 3 where there are two slots and a decimal
                                                }
                                                else if (splitTrailing[1].Contains("+") && splitTrailing[1].Contains("."))
                                                {
                                                    slot1 = splitTrailing[1].Split("+")[0].Split(".")[0];
                                                    slot2 = splitTrailing[1].Split("+")[1].Split(".")[1];
                                                    port = splitTrailing[2];

                                                    string linkValue = " " + slot1 + "+" + slot2 + "/" + port;
                                                    lineEntries.Add(linkValue);

                                                    // Case default where there is only one integer slot
                                                }
                                                else if (!splitTrailing[1].Contains("+") && !splitTrailing[1].Contains("."))
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
                                                if (!string.IsNullOrEmpty(slot))
                                                {

                                                    lineEntries.Add(slot);
                                                    lineEntries.Add(port);
                                                    slot = "";
                                                    port = "";
                                                }
                                                else if (!string.IsNullOrEmpty(slot1))
                                                {


                                                    lineEntries.Add(slot1);
                                                    lineEntries.Add(port);

                                                    for (int z = 0; z < lineEntries.Count - 2; z++)
                                                    {
                                                        copyEntry.Add(lineEntries[z]);
                                                    }
                                                    copyEntry.Add(slot2);
                                                    copyEntry.Add(port);
                                                    slot1 = "";
                                                    slot2 = "";
                                                    port = "";

                                                }
                                            }

                                            // Reconstruct the list
                                            lines[i] = string.Join(",", lineEntries);

                                        }

                                        else if (fileName.Contains("TN_RFInputPower"))
                                        {



                                            //int[] removedColumns = { 8, 10, 11, 14 };
                                            int[] removedColumns = { 10, 12, 13, 16 };

                                            // Remove disabled fields
                                            lineEntries = lineEntries
                                                .Select((x, Index) => removedColumns.Contains(Index) ? "" : x)
                                                .Where(x => !string.IsNullOrWhiteSpace(x))
                                                .ToList();


                                            if (i != 0 && lineEntries[4].Contains("."))
                                            {
                                                //extracting the slots and ports
                                                string objectValue = lineEntries[4];
                                                string first = objectValue.Split('/')[0];
                                                string slotInt = objectValue.Split("/")[1].Split(".")[0];
                                                string portInt = objectValue.Split("/")[1].Split(".")[1];
                                                string slotValue = first + "/" + slotInt + "+";

                                                lineEntries.Add(slotValue);
                                                lineEntries.Add(portInt);

                                            }

                                            // Reconstruct the list
                                            lines[i] = string.Join(",", lineEntries);

                                        }

                                    }
                                    csvWriter.WriteLine(lines[i]);

                                    // Adding a duplicate record with a second slot 
                                    if (copyEntry.Count > 0)
                                    {
                                        List<string> lines2 = new List<string>();

                                        lines2.Add(string.Join(",", copyEntry));

                                        csvWriter.WriteLine(lines2[0]);

                                        copyEntry.Clear();
                                        lines2.Clear();
                                    }
                                }


                            }
                            break;
                        }
                        catch (IOException exception) when (y<=tries) 
                        {
                            Console.WriteLine(exception.Message);
                            Thread.Sleep(delayTime);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        finally
                        {
                            targetPath = @"C:\Users\User\Desktop\G\Baby NI Project\Code\NI-BE\NI-BE\NI-BE\Data\OldData";
                            LoadData loadData = new LoadData();
                            loadData.ExecuteLoader(csvFile);

                        }
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

    }
}
