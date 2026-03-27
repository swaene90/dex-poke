namespace dex_poke.Shared.Services;

public interface IUserGamesService
{
    IReadOnlyList<string> OwnedGames { get; }
    void AddGame(string apiName);
    void RemoveGame(string apiName);
}
