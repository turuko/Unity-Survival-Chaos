using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Survival Chaos/Race")]
public class RaceData : ScriptableObject
{
    public string Name;
    public List<UnitData> Units;
}
