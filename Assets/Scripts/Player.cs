using System;
using Data;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Race playerRace;

    public string RaceName;

    public Castle Castle;

    public Building[] Towers = new Building[4];
    public Barracks[] Barracks = new Barracks[3];
    
    //DEBUG
    public string name;
    
    public void Initialize(Race race, string name)
    {
        playerRace = race;
        RaceName = playerRace.Name;
        this.name = name;
        Castle = new Castle(name + " Castle",5000, 2, 30, 50, 1, 400, BuildingType.Castle,this, 400, 1.7f);

        for (int i = 0; i < Towers.Length; i++)
        {
            Towers[i] = new Building(name + " Tower["+i+"]",1500, 1.5f, 15, 20, 0.7f, 300, BuildingType.Tower, this);
        }

        for (int i = 0; i < Barracks.Length; i++)
        {
            Barracks[i] = new Barracks(name + " Barrack["+i+"]",2500, 1.8f, 20, 25, 0.8f, 300, BuildingType.Barracks, this);
        }
    }

    private void Update()
    {
        Castle.Update(Time.deltaTime);
        foreach (var barrack in Barracks)
        {
            barrack.Update(Time.deltaTime);
        }
        foreach (var tower in Towers)
        {
            tower.Update(Time.deltaTime);
        }
    }
}