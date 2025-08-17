using System.Text.Json.Serialization;
using Wyrmquest.Models;



public class MapTile
{
    public string GetLocationImagePath() => $"/images/location/{LocationImage}";
    public string GetEnemyImagePath() => $"/images/enemy/{EnemyImage}";
    public string GetItemImagePath(string imageName) => $"/images/items/{imageName}";

    //[JsonPropertyName("mapId")]
    //public string MapId { get; set; } = "Town";

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
    public bool? CardinalOnly { get; set; } = false;

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("directions")]
    public Dictionary<string, DirectionTarget> Directions { get; set; } = new();
    [JsonPropertyName("locationImage")]
    public string? LocationImage { get; set; }

    [JsonPropertyName("enemyImage")]
    public string? EnemyImage { get; set; }
    [JsonPropertyName("items")]
    public List<GameItem> Items { get; set; } = new();
    public double EnemyChance { get; set; } = 0.0;
    [JsonPropertyName("region")]
    public string RegionName { get; set; }


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

public class MapData
{
    [JsonPropertyName("mapId")]
    public string MapId { get; set; }

    [JsonPropertyName("cardinalOnly")]
    public bool? CardinalOnly { get; set; }

    [JsonPropertyName("regions")]
    public Dictionary<string, Region> Regions {get; set; }

    [JsonPropertyName("tiles")]
    public List<MapTile> Tiles { get; set; } = new();
}
public class Enemy
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("imagePath")]
    public string ImagePath { get; set; }
    [JsonPropertyName("imageName")]
    public string ImageName { get; set; }
    [JsonPropertyName("region")]
    public string Region { get; set; } //Mainly for spawn weights
}

public class Region
{
    [JsonPropertyName("spawnChance")]
    public double SpawnChance { get; set; }
    [JsonPropertyName("enemySpawns")]
    public List<EnemySpawn> EnemySpawns { get; set; }
}

public class EnemySpawn
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("weight")]
    public int Weight { get; set; }
}
