using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public Map Map;
    public GameObject MapGameObject;

    [SerializeField]
    private GameObject playerPrefab;

    public List<RaceData> Races = new List<RaceData>();

    public List<Player> Players = new List<Player>();

    public float timePassed = 0f;

    private bool mapCreated = false;
    
    private void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("Two GameManagers");
            return;
        }

        Instance = this;
        //NewPlayer();
        NewPlayer("Player 1");
        NewPlayer("Player 2");
        NewPlayer("Player 3");
        NewPlayer("Player 4");
        
        CreateNewMap();
        SetupBarracksPaths();
        MapPartitioner.Instance.Init();
        UnitManager.Instance.Init();
    }

    private void NewPlayer(string playerName)
    {
        var playerGO = Instantiate(playerPrefab);
        playerGO.name = playerName;
        var player = playerGO.GetComponent<Player>();
        //Chose a random race for the player
        player.Initialize(new Race(Races[Random.Range(0, Races.Count)]), playerName);
        playerGO.name = playerName;
        Players.Add(player);
    }
    
    private void SetupBarracksPaths()
    {
        foreach (var player in Instance.Players)
        {
            foreach (var barrack in player.Barracks)
            {
                barrack.SetPath();
            }
        }
    }

    private void CreateNewMap()
    {
        /*var neutralBuildings = new NeutralBuilding[8];

        for (int i = 0; i < neutralBuildings.Length; i++)
        {
            // select a random neutral from predefined list
            neutralBuildings[i] = new NeutralBuilding(3, new Ability[3], 50);
        }
        */
        Map = new Map(/*neutralBuildings,*/ 150, 150, 10);

        CreateMapGameObject();
        mapCreated = true;
    }

    private void CreateMapGameObject()
    {
        MapGameObject = GameObject.Find("Ground");
        MapGameObject.tag = "Ground";
    }
}
