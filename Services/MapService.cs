using System.Text.Json;

namespace Wyrmquest.Services
{
    public static class MapService
    {
        public static Dictionary<(string mapId, int x, int y), LocationTile> MapTiles { get; private set; }

        public static void LoadMap(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var tiles = JsonSerializer.Deserialize<List<LocationTile>>(json);
            MapTiles = tiles.ToDictionary(t => (t.MapId, t.X, t.Y), t => t);
        }

        public static LocationTile GetTile(string mapId, int x, int y)
        {
            return MapTiles.TryGetValue((mapId, x, y), out var tile) ? tile : null;
        }

        public static bool HasLocation(string mapId, int x, int y)
        {
            return MapTiles.ContainsKey((mapId, x, y));
        }

        public static Dictionary<string, (int x, int y)> GetAvailableDirections(string mapId, int currentX, int currentY)
        {
            var directions = new Dictionary<string, (int dx, int dy)>
            {
                { "North", (0, 1) },
                { "South", (0, -1) },
                { "East",  (1, 0) },
                { "West",  (-1, 0) }
            };

            var available = new Dictionary<string, (int x, int y)>();

            foreach (var dir in directions)
            {
                int newX = currentX + dir.Value.dx;
                int newY = currentY + dir.Value.dy;

                if (HasLocation(mapId, newX, newY))
                {
                    available[dir.Key] = (newX, newY);
                }
            }

            return available;
        }
    }
}
