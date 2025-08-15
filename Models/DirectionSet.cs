namespace Wyrmquest.Models
{
    public static class DirectionSet
    {
        //N,S,E,W coordinate movement list
        public static readonly List<(string name, int dx, int dy)> Cardinal = new()
    {
        ("North", 0, 1),
        ("South", 0, -1),
        ("East", 1, 0),
        ("West", -1, 0)
    };
        //Includes diagonal movements
        public static readonly List<(string name, int dx, int dy)> All = new()
    {
        ("North", 0, 1),
        ("South", 0, -1),
        ("East", 1, 0),
        ("West", -1, 0),
        ("NorthEast", 1, 1),
        ("NorthWest", -1, 1),
        ("SouthEast", 1, -1),
        ("SouthWest", -1, -1)
    };
        //Ordering of how the Compass is displayed
        public static readonly List<string> CompassOrder = new()
        {
            "NW", "North", "NE",
            "West", " ",  "East",
            "SW", "South", "SE"
        };

        //Labels links and associates to positioning above
        public static readonly Dictionary<string, string> Labels = new()
        {
            ["NW"] = "NW",
            ["North"] = "North",
            ["NE"] = "NE",
            ["West"] = "West",
            [" "] = " ",
            ["East"] = "East",
            ["SW"] = "SW",
            ["South"] = "South",
            ["SE"] = "SE"
        };

        //Relation table for actual navigation coding. Not abbreviation for diagonals
        public static readonly Dictionary<string, string> CanonicalDirectionMap = new()
        {
            ["NW"] = "NorthWest",
            ["NE"] = "NorthEast",
            ["SW"] = "SouthWest",
            ["SE"] = "SouthEast",
            ["North"] = "North",
            ["South"] = "South",
            ["East"] = "East",
            ["West"] = "West"
        };
    }

    public class DirectionOption
        {
            public string Direction { get; set; }
            public string Label { get; set; }
            public bool IsAvailable { get; set; }

            public DirectionOption(string direction, string label, bool isAvailable)
            {
                Direction = direction;
                Label = label;
                IsAvailable = isAvailable;
            }
        }

        public class SpecialAction
        {
            public string Action { get; set; }
            public string Label { get; set; }

            public SpecialAction(string action, string label)
            {
                Action = action;
                Label = label;
            }
        }
    }

