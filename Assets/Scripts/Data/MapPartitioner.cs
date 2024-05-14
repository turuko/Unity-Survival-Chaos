using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;

namespace Data
{


    public class MapPartitioner : MonoBehaviour
    {
        public static MapPartitioner Instance;

        private Dictionary<Rect, List<Unit>> grid = new Dictionary<Rect, List<Unit>>();
        private Dictionary<Unit, int> currentCell = new Dictionary<Unit, int>();

        private const float SPECIAL_SQUARE_RADIUS = 20f;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("More than one MapPartitioner!");
                return;
            }

            Instance = this;
        }

        public void Init()
        {
            grid.Add(new Rect(0, 0, GameManager.Instance.Map.Width / 2f + GameManager.Instance.Map.LaneWidth / 2f ,
                GameManager.Instance.Map.Height / 2f + GameManager.Instance.Map.LaneWidth / 2f), new List<Unit>());
            grid.Add(new Rect(GameManager.Instance.Map.Width / 2f + GameManager.Instance.Map.LaneWidth / 2f, 0,
                GameManager.Instance.Map.Width / 2f - GameManager.Instance.Map.LaneWidth / 2f,
                GameManager.Instance.Map.Height / 2f + GameManager.Instance.Map.LaneWidth / 2f), new List<Unit>());
            grid.Add(new Rect(0, GameManager.Instance.Map.Height / 2f + GameManager.Instance.Map.LaneWidth / 2f,
                GameManager.Instance.Map.Width / 2f + GameManager.Instance.Map.LaneWidth / 2f,
                GameManager.Instance.Map.Height / 2f - GameManager.Instance.Map.LaneWidth / 2f), new List<Unit>());
            grid.Add(new Rect(GameManager.Instance.Map.Width / 2f + GameManager.Instance.Map.LaneWidth / 2f,
                GameManager.Instance.Map.Height / 2f + GameManager.Instance.Map.LaneWidth / 2f,
                GameManager.Instance.Map.Width / 2f - GameManager.Instance.Map.LaneWidth / 2f,
                GameManager.Instance.Map.Height / 2f - GameManager.Instance.Map.LaneWidth / 2f), new List<Unit>());

            grid.Add(new Rect(GameManager.Instance.Map.Width / 2f - SPECIAL_SQUARE_RADIUS,
                GameManager.Instance.Map.Height / 2f - SPECIAL_SQUARE_RADIUS, SPECIAL_SQUARE_RADIUS * 2f,
                SPECIAL_SQUARE_RADIUS * 2f), new List<Unit>());
            
            grid.Add(new Rect(GameManager.Instance.Map.BasePositions[0].x - SPECIAL_SQUARE_RADIUS,
                GameManager.Instance.Map.BasePositions[0].y - SPECIAL_SQUARE_RADIUS, SPECIAL_SQUARE_RADIUS * 2f,
                SPECIAL_SQUARE_RADIUS * 2f), new List<Unit>());
            grid.Add(new Rect(GameManager.Instance.Map.BasePositions[1].x - SPECIAL_SQUARE_RADIUS,
                GameManager.Instance.Map.BasePositions[1].y - SPECIAL_SQUARE_RADIUS, SPECIAL_SQUARE_RADIUS * 2f,
                SPECIAL_SQUARE_RADIUS * 2f), new List<Unit>());
            grid.Add(new Rect(GameManager.Instance.Map.BasePositions[2].x - SPECIAL_SQUARE_RADIUS,
                GameManager.Instance.Map.BasePositions[2].y - SPECIAL_SQUARE_RADIUS, SPECIAL_SQUARE_RADIUS * 2f,
                SPECIAL_SQUARE_RADIUS * 2f), new List<Unit>());
            grid.Add(new Rect(GameManager.Instance.Map.BasePositions[3].x - SPECIAL_SQUARE_RADIUS,
                GameManager.Instance.Map.BasePositions[3].y - SPECIAL_SQUARE_RADIUS, SPECIAL_SQUARE_RADIUS * 2f,
                SPECIAL_SQUARE_RADIUS * 2f), new List<Unit>());
            
            UnitManager.Instance.RegisterOnUnitCreated(x =>
            {
                AddUnit(x);
                x.RegisterOnDestroyedCallback(RemoveUnit);
                x.RegisterOnMovedCallback(OnUnitMove);
            });
        }

        private void AddUnit(Unit unit)
        {
            bool added = false;

            // Check cells with index 4 and above first
            for (int i = 4; i < grid.Keys.Count; i++)
            {
                var cell = grid.Keys.ElementAt(i);
                if (cell.Contains(new Vector2(unit.Position.x, unit.Position.z)))
                {
                    grid[cell].Add(unit);
                    currentCell[unit] = i;
                    added = true;
                    break;
                }
            }

            // If the unit is not in any cell with index 4 and above, check the remaining cells
            if (!added)
            {
                foreach (var cell in grid.Keys.Take(4))
                {
                    if (cell.Contains(new Vector2(unit.Position.x, unit.Position.z)))
                    {
                        grid[cell].Add(unit);
                        currentCell[unit] = grid.Keys.ToList().IndexOf(cell);
                        break;
                    }
                }
            }
        }

        private void RemoveUnit(Unit unit)
        {
            foreach (var cell in grid.Keys)
            {
                if (cell.Contains(new Vector2(unit.Position.x, unit.Position.z)))
                {
                    grid[cell].Remove(unit);
                }
            }
        }

        private void OnUnitMove(Unit unit)
        {
            int cellId = -1;

            // Check cells with index 4 and above first
            for (int i = 4; i < grid.Keys.Count; i++)
            {
                var cell = grid.Keys.ElementAt(i);
                if (cell.Contains(new Vector2(unit.Position.x, unit.Position.z)))
                {
                    cellId = i;
                    break;
                }
            }

            // If the unit is not in any cell with index 4 and above, check the remaining cells
            if (cellId == -1)
            {
                foreach (var cell in grid.Keys.Take(4))
                {
                    if (cell.Contains(new Vector2(unit.Position.x, unit.Position.z)))
                    {
                        cellId = grid.Keys.ToList().IndexOf(cell);
                        break;
                    }
                }
            }

            var currentId = currentCell[unit];
            if (cellId == currentId) return;
            RemoveUnit(unit);
            AddUnit(unit);
        }

        public List<Unit> Query(Unit unit)
        {
            var cell = currentCell[unit];
            return grid[grid.Keys.ElementAt(cell)];
        }



        public void OnDrawGizmos()
        {
            if (grid == null)
                return;
            foreach (var cell in grid.Keys)
            {
                DrawNodeGizmos(cell);
            }
            
        }

        private void DrawNodeGizmos(Rect rect)
        {
            Gizmos.color = Color.cyan;

            // Draw the bounds of the current node
            Gizmos.DrawWireCube(new Vector3(rect.x + rect.width / 2f, 0f, rect.y + rect.height / 2f),
                new Vector3(rect.width, 0f, rect.height));
        }
    }
}