using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Search_HDD
{
    class SearchDirectories
    {
        public static List<string> EndsWith(string path, string term, string copypath)
        {
            List<string> files = new List<string>();
            foreach (string file in Directory.EnumerateFiles(path).Where(x => x.ToLower().EndsWith(term.ToLower())))
            {
                if (!(file.Contains(copypath)))
                {
                    try
                    {
                        files.Add(file);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Found {file}");
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            foreach (string subdir in Directory.EnumerateDirectories(path))
            {
                try
                {
                    files.AddRange(EndsWith(subdir, term, copypath));
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(ex.Message);
                }
            }
            Console.ResetColor();
            return files;
        }

        public static List<string> StartsWith(string path, string term, string copypath)
        {
            List<string> files = new List<string>();
            foreach (string file in Directory.EnumerateFiles(path).Where(x => x.ToLower().Substring(x.LastIndexOf("\\") + 1).StartsWith(term.ToLower())))
            {
                if (!(file.Contains(copypath)))
                {
                    try
                    {
                        files.Add(file);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Found {file}");
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            foreach (string subdir in Directory.EnumerateDirectories(path))
            {
                try
                {
                    files.AddRange(StartsWith(subdir, term, copypath));
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(ex.Message);
                }
            }
            Console.ResetColor();
            return files;
        }

        public static List<string> FileNameContains(string path, string term, string copypath)
        {
            List<string> files = new List<string>();
            foreach (string file in Directory.EnumerateFiles(path).Where(x => x.ToLower().Substring(x.LastIndexOf("\\")).Contains(term.ToLower())))
            {
                if (!(file.Contains(copypath)))
                {
                    try
                    {
                        files.Add(file);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Found {file}");
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            foreach (string subdir in Directory.EnumerateDirectories(path))
            {
                try
                {
                    files.AddRange(FileNameContains(subdir, term, copypath));
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(ex.Message);
                }
            }
            Console.ResetColor();
            return files;
        }

        public static void Copy(List<string> files, string copypath, string Date, bool Copy)
        {
            if (!File.Exists(copypath + $"\\Paths Successfully Copied__{Date}.log"))
                File.Create(copypath + $"\\Paths Successfully Copied__{Date}.log").Close();
            if (!File.Exists(copypath + $"\\Paths Unsuccessfully Copied__{Date}.log"))
                File.Create(copypath + $"\\Paths Unsuccessfully Copied__{Date}.log").Close();

            Directory.CreateDirectory(copypath + "\\Results");
            StreamWriter sws = new StreamWriter(copypath + $"\\Paths Successfully Copied__{Date}.log", true);
            StreamWriter swf = new StreamWriter(copypath + $"\\Paths Unsuccessfully Copied__{Date}.log", true);

            foreach (string file in files)
            {
                string dest = Path.Combine(copypath + "\\Results", Path.GetFileName(file));
                try
                {
                    if (!File.Exists(dest))
                    {
                        if (Copy == true)
                        {
                            File.Copy(file, dest, true);
                            Console.WriteLine($"Copied {file}");
                        }
                        sws.WriteLine(file);
                    }
                    else
                    {
                        for (int i = 1; i < 10000; i++)
                        {
                            if (!File.Exists(copypath + "\\Results\\" + Path.GetFileNameWithoutExtension(file) + "_" + i + Path.GetExtension(file)))
                            {
                                File.Copy(file, copypath + "\\Results\\" + Path.GetFileNameWithoutExtension(file) + "_" + i + Path.GetExtension(file), true);
                                Console.WriteLine($"Copied {file}");
                                sws.WriteLine(file);
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"Failed copying {file}");
                    swf.WriteLine(ex.Message + $" ({file})");
                    Console.ResetColor();
                }
            }
            sws.Close();
            swf.Close();
        }
    }
}




