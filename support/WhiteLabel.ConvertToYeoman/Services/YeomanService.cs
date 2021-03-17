using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WhiteLabel.ConvertToYeoman.Helpers;

namespace WhiteLabel.ConvertToYeoman.Services
{
    public class YeomanService
    {
        public YeomanService()
        {

        }

        public void Migrate(string destinationDirectory)
        {
            var solutionDirectory = VisualStudioHelper.TryGetSolutionDirectoryInfo().FullName;

            if (string.IsNullOrEmpty(solutionDirectory))
            {
                ConsoleLogHelper.ShowInfoMessage("Solution directory has not been found.", ConsoleColor.Red);
                return;
            }

            ConsoleLogHelper.ShowInfoMessage("Yeoman migration started", ConsoleColor.White);
            ConsoleLogHelper.ShowInfoMessage($"Source: {solutionDirectory}", ConsoleColor.Cyan);
            ConsoleLogHelper.ShowInfoMessage($"Destination: {destinationDirectory}", ConsoleColor.Green);

            ConsoleLogHelper.ShowInfoMessage($"Copy solution", ConsoleColor.Blue);
            var numFilesCopied = FileHelper.CopyDirectory(solutionDirectory, destinationDirectory, 
                new [] { ".vs", "bin", "obj", ".git", "WhiteLabel.ConvertToYeoman" },
                new [] { ".gitignore" });

            // Remove ConvertToYeoman project from solution
            ConsoleLogHelper.ShowInfoMessage($"Remove Yeoman project from solution", ConsoleColor.Blue);
            var solutionFile = VisualStudioHelper.TryGetSolutionFileInfo(destinationDirectory);
            VisualStudioHelper.RemoveProjectFromSolution(solutionFile, "support/WhiteLabel.ConvertToYeoman/WhiteLabel.ConvertToYeoman.csproj");

            ConsoleLogHelper.ShowInfoMessage($"Modify project files for Yeoman", ConsoleColor.Blue);
            FileHelper.ReplaceContent(new[] { ".sln", ".csproj" }, destinationDirectory, "WhiteLabel", "<%= title %>");

            ConsoleLogHelper.ShowInfoMessage($"{numFilesCopied} files have been copied", ConsoleColor.Blue);
        }
    }
}
