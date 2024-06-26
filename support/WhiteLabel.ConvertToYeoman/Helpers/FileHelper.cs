﻿using System;
using System.IO;
using System.Linq;

namespace WhiteLabel.ConvertToYeoman.Helpers
{
    public static class FileHelper
    {
        public static int CopyDirectory(
            string sourcePath,
            string destinationDirectory,
            string[] directoriesToExclude,
            string[] filesToExclude
        )
        {
            var res = 0;
            //Now Create all of the directories
            foreach (
                var dirPath in Directory.GetDirectories(
                    sourcePath,
                    "*",
                    SearchOption.AllDirectories
                )
            )
                if (!PathContainsDirectory(dirPath, directoriesToExclude))
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationDirectory));

            //Copy all the files & Replaces any files with the same name
            foreach (
                var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories)
            )
                if (
                    !PathContainsDirectory(Path.GetDirectoryName(newPath), directoriesToExclude)
                    && !filesToExclude.Contains(Path.GetFileName(newPath))
                )
                {
                    File.Copy(newPath, newPath.Replace(sourcePath, destinationDirectory), true);
                    ConsoleLogHelper.ShowInfoMessage(
                        $"Copy",
                        ConsoleColor.Green,
                        newPath,
                        ConsoleColor.White
                    );

                    res += 1;
                }

            return res;
        }

        public static void ReplaceContent(
            string[] extensions,
            string destinationToSearch,
            string oldWord,
            string newWord
        )
        {
            foreach (
                var file in Directory.GetFiles(
                    destinationToSearch,
                    "*.*",
                    SearchOption.AllDirectories
                )
            )
                if (extensions.Contains(Path.GetExtension(file)))
                    ReplaceContent(Path.GetFileName(file), destinationToSearch, oldWord, newWord);
        }

        public static void ReplaceContent(
            string fileName,
            string destinationToSearch,
            string oldWord,
            string newWord
        )
        {
            foreach (
                var file in Directory.GetFiles(
                    destinationToSearch,
                    "*.*",
                    SearchOption.AllDirectories
                )
            )
                if (Path.GetFileName(file) == fileName)
                {
                    ConsoleLogHelper.ShowInfoMessage(
                        $"Modify",
                        ConsoleColor.Green,
                        file,
                        ConsoleColor.White
                    );

                    // Create a temporary file path where we can write modify lines
                    var tempFile = Path.Combine(
                        Path.GetDirectoryName(file),
                        $"{Path.GetFileNameWithoutExtension(file)}-temp{Path.GetExtension(file)}"
                    );

                    // Open a stream for the source file
                    using (var sourceFile = File.OpenText(file))
                    {
                        // Open a stream for the temporary file
                        using (var tempFileStream = new StreamWriter(tempFile))
                        {
                            string line;
                            // read lines while the file has them
                            while ((line = sourceFile.ReadLine()) != null)
                            {
                                // Do the word replacement
                                line = line.Replace(oldWord, newWord);
                                // Write the modified line to the new file
                                tempFileStream.WriteLine(line);
                            }
                        }
                    }

                    // Replace the original file with the temporary one
                    File.Replace(tempFile, file, null);
                }
        }

        private static bool PathContainsDirectory(string path, string[] directoriesToSearch)
        {
            var directory = new DirectoryInfo(path);
            if (directoriesToSearch.Contains(directory.Name))
                return true;

            if (directory.Parent != null)
                return PathContainsDirectory(directory.Parent.FullName, directoriesToSearch);
            else
                return false;
        }
    }
}
