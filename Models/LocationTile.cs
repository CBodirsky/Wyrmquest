using System.Text.Json.Serialization;

public class LocationTile
{
    [JsonPropertyName("mapId")]
    public string MapId { get; set; }

    [JsonPropertyName("x")]
    public int X { get; set; }

    [JsonPropertyName("y")]
    public int Y { get; set; }

    [JsonPropertyName("locationName")]
    public string LocationName { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("enemySpawns")]
    public List<string> EnemySpawns { get; set; }

    [JsonPropertyName("itemSpawnChance")]
    public double ItemSpawnChance { get; set; }

    [JsonPropertyName("events")]
    public List<string> Events { get; set; }
}
