using Microsoft.JSInterop;

namespace JobApplicationTracker.Client.Services;

public class LanguageService(IJSRuntime jsRuntime) : ILanguageService
{
    private const string StorageKey = "jobtracker.language";
    private const string DefaultLanguage = "sv";

    public string Language { get; private set; } = DefaultLanguage;

    public event Action? Changed;

    public async Task InitializeAsync()
    {
        try
        {
            var stored = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
            if (Translations.Values.ContainsKey(stored ?? ""))
            {
                Language = stored!;
            }
        }
        catch (JSException)
        {
            // localStorage unavailable (e.g. private browsing) -- keep the default language.
        }
    }

    public async Task SetLanguageAsync(string language)
    {
        if (!Translations.Values.ContainsKey(language) || language == Language)
        {
            return;
        }

        Language = language;

        try
        {
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, language);
        }
        catch (JSException)
        {
            // Preference just won't survive a reload -- not fatal.
        }

        Changed?.Invoke();
    }

    public string T(string key) =>
        Translations.Values[Language].TryGetValue(key, out var value) ? value : key;

    public string T(string key, params object[] args) =>
        string.Format(T(key), args);

    public string StatusLabel(string status) => T($"status.{status}");
}
