using System.Text.Json;
using Wyrmquest.Models;

namespace Wyrmquest.Services
{
    public class MapService
    {
        private readonly Dictionary<string, Dictionary<(int x, int y), MapTile>> _allMaps = new();
        private readonly Dictionary<string, MapData> _mapMetadata = new();
        private readonly EnemySpawnResolver _enemyResolver;

        public MapService(EnemySpawnResolver enemyResolver)
        {
            _enemyResolver = enemyResolver;
        }

        public MapData GetMap(string mapId)
        {
            return _mapMetadata.TryGetValue(mapId, out var map) ? map : null;
        }

        // Load all maps from given file paths
        public void LoadAllMaps(IEnumerable<string> filePaths)
        {
            foreach (var path in filePaths)
                LoadMap(path);
        }

        // Load a single map from JSON
        public void LoadMap(string filePath)
        {
            Console.WriteLine($"[DEBUG] LoadMap received path: {filePath}");

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Map file not found: {filePath}");
                return;
            }

            var json = File.ReadAllText(filePath);
            var mapData = JsonSerializer.Deserialize<MapData>(json);

            if (mapData?.Tiles != null && mapData.Tiles.Any())
            {
                _allMaps[mapData.MapId] = mapData.Tiles.ToDictionary(t => (t.X, t.Y), t => t);
                _mapMetadata[mapData.MapId] = mapData;
            }
            else
            {
                Console.WriteLine($"Map file loaded but contains no tiles: {filePath}");
            }
        }

        // Get tile at specific coordinates
        public MapTile GetTile(string mapId, int x, int y)
        {
            return _allMaps.TryGetValue(mapId, out var tileDict) &&
                   tileDict.TryGetValue((x, y), out var tile)
                ? tile
                : null;
        }

        // Check if a location exists
        public bool HasLocation(string mapId, int x, int y)
        {
            return _allMaps.TryGetValue(mapId, out var map) && map.ContainsKey((x, y));
        }

        // Get available directions from a tile
        public Dictionary<string, (int x, int y)> GetAvailableDirections(string mapId, int currentX, int currentY)
        {
            var available = new Dictionary<string, (int x, int y)>();
            var currentTile = GetTile(mapId, currentX, currentY);
            var map = _mapMetadata.TryGetValue(mapId, out var m) ? m : null;

            if (currentTile == null || map == null) return available;

            bool cardinalOnly = currentTile.CardinalOnly == true || map.CardinalOnly == true;
            var directionsToCheck = cardinalOnly ? DirectionSet.Cardinal : DirectionSet.All;

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
            Console.WriteLine($"Tile: {currentTile.LocationName} ({currentTile.X},{currentTile.Y})");
            if (currentTile.Directions != null)
            {
                foreach (var kvp in currentTile.Directions)
                {
                    Console.WriteLine($"Direction: {kvp.Key} → Map: {kvp.Value.MapId} ({kvp.Value.X},{kvp.Value.Y})");
                }
            }
            else
            {
                Console.WriteLine("Directions is null");
            }

            return available;
        }

        // Get region data (optional helper)
        public Region GetRegion(string mapId, string regionName)
        {
            if (_mapMetadata.TryGetValue(mapId, out var map) &&
                map.Regions != null &&
                map.Regions.TryGetValue(regionName, out var region))

            {
                foreach (var key in _mapMetadata[mapId].Regions.Keys)
                {
                    Console.WriteLine($"[DEBUG] Region key in map '{mapId}': '{key}'");
                }
                return region;
            }

            return null;
        }


        // Resolve enemy spawn
        public Enemy GetEnemy(MapTile tile, Region region)
        {
            return _enemyResolver.Resolve(tile, region);
        }
    }
}
