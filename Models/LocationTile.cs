using System.Text.Json.Serialization;
using Wyrmquest.Models;


public class LocationTile
{
    [JsonPropertyName("mapId")]
    public string MapId { get; set; } = "town";

    [JsonPropertyName("x")]
    public int X { get; set; }

    [JsonPropertyName("y")]
    public int Y { get; set; }

    [JsonPropertyName("locationName")]
    public string? LocationName { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("enemySpawns")]
    public List<string>? EnemySpawns { get; set; } = new();

    [JsonPropertyName("itemSpawnChance")]
    public double? ItemSpawnChance { get; set; }

    [JsonPropertyName("events")]
    public List<string>? Events { get; set; } = new();

    [JsonPropertyName("cardinalOnly")]
    public bool CardinalOnly { get; set; } = false;

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("directions")]
    public Dictionary<string, DirectionTarget> Directions { get; set; } = new();
    [JsonPropertyName("locationImage")]
    public string? LocationImage { get; set; }

    [JsonPropertyName("enemyImage")]
    public string? EnemyImage { get; set; }

}

public class DirectionTarget
{
    [JsonPropertyName("mapId")]
    public string MapId { get; set; }

    [JsonPropertyName("x")]
    public int X { get; set; }

    [JsonPropertyName("y")]
    public int Y { get; set; }
}

public class DirectionLink
{
    public string? Label { get; set; }           // "Enter Shop"
    public string? Direction { get; set; }       // "shop" or "north"
    public PlayerPosition Target { get; set; }
    public string RequiredStat { get; set; }    // e.g., "Strength"
    public int MinValue { get; set; }           // e.g., 10
    public string RequiredFlag { get; set; }    // e.g., "HasShopKey"
}