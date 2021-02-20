﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSceneLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerController.Instance.transform.position = GlobalControlSave.Instance.savedPlayerData.playerPosition;
    }
}
