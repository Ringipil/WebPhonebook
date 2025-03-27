using Microsoft.Extensions.Configuration;

namespace PhonebookServices
{
    public class NameLoader
    {
        public string NameFilesDirectory { get; private set; }
        public List<string> FirstNames { get; set; }
        public List<string> MiddleNames { get; set; }
        public List<string> LastNames { get; set; }

        public NameLoader(IConfiguration configuration)
        {
            NameFilesDirectory = configuration["UploadSettings:UploadFolder"] ?? "wwwroot/uploads";
        }

        public void LoadNamesFromFiles()
        {
            if (!Directory.Exists(NameFilesDirectory))
            {
                throw new DirectoryNotFoundException($"The directory '{NameFilesDirectory}' does not exist.");
            }

            FirstNames = File.ReadAllLines(Path.Combine(NameFilesDirectory, "firstNames.txt")).ToList();
            MiddleNames = File.ReadAllLines(Path.Combine(NameFilesDirectory, "middleNames.txt")).ToList();
            LastNames = File.ReadAllLines(Path.Combine(NameFilesDirectory, "lastNames.txt")).ToList();
        }
    }
}
