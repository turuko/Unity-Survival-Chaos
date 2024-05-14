using Data;
using UnityEngine;

[CreateAssetMenu(menuName = "Survival Chaos/Units")]
public class UnitData : ScriptableObject
{
    public string Name;
    public UnitType Type;
    public AttackType AttackType;
    public float attackRange;
    public float aggroRange;
    public float maxHealth;
    public float damage;
    public float attacksPerSecond;
}