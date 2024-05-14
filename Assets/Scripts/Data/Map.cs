using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data
{
    public class Map
    {
        public List<Building> AllBuildingsOnMap { get; private set; }

        //public NeutralBuilding[] neutralBuildings { get; private set; } = new NeutralBuilding[8];

        public Vector2[] BasePositions;
        public Vector2[] LaneMidpoints;
        public List<Vector2> PlayerPositions = new List<Vector2>();
        public List<Unit> AllUnits = new List<Unit>();

        public int Width { get; private set; }
        public int Height { get; private set; }
        
        public int LaneWidth { get; private set; }

        public Func<Barracks, UnitGameData, Player, Vector3, Unit> onUnitSpawned;
        public Map(/*NeutralBuilding[] neutralBuildings,*/ int height, int width, int laneWidth)
        {
            /*if (neutralBuildings.Length != this.neutralBuildings.Length)
            {
                throw new ArgumentException("neutralBuildings array not correct size", nameof(neutralBuildings));
            }*/

            Height = height;
            Width = width;
            LaneWidth = laneWidth;
            BasePositions = new Vector2[GameManager.Instance.Players.Capacity];
            LaneMidpoints = new Vector2[5];
            //this.neutralBuildings = neutralBuildings;
            AllBuildingsOnMap = new List<Building>();
            SetupMap();
            SetupLaneMidpoints();
        }

        private void SetupLaneMidpoints()
        {
            LaneMidpoints[0] = new Vector2(LaneWidth / 2f, LaneWidth / 2f);
            LaneMidpoints[1] = new Vector2(LaneWidth/2f, Width - LaneWidth/2f);
            LaneMidpoints[2] = new Vector2(Height - LaneWidth/2f, Width - LaneWidth/2f);
            LaneMidpoints[3] = new Vector2(Height - LaneWidth / 2f, LaneWidth /2f);
            LaneMidpoints[4] = new Vector2(Width / 2f, Height / 2f);
        }

        private void SetupMap()
        {
            /*neutralBuildings[0].SetPosition( new Vector2(Height * 0.25f, LaneWidth));
            neutralBuildings[1].SetPosition( new Vector2(Height * 0.75f, LaneWidth));
            neutralBuildings[2].SetPosition( new Vector2(Height * 0.75f, Width - LaneWidth));
            neutralBuildings[3].SetPosition( new Vector2(Height * 0.25f, Width - LaneWidth));
            neutralBuildings[4].SetPosition( new Vector2(LaneWidth, Width * 0.25f));
            neutralBuildings[5].SetPosition( new Vector2(LaneWidth, Width * 0.75f));
            neutralBuildings[6].SetPosition( new Vector2(Height - LaneWidth, Width * 0.75f));
            neutralBuildings[7].SetPosition( new Vector2(Height - LaneWidth, Width * 0.25f));
            */
            BasePositions[0] = new Vector2(Height / 2f , LaneWidth/2f);
            BasePositions[1] = new Vector2(Height / 2f, Width - LaneWidth/2f);
            BasePositions[2] = new Vector2(LaneWidth/2f, Width / 2f);
            BasePositions[3] = new Vector2(Height- LaneWidth/2f, Width / 2f);
            
            for (int i = 0; i < GameManager.Instance.Players.Count; i++)
            {
                var player = GameManager.Instance.Players[i];
                player.Castle.SetPosition(BasePositions[i]);
                player.transform.position = new Vector3(BasePositions[i].x, 0, BasePositions[i].y);
                PlayerPositions.Add(BasePositions[i]);
                AllBuildingsOnMap.Add(player.Castle);

                float halfSquareSize = LaneWidth * 0.5f;

                for (int j = 0; j < player.Towers.Length; j++)
                {
                    float angle = Mathf.PI / 4 + Mathf.PI / 2 * j;
                    float x = BasePositions[i].x + Mathf.Cos(angle) * halfSquareSize;
                    float y = BasePositions[i].y + Mathf.Sin(angle) * halfSquareSize;
                    player.Towers[j].SetPosition(new Vector2(x, y));
                }
                
                // Calculate the position of the first barracks
                Vector2 firstBarracksPos = BasePositions[i] + new Vector2(LaneWidth * 0.5f, -LaneWidth * 0.5f);
    
                // Calculate the vector from the castle to the center of the square
                Vector2 center = new Vector2(Height * 0.5f, Width * 0.5f);
                Vector2 castleToCenter = center - BasePositions[i];

                // Determine which direction the T should be facing
                Vector2 tDirection = castleToCenter.normalized;

                // Determine the position of the first barracks relative to the castle
                Vector2 relativeFirstBarracksPos = firstBarracksPos - BasePositions[i];
                float distanceFromCastle = relativeFirstBarracksPos.magnitude;
                Vector2 rotatedFirstBarracksPos = Quaternion.Euler(0, 0, 90) * tDirection * distanceFromCastle;
                Vector2 finalFirstBarracksPos = BasePositions[i] + rotatedFirstBarracksPos;

                // Determine the position of the second barracks relative to the castle
                Vector2 rotatedSecondBarracksPos = Quaternion.Euler(0, 0, -90) * tDirection * distanceFromCastle;
                Vector2 finalSecondBarracksPos = BasePositions[i] + rotatedSecondBarracksPos;

                // Determine the position of the third barracks relative to the castle
                Vector2 rotatedThirdBarracksPos = tDirection * distanceFromCastle;
                Vector2 finalThirdBarracksPos = BasePositions[i] + rotatedThirdBarracksPos;
                
                player.Barracks[0].SetPosition(finalFirstBarracksPos);
                player.Barracks[0].RegisterOnWaveSpawnedCallback(BarracksSpawn);
                
                player.Barracks[1].SetPosition(finalSecondBarracksPos);
                player.Barracks[1].RegisterOnWaveSpawnedCallback(BarracksSpawn);
                
                player.Barracks[2].SetPosition(finalThirdBarracksPos);
                player.Barracks[2].RegisterOnWaveSpawnedCallback(BarracksSpawn);


                AllBuildingsOnMap.AddRange(player.Towers);
                AllBuildingsOnMap.AddRange(player.Barracks);
            }
        }

        private void BarracksSpawn(Barracks barracks, List<UnitGameData> unitsToSpawn, Player player, Vector3 position)
        {
            foreach (var unitToSpawn in unitsToSpawn)
            {
                var unit = onUnitSpawned(barracks, unitToSpawn, player, position);
                AllUnits.Add(unit);
                unit.RegisterOnDestroyedCallback(x => AllUnits.Remove(unit));
            }   
        }


        public void RegisterOnUnitSpawnedCallback(Func<Barracks, UnitGameData, Player, Vector3, Unit> cb)
        {
            onUnitSpawned += cb;
        }
    }
}