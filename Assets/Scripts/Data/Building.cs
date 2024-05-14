using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Data
{
    public enum BuildingType
    {
        Castle,
        Tower,
        Barracks
    }
    
    public class Building
    {
        public BuildingType Type;
        public float MaxHealth;
        public float CurrentHealth;
        public float HealthRegen;
        public float Armor;

        public float Damage;
        public float AttackRange;
        public float AttackSpeed;

        public Vector2 Position;

        private Action<Building> onBuildingChanged;
        private Action<Building> onBuildingRemoved;

        protected Player player;

        public Player Player
        {
            get => player;
        }
        
        //DEBUG
        public string name;
        
        public Building(string name, float maxHealth, float healthRegen, float armor, float damage, float attackSpeed, float range, BuildingType type, Player player)
        {
            CurrentHealth = MaxHealth = maxHealth;
            HealthRegen = healthRegen;
            Armor = armor;
            Damage = damage;
            AttackSpeed = attackSpeed;
            AttackRange = range;
            Type = type;
            this.player = player;
            this.name = name;
        }

        public virtual void Update(float dt)
        {
            CurrentHealth += HealthRegen * dt;

            CurrentHealth = Mathf.Clamp(CurrentHealth,0, MaxHealth);
        }
        public void SetPosition(Vector2 position)
        {
            Position = position;
        }

        public void RegisterOnChangedCallback(Action<Building> cb)
        {
            onBuildingChanged += cb;
        }
        
        public void RegisterOnRemovedCallback(Action<Building> cb)
        {
            onBuildingRemoved += cb;
        }
        
        public void UnregisterOnChangedCallback(Action<Building> cb)
        {
            onBuildingChanged -= cb;
        }
        
        public void UnregisterOnRemovedCallback(Action<Building> cb)
        {
            onBuildingRemoved -= cb;
        }
    }

    public class Barracks : Building
    {
        private Queue<Vector2> path;

        public Queue<Vector2> Path
        {
            get => path;
        }
        
        private float spawnCooldown = 10f;
        private float timeSinceLastSpawn = 0f;
        
        private Action<Barracks, List<UnitGameData>, Player, Vector3> onWaveSpawned;
        
        public Barracks(string name, float maxHealth, float healthRegen, float armor, float damage, float attackSpeed, float range, BuildingType type, Player player) : base(name, maxHealth, healthRegen, armor, damage, attackSpeed, range, type, player)
        {
            
        }

        public void SetPath()
        {
            Queue<Vector2> newPath = new Queue<Vector2>();

            float closestDistance;
            Vector2 position = Vector2.negativeInfinity;
            Vector2 lastPosition = Position;

            void FindAndEnqueueClosestPoint(IEnumerable<Vector2> positions, Vector2 excludePoint)
            {
                closestDistance = float.PositiveInfinity;
                foreach (var point in positions)
                {
                    if (point == excludePoint)
                        continue;
                    if (!newPath.Contains(point) && Vector2.Distance(lastPosition, point) < closestDistance)
                    {
                        position = point;
                        closestDistance = Vector2.Distance(lastPosition, point);
                    }
                }
                if(position != Vector2.negativeInfinity && position != lastPosition)
                    newPath.Enqueue(position);
                lastPosition = position;
            }

            //First point in path
            FindAndEnqueueClosestPoint(GameManager.Instance.Map.LaneMidpoints, Vector2.negativeInfinity);
            //Second and last point if first point is the mid
            if (position == GameManager.Instance.Map.LaneMidpoints[4])
            {
                foreach (var basePosition in GameManager.Instance.Map.BasePositions)
                {
                    if (basePosition == player.Castle.Position)
                        continue;
                    if (Math.Abs(player.Castle.Position.x - basePosition.x) < 0.1f ||
                        Math.Abs(player.Castle.Position.y - basePosition.y) < 0.1f)
                    {
                        newPath.Enqueue(basePosition);
                        path = newPath;
                        return;
                    }
                }
            }
            //Second point
            FindAndEnqueueClosestPoint(GameManager.Instance.Map.BasePositions, player.Castle.Position);
            //Third point
            FindAndEnqueueClosestPoint(GameManager.Instance.Map.LaneMidpoints, GameManager.Instance.Map.LaneMidpoints[4]);
            //Fourth point
            FindAndEnqueueClosestPoint(GameManager.Instance.Map.BasePositions, player.Castle.Position);
            //Fifth point
            FindAndEnqueueClosestPoint(GameManager.Instance.Map.LaneMidpoints, GameManager.Instance.Map.LaneMidpoints[4]);
            //Sixth point
            FindAndEnqueueClosestPoint(GameManager.Instance.Map.BasePositions, player.Castle.Position);
            //Seventh point
            FindAndEnqueueClosestPoint(GameManager.Instance.Map.LaneMidpoints, GameManager.Instance.Map.LaneMidpoints[4]);

            path = newPath;
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            if (timeSinceLastSpawn <= 0f)
            {
                SpawnWave();
                timeSinceLastSpawn = spawnCooldown;
            }

            timeSinceLastSpawn -= dt;
        }
        
        void SpawnWave()
        {
            List<UnitGameData> unitsToSpawn = new List<UnitGameData>();
            for (int i = 0; i < 3; i++)
            {
                unitsToSpawn.Add(player.playerRace.Units[Random.Range(0, player.playerRace.Units.Count)]);
            }

            onWaveSpawned(this, unitsToSpawn, player, new Vector3(Position.x, 0, Position.y));
        }

        
        public void RegisterOnWaveSpawnedCallback(Action<Barracks, List<UnitGameData>, Player, Vector3> cb)
        {
            onWaveSpawned += cb;
        }
        
        public void UnregisterOnWaveSpawnedCallback(Action<Building, List<UnitGameData>, Player, Vector3> cb)
        {
            onWaveSpawned -= cb;
        }
    }

    public class Castle : Building
    {
        public float CurrentMana;
        public float MaxMana;
        public float ManaRegen;
        
        public Castle(string name, float maxHealth, float healthRegen, float armor, float damage, float attackSpeed, float range, BuildingType type, Player player, float maxMana, float manaRegen) : base(name, maxHealth, healthRegen, armor, damage, attackSpeed, range, type, player)
        {
            CurrentMana = MaxMana = maxMana;
            ManaRegen = manaRegen;
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            CurrentMana += ManaRegen * dt;

            CurrentMana = Mathf.Clamp(CurrentMana, MaxMana, 0);
        }
    }
}