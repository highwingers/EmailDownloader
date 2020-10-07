using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityWebGroup.Services.Duplicates
{
    // A Group key that can be passed to a separate method.  
    // Override Equals and GetHashCode to define equality for the key.  
    // Override ToString to provide a friendly name for Key.ToString()  

    public class QueryDuplicates
    {


        public string FindDuplicates(string path, bool byName = true, bool bySize = true)
        {
            try
            {
                // Change the root drive or folder if necessary.  
                string startFolder = path;

                // Make the lines shorter for the console display  
                int charsToSkip = startFolder.Length;

                // Take a snapshot of the file system.  
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(startFolder);
                IEnumerable<System.IO.FileInfo> fileList = dir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

                // Note the use of a compound key. Files that match  
                // all three properties belong to the same group.  
                // A named type is used to enable the query to be  
                // passed to another method. Anonymous types can also be used  
                // for composite keys but cannot be passed across method boundaries  
                //
                IEnumerable<System.Linq.IGrouping<PortableKey, string>> queryDupFiles;
                string title;
                StringBuilder s = new StringBuilder();

                if (byName && bySize)
                {
                  
                    title = "Duplicates By NAME & SIZE";


                    queryDupFiles =
                       from file in fileList
                       group file.FullName.Substring(charsToSkip) by
                           new PortableKey { Name = file.Name.Substring(0, file.Name.Length - 8), Length = file.Length } into fileGroup
                       where fileGroup.Count() > 1
                       select fileGroup;

                    var list = queryDupFiles.ToList();

                    s.AppendLine("-------------------------------------");
                    s.AppendLine(title);
                    s.AppendLine("-------------------------------------");

                    foreach (var item in list)
                    {
                        s.AppendLine($"Name: {item.Key.Name}, Size: {item.Key.Length}, Count: {item.Count()}");
                    }
  

                }
                else if (byName)
                {
                    title = "Duplicates By NAME";
                    queryDupFiles =
                       from file in fileList
                       group file.FullName.Substring(charsToSkip) by
                           new PortableKey { Name = file.Name.Substring(0, file.Name.Length - 8) } into fileGroup
                       where fileGroup.Count() > 1
                       select fileGroup;

                    s.AppendLine("-------------------------------------");
                    s.AppendLine(title);
                    s.AppendLine("-------------------------------------");
                    foreach (var item in queryDupFiles)
                    {
                        var o = fileList.Where(w => w.Name.Substring(0, w.Name.Length - 8) == item.Key.Name).Select(s1 => new PortableKey() { Name = s1.Name, Length = s1.Length,ItemCount=item.Count() }).ToList();
                        s.AppendLine($" *** File Name:{item.Key.Name} *** ");
                        foreach (var item2 in o)
                        {
                            s.AppendLine($"Size: {item2.Length}, Count: {item2.ItemCount}");
                        }
                        
                    }



                    


                }
                else if (bySize)
                {
                    title = "Duplicates By SIZE";
                    s.AppendLine("-------------------------------------");
                    s.AppendLine(title);
                    s.AppendLine("-------------------------------------");
                    queryDupFiles =
                       from file in fileList
                       group file.FullName.Substring(charsToSkip) by
                           new PortableKey { Length = file.Length } into fileGroup
                       where fileGroup.Count() > 1
                       select fileGroup;

                    s.AppendLine("-------------------------------------");
                    foreach (var item in queryDupFiles)
                    {
                        var o = fileList.Where(w => w.Length == item.Key.Length).Select(s1 => new PortableKey() { Name = s1.Name, Length = s1.Length, ItemCount = item.Count() }).ToList();
                        s.AppendLine($" *** File Size:{item.Key.Length} *** ");
                        foreach (var item2 in o)
                        {
                            s.AppendLine($"Name: {item2.Name},  Count: {item2.ItemCount}");
                        }

                    }
  


                }
                else
                {
                    title = "Duplicates By NAME & SIZE";
                    queryDupFiles =
                       from file in fileList
                       group file.FullName.Substring(charsToSkip) by
                           new PortableKey { Name = file.Name.Substring(0, file.Name.Length - 8), Length = file.Length } into fileGroup
                       where fileGroup.Count() > 1
                       select fileGroup;

                    var list = queryDupFiles.ToList();

                    s.AppendLine("-------------------------------------");
                    s.AppendLine(title);
                    s.AppendLine("-------------------------------------");

                    foreach (var item in list)
                    {
                        s.AppendLine($"Name: {item.Key.Name}, Size: {item.Key.Length}, Count: {item.Count()}");
                    }
                    s.AppendLine("-------------------------------------");


                }



                return s.ToString();
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
            

            //PageOutput<PortableKey, string>(queryDupFiles);
        }

        // A generic method to page the output of the QueryDuplications methods  
        // Here the type of the group must be specified explicitly. "var" cannot  
        // be used in method signatures. This method does not display more than one  
        // group per page.  
        private void PageOutput<K, V>(IEnumerable<System.Linq.IGrouping<K, V>> groupByExtList)
        {
            // Flag to break out of paging loop.  
            bool goAgain = true;

            // "3" = 1 line for extension + 1 for "Press any key" + 1 for input cursor.  
            int numLines = 100;

            // Iterate through the outer collection of groups.  
            foreach (var filegroup in groupByExtList)
            {

                // Start a new extension at the top of a page.  
                int currentLine = 0;

                // Output only as many lines of the current group as will fit in the window.  
                do
                {
                    //Console.Clear();
                    Console.WriteLine("Filename = {0}", filegroup.Key.ToString() == String.Empty ? "[none]" : filegroup.Key.ToString());

                    // Get 'numLines' number of items starting at number 'currentLine'.  
                    var resultPage = filegroup.Skip(currentLine).Take(numLines);

                    //Execute the resultPage query  
                    foreach (var fileName in resultPage)
                    {
                        Console.WriteLine("\t{0}", fileName);
                    }

                    // Increment the line counter.  
                    currentLine += numLines;

                    // Give the user a chance to escape.  
                    Console.WriteLine("Press any key to continue or the 'End' key to break...");
                    //ConsoleKey key = Console.ReadKey().Key;
                    //if (key == ConsoleKey.End)
                    //{
                    //    goAgain = false;
                    //    break;
                    //}
                } while (currentLine < filegroup.Count());

                if (goAgain == false)
                    break;
            }
        }
    }
}
