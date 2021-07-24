using System;
using IO = System.IO;
using Collection = System.Collections.Generic;

namespace ConsoleAppHelloWorld.App.FindDuplicateFile
{

    class AppMain
    {
        public static void Run()
        {
            var duplicateFiles = TryGetDuplicateFiles(@"I:\Music");

            if (duplicateFiles.Count > 0)
            {
                Console.WriteLine("Duplicate Files:");
                Console.WriteLine("==============================");
                foreach (var duplicates in duplicateFiles)
                {
                    Console.WriteLine(string.Join("\n", duplicates));
                    Console.WriteLine("------------------------------");
                }
            }
            else
            {
                Console.WriteLine("No duplicate files were found.");
            }
        }

        /// <summary>Find all duplicate files recursively in the given directory</summary>
        /// <param name="directory">Target Directory</param>
        /// <returns>A list with the grouped dupliate files</returns>
        private static Collection.List<Collection.List<string>> TryGetDuplicateFiles(string directory)
        {
            Collection.List<Collection.List<string>> duplicateFiles = new();

            var md5ToFilePath = new Collection.Dictionary<string, Collection.List<string>>();

            Collection.List<string> files = new();
            TryGetDirectoryFiles(directory, ref files);

            if (files.Count > 0)
            {
                int fileCount = files.Count;
                foreach (string filePath in files)
                {
                    Console.Write($"Total: {files.Count} - Remaining: {fileCount--}");
                    if (TryGetFileMd5Hash(filePath, out string md5Hash))
                    {
                        if (md5ToFilePath.TryGetValue(md5Hash, out var existingList))
                        {
                            existingList.Add(filePath);
                        }
                        else
                        {
                            var list = new Collection.List<string>() { filePath };
                            md5ToFilePath.Add(md5Hash, list);
                        }
                    }
                    Console.Write($"\r                              \r");
                }
            }

            if (md5ToFilePath.Count > 0)
            {
                foreach (var pair in md5ToFilePath)
                {
                    if (pair.Value.Count > 1)
                    {
                        duplicateFiles.Add(pair.Value);
                    }
                }
            }
            return duplicateFiles;
        }

        /// <summary>Get files in the directory, including file in it's sub-directory</summary>
        /// <param name="targetDirectory">Target Directory</param>
        /// <param name="files">File list output</param>
        private static void TryGetDirectoryFiles(string targetDirectory, ref Collection.List<string> files)
        {
            try
            {
                files.AddRange(IO.Directory.GetFiles(targetDirectory));
                var directories = IO.Directory.GetDirectories(targetDirectory);
                foreach (string directory in directories)
                {
                    TryGetDirectoryFiles(directory, ref files);
                }
            }
            catch (Exception e)
            {
                if (e is IO.DirectoryNotFoundException)
                {
                    Console.WriteLine($"{nameof(TryGetDirectoryFiles)}:DirectoryNotFound: {targetDirectory}");
                }
                else if (e is IO.IOException)
                {
                    Console.WriteLine($"{nameof(TryGetDirectoryFiles)}:IOError: {targetDirectory}");
                }
                else
                {
                    Console.WriteLine($"{nameof(TryGetDirectoryFiles)}:Error:{e.GetType()}: {e.Message}");
                }
            }
        }

        /// <summary>Get MD5 Hash of the given path of a file</summary>
        /// <param name="filePath">File Path</param>
        /// <param name="md5Hash">MD5 Hash output</param>
        /// <returns>True if MD5 was generated successfully, false otherwise</returns>
        private static bool TryGetFileMd5Hash(string filePath, out string md5Hash)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
            try
            {
                using var file = IO.File.OpenRead(filePath);
                md5Hash = Convert.ToHexString(md5.ComputeHash(file));
                return true;
            }
            catch (Exception e)
            {
                if (e is IO.FileNotFoundException)
                {
                    Console.WriteLine($"{nameof(TryGetFileMd5Hash)}:FileNotFound: {filePath}");
                }
                else if (e is IO.IOException)
                {
                    Console.WriteLine($"{nameof(TryGetFileMd5Hash)}:IOError: {filePath}");
                }
                else
                {
                    Console.WriteLine($"{nameof(TryGetFileMd5Hash)}:Error:{e.GetType()}: {e.Message}");
                }
                md5Hash = null;
                return false;
            }
        }
    }
}
