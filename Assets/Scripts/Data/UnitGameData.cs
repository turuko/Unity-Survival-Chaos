namespace Data
{
    public class UnitGameData
    {
        public string Name;
        public UnitType Type;
        public AttackType AttackType;
        public float attackRange;
        public float aggroRange;
        public float unitRadius;
        public float maxHealth;
        public float damage;
        public float attacksPerSecond;

        public UnitGameData(UnitData data)
        {
            Name = data.name;
            Type = data.Type;
            AttackType = data.AttackType;
            attackRange = data.attackRange;
            aggroRange = data.aggroRange;
            maxHealth = data.maxHealth;
            damage = data.damage;
            attacksPerSecond = data.attacksPerSecond;
        }
    }
}