using Microsoft.Extensions.Configuration;

namespace PhonebookServices
{
    public class NameLoader
    {
        public readonly string nameFilesDirectory;
        public List<string> FirstNames { get; set; }
        public List<string> MiddleNames { get; set; }
        public List<string> LastNames { get; set; }

        public NameLoader(IConfiguration configuration)
        {
            nameFilesDirectory = GetNameFilesDirectory(configuration);
        }

        public static string GetNameFilesDirectory(IConfiguration configuration)
        {
            return configuration["UploadSettings:UploadFolder"] ?? "wwwroot/uploads";
        }

        public void LoadNamesFromFiles()
        {
            if (!Directory.Exists(nameFilesDirectory))
            {
                throw new DirectoryNotFoundException($"The directory '{nameFilesDirectory}' does not exist.");
            }

            FirstNames = File.ReadAllLines(Path.Combine(nameFilesDirectory, "firstNames.txt")).ToList();
            MiddleNames = File.ReadAllLines(Path.Combine(nameFilesDirectory, "middleNames.txt")).ToList();
            LastNames = File.ReadAllLines(Path.Combine(nameFilesDirectory, "lastNames.txt")).ToList();
        }
    }
}
