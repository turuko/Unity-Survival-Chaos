using System.Collections.Generic;

namespace Data
{
    public class Race
    {
        public string Name;

        public List<UnitGameData> Units;

        public Race(RaceData data)
        {
            Name = data.Name;

            Units = new List<UnitGameData>();
            foreach (var unitData in data.Units)
            {
                Units.Add(new UnitGameData(unitData));
            }
        }
    }
}