namespace Wyrmquest.Models
{
    public class PlayerStats
    {
        public int Strength { get; set; } = 1;
        public int Dexterity { get; set; } = 1;
        public int Agility { get; set; } = 1;
        public int Evasion { get; set; } = 1;
        public int Intelligence { get; set; } = 1;
        public int Wisdom { get; set; } = 1;
        public int Luck { get; set; } = 1;

        public int Health { get; set; } = 100;
        public int Mana { get; set; } = 50;
    }
    public class PlayerPosition
    {
        public string MapId { get; set; } = "valley";
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
    }
}
