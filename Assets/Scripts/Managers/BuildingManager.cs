using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Managers
{
    public class BuildingManager : MonoBehaviour
    {
        private Dictionary<Building, GameObject> buildingGameObjectMap;

        private Map map => GameManager.Instance.Map;

        private void Start()
        {
            buildingGameObjectMap = new Dictionary<Building, GameObject>();

            foreach (var building in map.AllBuildingsOnMap)
            {
                BuildingCreated(building);
            }
        }

        private void BuildingCreated(Building building)
        {
            GameObject buildingGO = GetMeshForBuilding(building);
            
            buildingGameObjectMap.Add(building, buildingGO);

            buildingGO.name = building.Type + " (" + building.Position + ")";
            buildingGO.transform.position = new Vector3(building.Position.x, 1, building.Position.y);
            
            
            if (building.Type == BuildingType.Castle)
            {
                buildingGO.transform.position = new Vector3(building.Position.x, 2, building.Position.y);
                buildingGO.transform.localScale = new Vector3(2, 2, 2);
            }

            building.RegisterOnChangedCallback(OnBuildingChanged);
            building.RegisterOnRemovedCallback(OnBuildingRemoved);
            
        }
        
        private GameObject GetMeshForBuilding(Building building)
        {
            var mesh_go = Instantiate(MeshManager.Instance.GetMesh(building.Player.RaceName,building.Type.ToString()), transform, true);
            mesh_go.SetActive(true);
            return mesh_go;
        }
        
        void OnBuildingChanged(Building building)
        {
            
        }

        void OnBuildingRemoved(Building building)
        {
            if (!buildingGameObjectMap.TryGetValue(building, out var building_go))
            {
                Debug.LogError("Building not present on Map");
                return;
            }

            Destroy(building_go);
            buildingGameObjectMap.Remove(building);
        }
    }
}