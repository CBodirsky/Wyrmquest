using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Wyrmquest.Services;
using Wyrmquest.Models;

namespace Wyrmquest.Pages.Game
{
    public class IndexModel : PageModel
    {
        private const string PositionKey = "PlayerPosition";

        public PlayerStats Stats { get; set; } = PlayerDefaults.GetStats();
        public PlayerPosition Position { get; set; } = PlayerDefaults.GetPosition();
        public int PlayerHealth { get; set; } = 100;
        public int PlayerXP { get; set; } = 0;
        public int PlayerGold { get; set; } = 50;
        public string GameMessage { get; set; }
        public string ChatLog { get; set; } = "Welcome to Wyrmquest.";
        public List<DirectionLink> AvailableDirections { get; set; } = new();
        public string CurrentMapId => Position.MapId;
        public int PlayerX => Position.X;
        public int PlayerY => Position.Y;
        public LocationTile CurrentTile => MapService.GetTile(Position.MapId, Position.X, Position.Y);

        public List<DirectionOption> CompassDirections { get; set; } = new();
        public List<SpecialAction> SpecialActions { get; set; } = new();

        private void RefreshGameState(string view = "")
        {
            EnsureMapLoaded();
            UpdateGameMessage(view);
            UpdateAvailableDirections();
            UpdateCompassDirections();
            UpdateSpecialActions();
        }

        public void OnGet()
        {
            var view = Request.Query["view"];
            var direction = Request.Query["move"];

            LoadPositionFromSession();
            var mapPath = Path.Combine(AppContext.BaseDirectory, "Data", "Maps", $"{Position.MapId}.json");
            MapService.LoadMap(mapPath);


            if (!string.IsNullOrEmpty(direction))
            {
                MovePlayer(direction);
                //CheckForMapTransition();
                SavePositionToSession();

                ChatLog = $"System: You moved {direction}.<br />" + ChatLog;
            }

            RefreshGameState(view);
        }

        public void OnPost()
        {
            var direction = Request.Form["direction"];

            LoadPositionFromSession();

            if (!string.IsNullOrEmpty(direction))
            {
                MovePlayer(direction);
                SavePositionToSession();

                ChatLog = $"System: You moved {direction}.<br />" + ChatLog;
            }

            RefreshGameState();
        }

        private void UpdateCompassDirections()
        {
            var available = MapService.GetAvailableDirections(Position.MapId, Position.X, Position.Y);

            CompassDirections = DirectionSet.CompassOrder.Select(dir =>
            {
                if (dir == " ")
                {
                    return new DirectionOption("You", " ", false);
                }

                var label = DirectionSet.Labels[dir];
                var canonical = DirectionSet.CanonicalDirectionMap[dir];

                var isAvailable = available.ContainsKey(canonical);

                return new DirectionOption(canonical, label, isAvailable);
            }).ToList();
        }

        private void UpdateSpecialActions()
        {
            SpecialActions = CurrentTile?.Directions?
                .Where(kvp => !DirectionSet.CanonicalDirectionMap.Values.Contains(kvp.Key))
                .Select(kvp => new SpecialAction(kvp.Key, kvp.Key))
                .ToList() ?? new();
        }

        private void LoadPositionFromSession()
        {
            var posJson = HttpContext.Session.GetString(PositionKey);

            if (string.IsNullOrEmpty(posJson))
            {
                Position = PlayerDefaults.GetPosition();
                return;
            }

            Position = JsonSerializer.Deserialize<PlayerPosition>(posJson) ?? PlayerDefaults.GetPosition();
            Console.WriteLine($"Loaded position: {Position.MapId} ({Position.X}, {Position.Y})");

        }


        private void SavePositionToSession()
        {
            HttpContext.Session.SetString(PositionKey, JsonSerializer.Serialize(Position));
        }

        private void EnsureMapLoaded()
        {
            if (!MapService.HasLocation(Position.MapId, Position.X, Position.Y))
            {
                ChatLog = $"System: Invalid transition target ({Position.MapId}, {Position.X}, {Position.Y}).<br />" + ChatLog;
                Position = PlayerDefaults.GetPosition();
            }
        }

        private void UpdateGameMessage(string view = "")
        {
            switch (view)
            {
                case "stats":
                    GameMessage = $"Health: {PlayerHealth}, XP: {PlayerXP}, Gold: {PlayerGold}";
                    break;
                case "inventory":
                    GameMessage = "Your inventory is empty.";
                    break;
                case "settings":
                    GameMessage = "Settings are not yet implemented.";
                    break;
                default:
                    var tile = MapService.GetTile(Position.MapId, Position.X, Position.Y);
                    GameMessage = tile != null
                        ? $"{tile.LocationName}: {tile.Description}"
                        : "You can't go that way.";
                    break;
            }
        }

        private bool IsCardinal(string dir) =>
            new[] { "north", "south", "east", "west" }
            .Contains(dir.ToLower());

        private void UpdateAvailableDirections()
        {
            var currentTile = MapService.GetTile(Position.MapId, Position.X, Position.Y);
            var allowDiagonals = !currentTile.CardinalOnly;
            if (currentTile?.Directions == null)
            {
                AvailableDirections = new();
                return;
            }

            var named = currentTile.Directions
                .Where(d => !IsCardinal(d.Key))
                .OrderBy(d => d.Key)
                .Select(d => new DirectionLink
                {
                    Label = d.Key,
                    Direction = d.Key,
                    Target = new PlayerPosition
                    {
                        MapId = d.Value.MapId,
                        X = d.Value.X,
                        Y = d.Value.Y
                    }
                });

            var cardinal = currentTile.Directions
                .Where(d => IsCardinal(d.Key))
                .OrderBy(d => d.Key)
                .Select(d => new DirectionLink
                {
                    Label = d.Key,
                    Direction = d.Key,
                    Target = new PlayerPosition
                    {
                        MapId = d.Value.MapId,
                        X = d.Value.X,
                        Y = d.Value.Y
                    }
                });

            AvailableDirections = named.Concat(cardinal).ToList();
        }

        private void MovePlayer(string direction)
        {
            var currentTile = MapService.GetTile(Position.MapId, Position.X, Position.Y);

            // 1. Try named direction (special action)
            if (currentTile?.Directions != null)
            {
                var match = currentTile.Directions
                    .FirstOrDefault(kvp => kvp.Key.Equals(direction, StringComparison.OrdinalIgnoreCase));

                if (!match.Equals(default(KeyValuePair<string, DirectionTarget>)))
                {
                    Position.MapId = match.Value.MapId;
                    Position.X = match.Value.X;
                    Position.Y = match.Value.Y;

                    MapService.LoadMap($"Data/Maps/{Position.MapId}.json");
                    return;
                }
            }

            // 2. Try compass movement
            var offset = DirectionSet.All
                .FirstOrDefault(d => d.name.Equals(direction, StringComparison.OrdinalIgnoreCase));

            if (offset != default)
            {
                var newX = Position.X + offset.dx;
                var newY = Position.Y + offset.dy;
                if (MapService.HasLocation(Position.MapId, newX, newY))
                {
                    Position.X = newX;
                    Position.Y = newY;
                    return;
                }
            }
            // 3. Fallback message
            ChatLog = $"System: You can't go {direction}.<br />" + ChatLog;
        }
    }
}
