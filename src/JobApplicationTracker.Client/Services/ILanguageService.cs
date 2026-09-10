namespace JobApplicationTracker.Client.Services;

public interface ILanguageService
{
    string Language { get; }

    event Action? Changed;

    Task InitializeAsync();

    Task SetLanguageAsync(string language);

    string T(string key);

    string T(string key, params object[] args);

    string StatusLabel(string status);
}
