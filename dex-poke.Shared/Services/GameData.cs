namespace dex_poke.Shared.Services;

public record GameInfo(string ApiName, string DisplayName, int Generation, bool HasShiny);

public static class GameData
{
    public static readonly IReadOnlyList<GameInfo> AllGames =
    [
        new("red",              "Red",              1, false),
        new("blue",             "Blue",             1, false),
        new("yellow",           "Yellow",           1, false),
        new("gold",             "Gold",             2, true),
        new("silver",           "Silver",           2, true),
        new("crystal",          "Crystal",          2, true),
        new("ruby",             "Ruby",             3, true),
        new("sapphire",         "Sapphire",         3, true),
        new("firered",          "FireRed",          3, true),
        new("leafgreen",        "LeafGreen",        3, true),
        new("emerald",          "Emerald",          3, true),
        new("diamond",          "Diamond",          4, true),
        new("pearl",            "Pearl",            4, true),
        new("platinum",         "Platinum",         4, true),
        new("heartgold",        "HeartGold",        4, true),
        new("soulsilver",       "SoulSilver",       4, true),
        new("black",            "Black",            5, true),
        new("white",            "White",            5, true),
        new("black-2",          "Black 2",          5, true),
        new("white-2",          "White 2",          5, true),
        new("x",                "X",                6, true),
        new("y",                "Y",                6, true),
        new("omega-ruby",       "Omega Ruby",       6, true),
        new("alpha-sapphire",   "Alpha Sapphire",   6, true),
        new("sun",              "Sun",              7, true),
        new("moon",             "Moon",             7, true),
        new("ultra-sun",        "Ultra Sun",        7, true),
        new("ultra-moon",       "Ultra Moon",       7, true),
        new("sword",            "Sword",            8, true),
        new("shield",           "Shield",           8, true),
        new("brilliant-diamond","Brilliant Diamond", 8, true),
        new("shining-pearl",    "Shining Pearl",    8, true),
        new("scarlet",          "Scarlet",          9, true),
        new("violet",           "Violet",           9, true),
    ];

    public static readonly IReadOnlyDictionary<string, GameInfo> ByApiName =
        AllGames.ToDictionary(g => g.ApiName);

    public static string ShinyOdds(int generation) => generation switch
    {
        1     => "No shinies",
        <= 5  => "1 / 8,192",
        _     => "1 / 4,096"
    };

    public static string HiddenAbilityNote(int generation) => generation switch
    {
        <= 4  => "Not obtainable in the wild",
        5     => "Dream World events or Hidden Grottos only",
        6     => "Friend Safari (100%) or Horde encounter (20% chance one has HA)",
        7     => "SOS chaining — up to 20% after 30+ chained calls",
        8     => "Max/Dynamax Raid dens only",
        _     => "Tera Raid battles only"
    };

    public static string FormatLocationName(string apiName) =>
        System.Globalization.CultureInfo.CurrentCulture.TextInfo
            .ToTitleCase(apiName.Replace('-', ' '));

    public static string FormatMethod(string apiName) => apiName switch
    {
        "walk"              => "Walking",
        "surf"              => "Surfing",
        "old-rod"           => "Old Rod",
        "good-rod"          => "Good Rod",
        "super-rod"         => "Super Rod",
        "rock-smash"        => "Rock Smash",
        "headbutt"          => "Headbutt",
        "headbutt-normal"   => "Headbutt",
        "headbutt-special"  => "Headbutt (Special)",
        "only-one"          => "Static / Gift",
        "gift"              => "Gift",
        "grass-spots"       => "Rustling Grass",
        "dark-spots"        => "Rippling Water",
        "cave-spots"        => "Dust Cloud",
        "bridge-spots"      => "Bridge Shadow",
        "super-rod-spots"   => "Super Rod (Ripple)",
        _                   => System.Globalization.CultureInfo.CurrentCulture.TextInfo
                                   .ToTitleCase(apiName.Replace('-', ' '))
    };
}
