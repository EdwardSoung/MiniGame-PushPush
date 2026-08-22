using FirstVillain.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    public int Id { get { return _table.Id; } }
    public int STR { get { return _table.Str; } }
    public float RANGE { get { return _table.Range; } }
    public float SPEED { get { return _table.Speed; } }

    public E_TEAM Team { get; private set; }

    public string PrefapName { get { return _table.PrefabName; } }
    
    public string UIPrefabName { get { return _table.UIPrefabName; } }

    private JPlayerData _table;

    public PlayerInfo(JPlayerData data)
    {
        _table = data;
    }

    public void SetTeam(E_TEAM team)
    {
        Team = team;
    }
}
