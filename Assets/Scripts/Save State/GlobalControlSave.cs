using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControlSave : MonoBehaviour
{
    public static GlobalControlSave Instance;
    public PlayerStatistics savedPlayerData = new PlayerStatistics();
    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            Instance.savedPlayerData.FillBaseValues();
        } 
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}
