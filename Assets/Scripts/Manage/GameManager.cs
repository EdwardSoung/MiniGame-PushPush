using FirstVillain.Entities;
using FirstVillain.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : UnitySingleton<GameManager>
{

    #region Player Data
    public PlayerInfo MainPlayer 
    { 
        get
        {
            return PlayerPrefsManager.LoadMainPlayer();
        } 
    }

    public int PlayerCount
    {
        get
        {
            return _playerData.Count;
        }
    }

    private List<PlayerInfo> _playerData = new();

    public void SetPlayerData(List<JPlayerData> dataList)
    {
        foreach (var data in dataList)
        {
            _playerData.Add(new PlayerInfo(data));
        }
    }

    public PlayerInfo GetPlayerData(int id)
    {
        return _playerData.Find(arg => arg.Id == id);
    }
    
    public PlayerInfo GetPlayerDataByIndex(int idx)
    {
        if(idx < 0 || idx >= PlayerCount)
        {
            Debug.LogError("Plyer index out of range!!");
            return null;
        }
        return _playerData[idx];
    }

    public int GetPlayerDataIndex(PlayerInfo info)
    {
        return _playerData.FindIndex(arg => arg == info);
    }
    #endregion Player Data
}
