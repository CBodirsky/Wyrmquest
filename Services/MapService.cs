using System.Text.Json;
using Wyrmquest.Models;

namespace Wyrmquest.Services
{
    public static class MapService
    {
        public static Dictionary<string, Dictionary<(int x, int y), LocationTile>> AllMaps { get; private set; } = new();

        //Call JSON map files
        public static void LoadMap(string filePath)
        {
            Console.WriteLine($"[DEBUG] LoadMap received path: {filePath}");

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Map file not found: {filePath}");
                return;
            }

            var json = File.ReadAllText(filePath);
            var tiles = JsonSerializer.Deserialize<List<LocationTile>>(json);

            if (tiles != null && tiles.Any())
            {
                var mapId = tiles.First().MapId;
                AllMaps[mapId] = tiles.ToDictionary(t => (t.X, t.Y), t => t);
            }
            else
            {
                Console.WriteLine($"Map file loaded but contains no tiles: {filePath}");
            }
        }

        //Get tile information from map
        public static LocationTile GetTile(string mapId, int x, int y)
        {
            return AllMaps.TryGetValue(mapId, out var map) && map.TryGetValue((x, y), out var tile)
                ? tile
                : null;
        }

        public static bool HasLocation(string mapId, int x, int y)
        {
            return AllMaps.TryGetValue(mapId, out var map) && map.ContainsKey((x,y));
        }

        //Handles checking for neighbor tiles and navigation rules. Including non-cardinal movements
        public static Dictionary<string, (int x, int y)> GetAvailableDirections(string mapId, int currentX, int currentY)
        {
            var available = new Dictionary<string, (int x, int y)>();

            var currentTile = GetTile(mapId, currentX, currentY);
            if (currentTile == null) return available;

            // Choose direction set
            var directionsToCheck = currentTile.CardinalOnly
                ? DirectionSet.Cardinal
                : DirectionSet.All;

            // 1. Neighbor-based movement
            foreach (var dir in directionsToCheck)
            {
                int newX = currentX + dir.dx;
                int newY = currentY + dir.dy;

                if (HasLocation(mapId, newX, newY))
                {
                    available[dir.name] = (newX, newY);
                }
            }

            // 2. Manual directions
            if (currentTile.Directions != null)
            {
                foreach (var kvp in currentTile.Directions)
                {
                    available[kvp.Key] = (kvp.Value.X, kvp.Value.Y);
                }
            }
            return available;
        }
    }
}
