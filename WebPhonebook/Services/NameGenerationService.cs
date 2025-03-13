using WebPhonebook.Interfaces;
using PhonebookServices;

namespace WebPhonebook.Services
{
    public class NameGenerationService
    {
        private readonly NameLoader _nameLoader;
        private static string _generationStatus = " ";

        public NameGenerationService(NameLoader nameLoader)
        {
            _nameLoader = nameLoader ?? throw new ArgumentNullException(nameof(nameLoader));
        }

        public async Task StartGeneration(IDatabaseHandler dbHandler, CancellationToken cancellationToken, Action<int, double, bool> afterGeneration)
        {
            try
            {
                _nameLoader.LoadNamesFromFiles();

                await dbHandler.GenerateUniqueNames(
                    _nameLoader, cancellationToken,
                    status => _generationStatus = status,
                    afterGeneration
                );

                _generationStatus = "Generation complete.";
            }
            catch (OperationCanceledException)
            {
                _generationStatus = "Generation was stopped.";
            }
            catch (Exception ex)
            {
                _generationStatus = $"Error: {ex.Message}";
            }
        }

        public string GetGenerationStatus()
        {
            return _generationStatus;
        }
    }
}
