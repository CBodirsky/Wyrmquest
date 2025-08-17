using Wyrmquest.Models;

namespace Wyrmquest.Models
{
    public static class PlayerDefaults
    {
        public static PlayerStats GetStats() => new()
        {
            Strength = 1,
            Dexterity = 1,
            Agility = 1,
            Evasion = 1,
            Intelligence = 1,
            Wisdom = 1,
            Luck = 1,
            Health = 100,
            Mana = 50
        };

        public static PlayerPosition GetPosition() => new()
        {
            MapId = "Town",
            X = 0,
            Y = 0
        };
    }
}