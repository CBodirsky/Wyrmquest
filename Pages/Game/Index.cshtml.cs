using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Wyrmquest.Services;
using Wyrmquest.Models;

namespace Wyrmquest.Pages.Game
{
    public class IndexModel : PageModel
    {
        public PlayerStats Stats { get; set; } = new();

        private const string PositionKey = "PlayerPosition";

        public int PlayerHealth { get; set; } = 100;
        public int PlayerXP { get; set; } = 0;
        public int PlayerGold { get; set; } = 50;

        public string GameMessage { get; set; }
        public string ChatLog { get; set; } = "Welcome to Wyrmquest.";
        public PlayerPosition Position { get; set; } = new();
        public Dictionary<string, (int x, int y)> AvailableDirections { get; set; } = new();

        public string CurrentMapId => Position.MapId;
        public int PlayerX => Position.X;
        public int PlayerY => Position.Y;

        public void OnGet()
        {
            var view = Request.Query["view"];
            var direction = Request.Query["move"];

            EnsureMapLoaded();
            LoadPositionFromSession();

            if (!string.IsNullOrEmpty(direction))
            {
                MovePlayer(direction);
                SavePositionToSession();
                ChatLog = $"System: You moved {direction}.<br />" + ChatLog;
            }

            UpdateGameMessage(view);
            UpdateAvailableDirections();
        }

        public void OnPost()
        {
            var direction = Request.Form["direction"];

            EnsureMapLoaded();
            LoadPositionFromSession();

            if (!string.IsNullOrEmpty(direction))
            {
                MovePlayer(direction);
                SavePositionToSession();
                ChatLog = $"System: You moved {direction}.<br />" + ChatLog;
            }

            UpdateGameMessage();
            UpdateAvailableDirections();
        }

        private void EnsureMapLoaded()
        {
            if (MapService.MapTiles == null)
            {
                MapService.LoadMap("Data/valley.json");
            }
        }

        private void LoadPositionFromSession()
        {
            var posJson = HttpContext.Session.GetString(PositionKey);
            Position = posJson != null
                ? JsonSerializer.Deserialize<PlayerPosition>(posJson)
                : new PlayerPosition { MapId = "valley", X = 0, Y = 0 };
        }

        private void SavePositionToSession()
        {
            HttpContext.Session.SetString(PositionKey, JsonSerializer.Serialize(Position));
        }

        private void UpdateGameMessage(string view = "")
        {
            if (view == "stats")
            {
                GameMessage = $"Health: {PlayerHealth}, XP: {PlayerXP}, Gold: {PlayerGold}";
            }
            else if (view == "inventory")
            {
                GameMessage = "Your inventory is empty.";
            }
            else if (view == "settings")
            {
                GameMessage = "Settings are not yet implemented.";
            }
            else
            {
                var tile = MapService.GetTile(Position.MapId, Position.X, Position.Y);
                GameMessage = tile != null
                    ? $"{tile.LocationName}: {tile.Description}"
                    : "You can't go that way.";
            }
        }

        private void UpdateAvailableDirections()
        {
            AvailableDirections = MapService.GetAvailableDirections(Position.MapId, Position.X, Position.Y);
        }

        private void MovePlayer(string direction)
        {
            switch (direction.ToLower())
            {
                case "north": Position.Y += 1; break;
                case "south": Position.Y -= 1; break;
                case "east": Position.X += 1; break;
                case "west": Position.X -= 1; break;
                case "northeast": Position.Y += 1; Position.X += 1; break;
                case "northwest": Position.Y += 1; Position.X -= 1; break;
                case "southeast": Position.Y -= 1; Position.X += 1; break;
                case "southwest": Position.Y -= 1; Position.X -= 1; break;
            }
        }
    }
}
