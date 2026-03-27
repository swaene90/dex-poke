using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;

namespace dex_poke.Shared.Services;

// ── Pokemon models ────────────────────────────────────────────────────────────

public class Pokemon
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Height { get; set; }
    public int Weight { get; set; }
    public List<PokemonType> Types { get; set; } = [];
    public List<PokemonAbility> Abilities { get; set; } = [];
    public Sprites Sprites { get; set; } = new();
}

public class PokemonType
{
    public TypeInfo Type { get; set; } = new();
}

public class TypeInfo
{
    public string Name { get; set; } = string.Empty;
}

public class PokemonAbility
{
    public NamedResource Ability { get; set; } = new();

    [JsonPropertyName("is_hidden")]
    public bool IsHidden { get; set; }
}

public class Sprites
{
    [JsonPropertyName("front_default")]
    public string? FrontDefault { get; set; }
}

// ── Encounter models ──────────────────────────────────────────────────────────

public class EncounterLocation(
    string locationArea,
    string gameApiName,
    int maxChance,
    int minLevel,
    int maxLevel,
    string method)
{
    public string LocationArea { get; } = locationArea;
    public string GameApiName { get; } = gameApiName;
    public int MaxChance { get; } = maxChance;
    public int MinLevel { get; } = minLevel;
    public int MaxLevel { get; } = maxLevel;
    public string Method { get; } = method;
}

internal class RawEncounterArea
{
    [JsonPropertyName("location_area")]
    public NamedResource LocationArea { get; set; } = new();

    [JsonPropertyName("version_details")]
    public List<RawVersionDetail> VersionDetails { get; set; } = [];
}

internal class RawVersionDetail
{
    [JsonPropertyName("version")]
    public NamedResource Version { get; set; } = new();

    [JsonPropertyName("max_chance")]
    public int MaxChance { get; set; }

    [JsonPropertyName("encounter_details")]
    public List<RawEncounterDetail> EncounterDetails { get; set; } = [];
}

internal class RawEncounterDetail
{
    [JsonPropertyName("chance")]
    public int Chance { get; set; }

    [JsonPropertyName("min_level")]
    public int MinLevel { get; set; }

    [JsonPropertyName("max_level")]
    public int MaxLevel { get; set; }

    [JsonPropertyName("method")]
    public NamedResource Method { get; set; } = new();
}

public class NamedResource
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

// ── Service ───────────────────────────────────────────────────────────────────

public class PokeApiService
{
    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromDays(30);

    public PokeApiService(HttpClient http, IMemoryCache cache)
    {
        _http = http;
        _cache = cache;
    }

    public async Task<Pokemon?> GetPokemonAsync(string nameOrId)
    {
        var key = $"poke:{nameOrId.ToLower()}";
        if (_cache.TryGetValue(key, out Pokemon? cached))
            return cached;

        var pokemon = await _http.GetFromJsonAsync<Pokemon>(
            $"https://pokeapi.co/api/v2/pokemon/{nameOrId.ToLower()}");

        if (pokemon is not null)
            _cache.Set(key, pokemon, CacheDuration);

        return pokemon;
    }

    public async Task<IReadOnlyList<EncounterLocation>> GetEncountersAsync(string nameOrId)
    {
        var key = $"enc:{nameOrId.ToLower()}";
        if (_cache.TryGetValue(key, out IReadOnlyList<EncounterLocation>? cached))
            return cached!;

        var raw = await _http.GetFromJsonAsync<List<RawEncounterArea>>(
            $"https://pokeapi.co/api/v2/pokemon/{nameOrId.ToLower()}/encounters")
            ?? [];

        var results = raw
            .SelectMany(area => area.VersionDetails.Select(vd =>
            {
                // Pick the encounter detail with the highest individual chance for method display
                var best = vd.EncounterDetails.MaxBy(e => e.Chance) ?? vd.EncounterDetails.FirstOrDefault();
                return new EncounterLocation(
                    area.LocationArea.Name,
                    vd.Version.Name,
                    vd.MaxChance,
                    best?.MinLevel ?? 0,
                    best?.MaxLevel ?? 0,
                    best?.Method.Name ?? string.Empty
                );
            }))
            .OrderByDescending(e => e.MaxChance)
            .ToList();

        _cache.Set(key, (IReadOnlyList<EncounterLocation>)results, CacheDuration);
        return results;
    }
}
