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

        public string FormatGenerationMessage(int countAdded, double elapsedTime, bool wasStopped)
        {
            return wasStopped
                ? $"Generation stopped. {countAdded} names added in {elapsedTime:F2} seconds."
                : $"Generation complete. {countAdded} names added in {elapsedTime:F2} seconds.";
        }


        public async Task StartGeneration(IDatabaseHandler dbHandler, CancellationToken cancellationToken, Action<int, double, bool> afterGeneration)
        {
            try
            {
                _nameLoader.LoadNamesFromFiles();

                await dbHandler.GenerateUniqueNames(
                    _nameLoader, cancellationToken,
                    status => _generationStatus = status,
                    (countAdded, elapsedTime, wasStopped) =>
                    {
                        _generationStatus = FormatGenerationMessage(countAdded, elapsedTime, wasStopped);
                        afterGeneration(countAdded, elapsedTime, wasStopped);
                    }
                );
            }
            catch (OperationCanceledException)
            {
                _generationStatus = FormatGenerationMessage(0, 0, true);
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
