using dex_poke.Shared.Services;

namespace dex_poke.Services;

public class UserGamesService : IUserGamesService
{
    private const string PrefsKey = "owned_games";
    private readonly List<string> _games;

    public UserGamesService()
    {
        var saved = Preferences.Default.Get(PrefsKey, string.Empty);
        _games = string.IsNullOrEmpty(saved)
            ? []
            : [.. saved.Split(',', StringSplitOptions.RemoveEmptyEntries)];
    }

    public IReadOnlyList<string> OwnedGames => _games;

    public void AddGame(string apiName)
    {
        if (_games.Contains(apiName)) return;
        _games.Add(apiName);
        Save();
    }

    public void RemoveGame(string apiName)
    {
        _games.Remove(apiName);
        Save();
    }

    private void Save() =>
        Preferences.Default.Set(PrefsKey, string.Join(',', _games));
}
