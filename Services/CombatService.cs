//public static class CombatService
//{
//    public static string GenerateCombatMessage(string attacker, List<int> damageValues)
//    {
//        if (damageValues.Count <= 5)
//            return string.Join("\n", damageValues.Select(d => $"{attacker} hits for {d} damage!"));
//        else
//        {
//            int total = damageValues.Sum();
//            return $"{attacker} hits {damageValues.Count} times for {total} damage!";
//        }
//    }


//    //Revisit this. Just a placeholder attack system
//    int CalculateEnemyAttack(int baseAttack)
//    {
//        Random rand = new Random();
//        double variance = baseAttack * 0.5;
//        double min = baseAttack - variance;
//        double max = baseAttack + variance;

//        // Miss chance
//        if (rand.NextDouble() < 0.15) return 0;

//        // Roll damage
//        int damage = (int)Math.Round(min + rand.NextDouble() * (max - min));

//        // Critical hit
//        if (damage >= max - 1) damage = (int)(damage * 1.5);

//        return damage;
//    }

//    // Future expansion:
//    // public static int CalculateDamage(Player attacker, Enemy target) { ... }
//    // public static void ApplyStatusEffects(Entity entity) { ... }
//}
