﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopulateGrid : MonoBehaviour
{
	public GameObject prefab; // This is our prefab object that will be exposed in the inspector

	void Start()
	{
		Populate();
	}

    private void Update()
    {
        
    }

    void Populate()
	{
		GameObject newObj; // Create GameObject instance

		var items = PlayerControlSave.Instance.localPlayerData.playerItems;
        Debug.Log(items.Count);

        for (int i = 0; i < items.Count; i++)
        {
            // Create new instances of our prefab until we've created as many as we specified
            newObj = (GameObject)Instantiate(prefab, transform);
            var texts = newObj.GetComponentInChildren<TextMeshProUGUI>();
            Debug.Log(texts);

        }
    }
}