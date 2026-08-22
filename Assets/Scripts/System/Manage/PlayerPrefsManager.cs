using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsManager
{
    private static readonly string KEY_MAIN_PLAYER = "MainPlayerKey";
    public static void SaveMainPlayer(int id)
    {
        PlayerPrefs.SetInt(KEY_MAIN_PLAYER, id);
        PlayerPrefs.Save();
    }

    public static PlayerInfo LoadMainPlayer()
    {
        var id = PlayerPrefs.GetInt(KEY_MAIN_PLAYER, 1);
        return GameManager.Instance.GetPlayerData(id);
    }
}
