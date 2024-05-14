using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Utility;

namespace Managers
{
    public class UnitManager : MonoBehaviour
    {
        public static UnitManager Instance;

        private List<GameObject> unitGOs;

        private Action<Unit> onUnitCreated;
        //private Map map => GameManager.Instance.Map;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("More than one UnitManager!");
                return;
            }

            Instance = this;
        }

        public void Init()
        {
            unitGOs = new List<GameObject>();
            GameManager.Instance.Map.RegisterOnUnitSpawnedCallback(UnitCreated);
        }

        private Unit UnitCreated(Barracks barracks, UnitGameData unitData, Player player, Vector3 position)
        {
            GameObject unitGO = GetMeshForUnit(unitData, player, position);
            Unit unit = unitGO.GetComponent<Unit>();
            unit.InitFromData(unitData, position, player, barracks.Path);

            unitGO.name = unit.Name;
            unitGO.transform.SetParent(transform);
            
            unit.RegisterOnDestroyedCallback(OnUnitDestroyed);
            unitGOs.Add(unitGO);

            onUnitCreated?.Invoke(unit);
            return unit;
        }

        private GameObject GetMeshForUnit(UnitGameData unit, Player player, Vector3 position)
        {
            var mesh_go = Instantiate(MeshManager.Instance.GetMesh(player.RaceName, unit.Type.ToString()), position, Quaternion.identity);
            mesh_go.SetActive(true);
            return mesh_go;
        }

        void OnUnitDestroyed(Unit unit)
        {
            unitGOs.Remove(unit.gameObject);
            unit.UnregisterOnDestroyedCallback(OnUnitDestroyed);
        }

        public void RegisterOnUnitCreated(Action<Unit> cb)
        {
            onUnitCreated += cb;
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            Gizmos.color = Color.yellow;
            Handles.color = Color.yellow;
            foreach (var o in unitGOs.Select(x => x.GetComponent<Unit>()))
            {
                Gizmos.DrawSphere(o.Position, .5f);
                Handles.DrawWireDisc(o.transform.position, Vector3.up, o.AggroRange);
            }
            
            Gizmos.color = Color.black;
            Handles.color = Color.magenta;
            foreach (var o in unitGOs.Select(x => x.GetComponent<Unit>()))
            {
                Gizmos.DrawSphere(o.GetComponent<NavMeshAgent>().destination, 0.5f);
            }
            
            Handles.color = Color.red;
            foreach (var o in unitGOs.Select(x => x.GetComponent<Unit>()))
            {
                //Gizmos.DrawSphere(new Vector3(o.Key.Position.x, 0, o.Key.Position.y), .5f);
                Handles.DrawWireDisc(o.transform.position, Vector3.up, o.AttackRange);
            }
        }
    }
}