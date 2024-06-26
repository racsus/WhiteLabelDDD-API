﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WhiteLabel.ConvertToYeoman.Helpers
{
    public static class VisualStudioHelper
    {
        public static DirectoryInfo TryGetSolutionDirectoryInfo(string currentPath = null)
        {
            var directory = new DirectoryInfo(currentPath ?? Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.sln").Any())
                directory = directory.Parent;
            return directory;
        }

        public static FileInfo TryGetSolutionFileInfo(string directoryPath)
        {
            var directory = new DirectoryInfo(directoryPath);
            while (directory != null && !directory.GetFiles("*.sln").Any())
                directory = directory.Parent;
            return directory.GetFiles("*.sln").FirstOrDefault();
        }

        public static void RemoveProjectFromSolution(FileInfo solutionFile, string projectPath)
        {
            var process = new Process();
            var psi = new ProcessStartInfo();
            psi.FileName = "dotnet";
            psi.Arguments = $"sln {solutionFile.Name} remove {projectPath}";
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.WorkingDirectory = Path.GetDirectoryName(solutionFile.FullName);

            process.StartInfo = psi;
            process.Start();
            process.OutputDataReceived += (_, e) =>
            {
                Console.WriteLine(e.Data);
            };
            process.ErrorDataReceived += (_, e) =>
            {
                Console.WriteLine(e.Data);
            };
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }
    }
}
