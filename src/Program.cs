using System;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Models;
using System.Linq;
using System.Collections.Generic;

namespace dncConsoleApp
{
    class Program
    {
        private static string homeFolder = "/Users/michaeljosephinterapp/";

        static void Main(string[] args)
        {
            var sourceFolder = $"{homeFolder}Documents/migrate/";
            var configJsonPath = $"{homeFolder}git/wingman/ngWingman/src/console/migrationData.json";
            var json = File.ReadAllText(configJsonPath);
            var model = JsonConvert.DeserializeObject<AngularJsToAngularMigrationModel>(json);

            var sourceDirectoryInfo = new DirectoryInfo(sourceFolder);
            foreach (var migrationType in model.MigrationTypes)
            {
                foreach (var fileInfo in sourceDirectoryInfo.GetFiles("*." + migrationType.Extension))
                {
                    // Process the file
                    ProcessFile(fileInfo.FullName, migrationType);
                }
            }

            Console.WriteLine("Done.");
        }

        public static void ProcessFile(string filePath, MigrationType migrationType)
        {
            var fileContents = File.ReadAllText(filePath);
            var sb = new StringBuilder();
            var constructors = new List<string>();

            //      Look for matching patterns and replace with new values.
            foreach (var replacement in migrationType.Replacements.OrderBy(x => x.AddToConstructor))
            {
                if (fileContents.Contains(replacement.Old))
                {
                    fileContents = fileContents.Replace(replacement.Old, replacement.New);

                    // If the AddToConstructor value exists. Add to a separate string builder if not added already:
                    if (!string.IsNullOrEmpty(replacement.AddToConstructor) && !constructors.Contains(replacement.AddToConstructor))
                    {
                        if (sb.Length != 0) sb.Append(", ");
                        sb.Append(replacement.AddToConstructor);
                        constructors.Add(replacement.AddToConstructor);
                    }
                }
            }

            var outputFolder = Path.Combine(Path.GetDirectoryName(filePath), "_output");
            if (!Directory.Exists(outputFolder)) Directory.CreateDirectory(outputFolder);
            var outputPath = Path.Combine(outputFolder, Path.GetFileName(filePath));

            if (constructors.Count == 0)
            {
                File.WriteAllText(outputPath, fileContents);
                return;
            }

            var sb2 = new StringBuilder();
            sb2.Append(fileContents);
            sb2.AppendLine();
            sb2.AppendLine("constructor(" + sb.ToString() + ") {}");
            File.WriteAllText(outputPath, sb2.ToString());
        }
    }
}
