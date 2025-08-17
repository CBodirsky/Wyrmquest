using System.Text.Json;

public class EnemyRepository
{
    private readonly Dictionary<string, Enemy> _enemyDatabase;

    public EnemyRepository(string jsonPath)
    {
        var json = File.ReadAllText(jsonPath);
        var enemies = JsonSerializer.Deserialize<List<Enemy>>(json);
        _enemyDatabase = (enemies ?? Enumerable.Empty<Enemy>())
            .Where(e => !string.IsNullOrWhiteSpace(e?.Name))
            .ToDictionary(e => e.Name, StringComparer.OrdinalIgnoreCase);
    }

    public Enemy GetByName(string name)
    {
        if (_enemyDatabase.TryGetValue(name, out var enemy))
        {
            Console.WriteLine($"[ENEMY] Found enemy '{name}'");
            return enemy;
        }

        Console.WriteLine($"[ENEMY] Enemy '{name}' not found in repository");
        return null;
    }

}
