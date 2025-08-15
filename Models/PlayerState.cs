namespace Wyrmquest.Models
{
    public class PlayerStats
    {
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Agility { get; set; }
        public int Evasion { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Luck { get; set; }

        public int Health { get; set; }
        public int Mana { get; set; }
    }

    public class PlayerPosition
    {
        public string MapId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
