// I hate to say this, but Index...you're getting a little fat. We need to slim you down soon.
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

        public PlayerStats Stats { get; set; } = PlayerDefaults.GetStats(); //Placeholder for PlayerStats, remove later.
        public PlayerPosition Position { get; set; } = PlayerDefaults.GetPosition();
        public int PlayerHealth { get; set; } = 100; //Placeholder along with XP and Gold, remove later.
        public int PlayerXP { get; set; } = 0;
        public int PlayerGold { get; set; } = 50;
        public string GameMessage { get; set; }
        public string ChatLog { get; set; } = "Welcome to Wyrmquest.";
        public List<DirectionLink> AvailableDirections { get; set; } = new();
        public string CurrentMapId => Position.MapId;
        public MapData CurrentMap { get; set; }

        public Enemy SpawnedEnemy { get; set; }
        public MapTile CurrentTile =>
            CurrentMap?.Tiles.FirstOrDefault(t => t.X == Position.X && t.Y == Position.Y);
        public List<DirectionOption> CompassDirections { get; set; } = new();
        public List<SpecialAction> SpecialActions { get; set; } = new();
        public readonly MapService _mapService;
        public readonly EnemySpawnResolver _enemyService;

        public IndexModel(MapService mapService, EnemySpawnResolver enemyService) //Consider refactoring soon
        {
            _mapService = mapService;
            _enemyService = enemyService;
        }

        private void RefreshGameState(string view = "")
        {
            EnsureMapLoaded();
            UpdateGameMessage(view);
            UpdateSpecialActions();
            UpdateAvailableDirections();
            UpdateCompassDirections();
            
        }

        public void OnGet()
        {
            var view = Request.Query["view"];
            var direction = Request.Query["move"];

            LoadPositionFromSession();

            LoadAndAssignMap(Position.MapId); 
            ResolveCurrentTile();             

            if (!string.IsNullOrEmpty(direction))
            {
                UpdateSpecialActions(); //revisit this and OnPost entry. Required to have non-cardinal directions, but not sure this is a good way to handle it.

                MovePlayer(direction);
                SavePositionToSession();

                ChatLog = $"System: You moved {direction}.<br />" + ChatLog;

                LoadAndAssignMap(Position.MapId);
                ResolveCurrentTile();
            }

            var region = _mapService.GetRegion(Position.MapId, CurrentTile?.RegionName);
            SpawnedEnemy = _enemyService.Resolve(CurrentTile, region);
            if (SpawnedEnemy != null)
            {
                CurrentTile.EnemyImage = SpawnedEnemy.ImageName;
            }

            RefreshGameState(view);
        }

        public void OnPost()
        {
            var direction = Request.Form["direction"];

            LoadPositionFromSession();

            LoadAndAssignMap(Position.MapId);
            ResolveCurrentTile();

            if (!string.IsNullOrEmpty(direction))
            {
                UpdateSpecialActions();

                MovePlayer(direction);
                SavePositionToSession();

                ChatLog = $"System: You moved {direction}.<br />" + ChatLog;

                LoadAndAssignMap(Position.MapId);
                ResolveCurrentTile();
            }

            var region = _mapService.GetRegion(Position.MapId, CurrentTile?.RegionName);
            SpawnedEnemy = _enemyService.Resolve(CurrentTile, region);

            RefreshGameState();
        }

        private void LoadAndAssignMap(string mapId)
        {
            var mapPath = Path.Combine(AppContext.BaseDirectory, "Data", "Maps", $"{mapId}.json");
            _mapService.LoadMap(mapPath);
            CurrentMap = _mapService.GetMap(mapId);
        }

        private void ResolveCurrentTile()
        {
            var tile = CurrentMap?.Tiles.FirstOrDefault(t => t.X == Position.X && t.Y == Position.Y);
        }


        private void UpdateSpecialActions()
        {
            SpecialActions = CurrentTile?.Directions?
                .Where(kvp => !DirectionSet.CanonicalDirectionMap.Values.Contains(kvp.Key.ToLower()))
                .Select(kvp => new SpecialAction(kvp.Key.Trim(), kvp.Key.Trim(), kvp.Value))
                .ToList() ?? new();
        }

        private void UpdateCompassDirections()
        {
            var available = _mapService.GetAvailableDirections(Position.MapId, Position.X, Position.Y);

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
            if (!_mapService.HasLocation(Position.MapId, Position.X, Position.Y))
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
                    var tile = _mapService.GetTile(Position.MapId, Position.X, Position.Y);
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
            var CurrentTile = _mapService.GetTile(Position.MapId, Position.X, Position.Y);
            //Console.WriteLine($"Directions CurrentTile: {CurrentTile?.LocationName} at ({CurrentTile?.X},{CurrentTile?.Y})");
            var allowDiagonals = !CurrentTile.CardinalOnly;
            if (CurrentTile?.Directions == null)
            {
                AvailableDirections = new();
                return;
            }

            var named = CurrentTile.Directions
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

            var cardinal = CurrentTile.Directions
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
            var CurrentTile = _mapService.GetTile(Position.MapId, Position.X, Position.Y);
            Console.WriteLine($"MovePlayer CurrentTile: {CurrentTile?.LocationName} at ({CurrentTile?.X},{CurrentTile?.Y})");

            // 1. Try named direction (special action)
            var special = SpecialActions.FirstOrDefault(s => s.Action == direction);
            if (special != null)
            {
                Position.MapId = special.Target.MapId;
                Position.X = special.Target.X;
                Position.Y = special.Target.Y;
                _mapService.LoadMap(Path.Combine(AppContext.BaseDirectory, "Data", "Maps", $"{Position.MapId}.json"));
                CurrentMap = _mapService.GetMap(Position.MapId);
                var tile = CurrentMap?.Tiles.FirstOrDefault(t => t.X == Position.X && t.Y == Position.Y);
                Console.WriteLine($"[DEBUG] After jump: Tile = {tile?.LocationName} at ({tile?.X},{tile?.Y})");
                return;
            }

            // 2. Try compass movement
            var offset = DirectionSet.All
                .FirstOrDefault(d => d.name.Equals(direction, StringComparison.OrdinalIgnoreCase));
            if (offset != default)
            {
                var newX = Position.X + offset.dx;
                var newY = Position.Y + offset.dy;
                if (_mapService.HasLocation(Position.MapId, newX, newY))
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
    public class SpecialAction
    {
        public string Label { get; set; }
        public string Action { get; set; }
        public DirectionTarget Target { get; set; }
        public SpecialAction(string label, string action, DirectionTarget target)
        {
            Label = label;
            Action = action;
            Target = target;
        }
    }
}
