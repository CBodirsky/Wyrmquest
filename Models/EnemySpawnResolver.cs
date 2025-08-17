public class EnemySpawnResolver
{
    private readonly EnemyRepository _repository;
    private readonly Random _rng = new();

    public EnemySpawnResolver(EnemyRepository repository)
    {
        _repository = repository;
    }


    public Enemy? Resolve(MapTile tile, Region region)
    {
        Console.WriteLine($"[DEBUG] Attempting spawn at ({tile.X},{tile.Y}) - Region: {tile.RegionName}");

        if (region?.EnemySpawns == null || region.EnemySpawns.Count == 0)
        {
            Console.WriteLine($"[SPAWN] No spawn data for region '{tile.RegionName}'");
            return null;
        }

        if (_rng.NextDouble() < region.SpawnChance)
        {
            var weightedList = BuildWeightedList(region.EnemySpawns);
            if (weightedList.Count == 0)
            {
                Console.WriteLine($"[SPAWN] No valid enemies to spawn in region '{tile.RegionName}'");
                return null;
            }

            var name = weightedList[_rng.Next(weightedList.Count)];
            var enemy = _repository.GetByName(name);

            Console.WriteLine($"[SPAWN] Tile ({tile.X},{tile.Y}) in {tile.RegionName} spawned: {enemy?.Name ?? "None"}");
            return enemy;
        }

        Console.WriteLine($"[SPAWN] No spawn triggered at ({tile.X},{tile.Y}) due to spawnChance");
        return null;
    }

    private List<string> BuildWeightedList(List<EnemySpawn> spawns)
    {
        var list = new List<string>();
        foreach (var spawn in spawns)
        {
            for (int i = 0; i < spawn.Weight; i++)
                list.Add(spawn.Name);
        }
        return list;
    }
}
